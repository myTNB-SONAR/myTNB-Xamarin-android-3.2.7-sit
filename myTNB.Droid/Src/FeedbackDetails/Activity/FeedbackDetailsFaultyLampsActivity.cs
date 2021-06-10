using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.FeedbackDetails.Adapter;
using myTNB_Android.Src.FeedbackDetails.MVP;
using myTNB_Android.Src.FeedbackFullScreenImage.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.FeedbackDetails.Activity
{
    [Activity(Label = "@string/faulty_street_lamps_activity_title"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.FaultyStreetLamps")]
    public class FeedbackDetailsFaultyLampsActivity : BaseToolbarAppCompatActivity, FeedbackDetailsContract.FaultyLamps.IView
    {

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtInputLayoutFeedbackId)]
        TextInputLayout txtInputLayoutFeedbackId;

        [BindView(Resource.Id.txtInputLayoutDateTime)]
        TextInputLayout txtInputLayoutDateTime;


        [BindView(Resource.Id.txtInputLayoutState)]
        TextInputLayout txtInputLayoutState;

        [BindView(Resource.Id.txtInputLayoutLocation)]
        TextInputLayout txtInputLayoutLocation;

        [BindView(Resource.Id.txtInputLayoutPoleNo)]
        TextInputLayout txtInputLayoutPoleNo;

        [BindView(Resource.Id.txtInputLayoutFeedback)]
        TextInputLayout txtInputLayoutFeedback;

        [BindView(Resource.Id.txtInputLayoutStatus)]
        TextInputLayout txtInputLayoutStatus;


        [BindView(Resource.Id.txtFeedbackId)]
        EditText txtFeedbackId;

        [BindView(Resource.Id.txtFeedbackStatus)]
        EditText txtFeedbackStatus;

        [BindView(Resource.Id.txtFeedbackDateTime)]
        EditText txtFeedbackDateTime;


        [BindView(Resource.Id.txtState)]
        EditText txtState;

        [BindView(Resource.Id.txtLocation)]
        EditText txtLocation;

        [BindView(Resource.Id.txtPoleNo)]
        EditText txtPoleNo;

        [BindView(Resource.Id.txtFeedback)]
        EditText txtFeedback;

        [BindView(Resource.Id.txtRelatedScreenshotTitle)]
        TextView txtRelatedScreenshotTitle;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;

        FeedbackImageRecyclerAdapter adapter;

        GridLayoutManager layoutManager;

        SubmittedFeedbackDetails submittedFeedback;

        FeedbackDetailsContract.FaultyLamps.IUserActionsListener userActionsListener;
        FeedbackDetailsFaultyLampsPresenter mPresenter;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackDetailsFaultyStreetLampsView;
        }

        public void SetPresenter(FeedbackDetailsContract.FaultyLamps.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowImages(List<AttachedImage> list)
        {
            try
            {
                adapter.AddAll(list);
                if (list.Count <= 0)
                {
                    txtRelatedScreenshotTitle.Visibility = ViewStates.Gone;
                }
                else
                {
                    txtRelatedScreenshotTitle.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowInputData(string feedbackId, string dateTime, string state, string location, string pole_no, string feedback)
        {

        }

        public void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string state, string location, string pole_no, string feedback)
        {
            try
            {
                txtFeedbackId.Text = feedbackId;

                txtFeedbackStatus.Text = feedbackStatus;

                if (feedbackCode.Equals("CL01"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.createdColor)));
                }
                else if (feedbackCode.Equals("CL02"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.inProgressColor)));
                }
                else if (feedbackCode.Equals("CL03"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
                }
                else if (feedbackCode.Equals("CL04"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.completedColor)));
                }
                else if (feedbackCode.Equals("CL06"))
                {
                    txtFeedbackStatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.cancelledColor)));
                }
                txtFeedbackDateTime.Text = dateTime;
                txtState.Text = state;
                txtLocation.Text = location;
                txtPoleNo.Text = pole_no;
                txtFeedback.Text = feedback;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                string selectedFeedback = UserSessions.GetSelectedFeedback(PreferenceManager.GetDefaultSharedPreferences(this));
                submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);

                TextViewUtils.SetMuseoSans300Typeface(txtRelatedScreenshotTitle, txtFeedbackId
                    , txtFeedbackDateTime, txtState, txtLocation, txtPoleNo, txtFeedback, txtFeedbackStatus);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutFeedbackId, txtInputLayoutFeedback
                    , txtInputLayoutLocation, txtInputLayoutDateTime, txtInputLayoutPoleNo, txtInputLayoutState, txtInputLayoutStatus);
                TextViewUtils.SetTextSize9(txtRelatedScreenshotTitle);

                adapter = new FeedbackImageRecyclerAdapter(true);
                layoutManager = new GridLayoutManager(this, Constants.GRID_IMAGE_COUNT);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);

                adapter.SelectClickEvent += Adapter_SelectClickEvent;

                mPresenter = new FeedbackDetailsFaultyLampsPresenter(this, submittedFeedback, this);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Feedback Details");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void Adapter_SelectClickEvent(object sender, int e)
        {
            AttachedImage selectedImage = adapter.GetItemObject(e);
            var fullImageIntent = new Intent(this, typeof(FeedbackDetailsFullScreenImageActivity));
            fullImageIntent.PutExtra(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE, JsonConvert.SerializeObject(selectedImage));
            StartActivity(fullImageIntent);
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
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
    }
}