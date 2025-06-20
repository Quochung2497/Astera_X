using System.Threading.Tasks;

namespace Course.Services.SaveService
{
    public interface ISaveService
    {   
        Task SaveData<T>(string key, T value, ISavable self);
        Task<T> LoadData<T>(string key, T defaultValue, ISavable self);
        void DeleteData(string key);
        void DeleteAll();
    }
}

