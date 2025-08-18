using System.Threading.Tasks;

namespace PetoonsStudio.PSEngine.Framework
{
    public interface IStorage
    {
        Task<bool> SaveExists(string fileName, string folderName);
        Task<bool> Save(object saveObject, string fileName, string folderName);
        Task<T> Load<T>(string fileName, string folderName);
        Task<bool> DeleteSave(string fileName, string folderName);
    }
}