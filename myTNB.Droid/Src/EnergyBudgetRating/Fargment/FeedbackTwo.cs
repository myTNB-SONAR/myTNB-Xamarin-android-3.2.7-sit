﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.EnergyBudgetRating.Activity;
using myTNB.AndroidApp.Src.EnergyBudgetRating.Adapter;
using myTNB.AndroidApp.Src.EnergyBudgetRating.Model;
using myTNB.AndroidApp.Src.EnergyBudgetRating.MVP;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.Rating.Adapter;
using myTNB.AndroidApp.Src.Rating.Model;
using myTNB.AndroidApp.Src.Rating.MVP;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.EnergyBudgetRating.Model.RateUsStar;

namespace myTNB.AndroidApp.Src.EnergyBudgetRating.Fargment
{
    public class FeedbackTwo : AndroidX.Fragment.App.Fragment, EnergyBudgetRatingContract.IView
    {

        private EnergyBudgetRatingPresenter mPresenter;
        private EnergyBudgetRatingContract.IUserActionsListener userActionsListener;

        private FrameLayout rootView;

        private RecyclerView recyclerView;

        private RecyclerView recyclerViewGrid;

        private Button btnNoThank;

        private Button btnShare;

        private GridLayoutManager layoutManager;

        private RateUsStarsCustomAdapter adapter;

        private ImproveSelectAdapter adapterGrid;

        private EditText txtTellUsMore;

        private TextView txtTellUsMoreHintCount;

        private TextView titleSetUpFeedback;

        private TextView titleImproveSuggest;

        private TextView bodyImproveSuggest;

        private TextView titleStarRating;

        private List<ImproveSelectModel> activeQuestionList = new List<ImproveSelectModel>();

        private List<QuestionModel> improveSelectModels = new List<QuestionModel>();

        private List<RateUsStar> QuestionListing= new List<RateUsStar>();

        List<InputOptionValue> InputOptionValueList = new List<InputOptionValue>();

        int initialnumber = 250;

        private RateUsStar raseusStar;
        private EnergyBudgetRatingActivity ratingActivity;
        private string merchantTransId;
        private string deviceID;
        private int selectedRating;
        private string questionCatId;
        private bool fromClick;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mainView = inflater.Inflate(Resource.Layout.FeedBackEnergyBudget_Two_SubmitRatingView, container, false);
            try
            {
                mPresenter = new EnergyBudgetRatingPresenter(this);
                ratingActivity = ((EnergyBudgetRatingActivity)Activity);
               
                if (Arguments.ContainsKey("StarFromDashboardFragmentPage"))
                {
                    selectedRating = Arguments.GetInt("StarFromDashboardFragmentPage");
                }
                if (Arguments.ContainsKey("StarFromDashboardFragmentPageQuestion"))
                {
                    QuestionListing = JsonConvert.DeserializeObject<List<RateUsStar>>(Arguments.GetString("StarFromDashboardFragmentPageQuestion"));
                }

                recyclerView = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view);
                recyclerViewGrid = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view_grid);
                //rootView = mainView.FindViewById<FrameLayout>(Resource.Id.baseView);
                txtTellUsMore = mainView.FindViewById<EditText>(Resource.Id.txtTellUsMore);
                txtTellUsMoreHintCount = mainView.FindViewById<TextView>(Resource.Id.txtTellUsMoreHintCount);
                titleSetUpFeedback = mainView.FindViewById<TextView>(Resource.Id.titleSetUpFeedback);
                titleImproveSuggest = mainView.FindViewById<TextView>(Resource.Id.titleImproveSuggest);
                bodyImproveSuggest = mainView.FindViewById<TextView>(Resource.Id.bodyImproveSuggest);
                titleStarRating = mainView.FindViewById<TextView>(Resource.Id.titleLinearlayout);
                btnNoThank = mainView.FindViewById<Button>(Resource.Id.btnNoTQ);
                btnShare = mainView.FindViewById<Button>(Resource.Id.btnShare);

                TextViewUtils.SetMuseoSans500Typeface(btnNoThank, btnShare);
                TextViewUtils.SetMuseoSans500Typeface(titleSetUpFeedback, titleImproveSuggest, titleStarRating);
                TextViewUtils.SetMuseoSans500Typeface(txtTellUsMore);
                TextViewUtils.SetMuseoSans300Typeface(txtTellUsMoreHintCount, bodyImproveSuggest);

                TextViewUtils.SetTextSize16(btnNoThank, btnShare);
                TextViewUtils.SetTextSize16(titleSetUpFeedback, titleStarRating);
                TextViewUtils.SetTextSize12(txtTellUsMore);
                TextViewUtils.SetTextSize12(titleImproveSuggest);
                TextViewUtils.SetTextSize10(txtTellUsMoreHintCount, bodyImproveSuggest);

                titleSetUpFeedback.Text = Utility.GetLocalizedLabel("FeedBackEB", "title");
                titleImproveSuggest.Text = Utility.GetLocalizedLabel("FeedBackEB", "improveTitle");
                bodyImproveSuggest.Text = Utility.GetLocalizedLabel("FeedBackEB", "titleSelectApplies");
                txtTellUsMore.Hint = Utility.GetLocalizedLabel("FeedBackEB", "tellusMore");
                btnNoThank.Text = Utility.GetLocalizedLabel("FeedBackEB", "btnNoThank");
                btnShare.Text = Utility.GetLocalizedLabel("FeedBackEB", "btnShare");

                txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackEB", "hinttellusMore"));

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

                injectStarData();
                injectSelectData();
                SelectStarFromData(selectedRating);
                DisableShareButton();

                btnNoThank.Click += delegate
                {
                    MyTNBAccountManagement.GetInstance().SetIsFromClickAdapter(0);
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

        private void TxtTellUsMore_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                string tellusmore = txtTellUsMore.Text.Trim();
                if (tellusmore.Length > 0)
                {
                    int remainNumber = initialnumber - tellusmore.Length;
                    txtTellUsMoreHintCount.Text = (remainNumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackEB", "hinttellusMore"));
                }
                else
                {
                    txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " " + Utility.GetLocalizedLabel("FeedBackEB", "hinttellusMore"));
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
                        StarTitleChange(starSelect);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void StarTitleChange(int position)
        {
            try
            {
                if (position.Equals(1))
                {
                    titleStarRating.Text = InputOptionValueList[0].InputOptionValues;
                }
                else if (position.Equals(2))
                {
                    titleStarRating.Text = InputOptionValueList[1].InputOptionValues;
                }
                else if (position.Equals(3))
                {
                    titleStarRating.Text = InputOptionValueList[2].InputOptionValues;
                }
                else if (position.Equals(4))
                {
                    titleStarRating.Text = InputOptionValueList[3].InputOptionValues;
                }
                else
                {
                    titleStarRating.Text = InputOptionValueList[4].InputOptionValues;
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SelectStarFromData(int position)
        {
            try
            {
                if (adapter != null)
                {
                    if (MyTNBAccountManagement.GetInstance().IsFromClickAdapter() == position)
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
                        StarTitleChange(starSelect);
                        //adapter.NotifyDataSetChanged();

                        if (adapterGrid.IsAllQuestionAnswered())
                        {
                            EnableShareButton();
                        }
                        else
                        {
                            DisableShareButton();
                        }
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
                if (QuestionListing[0].QuestionType == "Rating")
                {
                    foreach (InputOptionValue data in QuestionListing[0].InputOptionValueList)
                    {
                        InputOptionValueList.Add(data);
                    }
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

        public void injectSelectData()
        {
            try
            {
                int IconPosition = 1;
                improveSelectModels.Clear();
                List<RateUsStar> activeQuestionListNew = new List<RateUsStar>();
                activeQuestionListNew = QuestionListing.GetRange(1, 4);
                foreach (RateUsStar data in activeQuestionListNew)
                {
                    var selectdata = new QuestionModel
                    {
                        WLTYQuestionId = data.WLTYQuestionId,
                        ModelCategories = "FeedbackOne",
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

            /*var data1 = new ImproveSelectModel
            {
                ModelCategories = "feedback_two",
                IconCategories = "1",
                IsSelected = false,
            };
            improveSelectModels.Add(data1);*/   
            
            adapterGrid.NotifyDataSetChanged();
        }

        public void prepareDataSubmit()
        {
            try
            {

                SubmitDataModel.InputAnswerT submitDataModel = new SubmitDataModel.InputAnswerT();
                List<SubmitDataModel.InputAnswerDetails> inputAnswerDetails = new List<SubmitDataModel.InputAnswerDetails>();
                SubmitDataModel.InputAnswerDetails yesOrNo = new SubmitDataModel.InputAnswerDetails();
                
                var rating = new SubmitDataModel.InputAnswerDetails()
                {
                    WLTYQuestionId = QuestionListing[0].WLTYQuestionId,
                    RatingInput = (MyTNBAccountManagement.GetInstance().IsFromClickAdapter() + 1).ToString(),
                    MultilineInput = string.Empty,
                };
                inputAnswerDetails.Add(rating);

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

                var multiline = new SubmitDataModel.InputAnswerDetails()
                {
                    WLTYQuestionId = QuestionListing[5].WLTYQuestionId,
                    RatingInput = string.Empty,
                    MultilineInput = txtTellUsMore.Text.Trim()
                };
                inputAnswerDetails.Add(multiline);
                string questiontypeId = "5";

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
