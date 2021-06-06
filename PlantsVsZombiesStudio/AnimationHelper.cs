using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PlantsVsZombiesStudio
{
    public partial class MainWindow : Window
    {
        private Action<bool> _onDialogCloseAction = null;
        private void ShowNotice(string title, string text, bool showCancleButton = false, Action<bool> onClose = null)
        {
            TopDialogHost.ShowDialog(null);
            Storyboard Board = new()
            {
                Duration = TimeSpan.FromMilliseconds(300)
            };
            DoubleAnimation Animation = new()
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(Animation, new PropertyPath(OpacityProperty));
            Board.Children.Add(Animation);
            TextNoticeTitle.Text = title;
            TextNoticeInformation.Text = text;
            CardNotice.Visibility = Visibility.Visible;
            if (showCancleButton)
            {
                ButtonCancleDialog.Visibility = Visibility.Visible;
                ButtonCloseDialog.Content = "YES";
            }
            else
            {
                ButtonCancleDialog.Visibility = Visibility.Collapsed;
                ButtonCloseDialog.Content = "CLOSE";
            }
            _onDialogCloseAction = onClose;
            Board.Begin(CardNotice);
        }
        private void ProcessButtonAnimation(object sender, Action work)
        {
            if (sender is Button button)
            {
                if (button.Tag == null || !(bool)button.Tag)
                {
                    button.Tag = true;
                    double Width = button.Width;
                    object Content = button.Content;
                    button.Content = FindResource("CircularProgressBar");
                    Storyboard Board = new();
                    DoubleAnimation Animation = new()
                    {
                        To = 60,
                        EasingFunction = new CubicEase
                        {
                            EasingMode = EasingMode.EaseOut
                        },
                        Duration = TimeSpan.FromMilliseconds(500.0)
                    };
                    Storyboard.SetTargetProperty(Animation, new PropertyPath("Width"));
                    Board.Children.Add(Animation);
                    Board.Completed += delegate (object s, EventArgs Event)
                    {
                        work();
                        Animation.To = Width;
                        Board = new Storyboard();
                        Board.Completed += delegate (object ss, EventArgs ee)
                        {
                            button.Tag = false;
                        };
                        Board.Children.Add(Animation);
                        Board.Begin(button);
                        button.Content = Content;
                    };
                    Board.Begin(button);
                }
            }
        }
        private void CloseCurrentDialog()
        {
            Storyboard Board = new()
            {
                Duration = TimeSpan.FromMilliseconds(300.0)
            };
            DoubleAnimation Animation = new()
            {
                From = 100,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300.0)
            };
            Storyboard.SetTargetProperty(Animation, new PropertyPath(OpacityProperty));
            Board.Children.Add(Animation);
            Board.Completed += Board_Completed;
            Board.Begin(CardNotice);
            TopDialogHost.CurrentSession?.Close();
        }
        private void Board_Completed(object sender, EventArgs e)
        {
            CardNotice.Visibility = Visibility.Collapsed;
        }
    }
}
