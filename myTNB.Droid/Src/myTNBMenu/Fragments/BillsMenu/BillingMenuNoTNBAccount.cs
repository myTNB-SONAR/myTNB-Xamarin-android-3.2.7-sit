using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.BillsMenu
{
    public class BillingMenuNoTNBAccount : BaseFragment
    {

        private AccountData selectedAccount;

        [BindView(Resource.Id.txtAccountName)]
        TextView txtAccountName;

        [BindView(Resource.Id.txtAccountNum)]
        TextView txtAccountNum;

        [BindView(Resource.Id.txtAddress)]
        TextView txtAddress;

        [BindView(Resource.Id.txtCurrentBill)]
        TextView txtCurrentBill;

        [BindView(Resource.Id.txtCurrentChargesTitle)]
        TextView txtCurrentChargesTitle;

        [BindView(Resource.Id.txtCurrentChargesContent)]
        TextView txtCurrentChargesContent;

        [BindView(Resource.Id.txtOutstandingChargesTitle)]
        TextView txtOutstandingChargesTitle;

        [BindView(Resource.Id.txtOutstandingChargesContent)]
        TextView txtOutstandingChargesContent;

        [BindView(Resource.Id.txtTotalDueTitle)]
        TextView txtTotalDueTitle;

        [BindView(Resource.Id.txtCurrency)]
        TextView txtCurrency;

        [BindView(Resource.Id.txtTotalDueContent)]
        TextView txtTotalDueContent;

        [BindView(Resource.Id.btnPay)]
        Button btnPay;

        [BindView(Resource.Id.btnBills)]
        RadioButton btnBills;

        [BindView(Resource.Id.btnPayment)]
        RadioButton btnPayment;

        [BindView(Resource.Id.txtBillPaymentHistoryTitle)]
        TextView txtBillPaymentHistoryTitle;

        [BindView(Resource.Id.txtFooter)]
        TextView txtFooter;

        [BindView(Resource.Id.txtFooter1)]
        TextView txtFooter1;

        public override int ResourceId()
        {
            return Resource.Layout.BillsView;
        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                }
            }
            catch (ClassCastException e)
            {

            }
            base.OnAttach(context);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            TextViewUtils.SetMuseoSans500Typeface(txtAccountName,
              txtCurrentBill,
              txtCurrentChargesTitle,
              txtCurrentChargesContent,
              txtOutstandingChargesTitle,
              txtOutstandingChargesContent,
              txtCurrency,
              btnPay,
              txtBillPaymentHistoryTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtAccountNum,
                txtAddress,
                txtFooter, txtFooter1
                );

            txtAccountName.Text = "";
            txtAccountNum.Text = "";
            txtAddress.Text = "";
            txtCurrentChargesContent.Text = "";


            txtFooter.Visibility = ViewStates.Gone;
            txtFooter1.Visibility = ViewStates.Gone;
            //btnPay.Enabled = false;
            //btnPay.Background = ContextCompat.GetDrawable(this.Activity, Resource.Drawable.silver_chalice_button_background);

            btnBills.Enabled = false;
            btnPayment.Enabled = false;
        }
    }
}