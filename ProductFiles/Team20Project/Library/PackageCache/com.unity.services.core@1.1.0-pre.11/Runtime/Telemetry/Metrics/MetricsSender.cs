using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Unity.Services.Core.Internal;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Unity.Services.Core.Telemetry.Internal
{
    class MetricsSender
    {
        public string MetricsUrl { get; }

        public MetricsSender([NotNull] string targetUrl, [NotNull] string servicePath)
        {
            MetricsUrl = $"{targetUrl}/{servicePath}";
        }

        public Task SendAsync(MetricsPayload payload)
        {
            var completionSource = new TaskCompletionSource<object>();

            try
            {
                var request = CreateRequest(payload);
                request.SendWebRequest()
                    .completed += OnSendCacheCompleted;
            }
            catch (Exception e)
            {
                completionSource.TrySetException(e);
            }

            return completionSource.Task;

            void OnSendCacheCompleted(UnityEngine.AsyncOperation operation)
            {
                var webRequest = ((UnityWebRequestAsyncOperation)operation).webRequest;

                if (webRequest.HasSucceeded())
                {
                    CoreLogger.LogVerbose("Metrics sent successfully");
                    completionSource.SetResult(null);
                }
                else
                {
                    // TODO: Exponential backoff strat with scheduler on failure

                    completionSource.TrySetException(
                        new Exception($"Error: {webRequest.error}\nBody: {webRequest.downloadHandler.text}"));
                }
            }
        }

        internal UnityWebRequest CreateRequest(MetricsPayload payload)
        {
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var serializedPayload = Encoding.UTF8.GetBytes(jsonPayload);
            var request = new UnityWebRequest(MetricsUrl, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(serializedPayload)
                {
                    contentType = UnityWebRequestUtils.JsonContentType,
                },
            };
            request.SetRequestHeader("Content-Type", UnityWebRequestUtils.JsonContentType);
            return request;
        }
    }
}
