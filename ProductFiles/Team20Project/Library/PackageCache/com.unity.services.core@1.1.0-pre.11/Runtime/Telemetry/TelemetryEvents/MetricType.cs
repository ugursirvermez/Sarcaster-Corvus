using System.Runtime.Serialization;

namespace Unity.Services.Core.Telemetry.Internal
{
    enum MetricType
    {
        [EnumMember(Value = "OBSERVER")]
        Observer = 0,
        [EnumMember(Value = "COUNTER")]
        Counter = 1,
        [EnumMember(Value = "TIMED")]
        Timed = 2,
    }
}
