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
using Android.Content.PM;
using myTNB_Android.Src.NotificationDetails.Activity.Base;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.NotificationDetails.MVP;
using myTNB_Android.Src.MakePayment.Activity;
using Newtonsoft.Json;
using myTNB_Android.Src.myTNBMenu.Models;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.NotificationNewBill.Activity;
using myTNB_Android.Src.NotificationDetails.Models;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.AddAccount.Fragment;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.NotificationDetails.Activity
{
    [Activity(Label = "@string/notification_detail_new_bill_activity_title"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Notification")]
    public class NotificationDetailsNewBillActivity : BaseNotificationDetailActivity , NotificationDetailNewBillContract.IView
    {

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtNotificationTitle)]
        TextView txtNotificationTitle;

        [BindView(Resource.Id.txtNotificationSubTitle)]
        TextView txtNotificationSubTitle;

        [BindView(Resource.Id.txtAccountNumberTitle)]
        TextView txtAccountNumberTitle;

        [BindView(Resource.Id.txtAccountNumberContent)]
        TextView txtAccountNumberContent;

        [BindView(Resource.Id.txtTotalOutstandingAmtTitle)]
        TextView txtTotalOutstandingAmtTitle;

        [BindView(Resource.Id.txtTotalOutstandingAmtContent)]
        TextView txtTotalOutstandingAmtContent;

        [BindView(Resource.Id.txtNotificationDetailsPaymentDueTitle)]
        TextView txtNotificationDetailsPaymentDueTitle;

        [BindView(Resource.Id.txtNotificationDetailsPaymentDueContent)]
        TextView txtNotificationDetailsPaymentDueContent;

        [BindView(Resource.Id.txtNotificationDetailsSubContent1)]
        TextView txtNotificationDetailsSubContent1;

        [BindView(Resource.Id.txtNotificationDetailsSubContent2)]
        TextView txtNotificationDetailsSubContent2;

        [BindView(Resource.Id.txtNotificationDetailsSubContent3)]
        TextView txtNotificationDetailsSubContent3;

        [BindView(Resource.Id.btnViewDetails)]
        Button btnViewDetails;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;

        NotificationDetailNewBillContract.IUserActionsListener newBillUserActionsListener;
        NotificationDetailNewBillPresenter newBillPresenter;

        MaterialDialog retrievalDialog;
        private LoadingOverlay loadingOverlay;

        SimpleDateFormat dateParser = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat monthFormatter = new SimpleDateFormat("MMM");
        DecimalFormat numberFormatter = new DecimalFormat("###,###,###,###,###,##0.00");

        public override View GetRootView()
        {
            return rootView;
        }


        public override int ResourceId()
        {
            return Resource.Layout.NewBillNotificationView;
        }

        public void SetPresenter(NotificationDetailNewBillContract.IUserActionsListener userActionListener)
        {
            this.newBillUserActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowDetails(AccountData selectedAccount)
        {
            Intent payment_activity = new Intent(this, typeof(NotificationNewBillViewDetailsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            StartActivity(payment_activity);
        }

        public void ShowPayment(AccountData selectedAccount)
        {
            //Intent payment_activity = new Intent(this, typeof(MakePaymentActivity));
            Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccount));
            StartActivity(payment_activity);
        }

        public void ShowRetrievalProgress()
        {
            //if (retrievalDialog != null && !retrievalDialog.IsShowing)
            //{
            //    retrievalDialog.Show();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideRetrievalProgress()
        {
            //if (retrievalDialog != null && retrievalDialog.IsShowing)
            //{
            //    retrievalDialog.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            retrievalDialog = new MaterialDialog.Builder(this)
            .Title(GetString(Resource.String.notification_detail_retrieval_progress_title))
            .Content(GetString(Resource.String.notification_detail_retrieval_progress_content))
            .Cancelable(false)
            .Progress(true, 0)
            .Build();
            // Create your application here

            TextViewUtils.SetMuseoSans500Typeface(btnViewDetails , 
                btnPay,
                txtAccountNumberTitle , 
                txtNotificationDetailsPaymentDueTitle, 
                txtNotificationSubTitle,
                txtNotificationTitle,
                txtTotalOutstandingAmtTitle);

            TextViewUtils.SetMuseoSans300Typeface(
                txtAccountNumberContent,
                txtNotificationDetailsPaymentDueContent,
                txtNotificationDetailsSubContent1,
                txtNotificationDetailsSubContent2,
                txtNotificationDetailsSubContent3,
                txtTotalOutstandingAmtContent);


            this.newBillPresenter = new NotificationDetailNewBillPresenter(this);
            this.newBillUserActionsListener.Start();


        }
        [OnClick(Resource.Id.btnPay)]
        void OnPay(object sender, EventArgs eventArgs)
        {
            this.newBillUserActionsListener.OnPayment(notificationDetails);
        }

        [OnClick(Resource.Id.btnViewDetails)]
        void OnViewDetails(object sender, EventArgs eventArgs)
        {
            this.newBillUserActionsListener.OnViewDetails(notificationDetails);
        }

        public override void ShowAccountNumber()
        {
            txtAccountNumberContent.Text = notificationDetails.AccountNum;
        }

        public void ShowMonthWildCard()
        {
            // TODO : ADD Month Name to notification_detail_new_bill_title_wildcard 
            if (notificationDetails.AccountDetails != null)
            {
                Date d = null;
                try
                {
                    d = dateParser.Parse(notificationDetails.AccountDetails.BillDate);
                }catch (ParseException pe)
                {

                }
                if (d != null)
                {
                    txtNotificationTitle.Text = GetString(Resource.String.notification_detail_new_bill_title_wildcard , monthFormatter.Format(d));
                }
                
            }
        }

        public void ShowBillDatedWildcard()
        {
            // TODO : ADD Bill date to notification_detail_new_bill_sub_title_wildcard
            if (notificationDetails.AccountDetails != null)
            {
                txtNotificationSubTitle.Text = GetString(Resource.String.notification_detail_new_bill_sub_title_wildcard, notificationDetails.AccountDetails.BillDate);
            }
           
        }

        public void ShowTotalOutstandingAmtWildcard()
        {
            // TODO : ADD Outstanding Amt 
            if (notificationDetails.AccountDetails != null)
            {
                txtTotalOutstandingAmtContent.Text = "RM" + numberFormatter.Format(notificationDetails.AccountDetails.AmountPayable);
            }
        }

        public void ShowPaymentDueWildcard()
        {
            // TODO : ADD PAyment Due
            if (notificationDetails.AccountDetails != null)
            {
                txtNotificationDetailsPaymentDueContent.Text = notificationDetails.AccountDetails.PaymentDueDate;
            }
        }
    }
}