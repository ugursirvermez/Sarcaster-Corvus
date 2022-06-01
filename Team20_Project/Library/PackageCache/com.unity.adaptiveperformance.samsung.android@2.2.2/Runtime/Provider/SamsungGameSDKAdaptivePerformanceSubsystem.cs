#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static System.Threading.Thread;
using UnityEngine.Scripting;
using UnityEngine.AdaptivePerformance.Provider;

[assembly: AlwaysLinkAssembly]
namespace UnityEngine.AdaptivePerformance.Samsung.Android
{
    internal static class GameSDKLog
    {
        static SamsungAndroidProviderSettings settings = SamsungAndroidProviderSettings.GetSettings();

        [Conditional("DEVELOPMENT_BUILD")]
        public static void Debug(string format, params object[] args)
        {
            if (settings != null && settings.samsungProviderLogging)
                UnityEngine.Debug.Log(System.String.Format("[Samsung GameSDK] " + format, args));
        }
    }

    internal class AsyncUpdater : IDisposable
    {
        private Thread m_Thread;
        private bool m_Disposed = false;
        private bool m_Quit = false;

        private List<Action> m_UpdateAction = new List<Action>();
        private int[] m_UpdateRequests = null;
        private bool[] m_RequestComplete = null;
        private int m_UpdateRequestReadIndex = 0;
        private int m_UpdateRequestWriteIndex = 0;

        private object m_Mutex = new object();
        private Semaphore m_Semaphore = null;

        public int Register(Action action)
        {
            if (m_Thread.IsAlive)
                return -1;

            int handle = m_UpdateAction.Count;
            m_UpdateAction.Add(action);

            return handle;
        }

        public void Start()
        {
            int maxRequests = m_UpdateAction.Count;
            if (maxRequests <= 0)
                return;

            m_Semaphore = new Semaphore(0, maxRequests);
            m_UpdateRequests = new int[maxRequests];
            m_RequestComplete = new bool[maxRequests];

            m_Thread.Start();
        }

        public bool RequestUpdate(int handle)
        {
            lock (m_Mutex)
            {
                int newWriteIndex = (m_UpdateRequestWriteIndex + 1) % m_UpdateRequests.Length;
                if (newWriteIndex == m_UpdateRequestReadIndex)
                {
                    return false;
                }
                m_UpdateRequests[m_UpdateRequestWriteIndex] = handle;
                m_RequestComplete[handle] = false;
                m_UpdateRequestWriteIndex = newWriteIndex;
            }

            m_Semaphore.Release();

            return true;
        }

        public bool IsRequestComplete(int handle)
        {
            lock (m_Mutex)
            {
                return m_RequestComplete[handle];
            }
        }

        public AsyncUpdater()
        {
            m_Thread = new Thread(new ThreadStart(ThreadProc));
            m_Thread.Name = "SamsungGameSDK";
        }

        private void ThreadProc()
        {
            AndroidJNI.AttachCurrentThread();

            while (true)
            {
                try
                {
                    m_Semaphore.WaitOne();
                }
                catch (Exception)
                {
                    break;
                }

                int handle = -1;

                lock (m_Mutex)
                {
                    if (m_Quit)
                        break;

                    if (m_UpdateRequestReadIndex != m_UpdateRequestWriteIndex)
                    {
                        handle = m_UpdateRequests[m_UpdateRequestReadIndex];
                        m_UpdateRequestReadIndex = (m_UpdateRequestReadIndex + 1) % m_UpdateRequests.Length;
                    }
                }

                if (handle >= 0)
                {
                    try
                    {
                        m_UpdateAction[handle].Invoke();
                    }
                    catch (Exception)
                    {
                    }

                    lock (m_Mutex)
                    {
                        m_RequestComplete[handle] = true;
                    }
                }
            }

            AndroidJNI.DetachCurrentThread();
        }

        private void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            if (disposing)
            {
                if (m_Thread.IsAlive)
                {
                    lock (m_Mutex)
                    {
                        m_Quit = true;
                    }

                    m_Semaphore.Release();
                    m_Thread.Join();
                }
            }
            m_Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    internal class AsyncValue<T>
    {
        private AsyncUpdater updater = null;
        private int updateHandle = -1;
        private bool pendingUpdate = false;
        private Func<T> updateFunc = null;
        private T newValue;

        private float updateTimeDeltaSeconds;
        private float updateTimestamp;

        public AsyncValue(AsyncUpdater updater, T value, float updateTimeDeltaSeconds, Func<T> updateFunc)
        {
            this.updater = updater;
            this.updateTimeDeltaSeconds = updateTimeDeltaSeconds;
            this.updateFunc = updateFunc;
            this.value = value;
            this.updateHandle = updater.Register(() => newValue = updateFunc());
        }

        public bool Update(float timestamp)
        {
            bool changed = false;

            if (pendingUpdate && updater.IsRequestComplete(updateHandle))
            {
                changed = !value.Equals(newValue);
                if (changed)
                    changeTimestamp = timestamp;

                value = newValue;
                updateTimestamp = timestamp;
                pendingUpdate = false;
            }

            if (!pendingUpdate)
            {
                if (timestamp - updateTimestamp > updateTimeDeltaSeconds)
                {
                    pendingUpdate = updater.RequestUpdate(updateHandle);
                }
            }
            return changed;
        }

        public void SyncUpdate(float timestamp)
        {
            var oldValue = value;
            updateTimestamp = timestamp;
            value = updateFunc();
            if (!value.Equals(oldValue))
                changeTimestamp = timestamp;
        }

        public T value { get; private set; }
        public float changeTimestamp { get; private set; }
    }

    [Preserve]
    public class SamsungGameSDKAdaptivePerformanceSubsystem : AdaptivePerformanceSubsystem, IApplicationLifecycle, IDevicePerformanceLevelControl
    {
        private NativeApi m_Api = null;

        private AsyncUpdater m_AsyncUpdater;

        private PerformanceDataRecord m_Data = new PerformanceDataRecord();
        private object m_DataLock = new object();

        private AsyncValue<double> m_SkinTemp = null;
        private AsyncValue<double> m_GPUTime = null;

        private Version m_Version = null;

        private float m_MinTempLevel = 0.0f;
        private float m_MaxTempLevel = 10.0f;
        bool m_PerformanceLevelControlSystemChange = false;

        private AutoVariableRefreshRate m_AutoVariableRefreshRate;

        override public IApplicationLifecycle ApplicationLifecycle { get { return this; } }
        override public IDevicePerformanceLevelControl PerformanceLevelControl { get { return this; } }

        public int MaxCpuPerformanceLevel { get; set; }
        public int MaxGpuPerformanceLevel { get; set; }

        static SamsungAndroidProviderSettings settings = SamsungAndroidProviderSettings.GetSettings();

        public SamsungGameSDKAdaptivePerformanceSubsystem()
        {
            MaxCpuPerformanceLevel = 3;
            MaxGpuPerformanceLevel = 3;

            m_Api = new NativeApi(OnPerformanceWarning, OnPerformanceLevelTimeout, () => (VariableRefreshRate.Instance as VRRManager)?.OnRefreshRateChanged(), OnCpuPerformanceBoostModeTimeout, OnGpuPerformanceBoostModeTimeout);
            m_AsyncUpdater = new AsyncUpdater();
            m_SkinTemp = new AsyncValue<double>(m_AsyncUpdater, -1.0, 2.7f, () => GetHighPrecisionSkinTempLevel());
            m_GPUTime = new AsyncValue<double>(m_AsyncUpdater, -1.0, 0.0f, () => m_Api.GetGpuFrameTime());

            Capabilities = Feature.CpuPerformanceLevel | Feature.GpuPerformanceLevel | Feature.PerformanceLevelControl | Feature.TemperatureLevel | Feature.WarningLevel | Feature.GpuFrameTime;

            m_AsyncUpdater.Start();
        }

        private void OnPerformanceWarning(WarningLevel warningLevel)
        {
            lock (m_DataLock)
            {
                m_Data.ChangeFlags |= Feature.WarningLevel;
                m_Data.ChangeFlags |= Feature.PerformanceLevelControl;
                m_Data.WarningLevel = warningLevel;
            }
        }

        private void OnPerformanceLevelTimeout()
        {
            lock (m_DataLock)
            {
                m_Data.ChangeFlags |= Feature.CpuPerformanceLevel;
                m_Data.ChangeFlags |= Feature.GpuPerformanceLevel;
                m_Data.CpuPerformanceLevel = Constants.UnknownPerformanceLevel;
                m_Data.GpuPerformanceLevel = Constants.UnknownPerformanceLevel;
            }
        }

        private void OnCpuPerformanceBoostModeTimeout()
        {
            lock (m_DataLock)
            {
                m_Data.ChangeFlags |= Feature.CpuPerformanceBoost;
                m_Data.CpuPerformanceBoost = false;
            }
        }

        private void OnGpuPerformanceBoostModeTimeout()
        {
            lock (m_DataLock)
            {
                m_Data.ChangeFlags |= Feature.GpuPerformanceBoost;
                m_Data.GpuPerformanceBoost = false;
            }
        }

        private float GetHighPrecisionSkinTempLevel()
        {
            return (float)m_Api.GetHighPrecisionSkinTempLevel();
        }

        private int GetClusterInfo()
        {
            return m_Api.GetClusterInfo();
        }

        private void ImmediateUpdateTemperature()
        {
            var timestamp = Time.time;
            m_SkinTemp.SyncUpdate(timestamp);

            lock (m_DataLock)
            {
                m_Data.ChangeFlags |= Feature.TemperatureLevel;
                m_Data.TemperatureLevel = GetTemperatureLevel();
            }
        }

        private static bool TryParseVersion(string versionString, out Version version)
        {
            try
            {
                version = new Version(versionString);
            }
            catch (Exception)
            {
                version = null;
                return false;
            }
            return true;
        }

        override public void Start()
        {
            if (m_Api.Initialize())
            {
                if (TryParseVersion(m_Api.GetVersion(), out m_Version))
                {
                    if (m_Version >= new Version(3, 5))
                    {
                        initialized = true;
                        MaxCpuPerformanceLevel = m_Api.GetMaxCpuPerformanceLevel();
                        MaxGpuPerformanceLevel = m_Api.GetMaxGpuPerformanceLevel();
                        Capabilities |= Feature.CpuPerformanceBoost | Feature.GpuPerformanceBoost;
                    }
                    else if (m_Version >= new Version(3, 4))
                    {
                        initialized = true;
                        MaxCpuPerformanceLevel = m_Api.GetMaxCpuPerformanceLevel();
                        MaxGpuPerformanceLevel = m_Api.GetMaxGpuPerformanceLevel();
                    }
                    else if (m_Version >= new Version(3, 2))
                    {
                        initialized = true;
                        MaxCpuPerformanceLevel = m_Api.GetMaxCpuPerformanceLevel();
                        MaxGpuPerformanceLevel = m_Api.GetMaxGpuPerformanceLevel();
                    }
                    else
                    {
                        m_Api.Terminate();
                        initialized = false;
                    }
                }

                m_Data.PerformanceLevelControlAvailable = true;
            }

            if (initialized)
            {
                ImmediateUpdateTemperature();

                Thread t = new Thread(CheckInitialTemperatureAndSendWarnings);
                t.Start();

                CheckAndInitializeVRR();
            }
        }

        void CheckAndInitializeVRR()
        {
            if (m_Api.IsVariableRefreshRateSupported())
            {
                if (VariableRefreshRate.Instance == null)
                {
                    VariableRefreshRate.Instance = new VRRManager(m_Api);
                    m_AutoVariableRefreshRate = new AutoVariableRefreshRate(VariableRefreshRate.Instance);
                }
            }
            else
            {
                VariableRefreshRate.Instance = null;
                m_AutoVariableRefreshRate = null;
            }
        }

        void CheckInitialTemperatureAndSendWarnings()
        {
            // If the device is already warm upon startup and past the throttling imminent warning level
            // the warning callback is not called as it's not available yet. We need to set it manually based on temperature as workaround.
            // On startup the temperature reading is always 0. After a couple of seconds a true value is returned. Therefore we wait for 2 seconds before we make the reading.
            Sleep(TimeSpan.FromSeconds(2));
            float currentTempLevel = GetHighPrecisionSkinTempLevel();

            if (m_Version >= new Version(3, 2))
            {
                if (currentTempLevel >= 7)
                    OnPerformanceWarning(WarningLevel.Throttling);
                else if (currentTempLevel >= 5)
                    OnPerformanceWarning(WarningLevel.ThrottlingImminent);
            }

            if (m_Version >= new Version(3, 5))
            {
                // Cluster info is not available in the same frame as game sdk init so we need to wait a bit.
                int clusterInfo = m_Api.GetClusterInfo();
                if (clusterInfo != -999)
                {
                    var aClusterInfo = new ClusterInfo();
                    aClusterInfo.BigCore = clusterInfo / 100;
                    aClusterInfo.MediumCore = (clusterInfo % 100) / 10;
                    aClusterInfo.LittleCore = (clusterInfo % 100) % 10;
                    lock (m_DataLock)
                    {
                        m_Data.ClusterInfo = aClusterInfo;
                        m_Data.ChangeFlags |= Feature.ClusterInfo;
                    }
                    Capabilities |= Feature.ClusterInfo;
                }
            }
        }

        override public void Stop()
        {
        }

        protected override void OnDestroy()
        {
            VariableRefreshRate.Instance = null;
            m_AutoVariableRefreshRate = null;

            if (initialized)
            {
                m_Api.Terminate();
                initialized = false;
            }

            m_AsyncUpdater.Dispose();
        }

        public override string Stats => $"SkinTemp={m_SkinTemp?.value ?? -1} GPUTime={m_GPUTime?.value ?? -1}";

        override public PerformanceDataRecord Update()
        {
            // GameSDK API is very slow (~4ms per call), so update those numbers once per frame from another thread

            float timeSinceStartup = Time.time;

            m_GPUTime.Update(timeSinceStartup);

            bool tempChanged = m_SkinTemp.Update(timeSinceStartup);

            (VariableRefreshRate.Instance as VRRManager)?.Update();

            if ((VariableRefreshRate.Instance as VRRManager) != null && settings.automaticVRR)
                if (QualitySettings.vSyncCount == 0)
                    m_AutoVariableRefreshRate.UpdateAutoVRR();

            if (m_PerformanceLevelControlSystemChange)
            {
                var temperatureLevel = (float)m_SkinTemp.value;
                if (temperatureLevel < 5)
                {
                    m_PerformanceLevelControlSystemChange = false;
                    lock (m_DataLock)
                    {
                        m_Data.PerformanceLevelControlAvailable = true;
                        m_Data.ChangeFlags |= Feature.PerformanceLevelControl;
                    }
                }
            }

            lock (m_DataLock)
            {
                if (tempChanged)
                {
                    m_Data.ChangeFlags |= Feature.TemperatureLevel;
                    m_Data.TemperatureLevel = GetTemperatureLevel();
                }

                m_Data.GpuFrameTime = LatestGpuFrameTime();
                m_Data.ChangeFlags |= Feature.GpuFrameTime;

                PerformanceDataRecord result = m_Data;
                m_Data.ChangeFlags = Feature.None;

                return result;
            }
        }

        public override Version Version
        {
            get
            {
                return m_Version;
            }
        }

        private static float NormalizeTemperatureLevel(float currentTempLevel, float minValue, float maxValue)
        {
            float tempLevel = -1.0f;
            if (currentTempLevel >= minValue && currentTempLevel <= maxValue)
            {
                tempLevel = currentTempLevel / maxValue;
                tempLevel = Math.Min(Math.Max(tempLevel, Constants.MinTemperatureLevel), maxValue);
            }
            return tempLevel;
        }

        private float NormalizeTemperatureLevel(float currentTempLevel)
        {
            return NormalizeTemperatureLevel(currentTempLevel, m_MinTempLevel, m_MaxTempLevel);
        }

        private float GetTemperatureLevel()
        {
            return NormalizeTemperatureLevel((float)m_SkinTemp.value);
        }

        private float LatestGpuFrameTime()
        {
            return (float)(m_GPUTime.value / 1000.0);
        }

        public bool SetPerformanceLevel(ref int cpuLevel, ref int gpuLevel)
        {
            if (cpuLevel < 0)
                cpuLevel = 0;
            else if (cpuLevel > MaxCpuPerformanceLevel)
                cpuLevel = MaxCpuPerformanceLevel;

            if (gpuLevel < 0)
                gpuLevel = 0;
            else if (gpuLevel > MaxGpuPerformanceLevel)
                gpuLevel = MaxGpuPerformanceLevel;

            if (m_Version == new Version(3, 2) && cpuLevel == 0)
                cpuLevel = 1;

            bool success = false;

            int result = m_Api.SetFreqLevels(cpuLevel, gpuLevel);
            success = result == 1;

            lock (m_DataLock)
            {
                var oldCpuLevel = m_Data.CpuPerformanceLevel;
                var oldGpuLevel = m_Data.GpuPerformanceLevel;

                m_Data.CpuPerformanceLevel = success ? cpuLevel : Constants.UnknownPerformanceLevel;
                m_Data.GpuPerformanceLevel = success ? gpuLevel : Constants.UnknownPerformanceLevel;

                if (m_Data.CpuPerformanceLevel != oldCpuLevel)
                    m_Data.ChangeFlags |= Feature.CpuPerformanceLevel;
                if (m_Data.GpuPerformanceLevel != oldGpuLevel)
                    m_Data.ChangeFlags |= Feature.GpuPerformanceLevel;

                if (result > 1)
                {
                    if (result == 2)
                    {
                        GameSDKLog.Debug($"Thermal Mitigation Logic is working and CPU({cpuLevel})/GPU({gpuLevel}) level change request was not approved.");
                    }
                    else if (result == 3)
                    {
                        GameSDKLog.Debug($"CPU or GPU Boost mode is active and CPU({cpuLevel})/GPU({gpuLevel}) level change request was not approved.");
                    }

                    m_Data.PerformanceLevelControlAvailable = false;
                    m_Data.ChangeFlags |= Feature.PerformanceLevelControl;
                    m_PerformanceLevelControlSystemChange = true;
                }
            }
            return success;
        }

        public bool EnableCpuBoost()
        {
            var result = m_Api.EnableCpuBoost();

            lock (m_DataLock)
            {
                var oldPerformanceBoost = m_Data.CpuPerformanceBoost;
                m_Data.CpuPerformanceBoost = result;
                if (m_Data.CpuPerformanceBoost != oldPerformanceBoost)
                    m_Data.ChangeFlags |= Feature.CpuPerformanceBoost;

                if (result)
                {
                    m_Data.PerformanceLevelControlAvailable = false;
                    m_Data.ChangeFlags |= Feature.PerformanceLevelControl;
                    m_PerformanceLevelControlSystemChange = true;
                }
            }
            return result;
        }

        public bool EnableGpuBoost()
        {
            var result = m_Api.EnableGpuBoost();

            lock (m_DataLock)
            {
                var oldPerformanceBoost = m_Data.GpuPerformanceBoost;
                m_Data.GpuPerformanceBoost = result;
                if (m_Data.GpuPerformanceBoost != oldPerformanceBoost)
                    m_Data.ChangeFlags |= Feature.GpuPerformanceBoost;

                if (result)
                {
                    m_Data.PerformanceLevelControlAvailable = false;
                    m_Data.ChangeFlags |= Feature.PerformanceLevelControl;
                    m_PerformanceLevelControlSystemChange = true;
                }
            }
            return result;
        }

        public void ApplicationPause() {}

        public void ApplicationResume()
        {
            //We need to re-initialize because some Android onForegroundchange() APIs do not detect the change (e.g. bixby)
            if (!m_Api.Initialize())
                GameSDKLog.Debug("Resume: reinitialization failed!");

            lock (m_DataLock)
            {
                m_Data.CpuPerformanceLevel = Constants.UnknownPerformanceLevel;
                m_Data.GpuPerformanceLevel = Constants.UnknownPerformanceLevel;
                m_Data.ChangeFlags |= Feature.CpuPerformanceLevel;
                m_Data.ChangeFlags |= Feature.GpuPerformanceLevel;
            }

            ImmediateUpdateTemperature();

            CheckAndInitializeVRR();

            (VariableRefreshRate.Instance as VRRManager)?.Resume();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            if (!SystemInfo.deviceModel.StartsWith("samsung", StringComparison.OrdinalIgnoreCase))
            {
                GameSDKLog.Debug($"The device {SystemInfo.deviceModel} is not a supported Samsung phone. This provider will not run. Aborting registering the Adaptive Performance provider descriptor.");
                return;
            }

            if (!NativeApi.IsAvailable())
            {
                GameSDKLog.Debug($"The native API for this provider is not available. Aborting registering the Adaptive Performance provider descriptor.");
                return;
            }

            AdaptivePerformanceSubsystemDescriptor.RegisterDescriptor(new AdaptivePerformanceSubsystemDescriptor.Cinfo
            {
                id = "SamsungGameSDK",
                subsystemImplementationType = typeof(SamsungGameSDKAdaptivePerformanceSubsystem)
            });
        }

        internal class NativeApi : AndroidJavaProxy
        {
            static private AndroidJavaObject s_GameSDK = null;
            static private IntPtr s_GameSDKRawObjectID;
            static private IntPtr s_GetGpuFrameTimeID;
            static private IntPtr s_GetHighPrecisionSkinTempLevelID;
            static private IntPtr s_GetClusterInfolID;

            static private bool s_isAvailable = false;
            static private jvalue[] s_NoArgs = new jvalue[0];

            private Action<WarningLevel> PerformanceWarningEvent;
            private Action PerformanceLevelTimeoutEvent;
            private Action CpuPerformanceBoostReleasedByTimeoutEvent;
            private Action GpuPerformanceBoostReleasedByTimeoutEvent;
            private Action RefreshRateChangedEvent;

            public NativeApi(Action<WarningLevel> sustainedPerformanceWarning, Action sustainedPerformanceTimeout, Action refreshRateChanged, Action cpuPerformanceBoostReleasedByTimeout, Action gpuPerformanceBoostReleasedByTimeout)
                : base("com.samsung.android.gamesdk.GameSDKManager$Listener")
            {
                PerformanceWarningEvent = sustainedPerformanceWarning;
                PerformanceLevelTimeoutEvent = sustainedPerformanceTimeout;
                RefreshRateChangedEvent = refreshRateChanged;
                CpuPerformanceBoostReleasedByTimeoutEvent = cpuPerformanceBoostReleasedByTimeout;
                GpuPerformanceBoostReleasedByTimeoutEvent = gpuPerformanceBoostReleasedByTimeout;
                StaticInit();
            }

            [Preserve]
            void onHighTempWarning(int warningLevel)
            {
                GameSDKLog.Debug("Listener: onHighTempWarning(warningLevel={0})", warningLevel);
                if (warningLevel == 0)
                    PerformanceWarningEvent(WarningLevel.NoWarning);
                else if (warningLevel == 1)
                    PerformanceWarningEvent(WarningLevel.ThrottlingImminent);
                else if (warningLevel == 2)
                    PerformanceWarningEvent(WarningLevel.Throttling);
            }

            [Preserve]
            void onReleasedByTimeout()
            {
                GameSDKLog.Debug("Listener: onReleasedByTimeout()");
                PerformanceLevelTimeoutEvent();
            }

            [Preserve]
            void onReleasedCpuBoost()
            {
                GameSDKLog.Debug("Listener: onReleasedCpuBoost()");
                CpuPerformanceBoostReleasedByTimeoutEvent();
            }

            [Preserve]
            void onReleasedGpuBoost()
            {
                GameSDKLog.Debug("Listener: onReleasedGPUBoost()");
                GpuPerformanceBoostReleasedByTimeoutEvent();
            }

            [Preserve]
            void onRefreshRateChanged()
            {
                GameSDKLog.Debug("Listener: onRefreshRateChanged()");
                RefreshRateChangedEvent();
            }

            static IntPtr GetJavaMethodID(IntPtr classId, string name, string sig)
            {
                AndroidJNI.ExceptionClear();
                var mid = AndroidJNI.GetMethodID(classId, name, sig);

                IntPtr ex = AndroidJNI.ExceptionOccurred();
                if (ex != (IntPtr)0)
                {
                    AndroidJNI.ExceptionDescribe();
                    AndroidJNI.ExceptionClear();
                    return (IntPtr)0;
                }
                else
                {
                    return mid;
                }
            }

            static private void StaticInit()
            {
                if (s_GameSDK == null)
                {
                    try
                    {
                        s_GameSDK = new AndroidJavaObject("com.samsung.android.gamesdk.GameSDKManager");
                        if (s_GameSDK != null)
                            s_isAvailable = s_GameSDK.CallStatic<bool>("isAvailable");
                    }
                    catch (Exception ex)
                    {
                        GameSDKLog.Debug($"GameSDK is not available due to {ex} Aborting Adaptive Performance initialization.");
                        s_isAvailable = false;
                        s_GameSDK = null;
                    }

                    if (s_isAvailable)
                    {
                        s_GameSDKRawObjectID = s_GameSDK.GetRawObject();
                        var classID = s_GameSDK.GetRawClass();

                        s_GetGpuFrameTimeID = GetJavaMethodID(classID, "getGpuFrameTime", "()D");
                        s_GetHighPrecisionSkinTempLevelID = GetJavaMethodID(classID, "getHighPrecisionSkinTempLevel", "()D");
                        s_GetClusterInfolID =  GetJavaMethodID(classID, "getClusterInfo", "()I");

                        if (s_GetGpuFrameTimeID == (IntPtr)0 || s_GetHighPrecisionSkinTempLevelID == (IntPtr)0)
                            s_isAvailable = false;
                    }
                }
            }

            static public bool IsAvailable()
            {
                StaticInit();
                return s_isAvailable;
            }

            public bool RegisterListener()
            {
                bool success = false;
                try
                {
                    success = s_GameSDK.Call<bool>("setListener", this);
                }
                catch (Exception)
                {
                    success = false;
                }

                if (!success)
                    GameSDKLog.Debug("failed to register listener");

                return success;
            }

            public void UnregisterListener()
            {
                bool success = true;
                try
                {
                    GameSDKLog.Debug("setListener(null)");
                    success = s_GameSDK.Call<bool>("setListener", (Object)null);
                }
                catch (Exception)
                {
                    success = false;
                }

                if (!success)
                    GameSDKLog.Debug("setListener(null) failed!");
            }

            public bool Initialize()
            {
                bool isInitialized = false;
                try
                {
                    Version initVersion;
                    if (TryParseVersion(GetVersion(), out initVersion))
                    {
                        if (initVersion >= new Version(3, 2))
                        {
                            isInitialized = s_GameSDK.Call<bool>("initialize", initVersion.ToString());
                        }
                        else
                        {
                            GameSDKLog.Debug("GameSDK {0} is not supported and will not be initialized, Adaptive Performance will not be used.", initVersion);
                        }

                        if (isInitialized)
                        {
                            isInitialized = RegisterListener();
                        }
                        else
                        {
                            GameSDKLog.Debug("GameSDK.initialize() failed!");
                        }
                    }
                }
                catch (Exception)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.initialize() failed!");
                }

                return isInitialized;
            }

            public void Terminate()
            {
                UnregisterListener();

                try
                {
                    var packageName = Application.identifier;
                    GameSDKLog.Debug("GameSDK.finalize({0})", packageName);
                    s_GameSDK.Call<bool>("finalize", packageName);
                }
                catch (Exception)
                {
                    GameSDKLog.Debug("GameSDK.finalize() failed!");
                }
            }

            public string GetVersion()
            {
                string sdkVersion = "";
                try
                {
                    sdkVersion = s_GameSDK.Call<string>("getVersion");
                }
                catch (Exception)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getVersion() failed!");
                }
                return sdkVersion;
            }

            public double GetHighPrecisionSkinTempLevel()
            {
                double currentTempLevel = -1.0;
                try
                {
                    currentTempLevel = AndroidJNI.CallDoubleMethod(s_GameSDKRawObjectID, s_GetHighPrecisionSkinTempLevelID, s_NoArgs);
                    if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
                    {
                        AndroidJNI.ExceptionDescribe();
                        AndroidJNI.ExceptionClear();
                    }
                }
                catch (Exception)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getHighPrecisionSkinTempLevel() failed!");
                }
                return currentTempLevel;
            }

            public double GetGpuFrameTime()
            {
                double gpuFrameTime = -1.0;
                try
                {
                    gpuFrameTime = AndroidJNI.CallDoubleMethod(s_GameSDKRawObjectID, s_GetGpuFrameTimeID, s_NoArgs);
                    if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
                    {
                        AndroidJNI.ExceptionDescribe();
                        AndroidJNI.ExceptionClear();
                    }
                }
                catch (Exception)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getGpuFrameTime() failed!");
                }

                return gpuFrameTime;
            }

            public int SetFreqLevels(int cpu, int gpu)
            {
                int result = 0;
                try
                {
                    result = s_GameSDK.Call<int>("setFreqLevels", cpu, gpu);
                    GameSDKLog.Debug("setFreqLevels({0}, {1}) -> {2}", cpu, gpu, result);
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.setFreqLevels({0}, {1}) failed: {2}", cpu, gpu, x);
                }
                return result;
            }

            public bool EnableCpuBoost()
            {
                bool result = false;
                try
                {
                    result = s_GameSDK.Call<bool>("setCpuBoostMode", 1);
                    GameSDKLog.Debug("setCpuBoostMode(1) -> {0}", result);
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.setCpuBoostMode(1) failed: {0}", x);
                }
                return result;
            }

            public bool EnableGpuBoost()
            {
                bool result = false;
                try
                {
                    result = s_GameSDK.Call<bool>("setGpuBoostMode", 1);
                    GameSDKLog.Debug("setGpuBoostMode(1) -> {0}", result);
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.setGpuBoostMode(1) failed: {0}", x);
                }
                return result;
            }

            public int GetClusterInfo()
            {
                int result = -999;
                try
                {
                    result = AndroidJNI.CallIntMethod(s_GameSDKRawObjectID, s_GetClusterInfolID, s_NoArgs);
                    if (AndroidJNI.ExceptionOccurred() != IntPtr.Zero)
                    {
                        AndroidJNI.ExceptionDescribe();
                        AndroidJNI.ExceptionClear();
                    }
                    GameSDKLog.Debug("getClusterInfo() -> {0}", result);
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getClusterInfo() failed: {0}", x);
                }
                return result;
            }

            public int GetMaxCpuPerformanceLevel()
            {
                int maxCpuPerformanceLevel = -1;
                try
                {
                    maxCpuPerformanceLevel = s_GameSDK.Call<int>("getCPULevelMax");
                }
                catch (Exception)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getCPULevelMax() failed!");
                }

                return maxCpuPerformanceLevel;
            }

            public int GetMaxGpuPerformanceLevel()
            {
                int maxGpuPerformanceLevel = -1;
                try
                {
                    maxGpuPerformanceLevel = s_GameSDK.Call<int>("getGPULevelMax");
                }
                catch (Exception)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getCPULevelMax() failed!");
                }

                return maxGpuPerformanceLevel;
            }

            public bool IsVariableRefreshRateSupported()
            {
                bool vrrSupported = false;
                try
                {
                    vrrSupported = s_GameSDK.Call<bool>("isGameSDKVariableRefreshRateSupported");
                    GameSDKLog.Debug("isGameSDKVariableRefreshRateSupported->{0}", vrrSupported);
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.isGameSDKVariableRefreshRateSupported() failed: " + x.Message);
                }

                return vrrSupported;
            }

            public int[] GetSupportedRefreshRates()
            {
                int[] result = null;
                try
                {
                    result = s_GameSDK.Call<int[]>("getSupportedRefreshRates");
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getSupportedRefreshRates() failed: " + x.Message);
                }

                return result != null ? result : new int[0];
            }

            public bool SetRefreshRate(int targetRefreshRate)
            {
                try
                {
                    s_GameSDK.Call("setRefreshRate", targetRefreshRate);
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.setRefreshRate() failed: " + x.Message);
                    return false;
                }
                return true;
            }

            public bool ResetRefreshRate()
            {
                try
                {
                    s_GameSDK.Call("resetRefreshRate");
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.resetRefreshRate() failed: " + x.Message);
                    return false;
                }
                return true;
            }

            public int GetCurrentRefreshRate()
            {
                int result = -1;
                try
                {
                    result = s_GameSDK.Call<int>("getCurrentRefreshRate");
                }
                catch (Exception x)
                {
                    GameSDKLog.Debug("[Exception] GameSDK.getCurrentRefreshRate() failed: " + x.Message);
                }
                return result;
            }
        }

        [Preserve]
        internal class VRRManager : IVariableRefreshRate
        {
            NativeApi m_Api;
            object m_RefreshRateChangedLock = new object();
            bool m_RefreshRateChanged;
            int[] m_SupportedRefreshRates = new int[0];
            int m_CurrentRefreshRate = -1;
            int m_LastSetRefreshRate = -1;

            private void UpdateRefreshRateInfo()
            {
                var supportedRefreshRates = m_Api.GetSupportedRefreshRates();
                if (settings.highSpeedVRR)
                {
                    m_SupportedRefreshRates = supportedRefreshRates;
                }
                else
                {
                    List<int> shrunkSupportedRefreshRates = new List<int>();
                    for (var i = 0; i < supportedRefreshRates.Length; ++i)
                    {
                        if (supportedRefreshRates[i] <= 60)
                            shrunkSupportedRefreshRates.Add(supportedRefreshRates[i]);
                    }
                    m_SupportedRefreshRates = shrunkSupportedRefreshRates.ToArray();
                }

                m_CurrentRefreshRate = m_Api.GetCurrentRefreshRate();
            }

            public VRRManager(NativeApi api)
            {
                m_Api = api;
                SetDefaultVRR();
                UpdateRefreshRateInfo();
            }

            // If HighSpeedVRR is not enabled we should not set over 60hz by default
            private void SetDefaultVRR()
            {
                if (settings.highSpeedVRR)
                    return;

                var index = Array.IndexOf(m_SupportedRefreshRates, 60);

                if (index != -1)
                {
                    SetRefreshRateByIndexInternal(index);
                }
            }

            public void Resume()
            {
                bool changed = false;

                var oldSupportedRefreshRates = m_SupportedRefreshRates;
                var oldRefreshRate = m_LastSetRefreshRate;

                UpdateRefreshRateInfo();

                if (m_CurrentRefreshRate != oldRefreshRate)
                    changed = true;
                else if (oldSupportedRefreshRates != m_SupportedRefreshRates)
                    changed = true;

                if (changed)
                {
                    lock (m_RefreshRateChangedLock)
                    {
                        m_RefreshRateChanged = true;
                    }
                }
            }

            public void Update()
            {
                bool refreshRateChanged = false;
                lock (m_RefreshRateChangedLock)
                {
                    refreshRateChanged = m_RefreshRateChanged;
                    m_RefreshRateChanged = false;
                }

                if (refreshRateChanged)
                {
                    UpdateRefreshRateInfo();

                    var index = Array.IndexOf(m_SupportedRefreshRates, m_LastSetRefreshRate);

                    if (index != -1)
                    {
                        SetRefreshRateByIndexInternal(index);
                    }
                    else if (index == -1 && m_LastSetRefreshRate != -1)
                    {
                        // Previous set refresh rate is not in available in the refreshrate list.
                        // Need to set 60Hz or lowest refresh rate possible.
                        // User sets 48Hz, but 48Hz is not on list anymore, because user changed Setting App - Display - Smooth option.
                        index = Array.IndexOf(m_SupportedRefreshRates, 60);

                        if (index != -1)
                            SetRefreshRateByIndexInternal(index);
                    }
                    RefreshRateChanged?.Invoke();
                }
            }

            public int[] SupportedRefreshRates { get { return m_SupportedRefreshRates; } }
            public int CurrentRefreshRate { get { return m_CurrentRefreshRate; } }

            public bool SetRefreshRateByIndex(int index)
            {
                // Refreshrate potentially set by user
                settings.automaticVRR = false;
                return SetRefreshRateByIndexInternal(index);
            }

            private bool SetRefreshRateByIndexInternal(int index)
            {
                if (index >= 0 && index < SupportedRefreshRates.Length)
                {
                    var refreshRateFromIndex = SupportedRefreshRates[index];
                    if (Application.targetFrameRate > 0 && index > 0 && SupportedRefreshRates[--index] > Application.targetFrameRate)
                    {
                        GameSDKLog.Debug("SetRefreshRateByIndex tries to set the refreshRateTarget {0} way higher than the targetFrameRate {1} which is not recommended due to temperature increase and unused performance.", refreshRateFromIndex, Application.targetFrameRate);
                    }
                    if (!settings.highSpeedVRR)
                    {
                        if (refreshRateFromIndex > 60)
                        {
                            GameSDKLog.Debug("High-Speed VRR is not enabled in the settings. Setting a refreshrate ({0}Hz) over 60Hz is not permitted due to temperature reasons.", refreshRateFromIndex);
                            return false;
                        }
                    }
                    if (m_Api.SetRefreshRate(refreshRateFromIndex))
                    {
                        m_CurrentRefreshRate = refreshRateFromIndex;
                        m_LastSetRefreshRate = refreshRateFromIndex;
                        return true;
                    }
                }
                return false;
            }

            public event VariableRefreshRateEventHandler RefreshRateChanged;

            public void OnRefreshRateChanged()
            {
                lock (m_RefreshRateChangedLock)
                {
                    m_RefreshRateChanged = true;
                }
            }
        }
        internal class AutoVariableRefreshRate
        {
            SamsungAndroidProviderSettings settings = SamsungAndroidProviderSettings.GetSettings();
            IVariableRefreshRate vrrManager;

            public AutoVariableRefreshRate(IVariableRefreshRate vrrManagerInstance)
            {
                vrrManager = vrrManagerInstance;
            }

            // Temperature checks of hardware are around 5sec and we don't need to check that often.
            float VrrUpdateTime = 1;
            int lastRefreshRateIndex = -1;

            public void UpdateAutoVRR()
            {
                VrrUpdateTime -= Time.unscaledDeltaTime;

                if (VrrUpdateTime <= 0)
                {
                    VrrUpdateTime = 1;

                    // targetFPS = 70 (in 48/60/96/120)-> vrr 96 never 120
                    // targetFPS = 40 (in 48/60/96/120)-> vrr 60 never 96
                    // targetFPS = 48/60/96/120 (in 48/60/96/120) -> vrr 48/60/96/12 never higher
                    // targetFPS = 70 (in 48/60)-> 60
                    var refreshRateIndex = vrrManager.SupportedRefreshRates.Length - 1;
                    // we look if a targetFrameRate is set, even in vsync mode were target framerate is ignored. Otherwise we use maximum framerate
                    if (Application.targetFrameRate > 0)
                    {
                        for (int i = 0; i < vrrManager.SupportedRefreshRates.Length; ++i)
                        {
                            if (Application.targetFrameRate > vrrManager.SupportedRefreshRates[i])
                            {
                                continue;
                            }
                            else
                            {
                                refreshRateIndex = i;
                                break;
                            }
                        }
                    }

                    if (lastRefreshRateIndex != refreshRateIndex)
                    {
                        lastRefreshRateIndex = refreshRateIndex;
                        vrrManager.SetRefreshRateByIndex(refreshRateIndex);
                        // automatic VRR gets disabled in SetRefreshRateByIndex and we want to ensure we still get updated.
                        settings.automaticVRR = true;
                    }
                }
            }
        }
    }
}
#endif
