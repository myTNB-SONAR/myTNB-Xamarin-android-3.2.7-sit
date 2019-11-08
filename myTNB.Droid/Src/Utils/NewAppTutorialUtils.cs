using Android.Content;
using Android.Support.V7.App;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using System.Collections.Generic;

namespace myTNB_Android.Src.Utils
{
    public class NewAppTutorialUtils
    {
        private static NewAppTutorialDialogFragment mDialog;

        public static void OnShowNewAppTutorial(Android.App.Activity ctx, Android.App.Fragment fragment, ISharedPreferences pref, List<NewAppModel> list)
        {
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
            }
            else
            {
                if (ctx is SSMRMeterHistoryActivity)
                {
                    ((SSMRMeterHistoryActivity)ctx).StopScrolling();
                }
            }

            mDialog = new NewAppTutorialDialogFragment(ctx, fragment, pref, list);
            mDialog.Cancelable = false;
            mDialog.Show(((AppCompatActivity)ctx).SupportFragmentManager, "NewAppTutorial Dialog");
        }

        public static void ForceCloseNewAppTutorial()
        {
            if (mDialog != null)
            {
                mDialog.CloseDialog();
                mDialog = null;
            }
        }

        public static void CloseNewAppTutorial()
        {
            if (mDialog != null)
            {
                mDialog = null;
            }
        }
    }
}