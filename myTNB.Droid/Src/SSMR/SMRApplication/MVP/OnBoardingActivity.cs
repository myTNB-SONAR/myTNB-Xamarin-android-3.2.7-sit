
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
using myTNB_Android.Src.SSMR.SMRApplication.Adapter;

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "OnBoardingActivity", Theme = "@style/Theme.Dashboard")]
    public class OnBoardingActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.viewpager)]
        ViewPager onBoardViewPager;


        OnBoardingSMRPresenter presenter;
        OnBoardingSMRAdapter onBoardingSMRAdapter;
        public override int ResourceId()
        {
            return Resource.Layout.OnboardingSSMRView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            presenter = new OnBoardingSMRPresenter();
            onBoardViewPager = (ViewPager)FindViewById(Resource.Id.onBoardingSMRViewPager);

            onBoardingSMRAdapter = new OnBoardingSMRAdapter(SupportFragmentManager);
            onBoardingSMRAdapter.SetData(this.presenter.GetOnBoardingSMRData());

            onBoardViewPager.Adapter = onBoardingSMRAdapter;
        }

        
    }
}
