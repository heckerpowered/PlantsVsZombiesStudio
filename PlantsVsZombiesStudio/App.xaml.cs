using System.Windows;

namespace PlantsVsZombiesStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            PlantsVsZombiesStudio.MainWindow.InitializeLanguage();
        }
    }
}
