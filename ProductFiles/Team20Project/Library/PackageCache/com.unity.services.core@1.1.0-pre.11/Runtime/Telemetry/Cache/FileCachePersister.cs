using System;
using System.IO;
using Newtonsoft.Json;
using Unity.Services.Core.Internal;
using UnityEngine;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Unity.Services.Core.Telemetry.Internal
{
    class FileCachePersister
    {
        const string k_FileNameFormat = "{0}_CachedMetrics";

        public string FilePath { get; }

        public bool CanPersist { get; } = IsFilePersistenceAvailable();

        public FileCachePersister([NotNull] string fileId)
        {
            FilePath = Path.Combine(Application.persistentDataPath, string.Format(k_FileNameFormat, fileId));
        }

        static bool IsFilePersistenceAvailable()
        {
            // Consoles are not supported yet.
            return !Application.isConsolePlatform
                && !string.IsNullOrEmpty(Application.persistentDataPath);
        }

        public void Persist(CachedPayload cache)
        {
            var serializedEvents = JsonConvert.SerializeObject(cache);
            File.WriteAllText(FilePath, serializedEvents);
        }

        public bool TryFetch(out CachedPayload persistedCache)
        {
            if (!File.Exists(FilePath))
            {
                persistedCache = default;
                return false;
            }

            try
            {
                var rawPersistedCache = File.ReadAllText(FilePath);
                persistedCache = JsonConvert.DeserializeObject<CachedPayload>(rawPersistedCache);
                return true;
            }
            catch (Exception e)
            {
                CoreLogger.LogException(e);
                persistedCache = default;
                return false;
            }
        }

        public void Delete() => File.Delete(FilePath);
    }
}
