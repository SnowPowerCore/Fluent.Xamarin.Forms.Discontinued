using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentSkiaSharpControls.Services.Utils.Settings
{
    public interface ISettingsService
    {
        string ProjectName { get; }

        string SupportNumber { get; }

        string SupportEmail { get; }

        string DefaultApiUrl { get; }

        string AppCenterAndroidKey { get; }

        string AppCenteriOSKey { get; }

        bool ContainsKey(string key);

        T GetValueOrDefault<T>(string key, T defaultValue = default, bool deserialize = false);

        List<T> GetValuesOrDefaults<T>(string[] keys, T defaultValue = default, bool deserialize = false);

        T GetApplicationResourceOrDefault<T>(string key, T defaultValue = default);

        Task AddOrUpdateValueAsync<T>(string key, T value, bool serialize = false);

        Task AddOrUpdateValuesAsync<T>(Dictionary<string, T> keyValues, bool serialize = false);

        Task RemoveValueAsync(string key);

        Task RemoveValuesAsync(string[] keys);
    }
}