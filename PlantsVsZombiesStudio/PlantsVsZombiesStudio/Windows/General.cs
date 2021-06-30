using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using PVZClass;

namespace PlantsVsZombiesStudio
{
    public partial class MainWindow : Window
    {
        private void CheckBoxBackgroundRun_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.BGRunable(CheckBoxBackgroundRun.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxFreePlant_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.FreePlantingCheat = CheckBoxFreePlant.IsChecked.GetValueOrDefault();
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxNoCd_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.NoCD(CheckBoxNoCd.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxAutoCollect_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.AutoCollect(CheckBoxAutoCollect.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxIgnoreResources_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.IgnoreRes(CheckBoxIgnoreResources.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxShowHiddenLevel_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.ShowHiddenLevel(CheckBoxShowHiddenLevel.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxOverlapping_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.OverlapPlanting(CheckBoxOverlapping.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxFullScreenFog_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.FullScreenFog(CheckBoxFullScreenFog.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxConveyNoDelay_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.ConveyorBeltNoDelay(CheckBoxConveyNoDelay.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxSuspendZombieSpawn_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.ZombieReverse(CheckBoxSuspendZombieSpawn.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxResourcesNoLimit_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.NoUpperLimit(CheckBoxResourcesNoLimit.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxFogHack_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.FogPerspect(CheckBoxFogHack.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxVaseHack_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.VasePerspect(CheckBoxVaseHack.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void CheckBoxLockShovel_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameExist)
            {
                PVZ.LockShovel(CheckBoxLockShovel.IsChecked.GetValueOrDefault());
            }
            else
            {
                ShowNotice(Query("game.not_found"), Query("game.find.first"));
            }
        }

        private void ButtonKillZombies_Click(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
            {
                if (IsGameExist)
                {
                    foreach (PVZ.Zombie zombie in PVZ.AllZombies)
                    {
                        zombie.AccessoriesType2HP = 0;
                        zombie.AccessoriesType1HP = 0;
                        zombie.BodyHP = 0;
                        zombie.Hit(int.MaxValue);
                    }
                }
                else
                {
                    ShowNotice(Query("game.not_found"), Query("game.find.first"));
                }
            });
        }

        private void ButtonHypnoZombies_Click(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
            {
                if (IsGameExist)
                {
                    foreach (PVZ.Zombie zombie in PVZ.AllZombies)
                    {
                        zombie.Hypnotized = true;
                    }
                }
                else
                {
                    ShowNotice(Query("game.not_found"), Query("game.find.first"));
                }
            });
        }

        private void ButtonCleanPlants_Click(object sender, RoutedEventArgs e)
        {
            ProcessButtonAnimation(sender, delegate
            {
                if (IsGameExist)
                {
                    foreach (PVZ.Plant plant in PVZ.AllPlants)
                    {
                        plant.Exist = false;
                    }
                }
                else
                {
                    ShowNotice(Query("game.not_found"), Query("game.find.first"));
                }
            });
        }
    }
}
