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
using MaterialDesignThemes.Wpf;
using MaterialDesignColors;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jvav.Syntax;
using Jvav.Binding;
using System;
using PlantsVsZombiesStudio.I18n;
using PlantsVsZombiesStudio.Setting;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Shell;

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

        public string Query(string key)
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
                      if (int.TryParse(TextBoxMoney.Text, out money))
                          PVZ.SaveData.Money = money;
                      else if (CheckBoxForceCast.IsChecked.GetValueOrDefault())
                      {
                          var syntaxTree = SyntaxTree.Parse(TextBoxMoney.Text);
                          var binder = new Binder();
                          var boundExpression = binder.BindExpression(syntaxTree.Root);
                          var diagnostics = binder.Diagnostic.ToArray();
                          var builder = new StringBuilder();
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
                              var evaluator = new Evaluator(boundExpression);
                              var result = evaluator.Evaluate();
                              if (result is int money)
                              {
                                  PVZ.SaveData.Money = money;
                              }
                              else
                                  ShowNotice(Query("error"), string.Format(Query("error.evaluator"), result));
                          }
                      }
                      else
                          ShowNotice(Query("error"), string.Format(Query("error.parse"), TextBoxMoney.Text), false, null);
                  else
                      ShowNotice(Query("game.not_found"), Query("game.find.before_modify_money"), false, null);
              });
        }
        private bool _forceClose = false;
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_forceClose)
                return;

            if (_isOpen)
                CloseCurrentDialog();

            ShowNotice(Query("quit"), Query("confirm.quit"), true, delegate (bool Quit)
            {
                if (Quit)
                    CloseWindow();
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
                         Application.Current.Shutdown();
                 });
            }
        }

        private void SaveSettings()
        {
            Settings.Put("evaluator", CheckBoxForceCast.IsChecked.GetValueOrDefault());
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
                    ShowNotice(Query("game.already_found.title"), Query("game.already_found.text"), false, null);
                else if (PVZ.RunGame())
                    Snack.MessageQueue.Enqueue(Query("game.found"));
                else
                    ShowNotice(Query("game.not_found"), Query("game.not_found.text"), false, null);
            });
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
            {
                if (PVZ.BaseAddress == 0)
                    ShowNotice(Query("game.not_in_game"), Query("game.join.before_modify_sun"), false, null);
                else if (int.TryParse(TextBoxSun.Text, out int Sun))
                    PVZ.Sun = Sun;
                else if (CheckBoxForceCast.IsChecked.GetValueOrDefault())
                {
                    var binder = new Binder();
                    var boundExpression = binder.BindExpression(SyntaxTree.Parse(TextBoxSun.Text).Root);
                    var diagnostics = binder.Diagnostic.ToArray();
                    var builder = new StringBuilder();
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
                        var evaluator = new Evaluator(boundExpression);
                        var result = evaluator.Evaluate();
                        if (result is int money)
                        {
                            PVZ.Sun = money;
                        }
                        else
                            ShowNotice(Query("error"), string.Format(Query("error.evaluator"), result));
                    }
                }
                else
                    ShowNotice(Query("error"), string.Format(Query("error.parse"), TextBoxSun.Text), false, null);
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
            Settings.InitializateSettings();
            LanguageManager.EnumLanguages();

            LanguageManager.CurrentLanguage = LanguageManager.LoadedLanguages[Settings.Query<string>("language")];
            CheckBoxForceCast.IsChecked = Settings.Query<bool>("evaluator");
            RegisterControl(CheckBoxForceCast, "control.force_cast");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exp)
            {
                UnhandledException(exp);
            }
            Task.Delay(-1);
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
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            Application.Current.MainWindow = window;
            window.Show();
            _forceClose = true;
            Close();
            GC.Collect();
        }
    }
}
