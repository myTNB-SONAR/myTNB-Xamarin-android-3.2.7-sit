using Android.OS;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MakePayment.Fragment
{
    public class PaymentSuccessFragment : AndroidX.Fragment.App.Fragment 
    {
        TextView lblTitleInfo;
        TextView lblTrxDateTime;
        TextView txtTrxDateTime;
        TextView lblTrxID;
        TextView txtTrxID;
        TextView lblTotalAmount;
        TextView txtTotalAmount;

        Button btnViewReceipt;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View rootView = inflater.Inflate(Resource.Layout.PaymentSuccessView, container, false);
            lblTitleInfo = rootView.FindViewById<TextView>(Resource.Id.txtTitleInfo);
            lblTrxDateTime = rootView.FindViewById<TextView>(Resource.Id.lblTrxDateTime);
            txtTrxDateTime = rootView.FindViewById<TextView>(Resource.Id.txtTrxDateTime);
            lblTrxID = rootView.FindViewById<TextView>(Resource.Id.lblTrxID);
            txtTrxID = rootView.FindViewById<TextView>(Resource.Id.txtTrxId);
            lblTotalAmount = rootView.FindViewById<TextView>(Resource.Id.lblTotalAmount);
            txtTotalAmount = rootView.FindViewById<TextView>(Resource.Id.txtTotalAmount);
            btnViewReceipt = rootView.FindViewById<Button>(Resource.Id.btnViewReceipt);

            TextViewUtils.SetMuseoSans300Typeface(lblTitleInfo, lblTotalAmount, lblTrxDateTime, lblTrxID);
            TextViewUtils.SetMuseoSans300Typeface(txtTrxDateTime, txtTotalAmount, txtTrxID);

            TextViewUtils.SetMuseoSans500Typeface(btnViewReceipt);

            return rootView;
        }
    }
}