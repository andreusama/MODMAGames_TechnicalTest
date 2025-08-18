using System.Threading.Tasks;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Multiplatform.Android
{
    public class AndroidManager : PersistentSingleton<AndroidManager>, IPlatformManager<AndroidConfig>
    {
        public async Task Initialize(AndroidConfig config)
        {
            PlatformServices androidServices = new PlatformServices();

            androidServices.Storage = PlatformManager.CreateStorageService(config, new AndroidStorage());
            androidServices.DownloadableContentFinder = new AndroidDownloadableContent();

            PlatformManager.Instance.SetPlatformServices(androidServices);

            await Task.CompletedTask;
        }
    }
}
