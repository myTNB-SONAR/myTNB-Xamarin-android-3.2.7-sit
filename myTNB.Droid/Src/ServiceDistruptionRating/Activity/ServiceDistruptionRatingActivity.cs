using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ServiceDistruptionRating.Adapter;
using myTNB_Android.Src.ServiceDistruptionRating.Model;
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
    public class ServiceDistruptionRatingActivity : BaseAppCompatActivity
    {

        private RecyclerView recyclerView;

        private RecyclerView recyclerViewGrid;

        private Button btnNoThank;

        private Button btnShare;

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

        private RateUsStar raseusStar;
        private string merchantTransId;
        private string deviceID;
        private int selectedRating;
        private bool btnYesClick = false;
        private bool btnNoClick = false;
        private bool selectStarComplete = false;
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
                //mPresenter = new EnergyBudgetRatingPresenter(this);

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
                btnNoThank = FindViewById<Button>(Resource.Id.btnNoTQ);
                btnShare = FindViewById<Button>(Resource.Id.btnShare);
                btnYesAbsolutely = FindViewById<LinearLayout>(Resource.Id.btnYes_Layout);
                btnNotReally = FindViewById<LinearLayout>(Resource.Id.btnNo_Layout);
                titleSetUpFeedback = FindViewById<TextView>(Resource.Id.titleSetUpFeedback);
                titleNumber = FindViewById<TextView>(Resource.Id.titleNumber);
                titleYes = FindViewById<TextView>(Resource.Id.titleYes);
                titleNo = FindViewById<TextView>(Resource.Id.titleNo);
                titleselectApply = FindViewById<TextView>(Resource.Id.titleselectApply);
                img_displayYes = FindViewById<ImageView>(Resource.Id.img_displayYes);
                img_displayNo = FindViewById<ImageView>(Resource.Id.img_displayNo);

                TextViewUtils.SetMuseoSans500Typeface(btnNoThank, btnShare);
                TextViewUtils.SetMuseoSans500Typeface(titleSetUpFeedback, titleYes, titleNo, titleNumber);
                TextViewUtils.SetMuseoSans500Typeface(txtTellUsMore);
                TextViewUtils.SetMuseoSans300Typeface(txtTellUsMoreHintCount, titleselectApply);

                TextViewUtils.SetTextSize16(btnNoThank, btnShare);
                TextViewUtils.SetTextSize16(titleSetUpFeedback, titleNumber);
                TextViewUtils.SetTextSize12(txtTellUsMore);
                TextViewUtils.SetTextSize10(titleYes, titleNo, titleselectApply, txtTellUsMoreHintCount);

                btnNoThank.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "btnNoThank");
                btnShare.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "btnShare");
                txtTellUsMore.Hint = Utility.GetLocalizedLabel("FeedBackEBNotification", "tellusMore");
                titleYes.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "thumbUpYes");
                titleNo.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "thumbDownNo");
                titleSetUpFeedback.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "title");
                //titleNumber.Text = activeQuestionListNo[0].Question; //Utility.GetLocalizedLabel("FeedBackEBNotification", "titleNumberOne");
                //TitleNumberTwo.Text = activeQuestionListNo[6].Question; //Utility.GetLocalizedLabel("FeedBackEBNotification", "titleNumberTwo");
                titleselectApply.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "titleSelectApplies");

                txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackEBNotification", "hinttellusMore"));

                DummyDataNo();
                layoutManager = new GridLayoutManager(this.ApplicationContext, 2);
                adapterGrid = new ImproveSelectAdapter(this.ApplicationContext, improveSelectModels);
                recyclerViewGrid.SetLayoutManager(layoutManager);
                recyclerViewGrid.SetAdapter(adapterGrid);
                adapterGrid.SelectUpdate += OnSelectUpdate;

                txtTellUsMore.TextChanged += TxtTellUsMore_TextChanged;
                btnYesAbsolutely.Click += BtnYesAbsolutely_Click;
                btnNotReally.Click += BtnNotReally_Click;

                //DisableShareButton();

                btnNoThank.Click += delegate
                {
                    //ShowSumitRateUsSuccess();
                };

                btnShare.Click += delegate
                {
                    //MyTNBAccountManagement.GetInstance().SetIsFinishFeedback(true);
                    //prepareDataSubmit();
                };

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
            btnShare.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableShareButton()
        {
            btnShare.Enabled = true;
            btnShare.Background = ContextCompat.GetDrawable(this.ApplicationContext, Resource.Drawable.green_button_background);
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

        //public override void OnBackPressed()
        //{
        //    try
        //    {
        //        int count = this.SupportFragmentManager.BackStackEntryCount;
        //        Log.Debug("OnBackPressed", "fragment stack count :" + count);
        //        if (currentFragment is FeedbackTwo || currentFragment is FeedbackOne)
        //        {
        //            Finish();
        //        }
        //        else
        //        {
        //            this.SupportFragmentManager.PopBackStack();
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Utility.LoggingNonFatalError(e);
        //    }
        //}


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
    }
}