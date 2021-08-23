using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.EnergyBudgetRating.Activity;
using myTNB_Android.Src.EnergyBudgetRating.Adapter;
using myTNB_Android.Src.EnergyBudgetRating.Model;
using myTNB_Android.Src.EnergyBudgetRating.MVP;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Rating.Adapter;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.Rating.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using static myTNB_Android.Src.EnergyBudgetRating.Model.RateUsStar;

namespace myTNB_Android.Src.EnergyBudgetRating.Fargment
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

        private RateUsStarsAdapter adapter;

        private ImproveSelectAdapter adapterGrid;

        private EditText txtTellUsMore;

        private TextView txtTellUsMoreHintCount;

        private List<RateUsStar> activeQuestionList = new List<RateUsStar>();

        private List<ImproveSelectModel> improveSelectModels = new List<ImproveSelectModel>();

        int initialnumber = 250;

        private RateUsStar raseusStar;
        private EnergyBudgetRatingActivity ratingActivity;
        private string merchantTransId;
        private string deviceID;
        private int selectedRating;
        private string questionCatId;

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

                if (Arguments.ContainsKey(Constants.QUESTION_ID_CATEGORY))
                {
                    questionCatId = Arguments.GetString(Constants.QUESTION_ID_CATEGORY);
                }
                if (Arguments.ContainsKey(Constants.DEVICE_ID_PARAM))
                {
                    deviceID = Arguments.GetString(Constants.DEVICE_ID_PARAM);
                }
                if (Arguments.ContainsKey(Constants.MERCHANT_TRANS_ID))
                {
                    merchantTransId = Arguments.GetString(Constants.MERCHANT_TRANS_ID);
                }
                if (Arguments.ContainsKey(Constants.SELECTED_RATING))
                {
                    selectedRating = Arguments.GetInt(Constants.SELECTED_RATING);
                }

                recyclerView = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view);
                recyclerViewGrid = mainView.FindViewById<RecyclerView>(Resource.Id.question_recycler_view_grid);
                //rootView = mainView.FindViewById<FrameLayout>(Resource.Id.baseView);
                txtTellUsMore = mainView.FindViewById<EditText>(Resource.Id.txtTellUsMore);
                txtTellUsMoreHintCount = mainView.FindViewById<TextView>(Resource.Id.txtTellUsMoreHintCount);
                btnNoThank = mainView.FindViewById<Button>(Resource.Id.btnNoTQ);
                btnShare = mainView.FindViewById<Button>(Resource.Id.btnShare);

                btnNoThank.Text = Utility.GetLocalizedCommonLabel("submit");
                btnShare.Text = Utility.GetLocalizedCommonLabel("submit");
                txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " characters left");

                layoutManager = new GridLayoutManager(Activity.ApplicationContext, 1);
                adapter = new RateUsStarsAdapter(Activity.ApplicationContext, activeQuestionList, selectedRating);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);
                adapter.RatingUpdate += OnRatingUpdate;

                layoutManager = new GridLayoutManager(Activity.ApplicationContext, 2);
                adapterGrid = new ImproveSelectAdapter(Activity.ApplicationContext, improveSelectModels);
                recyclerViewGrid.SetLayoutManager(layoutManager);
                recyclerViewGrid.SetAdapter(adapterGrid);
                adapterGrid.SelectUpdate += OnSelectUpdate;

                txtTellUsMore.TextChanged += TxtTellUsMore_TextChanged;
                injectDemoData();
                injectSelectData();
                //this.userActionsListener.GetQuestions(questionCatId);

                /*btnShare.Click += delegate
                {
                    if (adapter.GetInputAnswers().Count > 0)
                    {
                        this.userActionsListener.PrepareSubmitRateUsRequest(merchantTransId, deviceID, adapter.GetInputAnswers());
                    }
                };*/
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
                    txtTellUsMoreHintCount.Text = (remainNumber.ToString() + " characters left").ToString();
                }
                else
                {
                    txtTellUsMoreHintCount.Text = (initialnumber.ToString() + " characters left");
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
                    if (adapter.IsAllQuestionAnswered())
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

        void OnSelectUpdate(object sender, int position)
        {
            try
            {
                //adapterGrid.NotifyDataSetChanged();
                if (adapter != null)
                {
                    if (adapter.IsAllQuestionAnswered())
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

        public void injectDemoData()
        {
            List<InputOptionValue> InputOptionValueList = new List<InputOptionValue>();
            var inputOptionValueList = new InputOptionValue
            {
                InputOptionRate = "1",
                InputOptionValues = "2",
            };
            InputOptionValueList.Add(inputOptionValueList);

            var test = new RateUsStar
            {
                WLTYQuestionId = "",
                Question = "How satisfied are you with the myTNB app?",
                QuestionCategory = "Bill Related",
                QuestionType = "Rating",
                IsActive = true,
                IsMandatory = true,
                InputOptionValueList = InputOptionValueList,
            };
            activeQuestionList.Add(test);
            adapter.NotifyDataSetChanged();
        }

        public void injectSelectData()
        {
            var data1 = new ImproveSelectModel
            {
                ModelCategories = "feedback_two",
                IconCategories = "1",
                IsSelected = false,
            };
            improveSelectModels.Add(data1);

            var data2 = new ImproveSelectModel
            {
                ModelCategories = "feedback_two",
                IconCategories = "2",
                IsSelected = false,
            };
            improveSelectModels.Add(data2);

            var data3 = new ImproveSelectModel
            {
                ModelCategories = "feedback_two",
                IconCategories = "3",
                IsSelected = false,
            };
            improveSelectModels.Add(data3);

            var data4 = new ImproveSelectModel
            {
                ModelCategories = "feedback_two",
                IconCategories = "4",
                IsSelected = false,
            };
            improveSelectModels.Add(data4);
            adapterGrid.NotifyDataSetChanged();
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
