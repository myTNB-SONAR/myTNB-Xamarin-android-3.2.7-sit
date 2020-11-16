using Android.OS;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Utils;
using System;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    public class AddAccountTypeFragment : AndroidX.Fragment.App.Fragment 
    {

        LinearLayout radio_non_owner;
        LinearLayout radio_owner;

        TextView txtTitle;
        TextView txtYes;
        TextView txtNo;
        TextView txtOwnerRights;
        TextView txtNonOwnerRights;

        public AddAccountTypeFragment()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.AddAccountTypeViewNew, container, false);
            radio_non_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnNonOwner);
            radio_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnOwner);

            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            txtOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtOwnerConstrain);
            txtNonOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtNonOwnerConstrain);
            txtYes = rootView.FindViewById<TextView>(Resource.Id.txtYes);
            txtNo = rootView.FindViewById<TextView>(Resource.Id.txtNo);

            radio_non_owner.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isOwner", false);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };

            radio_owner.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isOwner", true);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };

            TextViewUtils.SetMuseoSans500Typeface(txtYes, txtNo, txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtOwnerRights, txtNonOwnerRights);

            txtTitle.Text = Utility.GetLocalizedLabel("AddAccount", "addByIDMessage");
            txtOwnerRights.Text = Utility.GetLocalizedLabel("AddAccount", "addAsOwnerMessage");
            txtNonOwnerRights.Text = Utility.GetLocalizedLabel("AddAccount", "addAsTenantMessage");
            txtYes.Text = Utility.GetLocalizedLabel("Common", "yes") + ".";
            txtNo.Text = Utility.GetLocalizedLabel("Common", "no") + ".";
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