using System.Collections.Generic;
using System.Windows;

namespace PlantsVsZombiesStudio.I18n
{
    public class Language
    {
        public Language(string languageName, Dictionary<string, string> dictionary)
        {
            LanguageName = languageName;
            Dictionary = dictionary;
        }

        public string LanguageName { get; }
        public IReadOnlyDictionary<string, string> Dictionary { get; }

        public string Query(string Key)
        {
            if (Dictionary.ContainsKey(Key))
                return Dictionary[Key];

            return Key;
        }

        public ResourceDictionary ToResourceDictionary()
        {
            var resourceDictionary = new ResourceDictionary();

            foreach (var item in Dictionary)
                resourceDictionary.Add(item.Key, item.Value);

            return resourceDictionary;
        }
    }
}
