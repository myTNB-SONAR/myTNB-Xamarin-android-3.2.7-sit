using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.EnergyBudgetRating.Fargment;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.ServiceDistruptionRating.Adapter;
using myTNB_Android.Src.ServiceDistruptionRating.Model;
using myTNB_Android.Src.ServiceDistruptionRating.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;
using static myTNB_Android.Src.ServiceDistruptionRating.Model.RateUsStar;

namespace myTNB_Android.Src.ServiceDistruptionRating.Activity
{
    [Activity(Label = "@string/app_name"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PreLogin")]
    public class ServiceDistruptionRatingActivity : BaseAppCompatActivity, ServiceDistruptionRatingContract.IView
    {
        private ServiceDistruptionRatingPresenter mPresenter;
        private ServiceDistruptionRatingContract.IUserActionsListener userActionsListener;

        private RecyclerView recyclerView;

        private RecyclerView recyclerViewGrid;

        private Button btnCancel;

        private Button btnSubmit;

        private LinearLayout btnYesAbsolutely;

        private LinearLayout btnNotReally;

        private GridLayoutManager layoutManager;

        private ImproveSelectAdapter adapterGrid;

        private EditText txtTellUsMore;

        private TextView txtTellUsMoreHintCount;

        private TextView titleSetUpFeedback;

        private TextView titleNumber;

        private TextView titleYes;

        private TextView titleNo;

        private TextView titleselectApply;

        private ImageView img_displayYes;

        private ImageView img_displayNo;

        private List<ImproveSelectModel> activeQuestionList = new List<ImproveSelectModel>();

        private List<QuestionModel> improveSelectModels = new List<QuestionModel>();

        private List<RateUsStar> QuestionListing = new List<RateUsStar>();

        private List<RateUsStar> activeQuestionListNo = new List<RateUsStar>();

        private List<RateUsStar> activeQuestionListYes = new List<RateUsStar>();

        int initialnumber = 250;

        private List<Item> YesOrNoItem = new List<Item>();
        private bool selectedItem;
        private bool btnYesClick = false;
        private bool btnNoClick = false;
        private bool selectIconComplete = false;
        List<InputOptionValue> InputOptionValueList = new List<InputOptionValue>();

        public override int ResourceId()
        {
            return Resource.Layout.FeedBackServiceDistruptionSubmitRatingView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new ServiceDistruptionRatingPresenter(this);

                //if (Arguments.ContainsKey("StarFromNotificationDetailPage"))
                //{
                //    YesOrNoSelect = Arguments.GetString("StarFromNotificationDetailPage");
                //}
                //if (Arguments.ContainsKey("StarFromNotificationDetailPageQuestionNo"))
                //{
                //    activeQuestionListNo = JsonConvert.DeserializeObject<List<RateUsStar>>(Arguments.GetString("StarFromNotificationDetailPageQuestionNo"));
                //}
                //if (Arguments.ContainsKey("StarFromNotificationDetailPageQuestionYes"))
                //{
                //    activeQuestionListYes = JsonConvert.DeserializeObject<List<RateUsStar>>(Arguments.GetString("StarFromNotificationDetailPageQuestionYes"));
                //}

                recyclerView = FindViewById<RecyclerView>(Resource.Id.question_recycler_view);
                recyclerViewGrid = FindViewById<RecyclerView>(Resource.Id.question_recycler_view_grid);
                txtTellUsMore = FindViewById<EditText>(Resource.Id.txtTellUsMore);
                txtTellUsMoreHintCount = FindViewById<TextView>(Resource.Id.txtTellUsMoreHintCount);
                btnCancel = FindViewById<Button>(Resource.Id.btnNoTQ);
                btnSubmit = FindViewById<Button>(Resource.Id.btnShare);
                btnYesAbsolutely = FindViewById<LinearLayout>(Resource.Id.btnYes_Layout);
                btnNotReally = FindViewById<LinearLayout>(Resource.Id.btnNo_Layout);
                titleSetUpFeedback = FindViewById<TextView>(Resource.Id.titleSetUpFeedback);
                titleNumber = FindViewById<TextView>(Resource.Id.titleNumber);
                titleYes = FindViewById<TextView>(Resource.Id.titleYes);
                titleNo = FindViewById<TextView>(Resource.Id.titleNo);
                titleselectApply = FindViewById<TextView>(Resource.Id.titleselectApply);
                img_displayYes = FindViewById<ImageView>(Resource.Id.img_displayYes);
                img_displayNo = FindViewById<ImageView>(Resource.Id.img_displayNo);

                TextViewUtils.SetMuseoSans500Typeface(btnCancel, btnSubmit);
                TextViewUtils.SetMuseoSans500Typeface(titleSetUpFeedback, titleYes, titleNo, titleNumber, titleselectApply);
                TextViewUtils.SetMuseoSans500Typeface(txtTellUsMore);
                TextViewUtils.SetMuseoSans300Typeface(txtTellUsMoreHintCount);

                TextViewUtils.SetTextSize16(btnCancel, btnSubmit);
                TextViewUtils.SetTextSize16(titleSetUpFeedback, titleNumber);
                TextViewUtils.SetTextSize12(txtTellUsMore, titleselectApply);
                TextViewUtils.SetTextSize10(titleYes, titleNo, txtTellUsMoreHintCount);

                titleNumber.Text = Utility.GetLocalizedLabel("FeedBackSD", "helpfulTitle");
                btnCancel.Text = Utility.GetLocalizedLabel("FeedBackSD", "btnCancel");
                btnSubmit.Text = Utility.GetLocalizedLabel("FeedBackSD", "btnSubmit");
                txtTellUsMore.Hint = Utility.GetLocalizedLabel("FeedBackSD", "tellusMore");
                titleYes.Text = Utility.GetLocalizedLabel("FeedBackSD", "thumbUpYes");
                titleNo.Text = Utility.GetLocalizedLabel("FeedBackSD", "thumbDownNo");
                titleSetUpFeedback.Text = Utility.GetLocalizedLabel("FeedBackSD", "title");
                titleselectApply.Text = Utility.GetLocalizedLabel("FeedBackSD", "titleSelectApplies");

                txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackSD", "hinttellusMore"));

                ModelSelectYesNo();
                enableBtnYes();
                disableBtnNo();
                DummyDataYes();
                layoutManager = new GridLayoutManager(this.ApplicationContext, 2);
                adapterGrid = new ImproveSelectAdapter(this.ApplicationContext, improveSelectModels);
                recyclerViewGrid.SetLayoutManager(layoutManager);
                recyclerViewGrid.SetAdapter(adapterGrid);
                adapterGrid.SelectUpdate += OnSelectUpdate;

                txtTellUsMore.TextChanged += TxtTellUsMore_TextChanged;
                btnYesAbsolutely.Click += BtnYesAbsolutely_Click;
                btnNotReally.Click += BtnNotReally_Click;

                DisableShareButton();

                btnCancel.Click += delegate
                {
                    OnBackPressed();
                };

                btnSubmit.Click += delegate
                {
                    MyTNBAccountManagement.GetInstance().SetIsFinishFeedback(true);
                    OnBackPressed();
                    prepareDataSubmit();
                };

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                //FirebaseAnalyticsUtils.SetScreenName(this, "Post-Payment Rating");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void DummyDataNo()
        {
            var selectdata = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackNo",
                IconCategories = "Not notified promptly on outage/restoration",
                IconPosition = 1,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata);

            var selectdata2 = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackNo",
                IconCategories = "Inaccurate information",
                IconPosition = 2,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata2);

            var selectdata3 = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackNo",
                IconCategories = "Inconsistent updates",
                IconPosition = 3,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata3);

            var selectdata4 = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackNo",
                IconCategories = "Hard to access to customer careline",
                IconPosition = 4,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata4);
            //adapterGrid.NotifyDataSetChanged();
        }

        public void DummyDataYes()
        {
            var selectdata = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackYes",
                IconCategories = "Notified promptly on outage/restoration",
                IconPosition = 1,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata);

            var selectdata2 = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackYes",
                IconCategories = "Accurate information",
                IconPosition = 2,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata2);

            var selectdata3 = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackYes",
                IconCategories = "Consistent updates",
                IconPosition = 3,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata3);

            var selectdata4 = new QuestionModel
            {
                WLTYQuestionId = "1",
                ModelCategories = "FeedbackYes",
                IconCategories = "Quick access to customer careline",
                IconPosition = 4,
                IsSelected = false,
            };
            improveSelectModels.Add(selectdata4);
            //adapterGrid.NotifyDataSetChanged();
        }

        private void ModelSelectYesNo()
        {
            var item1 = new Item
            {
                key = "YES",
                selected = true
            };
            YesOrNoItem.Add(item1);

            var item2 = new Item
            {
                key = "NO",
                selected = false
            };
            YesOrNoItem.Add(item2);
        }

        private void BtnYesAbsolutely_Click(object sender, EventArgs e)
        {
            try
            {
                if (!YesOrNoItem[0].selected)
                {
                    foreach (var item in YesOrNoItem)
                    {
                        if (item.key.Equals("YES"))
                        {
                            item.selected = true;
                        }
                        else
                        {
                            item.selected = false;
                        }
                    }
                    btnYesClick = true;
                    btnNoClick = false;
                    enableBtnYes();
                    disableBtnNo();
                    improveSelectModels.Clear();
                    DummyDataYes();
                    adapterGrid.NotifyDataSetChanged();
                    //injectSelectDataYes();
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
                if (!YesOrNoItem[1].selected)
                {
                    foreach (var item in YesOrNoItem)
                    {
                        if (item.key.Equals("NO"))
                        {
                            item.selected = true;
                        }
                        else
                        {
                            item.selected = false;
                        }
                    }
                    btnYesClick = false;
                    btnNoClick = true;
                    enableBtnNo();
                    disableBtnYes();
                    improveSelectModels.Clear();
                    DummyDataNo();
                    adapterGrid.NotifyDataSetChanged();
                    //injectSelectDataNo();
                    DisableShareButton();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
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
                        ModelCategories = "FeedbackNo",
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
                        ModelCategories = "FeedbackYes",
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

        void OnSelectUpdate(object sender, int position)
        {
            try
            {
                if (adapterGrid != null)
                {
                    if (adapterGrid.IsAllQuestionAnswered())
                    {
                        foreach (var item in YesOrNoItem)
                        {
                            if (item.selected)
                            {
                                selectIconComplete = true;
                            }
                        }

                        if (selectIconComplete)
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
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableShareButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.green_button_background);
        }

        public void enableBtnYes()
        {
            btnYesAbsolutely.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.blue_out_line_thin);
            img_displayYes.SetImageResource(Resource.Drawable.thumb_up_blue_yes);
            titleYes.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.powerBlue));
        }

        public void disableBtnYes()
        {
            btnYesAbsolutely.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.silver_chalice_button_outline);
            img_displayYes.SetImageResource(Resource.Drawable.thumb_up_grey_yes);
            titleYes.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverchalice));
        }

        public void enableBtnNo()
        {
            btnNotReally.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.blue_out_line_thin);
            img_displayNo.SetImageResource(Resource.Drawable.thumb_down_no_blue);
            titleNo.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.powerBlue));
        }

        public void disableBtnNo()
        {
            btnNotReally.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.silver_chalice_button_outline);
            img_displayNo.SetImageResource(Resource.Drawable.thumb_down_no_grey);
            titleNo.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverchalice));
        }

        private void TxtTellUsMore_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                string tellusmore = txtTellUsMore.Text.Trim();
                if (tellusmore.Length > 0)
                {
                    int remainNumber = initialnumber - tellusmore.Length;
                    txtTellUsMoreHintCount.Text = (remainNumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackSD", "hinttellusMore"));
                }
                else
                {
                    txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackSD", "hinttellusMore"));
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowThankYouFeedbackTooltips()
        {
            try
            {
                SetupFeedBackFragment.Create(this, SetupFeedBackFragment.ToolTipType.IMAGE_HEADER)
                    .SetCTALabel(Utility.GetLocalizedLabel("PushNotificationDetails", "feedback2SuccessButton"))
                    .SetTitle(Utility.GetLocalizedLabel("PushNotificationDetails", "feedback2SuccessTitle"))
                    .Build().Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                Finish();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void SetPresenter(ServiceDistruptionRatingContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowProgressDialog()
        {
            throw new NotImplementedException();
        }

        public void HideProgressDialog()
        {
            throw new NotImplementedException();
        }

        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string exception)
        {
            throw new NotImplementedException();
        }

        public void ShowGetQuestionSuccess(GetRateUsQuestionResponse response)
        {
            throw new NotImplementedException();
        }
    }
}