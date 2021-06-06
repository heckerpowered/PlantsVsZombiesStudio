using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZClass;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlantsVsZombiesStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsGameExist => PVZ.Game?.HasExited == false;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void ButtonModifyMoney_Click(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
              {
                  if (IsGameExist)
                      if (int.TryParse(TextBoxMoney.Text, out int Money))
                          PVZ.SaveData.Money = Money;
                      else if (CheckBoxForceCast.IsChecked.GetValueOrDefault())
                          PVZ.SaveData.Money = TextBoxMoney.Text.GetHashCode();
                      else
                          ShowNotice("ERROR", $"Can not parse \"{TextBoxMoney.Text}\" to an integer", false, null);
                  else
                      ShowNotice("GAME NOT FOUND", "Find game before modifying money.", false, null);
              });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ShowNotice("QUIT", "Are you sure want to quit?", true, delegate (bool Quit)
            {
                if (Quit)
                    Application.Current.Shutdown();
            });
            e.Cancel = true;
        }
        private void ButtonCancleDialog_Click(object sender, RoutedEventArgs e)
        {
            CloseCurrentDialog();
            _onDialogCloseAction?.Invoke(false);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
            {
                if (IsGameExist)
                    ShowNotice("GAME IS ALREADY FOUND", "The game process is already found.", false, null);
                else if (PVZ.RunGame())
                    Snack.MessageQueue.Enqueue("GAME FOUND");
                else
                    ShowNotice("GAME NOT FOUND", "Can not found the game process.", false, null);
            });
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
            {
                if (PVZ.BaseAddress == 0)
                    ShowNotice("NOT IN GAME", "Join a game before modifying sun.", false, null);
                else if (int.TryParse(TextBoxSun.Text, out int Sun))
                    PVZ.Sun = Sun;
                else if (CheckBoxForceCast.IsChecked.GetValueOrDefault())
                    PVZ.Sun = TextBoxSun.Text.GetHashCode();
                else
                    ShowNotice("ERROR", $"Can not parse \"{TextBoxSun.Text}\" to an integer", false, null);
            });
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CloseCurrentDialog();
            _onDialogCloseAction?.Invoke(true);
        }
    }
}
