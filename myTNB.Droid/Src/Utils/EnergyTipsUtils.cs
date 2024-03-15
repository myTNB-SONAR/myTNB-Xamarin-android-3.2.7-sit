using AFollestad.MaterialDialogs;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP;
using myTNB.AndroidApp.Src.SSMR.SSMRMeterReadingTooltip.MVP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.Utils
{
    public class EnergyTipsUtils
    {
        private static List<EnergySavingTipsModel> energyTipsList = new List<EnergySavingTipsModel>();

        public static void OnSetEnergyTipsList(List<EnergySavingTipsModel> energyList)
        {
            if (energyList != null)
            {
                if (energyTipsList.Count == 0)
                {
                    foreach(EnergySavingTipsModel item in energyList)
                    {
                        energyTipsList.Add(new EnergySavingTipsModel()
                        {
                            Title = item.Title,
                            Description = item.Description,
                            Image = item.Image,
                            isUpdateNeeded = true,
                            ImageBitmap = null,
                            ID = item.ID
                        });
                    }
                }
                else
                {
                    for (int j = 0; j < energyList.Count; j++)
                    {
                        bool isFound = false;
                        for (int i = 0; i < energyTipsList.Count; i++)
                        {
                            if (energyList[j].ID == energyTipsList[i].ID)
                            {
                                energyList[j].Title = energyTipsList[i].Title;
                                energyList[j].Description = energyTipsList[i].Description;
                                if (energyList[j].Image == energyTipsList[i].Image && !energyTipsList[i].isUpdateNeeded)
                                {
                                    energyList[j].isUpdateNeeded = false;
                                    energyList[j].ImageBitmap = energyTipsList[i].ImageBitmap;
                                }
                                else
                                {
                                    energyList[j].isUpdateNeeded = true;
                                    energyList[j].ImageBitmap = null;
                                }
                                isFound = true;
                                break;
                            }
                        }
                        if (!isFound)
                        {
                            energyList[j].isUpdateNeeded = true;
                            energyList[j].ImageBitmap = null;
                        }
                    }
                    energyTipsList.Clear();
                    energyTipsList.AddRange(energyList);
                }
            }

            if (energyTipsList.Count > 0)
            {
                _ = OnProcessEnergyTips();
            }
        }

        private static async Task OnProcessEnergyTips()
        {
            foreach(EnergySavingTipsModel item in energyTipsList)
            {
                if (item.isUpdateNeeded)
                {
                    EnergySavingTipsModel processItem = new EnergySavingTipsModel();
                    item.ImageBitmap = await GetPhoto(item.Image);
                    item.isUpdateNeeded = false;
                }
            }
        }

        private static async Task<Bitmap> GetPhoto(string imageUrl)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            try
            {
                await Task.Run(() =>
                {
                    imageBitmap = ImageUtils.GetImageBitmapFromUrl(imageUrl);
                }, cts.Token);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return imageBitmap;
        }

        public static List<EnergySavingTipsModel> GetAllItems()
        {
            return energyTipsList;
        }

    }
}