using System;
using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
    class MemoryCache
    {
        internal CachedPayload CachedPayload = new CachedPayload
        {
            Payload = new MetricsPayload
            {
                Metrics = new List<Metric>(),
            },
            TimeOfOccurenceTicks = 0,
        };

        public void SetCommonTags(
            IDictionary<string, string> commonTags, IDictionary<string, string> metricsCommonTags)
        {
            CachedPayload.Payload.CommonTags = new Dictionary<string, string>(commonTags);
            CachedPayload.Payload.MetricsCommonTags = new Dictionary<string, string>(metricsCommonTags);
        }

        public void Add(Metric metricEvent)
        {
            if (CachedPayload.TimeOfOccurenceTicks == 0)
            {
                CachedPayload.TimeOfOccurenceTicks = DateTime.UtcNow.Ticks;
            }

            CachedPayload.Payload.Metrics.Add(metricEvent);
        }

        public void ClearMetrics()
        {
            CachedPayload.TimeOfOccurenceTicks = 0;
            CachedPayload.Payload.Metrics.Clear();
        }
    }
}
