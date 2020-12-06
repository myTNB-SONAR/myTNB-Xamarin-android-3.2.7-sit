using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.AppBar;
using myTNB_Android.Src.Base.Activity;

using myTNB_Android.Src.Utils;
using System;
using System.Runtime;
using myTNB.Mobile;
using Newtonsoft.Json;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB;
using myTNB.Mobile.API.Managers.Rating;
using System.Linq;
using myTNB_Android.Src.Common;
using System.Collections.Generic;
using Android.Runtime;
using myTNB.Mobile.API.Models.Rating.PostSubmitRating;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.ApplicationStatusRating.Activity
{
    [Activity(Label = "Rate"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class RateUsActivity : BaseActivityCustom
    {
        private TextView txtPageTitleInfo;
        public RatingBar ratingBar;
        private TextView txtTitleQuestion;
        private TextView txtTellUsTitleInfo;
        private Button btnSubmit;
        private ListView rating_list_view;
        private SelectItemAdapter selectItemAdapter;
        private EditText txtTellUsMore;
        int selectedRating;
        public GetCustomerRatingMasterResponse getCustomerRatingMasterResponse;
        private QuestionAnswerSetsModel sequence1;
        private QuestionAnswerSetsModel sequence2;
        private QuestionAnswerSetsModel sequence3;
        List<Item> ratingItemList = new List<Item>();
        GetApplicationStatusDisplay applicationDetailDisplay;
        string srNumber = string.Empty;
        string applicationID = string.Empty;
        string backendAppID = string.Empty;
        string applicationType = string.Empty;
        string questionCategoryValue = string.Empty;
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
                UserEntity loggedUser = UserEntity.GetActive();
                
                    if (ConnectionUtils.HasInternetConnection(this))
                    {
                        ShowProgressDialog();
                    PostSubmitRatingResponse postSubmitRatingResponse = await RatingManager.Instance.SubmitRating(
                             loggedUser.UserName
                            , loggedUser.MobileNo
                            , srNumber
                            ,applicationID
                            ,backendAppID
                            ,applicationType
                            , questionCategoryValue
                            ,List <RatingAnswers> ratingInput);
                        HideProgressDialog();
                        if (postSubmitRatingResponse.StatusDetail.IsSuccess)
                        {
                            Toast.MakeText(this, postSubmitRatingResponse.StatusDetail.Message ?? string.Empty, ToastLength.Long).Show();
                            if (IsSaveFlow)
                            {
                                Intent applicationLandingIntent = new Intent(this, typeof(ApplicationStatusLandingActivity));
                                StartActivity(applicationLandingIntent);
                                IsSaveFlow = false;
                            }
                            SetResult(Result.Ok, new Intent());
                            Finish();
                        }
                        else
                        {
                            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                                .SetTitle(postSubmitRatingResponse.StatusDetail.Title)
                                .SetMessage(postSubmitRatingResponse.StatusDetail.Message)
                                .SetCTALabel(postSubmitRatingResponse.StatusDetail.PrimaryCTATitle)
                                .SetSecondaryCTALabel(postSubmitRatingResponse.StatusDetail.SecondaryCTATitle)
                                .SetSecondaryCTAaction(() => ShowStatusLanding())
                                .Build();
                            whereisMyacc.Show();
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
        public async void GetCustomerRatingAsync()
        {
            ShowProgressDialog();
            getCustomerRatingMasterResponse = await RatingManager.Instance.GetCustomerRatingMaster();
            if (!getCustomerRatingMasterResponse.StatusDetail.IsSuccess)
            {
                //ShowApplicaitonPopupMessage(this, response.StatusDetail);
            }



            try
            {
                FirebaseAnalyticsUtils.LogClickEvent(this, "Rating Buttom Clicked");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            HideProgressDialog();
        }




        AndroidX.Fragment.App.Fragment currentFragment;

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
        /* public async void ShowApplicaitonPopupMessage(Activity context, StatusDetail statusDetail)
         {
             MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                 .SetTitle(statusDetail.Title)
                 .SetMessage(statusDetail.Message)
                 .SetCTALabel(statusDetail.PrimaryCTATitle)
                 .Build();
             whereisMyacc.Show();

         }*/
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
                txtTellUsMore.Hint = Utility.GetLocalizedLabel("ApplicationStatusRating", "freeTextPlaceHolder");
                btnSubmit.Text = Utility.GetLocalizedLabel("ApplicationStatusRating", "submit");

                rating_list_view.multi(AbsListView.IMultiChoiceModeListener);
                btnSubmit.Enabled = false;
                btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);

                
                btnSubmit.Click += OnSubmit;
                rating_list_view.ItemClick += OnItemClick;
                GetCustomerRatingAsync();
                // OnLoadMainFragment();
                Bundle extras = Intent.Extras;
            


                if (extras != null && Intent.Extras.ContainsKey("selectedRating") && extras.ContainsKey("customerRatingMasterResponse") && extras.GetString("applicationDetailDisplay"))
                {
                    selectedRating = Convert.ToInt32(extras.GetString("selectedRating"));
                    applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));

                    srNumber = ApplicationDetails.SRNumber.IsValid()
                              ? ApplicationDetails.SRNumber
                              : ApplicationDetails.ApplicationRatingDetail != null
                                  ? ApplicationDetails.ApplicationRatingDetail.SRNo ?? string.Empty
                                  : string.Empty;
                    applicationID = ApplicationDetails.ApplicationDetail.ApplicationId ?? string.Empty;
                    backendAppID = ApplicationDetails.ApplicationDetail.BackendReferenceNo ?? string.Empty;
                    applicationType = ApplicationDetails.ApplicationTypeCode ?? string.Empty;

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
        internal void RatingQuestions()
        {
            if (getCustomerRatingMasterResponse != null && getCustomerRatingMasterResponse.Content != null)
            {
                if (selectedRating == 1)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["1"];
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["1"];
                }
                else if (selectedRating == 2)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["2"];
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["2"];
                }
                else if (selectedRating == 3)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["3"];
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["3"];
                }
                else if (selectedRating == 4)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["4"];
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["4"];
                }
                else if (selectedRating == 5)
                {
                    txtPageTitleInfo.Text = sequence1.AnswerDetail.AnswerSetValue["5"];
                    txtTitleQuestion.Text = sequence2.QuestionDetail.QuestionDescription["5"];
                }
            }
        }
        [Preserve]
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Item selectedItem = selectItemAdapter.GetItemObject(e.Position);

            ratingItemList.ForEach(item =>
            {
                item.selected = (selectedItem.title == item.title) ? true : false;
            });
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





      
    }
}
