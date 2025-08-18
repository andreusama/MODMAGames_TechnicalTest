using System.Threading.Tasks;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Multiplatform.Standalone
{
    public class StandaloneManager : PersistentSingleton<StandaloneManager>, IPlatformManager<StandaloneConfig>
    {
        public async Task Initialize(StandaloneConfig config)
        {
            PlatformServices standaloneServices = new PlatformServices();

            standaloneServices.Storage = new StandaloneStorage();
            standaloneServices.Achievements = new StandaloneAchievementUnlocker();
            standaloneServices.DownloadableContentFinder = new StandaloneDownloadableContent();

            PlatformManager.Instance.SetPlatformServices(standaloneServices);

            await Task.CompletedTask;
        }
    }
}
