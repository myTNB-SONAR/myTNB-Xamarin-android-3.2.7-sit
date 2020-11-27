
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;

using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Maintenance.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    [Activity(Label = "@string/app_name"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard"
        , NoHistory = true
        , Icon = "@drawable/ic_launcher"
       , LaunchMode = LaunchMode.SingleInstance)]
    public class NewWalkthroughActivity : BaseAppCompatActivity, ViewPager.IOnPageChangeListener , NewWalkthroughContract.IView
    {
        [BindView(Resource.Id.viewPager)]
        ViewPager viewPager;

        [BindView(Resource.Id.applicationIndicator)]
        RelativeLayout applicationIndicator;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.skip)]
        TextView btnSkip;

        [BindView(Resource.Id.btnStart)]
        Button btnStart;


        NewWalkthroughPresenter presenter;
        NewWalkthroughAdapter newWalkthroughAdapter;

        string currentAppNavigation;

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //throw new NotImplementedException();
        }

        public void OnPageScrollStateChanged(int state)
        {
            //throw new NotImplementedException();
        }

        public void OnPageSelected(int position)
        {
            if (newWalkthroughAdapter != null && newWalkthroughAdapter.Count > 1)
            {
                for (int i = 0; i < newWalkthroughAdapter.Count; i++)
                {
                    ImageView selectedDot = (ImageView)indicatorContainer.GetChildAt(i);
                    if (position == i)
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                    }
                    else
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                    }
                }

                if (position == (newWalkthroughAdapter.Count - 1))
                {
                    ShowSubmitButton(true);
                }
                else
                {
                    ShowSubmitButton(false);
                }
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.NewWalkthroughLayout;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            presenter = new NewWalkthroughPresenter(this);
            viewPager = (ViewPager)FindViewById(Resource.Id.viewPager);
            viewPager.AddOnPageChangeListener(this);
            newWalkthroughAdapter = new NewWalkthroughAdapter(SupportFragmentManager);

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APP_NAVIGATION_KEY))
                {
                    currentAppNavigation = extras.GetString(Constants.APP_NAVIGATION_KEY);

                    newWalkthroughAdapter.SetData(this.presenter.GenerateNewWalkthroughList(currentAppNavigation));

                    viewPager.Adapter = newWalkthroughAdapter;

                    UpdateAccountListIndicator();
                }
            }

            btnStart.Click += delegate
            {
                UserSessions.DoSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                UserSessions.DoUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                StartActivity();
            };

            btnSkip.Click += delegate
            {
                UserSessions.DoSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                UserSessions.DoUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                StartActivity();
            };

            TextViewUtils.SetMuseoSans500Typeface(btnSkip, btnStart);
            btnSkip.TextSize = TextViewUtils.GetFontSize(12f);
            btnStart.TextSize = TextViewUtils.GetFontSize(16f);
            btnSkip.Text = Utility.GetLocalizedLabel("Onboarding", "skip");
            btnStart.Text = Utility.GetLocalizedLabel("Onboarding", MyTNBAccountManagement.GetInstance().IsLargeFontDisabled() ? "letsStart" : "displaySize");
        }

        private void ShowSubmitButton(bool isShow)
        {
            if (isShow)
            {
                btnSkip.Visibility = ViewStates.Gone;
                btnStart.Visibility = ViewStates.Visible;
            }
            else
            {
                btnSkip.Visibility = ViewStates.Visible;
                btnStart.Visibility = ViewStates.Gone;
            }
        }

        private void UpdateAccountListIndicator()
        {
            if (newWalkthroughAdapter != null && newWalkthroughAdapter.Count > 1)
            {
                indicatorContainer.Visibility = ViewStates.Visible;
                for (int i = 0; i < newWalkthroughAdapter.Count; i++)
                {
                    ImageView image = new ImageView(this);
                    image.Id = i;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.RightMargin = 8;
                    layoutParams.LeftMargin = 8;
                    image.LayoutParameters = layoutParams;
                    if (i == 0)
                    {
                        image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                    }
                    else
                    {
                        image.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                    }
                    indicatorContainer.AddView(image, i);
                }
            }
            else
            {
                applicationIndicator.Visibility = ViewStates.Gone;
                btnStart.Visibility = ViewStates.Visible;
                RelativeLayout.LayoutParams param = btnStart.LayoutParameters as RelativeLayout.LayoutParams;
                param.AddRule(LayoutRules.AlignParentBottom);
                param.BottomMargin = (int)DPUtils.ConvertDPToPx(16f);
            }
        }

        public void StartActivity()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (currentAppNavigation == AppLaunchNavigation.Logout.ToString())
                {
                    ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
                    Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                    PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(PreLoginIntent);
                }
                else if (currentAppNavigation == AppLaunchNavigation.Dashboard.ToString())
                {
                    Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(DashboardIntent);
                }
                else if (currentAppNavigation == AppLaunchNavigation.PreLogin.ToString())
                {
                    Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                    PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(PreLoginIntent);
                }
                else if (currentAppNavigation == AppLaunchNavigation.Walkthrough.ToString())
                {
                    Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                    PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(PreLoginIntent);
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (currentAppNavigation == AppLaunchNavigation.Walkthrough.ToString())
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Pre-Login Walkthrough");
                }
                else
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "App Update Walkthrough");
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }


            try
            {
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                {
                    Drawable drawable = Resources.GetDrawable(Resource.Drawable.walkthrough_bg_install_1);
                    this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    this.Window.SetBackgroundDrawable(drawable);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string GetAppString(int id)
        {
            return this.GetString(id);
        }

        public void UpdateContent()
        {
            btnSkip.Text = Utility.GetLocalizedLabel("Onboarding", "skip");
           
            btnStart.Text = Utility.GetLocalizedLabel("Onboarding", MyTNBAccountManagement.GetInstance().IsLargeFontDisabled()? "letsStart":"displaySize");
            newWalkthroughAdapter.SetData(this.presenter.GenerateNewWalkthroughList(currentAppNavigation));
            newWalkthroughAdapter.NotifyDataSetChanged();
            viewPager.Invalidate();
            DismissProgressDialog();
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DismissProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnMaintenanceProceed()
        {
            DismissProgressDialog();
            Intent maintenanceScreen = new Intent(this, typeof(MaintenanceActivity));
            maintenanceScreen.PutExtra(Constants.MAINTENANCE_TITLE_KEY, MyTNBAccountManagement.GetInstance().GetMaintenanceTitle());
            maintenanceScreen.PutExtra(Constants.MAINTENANCE_MESSAGE_KEY, MyTNBAccountManagement.GetInstance().GetMaintenanceContent());
            StartActivity(maintenanceScreen);
        }
    }
}
