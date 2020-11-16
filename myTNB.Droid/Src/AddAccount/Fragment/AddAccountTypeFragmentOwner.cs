using Android.OS;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Utils;
using System;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    public class AddAccountTypeFragmentOwner : AndroidX.Fragment.App.Fragment 
    {

        LinearLayout btn_updateId;
        LinearLayout btn_continue;

        TextView txtTitle;
        TextView txtbodyOwner;
        TextView txttitleUpdateId;
        TextView txtbodyUpdateId;
        TextView txttitleContinue;
        TextView txtbodyContinue;

        public AddAccountTypeFragmentOwner()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.AddAccountTypeViewOwner, container, false);
            btn_updateId = rootView.FindViewById<LinearLayout>(Resource.Id.btnUpdate);
            btn_continue = rootView.FindViewById<LinearLayout>(Resource.Id.btnContinue);

            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            txtbodyOwner = rootView.FindViewById<TextView>(Resource.Id.txtbodyOwner);
            txttitleUpdateId = rootView.FindViewById<TextView>(Resource.Id.txttitleUpdateId);
            txttitleContinue = rootView.FindViewById<TextView>(Resource.Id.txttitleContinue);
            txtbodyContinue = rootView.FindViewById<TextView>(Resource.Id.txtbodyContinue);
            txtbodyUpdateId = rootView.FindViewById<TextView>(Resource.Id.txtbodyUpdateId);


            btn_updateId.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isUpdateId", false);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };

            btn_continue.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isUpdateId", true);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };

            TextViewUtils.SetMuseoSans500Typeface(txttitleUpdateId, txttitleContinue, txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtbodyOwner, txtbodyContinue, txtbodyUpdateId);

            txtTitle.Text = Utility.GetLocalizedLabel("AddAccount", "titleownerNoAccDec");
            txtbodyOwner.Text = Utility.GetLocalizedLabel("AddAccount", "bodyownerNoAccDec");
            txtbodyContinue.Text = Utility.GetLocalizedLabel("AddAccount", "bodyContinue");
            txtbodyUpdateId.Text = Utility.GetLocalizedLabel("AddAccount", "bodyUpdateId");
            txttitleUpdateId.Text = Utility.GetLocalizedLabel("AddAccount", "titleUpdateId");
            txttitleContinue.Text = Utility.GetLocalizedLabel("AddAccount", "titleContinue");
            return rootView;
        }


        private string GetHtmlText(String text)
        {
            if (((int)Build.VERSION.SdkInt) >= 24)
            {
                return text;
            }
            else
            {
                return text;
            }

        }
    }
}