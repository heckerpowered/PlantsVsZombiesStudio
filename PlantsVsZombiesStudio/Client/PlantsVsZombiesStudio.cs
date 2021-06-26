using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlantsVsZombiesStudio.I18n;

namespace PlantsVsZombiesStudio.Client
{
    public class PlantsVsZombiesStudio
    {
        private readonly PlantsVsZombiesStudioSession plantsVsZombiesStudioSession;
        private static PlantsVsZombiesStudio instance;

        public static PlantsVsZombiesStudio Instance => instance;

        public Logger Logger => logger;

        private Logger logger;
        public PlantsVsZombiesStudio(PlantsVsZombiesStudioSession plantsVsZombiesStudioSession)
        {
            this.plantsVsZombiesStudioSession = plantsVsZombiesStudioSession;
            instance = this;
            logger = new();
        }


        public static void ChangeLanguage(string languageName)
        {
            if (LanguageManager.CurrentLanguage.LanguageName != languageName)
            {
                LanguageManager.CurrentLanguage = LanguageManager.GetLanguageByName(languageName);
                LanguageManager.CurrentLanguage.SetAsDefaultLanguage();
            }
        }
    }
}
