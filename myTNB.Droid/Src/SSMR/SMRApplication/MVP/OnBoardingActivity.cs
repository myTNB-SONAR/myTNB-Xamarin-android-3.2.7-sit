
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;

using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Adapter;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "OnBoardingActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class OnBoardingActivity : BaseToolbarAppCompatActivity, ViewPager.IOnPageChangeListener, OnBoardingSMRContract.IView
    {
        [BindView(Resource.Id.onBoardingSMRViewPager)]
        ViewPager onBoardViewPager;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.skipOnboarding)]
        TextView skipOnboarding;

        [BindView(Resource.Id.btnStartApplication)]
        Button btnStartApplication;


        OnBoardingSMRPresenter presenter;
        OnBoardingSMRAdapter onBoardingSMRAdapter;

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {

                //Drawable background = (R.drawable.gradient_theme);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Resources.GetColor(Android.Resource.Color.Transparent));
                //Window.SetNavigationBarColor(Resources.GetColor(Android.Resource.Color.Transparent));
                Window.SetBackgroundDrawable(GetDrawable(Resource.Drawable.status_bar_gradient));
            }
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
            for (int i=0; i<2; i++)
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

            if (position == 1)
            {
                ShowSubmitButton(true);
            }
            else
            {
                ShowSubmitButton(false);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.OnboardingSSMRView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetToolBarTitle("");
            presenter = new OnBoardingSMRPresenter(this);
            onBoardViewPager = (ViewPager)FindViewById(Resource.Id.onBoardingSMRViewPager);
            onBoardViewPager.AddOnPageChangeListener(this);
            onBoardingSMRAdapter = new OnBoardingSMRAdapter(SupportFragmentManager);
            onBoardingSMRAdapter.SetData(MyTNBAppWalkthroughData.GetInstance().GetApplySSMROnboardingList());

            onBoardViewPager.Adapter = onBoardingSMRAdapter;

            UpdateAccountListIndicator();

            btnStartApplication.Click += delegate
            {
                MyTNBAccountManagement.GetInstance().UpdateIsSMROnboardingShown();
                StartSMRApplication();
            };

            skipOnboarding.Click += delegate
            {
                MyTNBAccountManagement.GetInstance().UpdateIsSMROnboardingShown();
                StartSMRApplication();
            };

            TextViewUtils.SetMuseoSans500Typeface(skipOnboarding, btnStartApplication);
            toolbar.BringToFront();
        }

        private void ShowSubmitButton(bool isShow)
        {
            if (isShow)
            {
                skipOnboarding.Visibility = ViewStates.Gone;
                btnStartApplication.Visibility = ViewStates.Visible;
            }
            else
            {
                skipOnboarding.Visibility = ViewStates.Visible;
                btnStartApplication.Visibility = ViewStates.Gone;
            }
        }

        private void UpdateAccountListIndicator()
        {
            indicatorContainer.Visibility = ViewStates.Visible;
            for (int i = 0; i < 2; i++)
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

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void StartSMRApplication()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent intent = new Intent(this, typeof(SSMRMeterHistoryActivity));
                StartActivity(intent);
                Finish();
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Apply SMR Onboarding");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
