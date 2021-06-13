using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVsZombiesStudio.I18n
{
    public static class LanguageManager
    {
        public static Dictionary<string, Language> LoadedLanguages => _languages;
        private static Dictionary<string, Language> _languages = new();

        public static Language CurrentLanguage { get; set; }

        public static Language LoadLanguage(string path)
        {
            var lines = File.ReadAllLines(path);
            var dictionary = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                var index = line.IndexOf('=');
                if (index != -1)
                {
                    dictionary.Add(line.Substring(0, index), line[(index + 1)..]);
                }
            }

            return new Language(Path.GetFileNameWithoutExtension(path), dictionary);
        }

        public static void EnumLanguages()
        {
            _languages.Clear();

            var directoryInfo = new DirectoryInfo("lang");

            if (!directoryInfo.Exists)
                directoryInfo.Create();

            foreach (var file in directoryInfo.GetFiles())
            {
                _languages.Add(Path.GetFileNameWithoutExtension(file.Name), LoadLanguage(file.FullName));
            }
        }
    }
}
