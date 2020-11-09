using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FluentSkiaSharpControls.Services.Utils.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Fields
        private IConfiguration Configuration { get; }
        #endregion

        #region Properties
        public string ProjectName => Configuration["ProjectName"];

        public string SupportNumber => Configuration["SupportNumber"];

        public string SupportEmail => Configuration["SupportEmail"];

        public string DefaultApiUrl => Configuration["DefaultApiUrl"];

        public string AppCenterAndroidKey => Configuration["AppCenterAndroidKey"];

        public string AppCenteriOSKey => Configuration["AppCenteriOSKey"];
        #endregion

        public SettingsService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Methods
        public bool ContainsKey(string key) =>
            Application.Current.Properties.ContainsKey(key);

        public Task AddOrUpdateValueAsync<T>(string key, T value, bool serialize = false)
        {
            if (serialize)
                Application.Current.Properties[key] = JsonConvert.SerializeObject(value);
            else Application.Current.Properties[key] = value;

            try
            {
                return Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
                return Task.CompletedTask;
            }
        }

        public Task AddOrUpdateValuesAsync<T>(Dictionary<string, T> keyValues, bool serialize = false)
        {
            if (keyValues == null) return Task.CompletedTask;

            foreach (var item in keyValues)
            {
                var key = item.Key;
                var value = item.Value;

                if (serialize)
                    Application.Current.Properties[key] = JsonConvert.SerializeObject(value);
                else Application.Current.Properties[key] = value;
            }

            try
            {
                return Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unable to save or update values within this dict" + keyValues,
                    " Message: " + ex.Message);
                return Task.CompletedTask;
            }
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default, bool deserialize = false)
        {
            T value = defaultValue;
            if (Application.Current.Properties.ContainsKey(key))
            {
                var data = Application.Current.Properties[key];
                if (deserialize) value = JsonConvert.DeserializeObject<T>((string)data);
                else value = (T)data;
            }
            return value;
        }

        public T GetApplicationResourceOrDefault<T>(string key, T defaultValue = default)
        {
            T value = defaultValue;
            if (Application.Current.Resources.ContainsKey(key))
                value = (T)Application.Current.Resources[key];
            return null != value ? value : defaultValue;
        }

        public List<T> GetValuesOrDefaults<T>(string[] keys, T defaultValue = default, bool deserialize = false)
        {
            var list = new List<T>();
            for (var i = 0; i < keys.Length; i++)
                list.Add(GetValueOrDefault(keys[i], defaultValue, deserialize));
            return list;
        }

        public Task RemoveValueAsync(string key)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                Application.Current.Properties.Remove(key);

                try
                {
                    return Application.Current.SavePropertiesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }

        public Task RemoveValuesAsync(string[] keys)
        {
            if (keys == null) return Task.CompletedTask;

            foreach (var item in keys)
            {
                if (Application.Current.Properties.ContainsKey(item))
                    Application.Current.Properties.Remove(item);
            }

            try
            {
                return Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unable to save or update values within this list " + keys,
                    " Message: " + ex.Message);
                return Task.CompletedTask;
            }
        }
        #endregion
    }
}