
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SMRApplication.Adapter;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "OnBoardingActivity", Theme = "@style/Theme.Dashboard")]
    public class OnBoardingActivity : BaseToolbarAppCompatActivity, ViewPager.IOnPageChangeListener, OnBoardingSMRContract.IView
    {
        [BindView(Resource.Id.viewpager)]
        ViewPager onBoardViewPager;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        [BindView(Resource.Id.dontShowAgainCheckbox)]
        CheckBox dontShowAgainCheckbox;

        [BindView(Resource.Id.skipOnboarding)]
        TextView skipOnboarding;

        [BindView(Resource.Id.dontShowMeAgainLabel)]
        TextView dontShowMeAgainLabel;

        [BindView(Resource.Id.btnStartApplication)]
        Button btnStartApplication;


        OnBoardingSMRPresenter presenter;
        OnBoardingSMRAdapter onBoardingSMRAdapter;

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
            for (int i=0; i<3; i++)
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

            if (position == 2)
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
            presenter = new OnBoardingSMRPresenter(this);
            onBoardViewPager = (ViewPager)FindViewById(Resource.Id.onBoardingSMRViewPager);
            onBoardViewPager.AddOnPageChangeListener(this);
            this.presenter.OnBoardingList();
            onBoardingSMRAdapter = new OnBoardingSMRAdapter(SupportFragmentManager);
            onBoardingSMRAdapter.SetData(this.presenter.GetOnBoardingSMRData());

            onBoardViewPager.Adapter = onBoardingSMRAdapter;

            UpdateAccountListIndicator();

            btnStartApplication.Click += delegate
            {

                _ = presenter.GetCARegisteredContactInfo();
            };

            skipOnboarding.Click += delegate
            {
                onBoardViewPager.SetCurrentItem(3,true);
            };

            TextViewUtils.SetMuseoSans500Typeface(dontShowMeAgainLabel, skipOnboarding, btnStartApplication);
        }

        private void ShowSubmitButton(bool isShow)
        {
            if (isShow)
            {
                dontShowAgainCheckbox.Visibility = ViewStates.Gone;
                skipOnboarding.Visibility = ViewStates.Gone;
                dontShowMeAgainLabel.Visibility = ViewStates.Gone;
                btnStartApplication.Visibility = ViewStates.Visible;
            }
            else
            {
                dontShowAgainCheckbox.Visibility = ViewStates.Visible;
                skipOnboarding.Visibility = ViewStates.Visible;
                dontShowMeAgainLabel.Visibility = ViewStates.Visible;
                btnStartApplication.Visibility = ViewStates.Gone;
            }
        }

        private void UpdateAccountListIndicator()
        {
            indicatorContainer.Visibility = ViewStates.Visible;
            for (int i = 0; i < 3; i++)
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

        public void StartSMRApplication(string email, string mobileNumber)
        {
            Intent intent = new Intent(this, typeof(ApplicationFormSMRActivity));
            intent.PutExtra("email", email);
            intent.PutExtra("mobileNumber", mobileNumber);
            StartActivity(intent);
        }
    }
}
