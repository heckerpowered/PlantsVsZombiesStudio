using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;

namespace PlantsVsZombiesStudio
{
    public partial class MainWindow : Window
    {
        private Action<bool> _onDialogCloseAction = null;
        private Action _nextAction;
        private bool _isOpen;

        private readonly Storyboard showStoryboard = new()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };
        private readonly DoubleAnimation showAnimation = new()
        {
            From = 0,
            To = 100,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        private readonly Storyboard closeStoryboard = new()
        {
            Duration = TimeSpan.FromMilliseconds(300)
        };
        private readonly DoubleAnimation closeAnimation = new()
        {
            From = 100,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        private void InitializeAnimation()
        {
            Storyboard.SetTargetProperty(showAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(closeAnimation, new PropertyPath(OpacityProperty));
            showStoryboard.Children.Add(showAnimation);
            closeStoryboard.Children.Add(closeAnimation);
            closeStoryboard.Completed += Board_Completed;
        }
        internal void ShowNotice(string title, object content, bool showCancleButton = false, Action<bool> onClose = null)
        {
            Dispatcher.Invoke(delegate
            {
                if (CardNotice.Tag is bool b && b)
                {
                    _nextAction = delegate { ShowNotice(title, content, showCancleButton, onClose); };
                    return;
                }

                TextNoticeTitle.Text = title;
                CardContent.Content = content;
                CardNotice.Visibility = Visibility.Visible;
                if (showCancleButton)
                {
                    ButtonCancleDialog.Visibility = Visibility.Visible;
                    ButtonCloseDialog.Content = Query("Yes");
                }
                else
                {
                    ButtonCancleDialog.Visibility = Visibility.Collapsed;
                    ButtonCloseDialog.Content = Query("Close");
                }
                _onDialogCloseAction = onClose;
                CardNotice.Tag = true;
                _isOpen = true;
                TopDialogHost.IsOpen = true;
                showStoryboard.Begin(CardNotice);
            });
        }
        private ProgressBar AllocateCircularProgressBar(bool isIndeterminate = true)
        {
            return new ProgressBar
            {
                Style = (Style)FindResource("MaterialDesignCircularProgressBar"),
                Width = 20,
                Height = 20,
                IsIndeterminate = isIndeterminate
            };
        }
        internal void ProcessButtonAnimation(object sender, Action work, bool Animation = true)
        {
            if (sender is Button button)
            {
                if (button.Tag == null || !(bool)button.Tag)
                {
                    button.Tag = true;
                    var width = button.ActualWidth;
                    button.Width = width;
                    object content = button.Content;
                    button.Content = AllocateCircularProgressBar();
                    var board = new Storyboard();
                    var animation = new DoubleAnimation()
                    {
                        To = Animation ? 60 : width,
                        EasingFunction = new CubicEase
                        {
                            EasingMode = EasingMode.EaseOut
                        },
                        Duration = TimeSpan.FromMilliseconds(500.0)
                    };
                    Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                    board.Children.Add(animation);
                    board.Completed += async delegate (object s, EventArgs Event)
                    {
                        try
                        {
                            await Task.Factory.StartNew(work);
                        }
                        catch (Exception e)
                        {
                            UnhandledException(e);
                        }
                        animation.To = width;
                        board = new Storyboard();
                        board.Completed += delegate (object ss, EventArgs ee)
                        {
                            button.Tag = false;
                        };
                        board.Children.Add(animation);
                        board.Begin(button);
                        button.Content = content;
                        TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                    };
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                    board.Begin(button);
                }
            }
        }

        public void ProcessButtonAnimationWithContent(object sender,string content, Action work, bool Animation = true)
        {
            if (sender is Button button)
            {
                if (button.Tag == null || !(bool)button.Tag)
                {
                    button.Tag = true;
                    var width = button.ActualWidth;
                    button.Width = width;
                    var originalContent = button.Content;
                    button.Content = content;
#pragma warning disable CS0618 // Type or member is obsolete
                    var formattedText = new FormattedText(content, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Brushes.Black);
#pragma warning restore CS0618 // Type or member is obsolete
                    var board = new Storyboard();
                    var animation = new DoubleAnimation()
                    {
                        To = Animation ? formattedText.Width : width,
                        EasingFunction = new CubicEase
                        {
                            EasingMode = EasingMode.EaseOut
                        },
                        Duration = TimeSpan.FromMilliseconds(500.0)
                    };
                    Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                    board.Children.Add(animation);
                    board.Completed += async delegate (object s, EventArgs Event)
                    {
                        try
                        {
                            await Task.Factory.StartNew(work);
                        }
                        catch (Exception e)
                        {
                            UnhandledException(e);
                        }
                        animation.To = width;
                        board = new Storyboard();
                        board.Completed += delegate (object ss, EventArgs ee)
                        {
                            button.Tag = false;
                        };
                        board.Children.Add(animation);
                        board.Begin(button);
                        button.Content = originalContent;
                        TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                    };
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                    board.Begin(button);
                }
            }
        }
        private void CloseCurrentDialog()
        {
            Dispatcher.Invoke(delegate
            {
                _isOpen = false;
                TopDialogHost.IsOpen = false;
                closeStoryboard.Begin(CardNotice);
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            });
        }
        private void Board_Completed(object sender, EventArgs e)
        {
            TopDialogHost.IsOpen = false;
            CardNotice.Visibility = Visibility.Collapsed;

            CardNotice.Tag = false;
            if (_nextAction != null)
            {
                _nextAction();
                _nextAction = null;
            }
        }

        private void EnqueueMessage(string message)
        {
            Dispatcher.Invoke(delegate
            {
                Snack.MessageQueue.Enqueue(message);
            });
        }
    }
}
