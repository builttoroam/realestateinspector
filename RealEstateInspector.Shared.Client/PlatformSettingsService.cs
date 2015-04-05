using System.Collections.Generic;
using System.IO;
#if NETFX_CORE
using Windows.Storage;
#endif
#if SILVERLIGHT
using System.IO.IsolatedStorage;
#endif
using System.Threading.Tasks;
using BuiltToRoam;
using Newtonsoft.Json;
using PCLStorage;
using RealEstateInspector.Core;

namespace RealEstateInspector.Shared.Client
{
    public class PlatformSettingsService : ISettingsService
    {

#if !(NETFX_CORE ||SILVERLIGHT)
        private IDictionary<string, string> Settings { get; set; }
#endif

        private bool Initialized { get; set; }
#pragma warning disable 1998 // Async required for local storage settings model
        private async Task Initialize()
#pragma warning restore 1998
        {
            if (!Initialized)
            {

#if !(NETFX_CORE ||SILVERLIGHT)
                var file =
                    await
                        FileSystem.Current.LocalStorage.GetFileAsync("settings.json");
                using (var stream = await file.OpenAsync(PCLStorage.FileAccess.Read))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        Settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    }
                }
                if (Settings == null)
                {
                    Settings=new Dictionary<string, string>();
                }
#endif

                Initialized = true;
            }
        }


        public async Task<string> Load(string key)
        {
            await Initialize();
#if NETFX_CORE
            object data = null;
            var dict = ApplicationData.Current.LocalSettings.Values;
#elif SILVERLIGHT
            string data = null;
            var dict = IsolatedStorageSettings.ApplicationSettings;
#else
            string data = null;
            var dict = Settings;
#endif
            dict.TryGetValue(key, out data);
            return data +"";
        }

        public async Task Save(string key, string value)
        {
            await Initialize();
#if NETFX_CORE
            var dict = ApplicationData.Current.LocalSettings.Values;
#elif SILVERLIGHT
            var dict = IsolatedStorageSettings.ApplicationSettings;
#else
            var dict = Settings;
#endif
            dict[key]=value;   
#if SILVERLIGHT
            dict.Save();
#endif
        }
    }
}