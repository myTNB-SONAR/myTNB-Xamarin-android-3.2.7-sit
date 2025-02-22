﻿using System;
using System.Collections.Generic;
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
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Maintenance.Activity;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.PreLogin.Activity;
using myTNB.AndroidApp.Src.Profile.Activity;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.NewWalkthrough.MVP
{
    [Activity(Label = "@string/app_name"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard"
        , NoHistory = true
        , Icon = "@drawable/ic_launcher"
       , LaunchMode = LaunchMode.SingleInstance)]
    public class NewWalkthroughActivity : BaseAppCompatActivity, ViewPager.IOnPageChangeListener, NewWalkthroughContract.IView
    {
        [BindView(Resource.Id.rootView)]
        RelativeLayout rootView;

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
        private Snackbar mLanguageSnackbar;

        List<NewWalkthroughModel> walkthroughModelList;
        string dynatraceSkipActionTag;

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
                NewWalkthroughModel walkthroughModel = walkthroughModelList[position];

                if (walkthroughModel != null)
                {
                    DynatraceHelper.OnTrack(walkthroughModel.DynatraceVisitTag);
                    dynatraceSkipActionTag = walkthroughModel.DynatraceActionTag;
                }

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
                //UserEntity activeUser = UserEntity.GetActive();
                //if (position == (MyTNBAccountManagement.GetInstance().IsAppointmentDisabled
                //   ? (newWalkthroughAdapter.Count - 1)
                //   : (newWalkthroughAdapter.Count - 2)))
                //{
                //    ShowSubmitButton(true);
                //}
                //else
                //{
                //    ShowSubmitButton(false);
                //}
                //if (position == (MyTNBAccountManagement.GetInstance().IsDigitalBillDisabled
                //    ? (newWalkthroughAdapter.Count - 1)
                //    : (newWalkthroughAdapter.Count - 2)) && activeUser != null)
                //{
                //    ISharedPreferences mPref = PreferenceManager.GetDefaultSharedPreferences(this);
                //    ISharedPreferencesEditor editor = mPref.Edit();
                //    editor.PutBoolean("hasItemizedBillingNMSMTutorialShown", false);
                //    editor.Apply();
                //}
                if (walkthroughModel.Type == NewWalkthroughType.FontSize)
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
            walkthroughModelList = new List<NewWalkthroughModel>();

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APP_NAVIGATION_KEY))
                {
                    currentAppNavigation = extras.GetString(Constants.APP_NAVIGATION_KEY);
                    walkthroughModelList = this.presenter.GenerateNewWalkthroughList(currentAppNavigation);
                    newWalkthroughAdapter.SetData(walkthroughModelList);

                    viewPager.Adapter = newWalkthroughAdapter;

                    UpdateAccountListIndicator();
                }
            }

            if (MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
            {
                btnStart.Click += delegate
                {
                    UserSessions.DoSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                    UserSessions.DoUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                    StartActivity();
                };
            }
            else
            {
                btnStart.Click += delegate
                {
                    this.SetIsClicked(true);
                    Intent nextIntent = new Intent(this, typeof(AppLargeFontActivity));
                    nextIntent.PutExtra("APP_FONTCHANGE_REQUEST", AppLaunchNavigation.LargeFont.ToString());
                    nextIntent.PutExtra(Constants.APP_NAVIGATION_KEY, currentAppNavigation);
                    StartActivity(nextIntent);
                };
            }
            btnSkip.Click += delegate
            {
                DynatraceHelper.OnTrack(dynatraceSkipActionTag);
                UserSessions.DoSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                UserSessions.DoUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this));
                StartActivity();
            };


            TextViewUtils.SetMuseoSans500Typeface(btnSkip, btnStart);
            TextViewUtils.SetTextSize12(btnSkip);
            TextViewUtils.SetTextSize16(btnStart);
            btnSkip.Text = Utility.GetLocalizedLabel("Onboarding", "skip");
            if (!MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
            {
                btnStart.Text = Utility.GetLocalizedLabel("Onboarding", "setSize");
            }
            btnStart.Visibility = ViewStates.Gone;

            if (walkthroughModelList.Count > 0)
            {
                NewWalkthroughModel walkthroughModel = walkthroughModelList[0];

                if (walkthroughModel != null)
                {
                    DynatraceHelper.OnTrack(walkthroughModel.DynatraceVisitTag);
                    dynatraceSkipActionTag = walkthroughModel.DynatraceActionTag;
                }
            }
        }

        private void ShowSubmitButton(bool isShow)
        {
            if (isShow && !MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
            {
                btnStart.Visibility = ViewStates.Visible;
            }
            else
            {
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
                if (MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
                {
                    btnStart.Visibility = ViewStates.Gone;
                }
                else
                {
                    btnStart.Visibility = ViewStates.Visible;
                }

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
                    Drawable drawable = Resources.GetDrawable(Resource.Drawable.Onboarding_BG_One);
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
            if (MyTNBAccountManagement.GetInstance().IsLargeFontDisabled())
            {
                btnStart.Visibility = ViewStates.Gone;
            }
            else
            {
                btnStart.Text = Utility.GetLocalizedLabel("Onboarding", "setSize");
                //btnStart.Visibility = ViewStates.Visible;
            }

            walkthroughModelList = this.presenter.GenerateNewWalkthroughList(currentAppNavigation);
            newWalkthroughAdapter.SetData(walkthroughModelList);
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