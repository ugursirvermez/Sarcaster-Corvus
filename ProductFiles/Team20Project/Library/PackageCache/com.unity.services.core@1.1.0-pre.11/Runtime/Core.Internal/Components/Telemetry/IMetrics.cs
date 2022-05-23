using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
    /// <summary>
    /// Object used to send metrics event to the backend.
    /// </summary>
    interface IMetrics
    {
        /// <summary>
        /// All tags sent along with any events sent by this sender.
        /// </summary>
        IDictionary<string, string> CommonTags { get; }

        /// <summary>
        /// Send a metric that can arbitrarily go up or down to the telemetry service.
        /// </summary>
        /// <param name="name">
        /// Name of the event.
        /// </param>
        /// <param name="value">
        /// Value of the metric.
        /// </param>
        /// <param name="tags">
        /// Event tags.
        /// </param>
        void SendMeasuredMetric(string name, double value = 0, IDictionary<string, string> tags = null);

        /// <summary>
        /// Send a metric that lasts over time to the telemetry service.
        /// </summary>
        /// <param name="name">
        /// Name of the event.
        /// </param>
        /// <param name="time">
        /// Duration of the operation the event is tracking.
        /// </param>
        /// <param name="tags">
        /// Event tags.
        /// </param>
        void SendTimedMetric(string name, double time, IDictionary<string, string> tags = null);

        /// <summary>
        /// Send a metric that can only be incremented to the telemetry service.
        /// </summary>
        /// <param name="name">
        /// Name of the event.
        /// </param>
        /// <param name="value">
        /// Value of the metric.
        /// </param>
        /// <param name="tags">
        /// Event tags.
        /// </param>
        void SendCountedMetric(string name, double value = 1, IDictionary<string, string> tags = null);
    }
}
