
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP
{
    [Activity(Label = "Check Status", Theme = "@style/Theme.RegisterForm")]
    public class ApplicationStatusLandingActivity : BaseActivityCustom, ApplicationStatusLandingContract.IView
    {
        [BindView(Resource.Id.rootview)]
        LinearLayout rootview;

        [BindView(Resource.Id.applicationStatusLandingTitleLayout)]
        RelativeLayout applicationStatusLandingTitleLayout;

        [BindView(Resource.Id.txtApplicationStatusLandingTitle)]
        TextView txtApplicationStatusLandingTitle;

        [BindView(Resource.Id.applicationStatusLandingFilterImg)]
        ImageView applicationStatusLandingFilterImg;

        [BindView(Resource.Id.applicationStatusLandingEmptyLayout)]
        LinearLayout applicationStatusLandingEmptyLayout;

        [BindView(Resource.Id.applicationStatusLandingEmptyImg)]
        ImageView applicationStatusLandingEmptyImg;

        [BindView(Resource.Id.txtApplicationStatusLandingEmpty)]
        TextView txtApplicationStatusLandingEmpty;

        [BindView(Resource.Id.applicationStatusLandingShimmerLayout)]
        LinearLayout applicationStatusLandingShimmerLayout;

        [BindView(Resource.Id.applicationStatusLandingListContentShimmer)]
        ShimmerFrameLayout applicationStatusLandingListContentShimmer;

        [BindView(Resource.Id.applicationStatusLandingRecyclerView)]
        RecyclerView applicationStatusLandingRecyclerView;

        [BindView(Resource.Id.viewMoreContainer)]
        LinearLayout viewMoreContainer;

        [BindView(Resource.Id.viewMoreLabel)]
        TextView viewMoreLabel;

        [BindView(Resource.Id.viewMoreImg)]
        ImageView viewMoreImg;

        [BindView(Resource.Id.applicationStatusLandingBottomLayout)]
        LinearLayout applicationStatusLandingBottomLayout;

        [BindView(Resource.Id.btnSearchApplicationStatus)]
        Button btnSearchApplicationStatus;

        ApplicationStatusLandingPresenter mPresenter;

        const string PAGE_ID = "ApplicationStatus";

        //String email, mobileNumber;
        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusLandingLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new ApplicationStatusLandingPresenter(this);

            TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusLandingEmpty);
            TextViewUtils.SetMuseoSans500Typeface(btnSearchApplicationStatus, txtApplicationStatusLandingTitle, viewMoreLabel);

            
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
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

        public void HideProgressDialog()
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

        /*[OnClick(Resource.Id.btnSubmitRegistration)]
        void SubmitRegistration(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

            }
        }*/


        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void EnableButton()
        {
            this.SetIsClicked(false);
        }

        /*[OnClick(Resource.Id.txtTermsAndCondition)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                StartActivity(typeof(TermsAndConditionActivity));
            }
        }*/

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Application Status Landing");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
