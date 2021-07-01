using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Linq;
using PlantsVsZombiesStudio.I18n;
using MultiThreadDownloader;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.IO.Compression;

namespace PlantsVsZombiesStudio.Client
{
    public class PlantsVsZombiesStudio
    {
        private readonly PlantsVsZombiesStudioSession plantsVsZombiesStudioSession;
        private MainWindow MainWindow => plantsVsZombiesStudioSession.MainWindow;
        private static PlantsVsZombiesStudio instance;

        public static PlantsVsZombiesStudio Instance => instance;

        public Logger Logger => logger;

        private readonly Logger logger;
        public PlantsVsZombiesStudio(PlantsVsZombiesStudioSession plantsVsZombiesStudioSession)
        {
            this.plantsVsZombiesStudioSession = plantsVsZombiesStudioSession;
            instance = this;
            logger = new();
        }


        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="languageName">语言的ID</param>
        public static void ChangeLanguage(string languageName)
        {
            LanguageManager.CurrentLanguage = LanguageManager.GetLanguageByName(languageName);
            LanguageManager.CurrentLanguage.SetAsDefaultLanguage();
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public void Update()
        {
            MainWindow.ProcessButtonAnimation(MainWindow.ButtonCheckUpdate, delegate
            {
                Dictionary<string, string> expiredFiles = VersionChecker.GetExpiredFiles();
                if (expiredFiles.Any())
                {
                    MainWindow.ShowNotice(MainWindow.Query("update.found"), string.Format(MainWindow.Query("confirm.update"), expiredFiles.Count), true, async delegate (bool c)
                      {
                          if (c)
                          {
                              List<SingleThreadDownloadTask> tasks = new();

                              foreach (KeyValuePair<string, string> expiredFile in expiredFiles)
                              {
                                  string url = Path.Combine("https://plants-vs-zombies-studio-1256953837.cos.ap-chengdu.myqcloud.com/Storage/bin/net6.0-windows", expiredFile.Key);
                                  SingleThreadDownloadTask task = new(new(url, $"{expiredFile.Key}.update"));
                                  tasks.Add(task);
                                  task.Compeleted += delegate
                                  {
                                      MainWindow.EnqueueMessage($"File downloaded:{task.Options.Path}");
                                  };
                                  _ = Task.Factory.StartNew(task.Download);
                              }

                              MainWindow.ShowNotice("UPDATING", MainWindow.AllocateCircularProgressBar());
                              double progress;
                              do
                              {
                                  double totalDownloaded = 0;
                                  double totalContentLength = 0;

                                  foreach (SingleThreadDownloadTask item in tasks)
                                  {
                                      totalDownloaded += item.DownloadedSize;
                                      totalContentLength += item.ContentLength;
                                  }

                                  progress = totalDownloaded / totalContentLength;
                                  MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                                  MainWindow.TaskbarItemInfo.ProgressValue = progress;
                                  await Task.Delay(100);
                              } while (progress != 1);

                              MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                              MainWindow.CloseCurrentDialog();
                              MainWindow.ShowNotice(MainWindow.Query("update.install"), MainWindow.Query("update.install.restart"), true, delegate (bool c)
                               {
                                   if (c)
                                   {
                                       if (File.Exists("PlantsVsZombiesStudioInstaller.dll.update"))
                                       {
                                           if (File.Exists("PlantsVsZombiesStudioInstaller.dll"))
                                           {
                                               File.Delete("PlantsVsZombiesStudioInstaller.dll");
                                           }
                                           File.Move("PlantsVsZombiesStudioInstaller.dll.update", "PlantsVsZombiesStudioInstaller.dll");
                                       }

                                       if (File.Exists("PlantsVsZombiesStudioInstaller.exe.update"))
                                       {
                                           if (File.Exists("PlantsVsZombiesStudioInstaller.exe"))
                                           {
                                               File.Delete("PlantsVsZombiesStudioInstaller.exe");
                                           }
                                           File.Move("PlantsVsZombiesStudioInstaller.exe.update", "PlantsVsZombiesStudioInstaller.exe");
                                       }

                                       _ = Process.Start("PlantsVsZombiesStudioInstaller.exe");
                                       MainWindow.CloseWindow();
                                   }
                               });
                          }
                      });
                }
                else
                {
                    MainWindow.ShowNotice(MainWindow.Query("update.not_found"), MainWindow.Query("update.not_found.text"));
                }
            });
        }

        public void Reload()
        {
            Logger.Dispose();
            MainWindow._forceClose = true;
            MainWindow window = new();
            Application.Current.MainWindow = window;
            window.Show();
            MainWindow.Close();
            GC.Collect();
        }

        public void GameFound()
        {
            //MainWindow.Dispatcher.Invoke(delegate
            //{
            //    foreach (object children in MainWindow.StackPanelGeneral1.Children)
            //    {
            //        if (children is CheckBox c)
            //        {
            //            c.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, c));
            //        }
            //    }
            //
            //    foreach (object children in MainWindow.StackPanelGeneral2.Children)
            //    {
            //        if (children is CheckBox c)
            //        {
            //            c.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, c));
            //        }
            //    }
            //});
        }

        public async void DownloadCurrentVersion()
        {
            if (MainWindow._pvzVersion == null)
            {
                MainWindow.ShowNotice(MainWindow.Query("unable_to_download"), MainWindow.Query("select_item_first"));
            }
            else
            {
                MainWindow.EnqueueMessage(MainWindow.Query("download_started"));
                if (!Directory.Exists("Download"))
                {
                    Directory.CreateDirectory("Download");
                }

                PVZVersion version = MainWindow._pvzVersion;
                MultiThreadDownloadTask task = new(new($"https://plants-vs-zombies-studio-1256953837.cos.ap-chengdu.myqcloud.com/Storage/game/{version.FileName}", 32, $"Download/{version.FileName}"));
                ProgressBar bar = MainWindow.AllocateCircularProgressBar();
                MainWindow.ShowNotice(MainWindow.Query("downloading"), bar);
                await Task.Factory.StartNew(task.Download);
                double progress;
                do
                {
                    progress = (double)task.DownloadedSize / task.ContentLength;
                    MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    MainWindow.TaskbarItemInfo.ProgressValue = progress;
                    if(MainWindow.CardContent.Content == bar)
                    {
                        MainWindow.TextNoticeTitle.Text = $"{MainWindow.Query("downloading")}({progress*100:0.00}%)";
                    }
                    await Task.Delay(100);
                } while (progress != 1 && !task.IsCompeleted);

                MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                MainWindow.CloseCurrentDialog();
                string path = $"Download\\{version.FileName}";
                await Task.Factory.StartNew(delegate
                {
                    ZipFile.ExtractToDirectory(path, "Download", true);
                });

                MainWindow.Dispatcher.Invoke(delegate
                {
                    MainWindow.ShowNotice(MainWindow.Query("download_compelete"), string.Format(MainWindow.Query("file_downloaded"), version.FileName));
                });
            }
        }
    }
}
