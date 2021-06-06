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
            Storyboard board = new()
            {
                Duration = TimeSpan.FromMilliseconds(300)
            };
            DoubleAnimation animation = new()
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            board.Children.Add(animation);
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
            board.Begin(CardNotice);
        }
        private void ProcessButtonAnimation(object sender, Action work)
        {
            if (sender is Button button)
            {
                if (button.Tag == null || !(bool)button.Tag)
                {
                    button.Tag = true;
                    double width = button.Width;
                    object content = button.Content;
                    button.Content = FindResource("CircularProgressBar");
                    Storyboard board = new();
                    DoubleAnimation animation = new()
                    {
                        To = 60,
                        EasingFunction = new CubicEase
                        {
                            EasingMode = EasingMode.EaseOut
                        },
                        Duration = TimeSpan.FromMilliseconds(500.0)
                    };
                    Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                    board.Children.Add(animation);
                    board.Completed += delegate (object s, EventArgs Event)
                    {
                        work();
                        animation.To = width;
                        board = new Storyboard();
                        board.Completed += delegate (object ss, EventArgs ee)
                        {
                            button.Tag = false;
                        };
                        board.Children.Add(animation);
                        board.Begin(button);
                        button.Content = content;
                    };
                    board.Begin(button);
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
