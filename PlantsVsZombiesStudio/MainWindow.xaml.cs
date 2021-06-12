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
                              ShowNotice("EVALUATE ERROR", builder.ToString(), false, null);
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
                                  ShowNotice("ERROR", $"Can not parse evaluate result '{result}' to an integer.");
                          }
                      }
                      else
                          ShowNotice("ERROR", $"Can not parse \"{TextBoxMoney.Text}\" to an integer", false, null);
                  else
                      ShowNotice("GAME NOT FOUND", "Find game before modifying money.", false, null);
              });
        }
        private bool _forceClose = false;
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_forceClose)
                return;
            ShowNotice("QUIT", "Are you sure want to quit?", true, delegate (bool Quit)
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
                Settings.SaveSettings();
                Application.Current.Shutdown();
            }
            catch (Exception e)
            {
                ShowNotice("ERROR WHILE QUIT", $"{e.Message}\nClick \"YES\" to force quit." ,true, delegate(bool Quit)
                {
                    Application.Current.Shutdown();
                });
            }
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
                        ShowNotice("EVALUATE ERROR", builder.ToString(), false, null);
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
                            ShowNotice("ERROR", $"Can not parse evaluate result '{result}' to an integer.");
                    }
                }
                else
                    ShowNotice("ERROR", $"Can not parse \"{TextBoxSun.Text}\" to an integer", false, null);
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
            ShowNotice("AN ERROR OCCURED", e.Message);
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
