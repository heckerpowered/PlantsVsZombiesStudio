using PlantsVsZombiesStudio.I18n;

namespace PlantsVsZombiesStudio
{
    public static class StringExtension
    {
        public static string Query(this string key)
        {
            return LanguageManager.CurrentLanguage.Query(key);
        }
    }
}
