using System.Runtime.CompilerServices;

// Access to AsyncOperation (deprecated)
[assembly: InternalsVisibleTo("Unity.Services.Authentication")]
[assembly: InternalsVisibleTo("Unity.Services.Authentication.Editor")]
#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("Unity.Services.Authentication.Tests")]
[assembly: InternalsVisibleTo("Unity.Services.Authentication.EditorTests")]
#endif

// Required for access to Networking API
[assembly: InternalsVisibleTo("Unity.Services.Core.Editor")]
[assembly: InternalsVisibleTo("Unity.Services.Core.Networking")]

// Required for CoreLogger access
[assembly: InternalsVisibleTo("Unity.Services.Core.Configuration")]
[assembly: InternalsVisibleTo("Unity.Services.Core.Configuration.Editor")]
[assembly: InternalsVisibleTo("Unity.Services.Core.Registration")]
[assembly: InternalsVisibleTo("Unity.Services.Core.Scheduler")]
[assembly: InternalsVisibleTo("Unity.Services.Core.Telemetry")]

// Test assemblies
#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("Unity.Services.Core.Tests")]
[assembly: InternalsVisibleTo("Unity.Services.Core.TestUtils")]

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Unity.Services.Core.EditorTests")]
#endif
