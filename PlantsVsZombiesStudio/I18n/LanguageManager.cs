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
    }
}
