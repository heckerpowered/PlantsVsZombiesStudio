using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVsZombiesStudio.Setting
{
    public class Settings
    {
        public const string Path = "settings.json";

        private static readonly Dictionary<string, Settings> _settingsDictionary = new();
        public Settings(string name, object value)
        {
            Name = name;
            Value = value;
            _settingsDictionary.Add(Name, this);
        }

        protected virtual SettingType Type { get; }
        private string Name { get; }
        private object Value { get; set; }

        private static bool _initializated = false;

        public static T Query<T>(string name) => (T)_settingsDictionary[name].Value;

        public static void Put(string name, object value) => _settingsDictionary[name].Value = value;

        public static void InitializateSettings()
        {
            if (_initializated)
                return;

            _initializated = true;

            if (!File.Exists(Path))
                File.Create(Path).Close();

            if (LoadSettings() == null)
            {
                _ = new Settings("evaluator", false);

                SaveSettings();
            }
        }

        public static void SaveSettings() => File.WriteAllText(Path, JsonConvert.SerializeObject(_settingsDictionary.Values));

        public static IEnumerable<Settings> LoadSettings() => JsonConvert.DeserializeObject<IEnumerable<Settings>>(File.ReadAllText(Path));
    }
}
