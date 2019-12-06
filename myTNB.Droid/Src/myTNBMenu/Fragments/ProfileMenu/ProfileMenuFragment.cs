
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ProfileMenu
{
    public class ProfileMenuFragment : BaseFragmentCustom
	{
        const string PAGE_ID = "Profile";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                    ((DashboardHomeActivity)Activity).SetToolBarTitle(GetLabelByLanguage("title"));
                }
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Profile");
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            base.OnAttach(context);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            try
            {
                Context context = Activity.ApplicationContext;
                var name = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
                var code = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
                if (name != null)
                {
                    //txt_app_version.Text = GetLabelByLanguage("appVersion") + " " + name;
                }

                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (System.Exception e)
            {
                Log.Debug("Package Manager", e.StackTrace);
                //txt_app_version.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            ((DashboardHomeActivity)Activity).ReloadProfileMenu();
        }

        public override int ResourceId()
        {
            return Resource.Layout.ProfileMenuFragmentLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
