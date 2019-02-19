using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using myTNB_Android.Src.ManageCards.Adapter;
using Android.Support.V7.Widget;
using CheeseBind;
using myTNB_Android.Src.ManageCards.MVP;
using myTNB_Android.Src.ManageCards.Models;
using Refit;
using Android.Support.Design.Widget;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static AFollestad.MaterialDialogs.MaterialDialog;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.ManageCards.Activity
{
    [Activity(Label = "@string/manage_cards_activity_title"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.ManageCards")]
    public class ManageCardsActivity : BaseToolbarAppCompatActivity , ManageCardsContract.IView
    {

        [BindView(Resource.Id.layout_cards)]
        LinearLayout layoutCards;

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

        ManageCardsAdapter mAdapter;

        LinearLayoutManager mLayoutManager;

        ManageCardsContract.IUserActionsListener userActionsListener;
        ManageCardsPresenter mPresenter;

        MaterialDialog progress;

        private LoadingOverlay loadingOverlay;

        AlertDialog removeDialog;

        List<CreditCardData> cardsList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string cardsListString = Intent.Extras.GetString(Constants.CREDIT_CARD_LIST);
            cardsList = JsonConvert.DeserializeObject<List<CreditCardData>>(cardsListString);

            Console.WriteLine("Card List " + cardsListString);

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

            TextViewUtils.SetMuseoSans300Typeface(txtManageCardsTitle, txtEmptyCard);

            mPresenter = new ManageCardsPresenter(this);
            this.userActionsListener.Start();
        }
        [Preserve]
        private void MAdapter_RemoveClick(object sender, int e)
        {

            if (removeDialog != null && removeDialog.IsShowing)
            {
                removeDialog.Dismiss();
            }
            CreditCardData creditCardData = mAdapter.GetItemObject(e);
            string lastDigit = creditCardData.LastDigits.Substring(creditCardData.LastDigits.Length - 4);
            removeDialog = new AlertDialog.Builder(this)

                .SetTitle(Resource.String.manage_cards_remove_dialog_title)
                .SetMessage(GetString(Resource.String.manage_cards_remove_content_wildcard , lastDigit))
                .SetNegativeButton(Resource.String.manage_cards_btn_cancel , 
                delegate {
                    removeDialog.Dismiss();
                })
                .SetPositiveButton(Resource.String.manage_cards_btn_ok , 
                delegate 
                {
                    this.userActionsListener.OnRemove(creditCardData, e);
                })
                .Show()
                ;

            
            
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ManageCardsView;
        }

        public void SetPresenter(ManageCardsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowCards()
        {
            if (cardsList.Count == 0)
            {
                layoutEmptyCards.Visibility = ViewStates.Visible;
                layoutCards.Visibility = ViewStates.Gone;
            }
            else
            {
                layoutCards.Visibility = ViewStates.Visible;
                layoutEmptyCards.Visibility = ViewStates.Gone;
                mAdapter.AddAll(cardsList);
            }
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowProgressDialog()
        {
            //if (progress != null && !progress.IsShowing)
            //{
            //    progress.Show();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideProgressDialog()
        {
            //if (progress != null && progress.IsShowing)
            //{
            //    progress.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }

        public void ShowRemoveSuccess(CreditCardData RemovedCard , int position)
        {
            cardsList.RemoveAt(position);
            if (cardsList.Count == 0)
            {
                layoutEmptyCards.Visibility = ViewStates.Visible;
                layoutCards.Visibility = ViewStates.Gone;
                this.userActionsListener.OnRemoveStay(RemovedCard , position);

            }
            else
            {
                Intent creditCard = new Intent();
                creditCard.PutExtra(Constants.REMOVED_CREDIT_CARD, JsonConvert.SerializeObject(RemovedCard));
                SetResult(Result.Ok, creditCard);
                Finish();
            }

        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_cancelled_exception_btn_retry), delegate {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_api_exception_btn_retry), delegate {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_unknown_exception_btn_retry), delegate {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void ShowErrorMessage(string message)
        {
            Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
                        .SetAction(GetString(Resource.String.manage_cards_btn_close),
                         (view) =>
                         {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        ).Show();
        }

        public void ShowSnackbarRemovedSuccess(CreditCardData RemovedCard , int position)
        {
            var creditCard = new Intent();
            creditCard.PutExtra(Constants.REMOVED_CREDIT_CARD, JsonConvert.SerializeObject(RemovedCard));
            SetResult(Result.Ok, creditCard);
            string lastDigits = RemovedCard.LastDigits.Substring(RemovedCard.LastDigits.Length - 4);
            Snackbar.Make(rootView, GetString(Resource.String.manage_cards_card_remove_successfully_wildcard, lastDigits), Snackbar.LengthIndefinite)
                       .SetAction(GetString(Resource.String.manage_cards_btn_close),
                        (view) =>
                        {

                            // EMPTY WILL CLOSE SNACKBAR
                        }
                       ).Show();
        }
    }
}