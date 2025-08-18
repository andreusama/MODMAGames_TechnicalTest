using System.Threading.Tasks;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public interface IPlatformManager<T>
    {
        public Task Initialize(T config);
    }

}
