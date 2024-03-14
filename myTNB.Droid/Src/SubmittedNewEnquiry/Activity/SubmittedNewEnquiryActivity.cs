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


using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.FeedbackFullScreenImage.Activity;
using myTNB.Android.Src.FeedbackGeneralEnquiryStepOne.Adapter;
using myTNB.Android.Src.SubmittedNewEnquiry.MVP;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using myTNB.Android.Src.Base.Models;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Content;

namespace myTNB.Android.Src.SubmittedNewEnquiry.Activity
{

    [Activity(Label = "Submitted Enquiry"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class SubmittedNewEnquiryActivity : BaseToolbarAppCompatActivity, SubmittedNewEnquiryContract.IView
    {
        SubmittedNewEnquiryContract.IUserActionsListener userActionListener;
        SubmittedNewEnquiryPresenter mPresenter;
        LinearLayoutManager layoutManager;
        FeedbackGeneralEnquiryStepOneImageRecyclerAdapter adapter;


        [BindView(Resource.Id.txtRelatedScreenshotTitle)]
        TextView txtRelatedScreenshotTitle;

        [BindView(Resource.Id.txtFeedbackStatus)]
        TextView txtFeedbackStatus;

        [BindView(Resource.Id.txtYourMessage)]
        TextView txtYourMessage;


        [BindView(Resource.Id.txtEnquiryDetails)]
        TextView txtEnquiryDetails;

        [BindView(Resource.Id.txtforMyhouse)]
        TextView txtforMyhouse;

        [BindView(Resource.Id.txtStatus)]
        TextView txtStatus;

        [BindView(Resource.Id.txtFeedback)]
        EditText txtFeedback;


        [BindView(Resource.Id.recyclerView)]
        RecyclerView recyclerView;


        SubmittedFeedbackDetails submittedFeedback;

        SubmittedNewEnquiryContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {

                SetToolBarTitle("Submitted Enquiry");


                string selectedFeedback = UserSessions.GetSelectedFeedback(PreferenceManager.GetDefaultSharedPreferences(this));
                submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);
                TextViewUtils.SetTextSize9(txtRelatedScreenshotTitle);
                TextViewUtils.SetTextSize10(txtYourMessage);
                TextViewUtils.SetTextSize14(txtFeedback, txtforMyhouse);
                TextViewUtils.SetTextSize16(txtEnquiryDetails, txtFeedbackStatus, txtStatus);

                // set font
                //TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutGeneralEnquiry1);
                //TextViewUtils.SetMuseoSans300Typeface(txtMaxCharacters, txtRelatedScreenshotTitle, txtMaxImageContent);
                //TextViewUtils.SetMuseoSans500Typeface(txtstep1of2, IwantToEnquire, btnNext, uploadSupportingDoc);

                //add listener 
                //txtGeneralEnquiry1.AddTextChangedListener(new InputFilterFormField(txtGeneralEnquiry1, txtInputLayoutGeneralEnquiry1));
                //txtGeneralEnquiry1.TextChanged += TextChanged;
                //txtGeneralEnquiry1.SetOnTouchListener(this);

                adapter = new FeedbackGeneralEnquiryStepOneImageRecyclerAdapter(true);
                adapter.Insert(new Base.Models.AttachedImage()
                {
                    ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                });
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);

                // adapter listener
                adapter.SelectClickEvent += Adapter_SelectClickEvent;
                mPresenter = new SubmittedNewEnquiryPresenter(this, submittedFeedback, this);
                this.userActionsListener.Start();


            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void Adapter_SelectClickEvent(object sender, int e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                AttachedImage selectedImage = adapter.GetItemObject(e);
                var fullImageIntent = new Intent(this, typeof(FeedbackDetailsFullScreenImageActivity));
                fullImageIntent.PutExtra(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE, JsonConvert.SerializeObject(selectedImage));
                StartActivity(fullImageIntent);
            }
        }


        public void SetPresenter(SubmittedNewEnquiryContract.IUserActionsListener userActionListener)
        {
            this.userActionListener = userActionListener;
        }

        public void ShowImages(List<AttachedImage> list)
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

        public override int ResourceId()
        {
            return Resource.Layout.SubmittedNewEnquiryView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void ShowInputData(string feedbackId, string feedbackStatus, string feedbackCode, string dateTime, string accountNoName, string feedback)
        {
            try
            {
                // txtFeedbackId.Text = feedbackId;
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

                // txtFeedbackDateTime.Text = dateTime;
                // txtAccountNo.Text = accountNoName;
                txtFeedback.Text = feedback;
                TextViewUtils.SetTextSize14(txtFeedback);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}