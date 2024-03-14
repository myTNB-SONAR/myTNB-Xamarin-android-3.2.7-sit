using Android.Content;
using AndroidX.AppCompat.App;
using myTNB.Android.Src.myTNBMenu.Fragments;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using myTNB.Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP;
using myTNB.Android.Src.NewAppTutorial.MVP;
using myTNB.Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB.Android.Src.SSMRMeterHistory.MVP;
using System.Collections.Generic;

namespace myTNB.Android.Src.Utils
{
    public class NewAppTutorialUtils
    {
        private static NewAppTutorialDialogFragment mDialog;

        public static void OnShowNewAppTutorial(Android.App.Activity ctx, AndroidX.Fragment.App.Fragment fragment, ISharedPreferences pref, List<NewAppModel> list, bool mIndicationShowTop = false)
        {
            try
            {
                if (list == null || (list != null && list.Count == 0))
                {
                    return;
                }

                if (mDialog != null)
                {
                    mDialog.CloseDialog();
                    mDialog = null;
                }

                if (fragment != null)
                {
                    if (fragment is HomeMenuFragment)
                    {
                        ((HomeMenuFragment)fragment).StopScrolling();
                    }
                    else if (fragment is ItemisedBillingMenuFragment)
                    {
                        ((ItemisedBillingMenuFragment)fragment).StopScrolling();
                    }
                    else if (fragment is DashboardChartFragment)
                    {
                        ((DashboardChartFragment)fragment).StopScrolling();
                    }
                    else if (fragment is RewardMenuFragment)
                    {
                        ((RewardMenuFragment)fragment).StopScrolling();
                    }
                    else if (fragment is WhatsNewMenuFragment)
                    {
                        ((WhatsNewMenuFragment)fragment).StopScrolling();
                    }
                }
                else
                {
                    if (ctx is SSMRMeterHistoryActivity)
                    {
                        ((SSMRMeterHistoryActivity)ctx).StopScrolling();
                    }
                    else if (ctx is SubmitMeterReadingActivity)
                    {
                        ((SubmitMeterReadingActivity)ctx).StopScrolling();
                    }
                }

                mDialog = new NewAppTutorialDialogFragment(ctx, fragment, pref, list, mIndicationShowTop);
                mDialog.Cancelable = false;
                mDialog.Show(((AppCompatActivity)ctx).SupportFragmentManager, "NewAppTutorial Dialog");
            }
            catch (System.Exception e)
            {
                ForceCloseNewAppTutorial();
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void ForceCloseNewAppTutorial()
        {
            try
            {
                if (mDialog != null)
                {
                    mDialog.CloseDialog();
                    mDialog = null;
                }
            }
            catch (System.Exception e)
            {
                CloseNewAppTutorial();
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void CloseNewAppTutorial()
        {
            try
            {
                if (mDialog != null)
                {
                    mDialog = null;
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}