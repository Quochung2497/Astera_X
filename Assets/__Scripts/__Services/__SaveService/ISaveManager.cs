using System.Threading.Tasks;

namespace Course.Services.SaveService
{
    public interface ISaveManager
    {
        /// <summary>
        /// Save an arbitrary value under the given key (async).
        /// </summary>
        Task SaveData<T>(string key, T value, ISavable self);

        /// <summary>
        /// Load a value by key (async), or return the provided default if missing or corrupt.
        /// </summary>
        Task<T> LoadData<T>(string key, T defaultValue, ISavable self);

        /// <summary>
        /// Remove a single entry from save.
        /// </summary>
        void DeleteData(string key);

        /// <summary>
        /// Wipe all saved data.
        /// </summary>
        void DeleteAll();
    }
}