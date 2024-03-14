using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.Android.Src.Base;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.EnergyBudgetRating.Activity;
using myTNB.Android.Src.EnergyBudgetRating.Adapter;
using myTNB.Android.Src.EnergyBudgetRating.Model;
using myTNB.Android.Src.EnergyBudgetRating.MVP;
using myTNB.Android.Src.MyTNBService.Response;
using myTNB.Android.Src.Rating.Adapter;
using myTNB.Android.Src.Rating.Model;
using myTNB.Android.Src.Rating.MVP;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static myTNB.Android.Src.EnergyBudgetRating.Model.RateUsStar;

namespace myTNB.Android.Src.EnergyBudgetRating.Fargment
{
    public class FeedbackOne : AndroidX.Fragment.App.Fragment, EnergyBudgetRatingContract.IView
    {

        private EnergyBudgetRatingPresenter mPresenter;
        private EnergyBudgetRatingContract.IUserActionsListener userActionsListener;

        private FrameLayout rootView;

        private RecyclerView recyclerView;

        private RecyclerView recyclerViewGrid;

        private Button btnNoThank;

        private Button btnShare;

        private LinearLayout btnYesAbsolutely;

        private LinearLayout btnNotReally;

        private GridLayoutManager layoutManager;

        private RateUsStarsCustomAdapter adapter;

        private ImproveSelectAdapter adapterGrid;

        private EditText txtTellUsMore;

        private TextView txtTellUsMoreHintCount;
        
        private TextView titleSetUpFeedback;
        
        private TextView titleNumber;

        private TextView titleYes;

        private TextView titleNo;

        private TextView titleselectApply;

        private TextView TitleNumberTwo;

        private ImageView img_displayYes;

        private ImageView img_displayNo;

        private List<ImproveSelectModel> activeQuestionList = new List<ImproveSelectModel>();

        private List<QuestionModel> improveSelectModels = new List<QuestionModel>();

        private List<RateUsStar> QuestionListing = new List<RateUsStar>();

        private List<RateUsStar> activeQuestionListNo = new List<RateUsStar>();

        private List<RateUsStar> activeQuestionListYes = new List<RateUsStar>();

        int initialnumber = 250;

        private RateUsStar raseusStar;
        private EnergyBudgetRatingActivity ratingActivity;
        private string merchantTransId;
        private string deviceID;
        private int selectedRating;
        private string YesOrNoSelect;
        private bool btnYesClick = false;
        private bool btnNoClick = false;
        private bool selectStarComplete = false;
        private bool selectIconComplete = false;
        List<InputOptionValue> InputOptionValueList = new List<InputOptionValue>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.FeedBackEnergyBudget_One_SubmitRatingView, container, false);
            try
            {
                mPresenter = new EnergyBudgetRatingPresenter(this);
                ratingActivity = ((EnergyBudgetRatingActivity)Activity);

                if (Arguments.ContainsKey("StarFromNotificationDetailPage"))
                {
                    YesOrNoSelect = Arguments.GetString("StarFromNotificationDetailPage");
                }
                if (Arguments.ContainsKey("StarFromNotificationDetailPageQuestionNo"))
                {
                    activeQuestionListNo = JsonConvert.DeserializeObject<List<RateUsStar>>(Arguments.GetString("StarFromNotificationDetailPageQuestionNo"));
                }
                if (Arguments.ContainsKey("StarFromNotificationDetailPageQuestionYes"))
                {
                    activeQuestionListYes = JsonConvert.DeserializeObject<List<RateUsStar>>(Arguments.GetString("StarFromNotificationDetailPageQuestionYes"));
                }

                recyclerView = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view);
                recyclerViewGrid = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view_grid);
                //rootView = mainView.FindViewById<FrameLayout>(Resource.Id.baseView);
                txtTellUsMore = mainView.FindViewById<EditText>(Resource.Id.txtTellUsMore);
                txtTellUsMoreHintCount = mainView.FindViewById<TextView>(Resource.Id.txtTellUsMoreHintCount);
                btnNoThank = mainView.FindViewById<Button>(Resource.Id.btnNoTQ);
                btnShare = mainView.FindViewById<Button>(Resource.Id.btnShare);
                btnYesAbsolutely = mainView.FindViewById<LinearLayout>(Resource.Id.btnYes_Layout);
                btnNotReally = mainView.FindViewById<LinearLayout>(Resource.Id.btnNo_Layout);
                titleSetUpFeedback = mainView.FindViewById<TextView>(Resource.Id.titleSetUpFeedback);
                titleNumber = mainView.FindViewById<TextView>(Resource.Id.titleNumber);
                titleYes = mainView.FindViewById<TextView>(Resource.Id.titleYes);
                titleNo = mainView.FindViewById<TextView>(Resource.Id.titleNo);
                titleselectApply = mainView.FindViewById<TextView>(Resource.Id.titleselectApply);
                TitleNumberTwo = mainView.FindViewById<TextView>(Resource.Id.TitleNumberTwo);
                img_displayYes = mainView.FindViewById<ImageView>(Resource.Id.img_displayYes);
                img_displayNo = mainView.FindViewById<ImageView>(Resource.Id.img_displayNo);

                TextViewUtils.SetMuseoSans500Typeface(btnNoThank, btnShare);
                TextViewUtils.SetMuseoSans500Typeface(titleSetUpFeedback, titleYes, titleNo, TitleNumberTwo, titleNumber);
                TextViewUtils.SetMuseoSans500Typeface(txtTellUsMore);
                TextViewUtils.SetMuseoSans300Typeface(txtTellUsMoreHintCount, titleselectApply);

                TextViewUtils.SetTextSize16(btnNoThank, btnShare);
                TextViewUtils.SetTextSize16(titleSetUpFeedback, TitleNumberTwo, titleNumber);
                TextViewUtils.SetTextSize12(txtTellUsMore);
                TextViewUtils.SetTextSize10(titleYes, titleNo, titleselectApply, txtTellUsMoreHintCount);

                btnNoThank.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "btnNoThank");
                btnShare.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "btnShare");
                txtTellUsMore.Hint = Utility.GetLocalizedLabel("FeedBackEBNotification", "tellusMore");
                titleYes.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "thumbUpYes");
                titleNo.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "thumbDownNo");
                titleSetUpFeedback.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "title");
                titleNumber.Text = activeQuestionListNo[0].Question; //Utility.GetLocalizedLabel("FeedBackEBNotification", "titleNumberOne");
                TitleNumberTwo.Text = activeQuestionListNo[6].Question; //Utility.GetLocalizedLabel("FeedBackEBNotification", "titleNumberTwo");
                titleselectApply.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "titleSelectApplies");

                txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackEBNotification", "hinttellusMore"));

                layoutManager = new GridLayoutManager(Activity.ApplicationContext, 5);
                adapter = new RateUsStarsCustomAdapter(Activity.ApplicationContext, activeQuestionList, selectedRating);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);
                adapter.RatingUpdate += OnRatingUpdate;

                layoutManager = new GridLayoutManager(Activity.ApplicationContext, 2);
                adapterGrid = new ImproveSelectAdapter(Activity.ApplicationContext, improveSelectModels);
                recyclerViewGrid.SetLayoutManager(layoutManager);
                recyclerViewGrid.SetAdapter(adapterGrid);
                adapterGrid.SelectUpdate += OnSelectUpdate;

                txtTellUsMore.TextChanged += TxtTellUsMore_TextChanged;
                btnYesAbsolutely.Click += BtnYesAbsolutely_Click;
                btnNotReally.Click += BtnNotReally_Click;

                if (YesOrNoSelect.Equals("Yes"))
                {
                    btnYesClick = true;
                    btnNoClick = false;
                    enableBtnYes();
                    disableBtnNo();
                    injectSelectDataYes();
                }
                else
                {
                    btnNoClick = true;
                    btnYesClick = false;
                    enableBtnNo();
                    disableBtnYes();
                    injectSelectDataNo();
                }

                injectStarData();
                DisableShareButton();

                btnNoThank.Click += delegate
                {
                    ShowSumitRateUsSuccess();
                };

                btnShare.Click += delegate
                {
                    MyTNBAccountManagement.GetInstance().SetIsFinishFeedback(true);
                    prepareDataSubmit();
                };

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return mainView;
        }

        private void BtnYesAbsolutely_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnYesClick)
                {
                    btnYesClick = true;
                    btnNoClick = false;
                }
                else
                {
                    btnYesClick = false;
                }

                if (btnYesClick && !btnNoClick)
                {
                    enableBtnYes();
                    disableBtnNo();
                    injectSelectDataYes();
                    DisableShareButton();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void BtnNotReally_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNoClick)
                {
                    btnNoClick = true;
                    btnYesClick = false;
                }
                else
                {
                    btnNoClick = false;
                }

                if (!btnYesClick && btnNoClick)
                {
                    enableBtnNo();
                    disableBtnYes();
                    injectSelectDataNo();
                    DisableShareButton();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void enableBtnYes()
        {
            btnYesAbsolutely.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.blue_out_line_thin);
            img_displayYes.SetImageResource(Resource.Drawable.thumb_up_blue_yes);
            titleYes.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.powerBlue));
        }

        public void disableBtnYes()
        {
            btnYesAbsolutely.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.silver_chalice_button_outline);
            img_displayYes.SetImageResource(Resource.Drawable.thumb_up_grey_yes);
            titleYes.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverchalice));
        }

        public void enableBtnNo()
        {
            btnNotReally.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.blue_out_line_thin);
            img_displayNo.SetImageResource(Resource.Drawable.thumb_down_no_blue);
            titleNo.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.powerBlue));
        }

        public void disableBtnNo()
        {
            btnNotReally.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.silver_chalice_button_outline);
            img_displayNo.SetImageResource(Resource.Drawable.thumb_down_no_grey);
            titleNo.SetTextColor(ContextCompat.GetColorStateList(this.Activity, Resource.Color.silverchalice));
        }

        private void TxtTellUsMore_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                string tellusmore = txtTellUsMore.Text.Trim();
                if (tellusmore.Length > 0)
                {
                    int remainNumber = initialnumber - tellusmore.Length;
                    txtTellUsMoreHintCount.Text = (remainNumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackEBNotification", "hinttellusMore"));
                }
                else
                {
                    txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackEBNotification", "hinttellusMore"));
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void SetPresenter(EnergyBudgetRatingContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        void OnRatingUpdate(object sender, int position)
        {
            try
            {
                if (adapter != null)
                {
                    if (MyTNBAccountManagement.GetInstance().IsFromClickAdapter() != position)
                    {
                        MyTNBAccountManagement.GetInstance().SetIsFromClickAdapter(position);
                        List<ImproveSelectModel> activeStarSelectList = new List<ImproveSelectModel>();
                        int starSelect = position + 1;
                        foreach (ImproveSelectModel data in activeQuestionList)
                        {
                            int NoStar = int.Parse(data.IconCategories);
                            if (NoStar < starSelect || NoStar == starSelect)
                            {
                                data.IsSelected = true;
                            }
                            else
                            {
                                data.IsSelected = false;
                            }
                            activeStarSelectList.Add(data);
                        }
                        activeQuestionList = null;
                        activeQuestionList = activeStarSelectList;
                        layoutManager = new GridLayoutManager(Activity.ApplicationContext, 5);
                        adapter = new RateUsStarsCustomAdapter(Activity.ApplicationContext, activeQuestionList, selectedRating);
                        recyclerView.SetLayoutManager(layoutManager);
                        recyclerView.SetAdapter(adapter);
                        adapter.RatingUpdate += OnRatingUpdate;
                        selectStarComplete = true;

                        if (adapterGrid != null)
                        {
                            if (adapterGrid.IsAllQuestionAnswered())
                            {
                                EnableShareButton();
                            }
                            else
                            {
                                DisableShareButton();
                            }
                        }
                        else
                        {
                            DisableShareButton();
                        }
                    }
                }
                else
                {
                    selectStarComplete = false;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnSelectUpdate(object sender, int position)
        {
            try
            {
                //adapterGrid.NotifyDataSetChanged();
                if (adapterGrid != null)
                {
                    if (adapterGrid.IsAllQuestionAnswered())
                    {
                        selectIconComplete = true;
                        if (selectStarComplete)
                        {
                            EnableShareButton();
                        }
                        else
                        {
                            DisableShareButton();
                        }
                    }
                    else
                    {
                        selectIconComplete = false;
                        DisableShareButton();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DisableShareButton()
        {
            btnShare.Enabled = false;
            btnShare.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableShareButton()
        {
            btnShare.Enabled = true;
            btnShare.Background = ContextCompat.GetDrawable(Activity.ApplicationContext, Resource.Drawable.green_button_background);
        }

        public void injectStarData()
        {
            try
            {
                foreach (InputOptionValue data in QuestionListing[0].InputOptionValueList)
                {
                    InputOptionValueList.Add(data);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            var data1 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "1",
                IsSelected = false,
            };
            activeQuestionList.Add(data1);

            var data2 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "2",
                IsSelected = false,
            };
            activeQuestionList.Add(data2);

            var data3 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "3",
                IsSelected = false,
            };
            activeQuestionList.Add(data3);

            var data4 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "4",
                IsSelected = false,
            };
            activeQuestionList.Add(data4);

            var data5 = new ImproveSelectModel
            {
                ModelCategories = "star",
                IconCategories = "5",
                IsSelected = false,
            };
            activeQuestionList.Add(data5);
            adapter.NotifyDataSetChanged();
        }

        public void injectSelectDataNo()
        {
            try
            {
                int IconPosition = 1;
                improveSelectModels.Clear();
                List<RateUsStar> activeQuestionListNoListNew = new List<RateUsStar>();
                activeQuestionListNoListNew = activeQuestionListNo.GetRange(1, 4);
                foreach (RateUsStar data in activeQuestionListNoListNew)
                {
                    var selectdata = new QuestionModel
                    {
                        WLTYQuestionId = data.WLTYQuestionId,
                        ModelCategories = "FeedbackTwoA",
                        IconCategories = data.Question,
                        IconPosition = IconPosition++,
                        IsSelected = false,
                    };
                    improveSelectModels.Add(selectdata);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }           
            adapterGrid.NotifyDataSetChanged();
        }

        public void injectSelectDataYes()
        {
            try
            {
                int IconPosition = 1;
                improveSelectModels.Clear();
                List<RateUsStar> activeQuestionListYesListNew = new List<RateUsStar>();
                activeQuestionListYesListNew = activeQuestionListYes.GetRange(1, 4);
                foreach (RateUsStar data in activeQuestionListYesListNew)
                {
                    var selectdata = new QuestionModel
                    {
                        WLTYQuestionId = data.WLTYQuestionId,
                        ModelCategories = "FeedbackTwoB",
                        IconCategories = data.Question,
                        IconPosition = IconPosition++,
                        IsSelected = false,
                    };
                    improveSelectModels.Add(selectdata);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }            
            adapterGrid.NotifyDataSetChanged();
        }

        public void prepareDataSubmit()
        {
            try
            {

                SubmitDataModel.InputAnswerT submitDataModel = new SubmitDataModel.InputAnswerT();
                List<SubmitDataModel.InputAnswerDetails> inputAnswerDetails = new List<SubmitDataModel.InputAnswerDetails>();
                SubmitDataModel.InputAnswerDetails yesOrNo = new SubmitDataModel.InputAnswerDetails();
                if (btnYesClick)
                {
                    if (activeQuestionListYes != null)
                    {
                        yesOrNo.WLTYQuestionId = activeQuestionListYes[0].WLTYQuestionId;
                        yesOrNo.RatingInput = string.Empty;
                        yesOrNo.MultilineInput = activeQuestionListYes[0].InputOptionValueList[0].InputOptionValues;
                    }
                }
                else
                {
                    if (activeQuestionListNo != null)
                    {
                        yesOrNo.WLTYQuestionId = activeQuestionListNo[0].WLTYQuestionId;
                        yesOrNo.RatingInput = string.Empty;
                        yesOrNo.MultilineInput = activeQuestionListYes[0].InputOptionValueList[1].InputOptionValues;
                    }
                }
                inputAnswerDetails.Add(yesOrNo);

                foreach (QuestionModel item in improveSelectModels)
                {
                    if (item.IsSelected)
                    {
                        var data = new SubmitDataModel.InputAnswerDetails()
                        {
                            WLTYQuestionId = item.WLTYQuestionId,
                            RatingInput = string.Empty,
                            MultilineInput = item.IsSelected ? "YES" : "NO",
                        };
                        inputAnswerDetails.Add(data);
                    }
                }

                if (btnYesClick)
                {
                    var multiline = new SubmitDataModel.InputAnswerDetails()
                    {
                        WLTYQuestionId = activeQuestionListYes[5].WLTYQuestionId,
                        RatingInput = string.Empty,
                        MultilineInput = txtTellUsMore.Text.Trim()
                    };
                    inputAnswerDetails.Add(multiline);

                    var rating = new SubmitDataModel.InputAnswerDetails()
                    {
                        WLTYQuestionId = activeQuestionListYes[6].WLTYQuestionId,
                        RatingInput = (MyTNBAccountManagement.GetInstance().IsFromClickAdapter() + 1).ToString(),
                        MultilineInput = string.Empty,
                    };
                    inputAnswerDetails.Add(rating);
                }
                else
                {
                    var multiline = new SubmitDataModel.InputAnswerDetails()
                    {
                        WLTYQuestionId = activeQuestionListNo[5].WLTYQuestionId,
                        RatingInput = string.Empty,
                        MultilineInput = txtTellUsMore.Text.Trim()
                    };
                    inputAnswerDetails.Add(multiline);

                    var rating = new SubmitDataModel.InputAnswerDetails()
                    {
                        WLTYQuestionId = activeQuestionListNo[6].WLTYQuestionId,
                        RatingInput = (MyTNBAccountManagement.GetInstance().IsFromClickAdapter() + 1).ToString(),
                        MultilineInput = string.Empty,
                    };
                    inputAnswerDetails.Add(rating);
                }
                string questiontypeId = "6";
                mPresenter.SubmitRateUs(inputAnswerDetails, questiontypeId);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public void ShowError(string exception)
        {
            Snackbar showErrorSnackbar = Snackbar.Make(rootView, exception, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"),
                (view) =>
                {

                    // EMPTY WILL CLOSE SNACKBAR
                }
            );
            View v = showErrorSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            showErrorSnackbar.Show();
        }

        public void ShowGetQuestionSuccess(GetRateUsQuestionResponse response)
        {
            try
            {
                if (response != null)
                {
                    if (response.GetData().Count > 0)
                    {
                        foreach (RateUsQuestion que in response.GetData())
                        {
                            /*if (que.IsActive)
                            {
                                activeQuestionList.Add(que);
                            }*/
                        }
                        adapter.NotifyDataSetChanged();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRetryOptionsUnknownException(Exception exception)
        {

        }

        public void ShowSumitRateUsSuccess()
        {
            Bundle bundle = new Bundle();
            ratingActivity.nextFragment(this, bundle);
        }
    }
}
