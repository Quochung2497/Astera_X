using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Course.Services.SaveService
{
    internal class SaveService : ISaveService
    {
        private readonly string _filePath;
        private Dictionary<string,string> _store;

       public SaveService()
        {
            _filePath = Path.Combine(Application.persistentDataPath, "AsteraX.save");
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    _store = JsonUtility.FromJson<Wrapper>(json)
                              ?.ToDictionary()
                           ?? new Dictionary<string,string>();
                }
                catch
                {
                    _store = new Dictionary<string,string>();
                }
            }
            else
            {
                _store = new Dictionary<string,string>();
            }
        }

        // ———— Sync helpers ————

        void SaveDataSync<T>(string key, T value)
        {
            string json = typeof(T).IsPrimitive
                ? Convert.ToString(value, CultureInfo.InvariantCulture)
                : JsonUtility.ToJson(value);

            _store[key] = json;
            Flush();
        }

        T LoadDataSync<T>(string key, T defaultValue)
        {
            if (!_store.TryGetValue(key, out var json))
                return defaultValue;

            try
            {
                if (typeof(T).IsPrimitive)
                {
                    // primitives come back as strings, so convert
                    return (T)Convert.ChangeType(json, typeof(T), CultureInfo.InvariantCulture);
                }
                else
                {
                    // for complex types use JsonUtility—but if it ever returns null, we still fall back
                    var deserialized = JsonUtility.FromJson<T>(json);
                    return deserialized != null
                        ? deserialized
                        : defaultValue;
                }
            }
            catch
            {
                // on any parse error, just give them their default back
                return defaultValue;
            }
        }

        void Flush()
        {
            var wrapper = new Wrapper(_store);
            var json    = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(_filePath, json);
            // Debug.Log($"[SaveService] Flushed {_store.Count} entries to {_filePath}");
        }

        // ———— Async interface impl ————

        public async Task SaveData<T>(string key, T value, ISavable self)
        {
            SaveDataSync(key, value);
            await Task.CompletedTask;
        }

        public async Task<T> LoadData<T>(string key, T defaultValue, ISavable self)
        {
            var result = LoadDataSync(key, defaultValue);
            return await Task.FromResult(result);
        }

        public void DeleteData(string key)
        {
            if (_store.Remove(key))
                Flush();
        }

        public void DeleteAll()
        {
            _store.Clear();
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }
    }
    
    
    [Serializable]
    class Wrapper
    {
        public List<string> keys;
        public List<string> values;

        public Wrapper(Dictionary<string,string> dict)
        {
            keys   = new List<string>(dict.Keys);
            values = new List<string>(dict.Values);
        }

        public Dictionary<string,string> ToDictionary()
        {
            var d = new Dictionary<string,string>();
            for (int i = 0; i < keys.Count; i++)
                d[keys[i]] = values[i];
            return d;
        }
    }
}