using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unity.Services.Core.Telemetry.Internal
{
    [Serializable]
    struct MetricsPayload
    {
        [JsonProperty("commonTags")]
        public Dictionary<string, string> CommonTags;

        [JsonProperty("metricsCommonTags")]
        public Dictionary<string, string> MetricsCommonTags;

        [JsonProperty("metrics")]
        public List<Metric> Metrics;
    }
}
