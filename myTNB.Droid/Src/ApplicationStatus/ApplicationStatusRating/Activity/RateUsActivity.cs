using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.API.Managers.Rating;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB.Mobile.API.Models.Rating.PostSubmitRating;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
namespace myTNB_Android.Src.ApplicationStatusRating.Activity
{
    [Activity(Label = "Rate"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class RateUsActivity : BaseActivityCustom, View.IOnTouchListener
    {
        private CoordinatorLayout rootView;
        private TextInputLayout txtInputLayoutTellUsMore;
        private TextView txtPageTitleInfo;
        private RatingBar ratingBar;
        private TextView txtTitleQuestion;
        private TextView txtTellUsTitleInfo;
        private Button btnSubmit;
        private ListView rating_list_view;
        private SelectItemAdapter selectItemAdapter;
        private EditText txtTellUsMore;
        private int selectedRating;
        private string selectedAnswerDescriptions = string.Empty;
        private string selectedAnswerValues = string.Empty;
        private GetCustomerRatingMasterResponse getCustomerRatingMasterResponse;
        private PostSubmitRatingResponse postSubmitRatingResponse;
        private QuestionAnswerSetsModel sequence1;
        private QuestionAnswerSetsModel sequence2;
        private QuestionAnswerSetsModel sequence3;
        private List<Item> ratingItemList = new List<Item>();
        private List<Item> selectedRatingItemList = new List<Item>();
        private GetApplicationStatusDisplay applicationDetailDisplay;
        string srNumber = string.Empty;
        string applicationID = string.Empty;
        string backendAppID = string.Empty;
        string applicationType = string.Empty;
        string questionCategoryValue = string.Empty;
        string txtPageTitleInfoValue = string.Empty;
        int txtTitleQuestionValue;

        private List<RatingAnswers> ratingAnswers = new List<RatingAnswers>();
        private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] Rating");
            SubmitRating();
        }
        private async void SubmitRating()
        {
            try
            {
                if (sequence1 != null)
                {
                    RatingAnswers ratingAnswer = new RatingAnswers();
                    ratingAnswer.QuestionId = sequence1.QuestionDetail.QuestionId;
                    ratingAnswer.QuestionDescription = sequence1.QuestionDetail.QuestionDescription["0"];
                    ratingAnswer.AnswerTypeId = sequence1.AnswerDetail.AnswerTypeId;
                    ratingAnswer.AnswerDescription = txtPageTitleInfo.Text;
                    ratingAnswer.AnswerValue = txtPageTitleInfoValue;
                    ratingAnswers.Add(ratingAnswer);
                }
                if (sequence2 != null)
                {
                    selectedRatingItemList.ForEach(item =>
                    {
                        selectedAnswerDescriptions += item.title + ";";
                        selectedAnswerValues += item.type + ";";
                    });
                    if (selectedAnswerDescriptions != string.Empty && selectedAnswerDescriptions.Length != 0)
                    {
                        selectedAnswerDescriptions = selectedAnswerDescriptions.Remove(selectedAnswerDescriptions.Length - 1, 1);
                    }
                    if (selectedAnswerValues != string.Empty && selectedAnswerValues.Length != 0)
                    {
                        selectedAnswerValues = selectedAnswerValues.Remove(selectedAnswerValues.Length - 1, 1);
                    }

                    RatingAnswers ratingAnswer = new RatingAnswers();
                    ratingAnswer.QuestionId = sequence2.QuestionDetail.QuestionId;
                    ratingAnswer.QuestionDescription = txtTitleQuestion.Text;
                    ratingAnswer.AnswerTypeId = txtTitleQuestionValue;
                    ratingAnswer.AnswerDescription = selectedAnswerDescriptions;
                    ratingAnswer.AnswerValue = selectedAnswerValues;
                    ratingAnswers.Add(ratingAnswer);
                }
                if (sequence3 != null)
                {
                    RatingAnswers ratingAnswer = new RatingAnswers();
                    ratingAnswer.QuestionId = sequence3.QuestionDetail.QuestionId;
                    ratingAnswer.QuestionDescription = sequence3.QuestionDetail.QuestionDescription["0"];
                    ratingAnswer.AnswerTypeId = sequence3.AnswerDetail.AnswerTypeId;
                    ratingAnswer.AnswerDescription = txtTellUsMore.Text;
                    ratingAnswer.AnswerValue = txtTellUsMore.Text;
                    ratingAnswers.Add(ratingAnswer);
                }

                UserEntity loggedUser = UserEntity.GetActive();

                if (ConnectionUtils.HasInternetConnection(this))
                {
                    ShowProgressDialog();
                    postSubmitRatingResponse = await RatingManager.Instance.SubmitRating(
                             loggedUser.UserName
                            , loggedUser.MobileNo
                            , srNumber
                            , applicationID
                            , backendAppID
                            , applicationType
                            , questionCategoryValue
                            , ratingAnswers);
                    HideProgressDialog();
                    if (postSubmitRatingResponse.StatusDetail.IsSuccess)
                    {
                        Intent intent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                        intent.PutExtra("applicationRated", selectedRating.ToString());
                        intent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(applicationDetailDisplay));
                        intent.PutExtra("submitRatingResponseStatus", JsonConvert.SerializeObject(postSubmitRatingResponse.StatusDetail));
                        StartActivity(intent);
                        SetResult(Result.Ok, new Intent());
                        Finish();
                    }
                    else
                    {
                        ShowApplicaitonPopupMessage(this, postSubmitRatingResponse.StatusDetail);
                    }
                }
                else
                {
                    ShowNoInternetSnackbar();
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
            this.SetIsClicked(false);
        }
        public async void ShowApplicaitonPopupMessage(Android.App.Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();

        }
        private Snackbar mNoInternetSnackbar;

        private string PAGE_ID = "Rate";

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatus_RateUsQuestionItemView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                txtPageTitleInfo = FindViewById<TextView>(Resource.Id.txtPageTitleInfo);
                ratingBar = FindViewById<RatingBar>(Resource.Id.ratingBar);
                txtTitleQuestion = FindViewById<TextView>(Resource.Id.txtTitleQuestion);
                txtTellUsTitleInfo = FindViewById<TextView>(Resource.Id.txtTellUsTitleInfo);
                btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);
                rating_list_view = FindViewById<ListView>(Resource.Id.rating_list_view);
                txtTellUsMore = FindViewById<EditText>(Resource.Id.txtTellUsMore);
                txtInputLayoutTellUsMore = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutTellUsMore);
                txtTellUsMore.SetOnTouchListener(this);
                rootView = FindViewById<CoordinatorLayout>(Resource.Id.rootview);
                txtTellUsMore.Hint = Utility.GetLocalizedLabel("ApplicationStatusRating", "freeTextPlaceHolder");

                btnSubmit.Text = Utility.GetLocalizedLabel("ApplicationStatusRating", "submit");
                txtTellUsMore.TextChanged += TextChanged;
                txtTellUsMore.SetOnTouchListener(this);

                btnSubmit.Enabled = false;
                btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);

                TextViewUtils.SetMuseoSans300Typeface(txtTellUsMore);
                TextViewUtils.SetMuseoSans500Typeface(txtPageTitleInfo, txtTitleQuestion, txtTellUsTitleInfo);

                rating_list_view.ItemClick += OnItemClick;

                Bundle extras = Intent.Extras;

                SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusRating", "title"));

                if (extras != null && Intent.Extras.ContainsKey("selectedRating") && extras.ContainsKey("customerRatingMasterResponse") && extras.ContainsKey("applicationDetailDisplay"))
                {
                    selectedRating = Convert.ToInt32(extras.GetString("selectedRating"));
                    if (selectedRating != 0)
                    {

                        btnSubmit.Enabled = true;
                        btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
                    }
                    else
                    {
                        btnSubmit.Enabled = false;
                        btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);

                    }
                    applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));

                    srNumber = applicationDetailDisplay.SRNumber != null
                              ? applicationDetailDisplay.SRNumber
                              : applicationDetailDisplay.ApplicationRatingDetail != null
                                && applicationDetailDisplay.ApplicationRatingDetail.CustomerRating != null
                                  ? applicationDetailDisplay.ApplicationRatingDetail.CustomerRating.SRNo ?? string.Empty
                                  : string.Empty;
                    applicationID = applicationDetailDisplay.ApplicationDetail.ApplicationId ?? string.Empty;
                    backendAppID = applicationDetailDisplay.ApplicationDetail.BackendReferenceNo ?? string.Empty;
                    applicationType = applicationDetailDisplay.ApplicationTypeCode ?? string.Empty;

                    ratingBar.Rating = selectedRating;

                    getCustomerRatingMasterResponse = JsonConvert.DeserializeObject<GetCustomerRatingMasterResponse>(extras.GetString("customerRatingMasterResponse"));

                    if (getCustomerRatingMasterResponse != null && getCustomerRatingMasterResponse.Content != null)
                    {
                        sequence1 = getCustomerRatingMasterResponse.Content.QuestionAnswerSets.Where(x => x.Sequence == 1).FirstOrDefault();
                        sequence2 = getCustomerRatingMasterResponse.Content.QuestionAnswerSets.Where(x => x.Sequence == 2).FirstOrDefault();
                        sequence3 = getCustomerRatingMasterResponse.Content.QuestionAnswerSets.Where(x => x.Sequence == 3).FirstOrDefault();
                        questionCategoryValue = getCustomerRatingMasterResponse.Content.QuestionCategoryDescription;
                        RatingQuestions();
                    }
                    foreach (var answer in sequence2.AnswerDetail.AnswerSetValue)
                    {
                        Item item = new Item();
                        item.title = answer.Value;
                        item.type = answer.Key;
                        item.selected = false;
                        ratingItemList.Add(item);
                    }

                    selectItemAdapter = new SelectItemAdapter(this, ratingItemList);
                    rating_list_view.Adapter = selectItemAdapter;

                }
                ratingBar.RatingBarChange += (o, e) =>
                {
                    ratingBar.Rating = e.Rating;
                    selectedRating = ((int)e.Rating);
                    if (selectedRating != 0)
                    {

                        btnSubmit.Enabled = true;
                        btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
                    }
                    else
                    {
                        btnSubmit.Enabled = false;
                        btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);

                    }
                    RatingQuestions();
                };
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                FeedBackCharacCount();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

        }
        private void FeedBackCharacCount()
        {
            try
            {
                string feedback = txtTellUsMore.Text;
                int char_count = 0;

                if (!string.IsNullOrEmpty(feedback))
                {
                    char_count = feedback.Length;
                }

                if (char_count > 0)
                {
                    int char_left = Constants.FEEDBACK_CHAR_LIMIT - char_count;
                    txtInputLayoutTellUsMore.Error = char_left + " " + GetString(Resource.String.feedback_character_left);
                }
                else
                {
                    txtInputLayoutTellUsMore.Error = GetString(Resource.String.feedback_total_character_left);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        internal void RatingQuestions()
        {
            if (getCustomerRatingMasterResponse != null && getCustomerRatingMasterResponse.Content != null)
            {
                if (selectedRating == 1)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["1"];
                    txtPageTitleInfoValue = "1";
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["1"];
                    txtTitleQuestionValue = 1;
                }
                else if (selectedRating == 2)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["2"];
                    txtPageTitleInfoValue = "2";
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["2"];
                    txtTitleQuestionValue = 2;
                }
                else if (selectedRating == 3)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["3"];
                    txtPageTitleInfoValue = "3";
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["3"];
                    txtTitleQuestionValue = 3;
                }
                else if (selectedRating == 4)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["4"];
                    txtPageTitleInfoValue = "4";
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["4"];
                    txtTitleQuestionValue = 4;
                }
                else if (selectedRating == 5)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["5"];
                    txtPageTitleInfoValue = "5";
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["5"];
                    txtTitleQuestionValue = 5;
                }
            }
        }
        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Item selectedItem = selectItemAdapter.GetItemObject(e.Position);

            if (selectedRatingItemList.Contains(selectedItem))
            {
                selectedItem.selected = false;
                selectedRatingItemList.Remove(selectedItem);
            }
            else
            {
                selectedItem.selected = true;
                selectedRatingItemList.Add(selectedItem);
            }

            selectItemAdapter.NotifyDataSetChanged();

        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Application Rating");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (v is EditText)
            {
                EditText eTxtView = v as EditText;
                if (e.Action == MotionEventActions.Up)
                {
                    if (eTxtView.Id == Resource.Id.txtTellUsMore)
                    {
                        txtInputLayoutTellUsMore.Hint = Utility.GetLocalizedLabel("ApplicationStatusRating", "freeTextTitle");
                    }
                }
            }
            return false;
        }
    }
}