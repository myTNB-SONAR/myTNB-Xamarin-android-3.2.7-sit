using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;


using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Adapter;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.ManageCards.MVP;
using myTNB_Android.Src.MyAccount.Adapter;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.ManageCards.Activity
{
    [Activity(Label = "@string/manage_cards_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.ManageCards")]
    public class ManageCardsActivity : BaseActivityCustom, ManageCardsContract.IView
    {

        [BindView(Resource.Id.layout_cards)]
        LinearLayout layoutCards;

        [BindView(Resource.Id.layout_autopay)]
        LinearLayout layoutAutopay;

        [BindView(Resource.Id.layout_label_autopay)]
        LinearLayout layoutLabelAutopay;

        [BindView(Resource.Id.layout_autopay_listview)]
        LinearLayout layoutListviewAutopay;

        [BindView(Resource.Id.layout_empty_cards)]
        LinearLayout layoutEmptyCards;

        [BindView(Resource.Id.recyclerView)]
        RecyclerView mRecyclerView;

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtManageCardsTitle)]
        TextView txtManageCardsTitle;

        [BindView(Resource.Id.txtEmptyCard)]
        TextView txtEmptyCard;

        [BindView(Resource.Id.txtManageAutoPayTitle)]
        TextView txtManageAutoPayTitle;

        [BindView(Resource.Id.txtTitle)]
        TextView txtTitle;

        [BindView(Resource.Id.txtValue)]
        TextView txtValue;

        [BindView(Resource.Id.listView)]
        ListView listView;

        [BindView(Resource.Id.btnAutoPay)]
        Button btnAutoPay;

/*        [BindView(Resource.Id.btnLoadMore)]
        Button btnLoadMore;*/

        [BindView(Resource.Id.imgEmptyCard)]
        ImageView imgEmptyCard;

        ManageCardsAdapter mAdapter;

        MyAccountAdapter AccAdapter;

        LinearLayoutManager mLayoutManager;

        ManageCardsContract.IUserActionsListener userActionsListener;
        ManageCardsPresenter mPresenter;

        MaterialDialog progress;

        private string PAGE_ID = "ManageCards";

        AlertDialog removeDialog;

        List<CreditCardData> cardsList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.CREDIT_CARD_LIST))
                    {
                        string cardsListString = extras.GetString(Constants.CREDIT_CARD_LIST);
                        //cardsList = JsonConvert.DeserializeObject<List<CreditCardData>>(cardsListString);
                        cardsList = DeSerialze<List<CreditCardData>>(cardsListString);
                    }
                }

                //Console.WriteLine("Card List " + cardsListString);

                progress = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.manage_cards_progress_title))
                    .Content(GetString(Resource.String.manage_cards_progress_content))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                mAdapter = new ManageCardsAdapter(true);
                mAdapter.RemoveClick += MAdapter_RemoveClick;
                mLayoutManager = new LinearLayoutManager(this);
                mRecyclerView.SetLayoutManager(mLayoutManager);
                mRecyclerView.SetAdapter(mAdapter);

                TextViewUtils.SetMuseoSans300Typeface(txtEmptyCard, txtValue);
                TextViewUtils.SetMuseoSans500Typeface(txtManageAutoPayTitle, txtManageCardsTitle, txtTitle);

                txtManageAutoPayTitle.Text = GetLabelByLanguage("titleAutoPay");
                txtTitle.Text = GetLabelByLanguage("titleNoAutoPay");
                txtValue.Text = GetLabelByLanguage("bodyNoAutoPay");
                txtManageCardsTitle.Text = GetLabelByLanguage("titleCard");
                txtEmptyCard.Text = GetLabelByLanguage("noCards");

                AccAdapter = new MyAccountAdapter(this, false);
                listView.Adapter = AccAdapter;


                mPresenter = new ManageCardsPresenter(this);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]
        private void MAdapter_RemoveClick(object sender, int e)
        {
            try
            {
                if (removeDialog != null && removeDialog.IsShowing)
                {
                    removeDialog.Dismiss();
                }
                CreditCardData creditCardData = mAdapter.GetItemObject(e);
                string lastDigit = creditCardData.LastDigits.Substring(creditCardData.LastDigits.Length - 4);
                removeDialog = new AlertDialog.Builder(this)

                    .SetTitle(GetLabelByLanguage("removeCardTitle"))
                    .SetMessage(string.Format(GetLabelByLanguage("removeCardMessage"), lastDigit))
                    .SetNegativeButton(GetLabelCommonByLanguage("cancel"),
                    delegate
                    {
                        removeDialog.Dismiss();
                    })
                    .SetPositiveButton(GetLabelCommonByLanguage("ok"),
                    delegate
                    {
                        this.userActionsListener.OnRemove(creditCardData, e);
                    })
                    .Show()
                    ;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }


        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ManageCardsViewNew;
        }

        public void SetPresenter(ManageCardsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Manage Cards");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnAutoPay)]
        void OnClickAddAccount(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                ShowAutoPayAccount();
            }
        }

        public void ShowAutoPayAccount()
        {
            try
            {
                if (cardsList.Count == 0)
                {
                    layoutEmptyCards.Visibility = ViewStates.Visible;
                    layoutCards.Visibility = ViewStates.Gone;
                    layoutLabelAutopay.Visibility = ViewStates.Gone;
                    layoutAutopay.Visibility = ViewStates.Gone;
                    layoutListviewAutopay.Visibility = ViewStates.Gone;
                }
                else
                {
                    layoutLabelAutopay.Visibility = ViewStates.Visible;
                    layoutAutopay.Visibility = ViewStates.Gone;
                    layoutCards.Visibility = ViewStates.Visible;
                    layoutListviewAutopay.Visibility = ViewStates.Visible;
                    layoutEmptyCards.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowCards()
        {
            try
            {
                if (cardsList.Count == 0)
                {
                    layoutEmptyCards.Visibility = ViewStates.Visible;
                    layoutCards.Visibility = ViewStates.Gone;
                    layoutLabelAutopay.Visibility = ViewStates.Gone;
                    layoutAutopay.Visibility = ViewStates.Gone;
                }
                else
                {
                    layoutLabelAutopay.Visibility = ViewStates.Visible;
                    layoutAutopay.Visibility = ViewStates.Visible;
                    layoutCards.Visibility = ViewStates.Visible;
                    layoutEmptyCards.Visibility = ViewStates.Gone;
                    mAdapter.AddAll(cardsList);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowAccountList(List<CustomerBillingAccount> accountList)
        {
            try
            {
                AccAdapter.AddAll(accountList);
                AccAdapter.NotifyDataSetChanged();
                listView.SetNoScroll();
                //listView.AddFooterView(btnLoadMore);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public void ShowRemoveSuccess(CreditCardData RemovedCard, int position)
        {
            try
            {
                cardsList.RemoveAt(position);
                if (cardsList.Count == 0)
                {
                    LinearLayout.LayoutParams imgEmptyCardParams = imgEmptyCard.LayoutParameters as LinearLayout.LayoutParams;

                    imgEmptyCardParams.Width = GetDeviceHorizontalScaleInPixel(0.3f);
                    imgEmptyCardParams.Height = GetDeviceHorizontalScaleInPixel(0.3f);
                    imgEmptyCardParams.TopMargin = GetDeviceHorizontalScaleInPixel(0.155f);
                    imgEmptyCard.RequestLayout();

                    layoutEmptyCards.Visibility = ViewStates.Visible;
                    layoutCards.Visibility = ViewStates.Gone;
                    layoutLabelAutopay.Visibility = ViewStates.Gone;
                    layoutAutopay.Visibility = ViewStates.Gone;
                    listView.Visibility = ViewStates.Gone;
                    this.userActionsListener.OnRemoveStay(RemovedCard, position);

                }
                else
                {
                    Intent creditCard = new Intent();
                    creditCard.PutExtra(Constants.REMOVED_CREDIT_CARD, JsonConvert.SerializeObject(RemovedCard));
                    SetResult(Result.Ok, creditCard);
                    Finish();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();

        }

        public void ShowErrorMessage(string message)
        {
            Snackbar errorSnackbar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
                        .SetAction(GetLabelCommonByLanguage("close"),
                         (view) =>
                         {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        );
            View v = errorSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            errorSnackbar.Show();
        }

        public void ShowSnackbarRemovedSuccess(CreditCardData RemovedCard, int position)
        {
            var creditCard = new Intent();
            creditCard.PutExtra(Constants.REMOVED_CREDIT_CARD, JsonConvert.SerializeObject(RemovedCard));
            SetResult(Result.Ok, creditCard);
            string lastDigits = RemovedCard.LastDigits.Substring(RemovedCard.LastDigits.Length - 4);
            Snackbar removeSnackbar = Snackbar.Make(rootView, string.Format(GetLabelByLanguage("cardRemoveSuccess"), lastDigits), Snackbar.LengthIndefinite)
                       .SetAction(GetLabelCommonByLanguage("close"),
                        (view) =>
                        {

                            // EMPTY WILL CLOSE SNACKBAR
                        }
                       );
            View v = removeSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            removeSnackbar.Show();
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
