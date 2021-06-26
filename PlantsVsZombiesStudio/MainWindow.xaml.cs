using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PlantsVsZombiesStudio.Client;
using System.Windows.Shell;

using Jvav.Binding;
using Jvav.Syntax;

using PlantsVsZombiesStudio.I18n;
using PlantsVsZombiesStudio.Setting;

using PVZClass;

namespace PlantsVsZombiesStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public static bool IsGameExist => PVZ.Game?.HasExited == false;
        public static bool NeedToReload;

        public MainWindow()
        {
            DateTime time = DateTime.Now; 
            Client.PlantsVsZombiesStudio client = new(new (this));
            client.Logger.WriteLine("Constructing PVZSTUDIO...");
            InitializeComponent();
            InitializeAnimation();
            if (LanguageManager.CurrentLanguage.Dictionary.ContainsKey("#window_title"))
            {
                Title = Query("#window_title");
            }
            client.Logger.WriteLine($"Constructing PVZSTUDIO ≤ {(int)(DateTime.Now - time).TotalMilliseconds}ms elapsed");
        }

        private SoundPlayer audioButton;
        private Client.PlantsVsZombiesStudio Instance => Client.PlantsVsZombiesStudio.Instance;
        public static void InitializeLanguage()
        {
            Settings.InitializateSettings();
            LanguageManager.EnumLanguages();
            LanguageManager.CurrentLanguage = LanguageManager.LoadedLanguages[Settings.Query<string>("language")];
            NeedToReload = true;
        }

        private void InitializeBorders()
        {
            BorderTextBoxSun.Width = BorderButtonModifySun.ActualWidth;
            BorderButtonModifySun.Width = BorderButtonModifySun.ActualWidth;
            TextBoxMoney.Width = ButtonModifyMoney.ActualWidth;
        }

        public static string Query(string key)
        {
            return LanguageManager.CurrentLanguage.Query(key);
        }

        [SuppressMessage("Style", "IDE0018:Inline variable declaration", Justification = "<Pending>")]
        private void ButtonModifyMoney_Click(object sender, RoutedEventArgs e)
        {
            int money;
            ProcessButtonAnimation(sender, delegate
              {
                  if (IsGameExist)
                  {
                      if (int.TryParse(TextBoxMoney.Text, out money))
                      {
                          PVZ.SaveData.Money = money;
                      }
                      else if (CheckBoxForceCast.IsChecked.GetValueOrDefault())
                      {
                          SyntaxTree syntaxTree = SyntaxTree.Parse(TextBoxMoney.Text);
                          Binder binder = new();
                          BoundExpression boundExpression = binder.BindExpression(syntaxTree.Root);
                          string[] diagnostics = binder.Diagnostic.ToArray();
                          StringBuilder builder = new();
                          if (diagnostics.Any())
                          {
                              foreach (string diagnostic in diagnostics)
                              {
                                  _ = builder.AppendLine(diagnostic);
                              }
                              ShowNotice(Query("evaluator.error"), builder.ToString(), false, null);
                          }
                          else
                          {
                              Evaluator evaluator = new Evaluator(boundExpression);
                              object result = evaluator.Evaluate();
                              if (result is int money)
                              {
                                  PVZ.SaveData.Money = money;
                              }
                              else
                              {
                                  ShowNotice(Query("error"), string.Format(Query("error.evaluator"), result));
                              }
                          }
                      }
                      else
                      {
                          ShowNotice(Query("error"), string.Format(Query("error.parse"), TextBoxMoney.Text), false, null);
                      }
                  }
                  else
                  {
                      ShowNotice(Query("game.not_found"), Query("game.find.before_modify_money"), false, null);
                  }
              });
        }

        private const string LanguageNameKey = "#language_name";
        private bool _forceClose;
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_forceClose)
            {
                return;
            }

            if (_isOpen)
            {
                CloseCurrentDialog();
            }

            ShowNotice(Query("quit"), Query("confirm.quit"), true, delegate (bool Quit)
            {
                if (Quit)
                {
                    CloseWindow();
                }
            });
            e.Cancel = true;
        }

        private void CloseWindow()
        {
            try
            {
                SaveSettings();
                Settings.SaveSettings();
                Application.Current.Shutdown();
            }
            catch (Exception e)
            {
                ShowNotice(Query("error.while_quit"), string.Format(Query("error.force_quit"), e.Message), true, delegate (bool Quit)
                 {
                     if (Quit)
                     {
                         Application.Current.Shutdown();
                     }
                 });
            }
        }

        private void SaveSettings()
        {
            Settings.Put("evaluator", CheckBoxForceCast.IsChecked.GetValueOrDefault());
            Settings.Put("language", LanguageManager.GetLanguageByName(ComboBoxLanguages.SelectedItem.ToString()).LanguageName);
        }

        private void ButtonCancleDialog_Click(object sender, RoutedEventArgs e)
        {
            CloseCurrentDialog();
            _onDialogCloseAction?.Invoke(false);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonFindGame.IsEnabled = false;

            audioButton.Stop();
            _ = Task.Factory.StartNew(audioButton.PlaySync);
            ProcessButtonAnimationWithContent(ButtonFindGame, "SEARCHING FOR GAME PROCESS", delegate
            {
                while (!PVZ.RunGame())
                {
                    Thread.Sleep(500);
                }

                PVZ.Game.EnableRaisingEvents = true;
                PVZ.Game.Exited += delegate
                {
                    Dispatcher.Invoke(delegate
                    {
                        ButtonFindGame.IsEnabled = true;
                    });
                };
            });
            //ProcessButtonAnimation(sender, delegate
            //{
            //    if (IsGameExist)
            //    {
            //        ShowNotice(Query("game.already_found.title"), Query("game.already_found.text"), false, null);
            //    }
            //    else if (PVZ.RunGame())
            //    {
            //        EnqueueMessage(Query("game.found"));
            //    }
            //    else
            //    {
            //        ShowNotice(Query("game.not_found"), Query("game.not_found.text"), false, null);
            //    }
            //});
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string text = TextBoxSun.Text;
            bool forceCast = CheckBoxForceCast.IsChecked.GetValueOrDefault();
            ProcessButtonAnimation(sender, delegate
            {
                if (PVZ.BaseAddress == 0)
                {
                    ShowNotice(Query("game.not_in_game"), Query("game.join.before_modify_sun"), false, null);
                }
                else if (int.TryParse(text, out int Sun))
                {
                    PVZ.Sun = Sun;
                }
                else if (forceCast)
                {
                    Binder binder = new();
                    string[] diagnostics = binder.Diagnostic.ToArray();
                    StringBuilder builder = new();
                    if (diagnostics.Any())
                    {
                        foreach (var diagnostic in diagnostics)
                        {
                            builder.AppendLine(diagnostic);
                        }

                        ShowNotice(Query("evaluator.error"), builder.ToString(), false, null);
                    }
                    else
                    {
                        BoundExpression boundExpression = binder.BindExpression(SyntaxTree.Parse(text).Root);
                        Evaluator evaluator = new(boundExpression);
                        object result = evaluator.Evaluate();
                        if (result is int money)
                        {
                            PVZ.Sun = money;
                        }
                        else
                        {
                            ShowNotice(Query("error"), string.Format(Query("error.evaluator"), result));
                        }
                    }
                }
                else
                {
                    ShowNotice(Query("error"), string.Format(Query("error.parse"), text), false, null);
                }
            });
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CloseCurrentDialog();
            _onDialogCloseAction?.Invoke(true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Snack.MessageQueue = new();

            foreach (KeyValuePair<string, Language> item in LanguageManager.LoadedLanguages)
            {
                _ = ComboBoxLanguages.Items.Add(item.Value.Query(LanguageNameKey));
            }

            CheckBoxForceCast.IsChecked = Settings.Query<bool>("evaluator");
            ComboBoxLanguages.Text = LanguageManager.CurrentLanguage.Query(LanguageNameKey);

            audioButton = new SoundPlayer(PlantsVsZombiesStudio.Resources.ButtonPressed);
            audioButton.Load();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exp)
            {
                UnhandledException(exp);
            }
            _ = Task.Delay(-1);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            UnhandledException(e.Exception);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            UnhandledException(e.Exception);
        }
        private void UnhandledException(Exception e)
        {
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            ShowNotice(Query("error.occured"), e.Message);
            Instance.Logger.WriteLine(e.ToString());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MainWindow window = new();
            Application.Current.MainWindow = window;
            window.Show();
            _forceClose = true;
            Close();
            GC.Collect();
        }

        private void ButtonCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
            {
                Ping ping = new();
                if (ping.Send("gitee.com").Status != IPStatus.Success)
                {
                    ShowNotice(Query("network.connection.disconnect"), Query("network.connection.disconnect.text"));
                    return;
                }
            });
        }

        private void ComboBoxLanguages_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Client.PlantsVsZombiesStudio.ChangeLanguage(ComboBoxLanguages.Text);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InitializeBorders();
        }

        public void Dispose()
        {
            audioButton.Dispose();
        }
    }
}
