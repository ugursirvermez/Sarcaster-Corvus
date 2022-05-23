using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Mediation
{
    class MediationServiceInitializer : IInitializablePackage
    {
        internal const string keyGameId = "com.unity.ads.game-id";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new MediationServiceInitializer())
                .DependsOn<IInstallationId>()
                .DependsOn<IProjectConfiguration>();
        }

        public async Task Initialize(CoreRegistry registry)
        {
            IInstallationId       installationId = registry.GetServiceComponent<IInstallationId>();
            IProjectConfiguration projectConfig  = registry.GetServiceComponent<IProjectConfiguration>();

            await Initialize(installationId, projectConfig);
        }

        internal async Task Initialize(IInstallationId installationId, IProjectConfiguration projectConfiguration)
        {
            string installId = installationId.GetOrCreateIdentifier();
            string gameId    = projectConfiguration.GetString(keyGameId);

            if (!Application.isEditor && string.IsNullOrEmpty(gameId))
            {
                Debug.LogError("No gameId was set for the mediation service. Please make sure your project is linked to the dashboard when you build your application.");
            }

            await MediationService.Initialize(gameId, installId);
        }
    }
}
