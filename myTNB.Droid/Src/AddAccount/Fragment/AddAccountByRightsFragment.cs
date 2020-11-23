using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Utils;
using Org.Xml.Sax;
using System;
using System.Linq;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    public class AddAccountByRightsFragment : AndroidX.Fragment.App.Fragment 
    {
        bool isOwner = false;
        LinearLayout radio_non_owner;
        LinearLayout radio_owner;

        TextView txtTitle;
        TextView txtYes;
        TextView txtNo;
        TextView txtOwnerRights;
        TextView txtNonOwnerRights;

        LinearLayout layoutOtherInfo;

        TextView txtTitleInfo, txtOutstandingPayment, txtCurrentBill, txtBillHistory, txtAllTransactionDetails, txtUsageHistory;

        public AddAccountByRightsFragment()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            isOwner = Arguments.GetBoolean("isOwner");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.AddAccountTypeView, container, false);
            radio_non_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnNonOwner);
            radio_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnOwner);

            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            txtOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtOwnerConstrain);
            txtNonOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtNonOwnerConstrain);
            txtYes = rootView.FindViewById<TextView>(Resource.Id.txtYes);
            txtNo = rootView.FindViewById<TextView>(Resource.Id.txtNo);
            
            layoutOtherInfo = rootView.FindViewById<LinearLayout>(Resource.Id.layoutOtherInfo);
            layoutOtherInfo.Visibility = ViewStates.Visible;
           
           

            txtTitleInfo = rootView.FindViewById<TextView>(Resource.Id.txtTitleInfo);

            txtOutstandingPayment = rootView.FindViewById<TextView>(Resource.Id.txtOutstandingPayment);
            txtCurrentBill = rootView.FindViewById<TextView>(Resource.Id.txtCurrentBill);
            txtBillHistory = rootView.FindViewById<TextView>(Resource.Id.txtBillHistory);
            txtAllTransactionDetails = rootView.FindViewById<TextView>(Resource.Id.txtAllTransactionDetails);
            txtUsageHistory = rootView.FindViewById<TextView>(Resource.Id.txtUsageHistory);

            txtTitle.Text = Utility.GetLocalizedLabel("AddAccount", "addByRightsMessage");
            txtOwnerRights.Text = Utility.GetLocalizedLabel("AddAccount", "addAsTenantWithICMessage");
            txtNonOwnerRights.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("AddAccount", "addAsTenantWithoutICMessage"));
            txtYes.Text = Utility.GetLocalizedLabel("Common","yes") + ".";
            txtNo.Text = Utility.GetLocalizedLabel("Common","no") + ".";

            txtOutstandingPayment.Text = Utility.GetLocalizedLabel("AddAccount", "outstandingPayment");
            txtCurrentBill.Text = Utility.GetLocalizedLabel("AddAccount", "usageGraph");
            txtBillHistory.Text = Utility.GetLocalizedLabel("AddAccount", "paymentHistory");
            txtAllTransactionDetails.Text = Utility.GetLocalizedLabel("AddAccount", "currentBill");
            txtUsageHistory.Text = Utility.GetLocalizedLabel("AddAccount", "pastBills");

            txtNo.TextSize = TextViewUtils.GetFontSize(18);
            txtOwnerRights.TextSize = TextViewUtils.GetFontSize(18);
            txtTitle.TextSize = TextViewUtils.GetFontSize(18);
            txtTitleInfo.TextSize = TextViewUtils.GetFontSize(18);
            txtOutstandingPayment.TextSize = TextViewUtils.GetFontSize(14);
            txtCurrentBill.TextSize = TextViewUtils.GetFontSize(14);
            txtBillHistory.TextSize = TextViewUtils.GetFontSize(14);
            txtAllTransactionDetails.TextSize = TextViewUtils.GetFontSize(14);
            txtUsageHistory.TextSize = TextViewUtils.GetFontSize(14);
            txtYes.TextSize = TextViewUtils.GetFontSize(18);
            txtNonOwnerRights.TextSize = TextViewUtils.GetFontSize(18);

            radio_non_owner.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isOwner", isOwner);
                bundle.PutBoolean("hasRights", false);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };

            radio_owner.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isOwner", isOwner);
                bundle.PutBoolean("hasRights", true);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };


            TextViewUtils.SetMuseoSans500Typeface(txtYes, txtNo, txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtOwnerRights, txtNonOwnerRights, txtTitleInfo, txtOutstandingPayment, txtCurrentBill, txtBillHistory, txtAllTransactionDetails, txtUsageHistory);

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

        private ISpanned GetHtmlText(string text, Html.IImageGetter imageGetter)
        {
            if (((int)Build.VERSION.SdkInt) >= 24)
            {
                return Html.FromHtml(text, FromHtmlOptions.ModeLegacy, imageGetter, new ILTagHandler());
            }
            else
            {
                return Html.FromHtml(text, imageGetter, new ILTagHandler());
            }
        }


        private class ImgSrcGetter : Java.Lang.Object, Html.IImageGetter
        {
            private Context context;

            public ImgSrcGetter(Context context)
            {
                this.context = context;
            }

            public Drawable GetDrawable(string source)
            {
                int id;
                if (source.Contains("ic_check.png"))
                {
                    id = Resource.Drawable.ic_check;
                }
                else
                {
                    id = Resource.Drawable.ic_wrong;
                }

                Drawable d = ContextCompat.GetDrawable(context, id);
                d.SetBounds(0, 0, d.IntrinsicWidth, d.IntrinsicHeight);
                return d;
            }
        }

        private class ILTagHandler : Java.Lang.Object, Html.ITagHandler
        {
            public void HandleTag(bool opening, string tag, IEditable output, IXMLReader xmlReader)
            {

                if (tag.Equals("li") && opening)
                {
                    output.Append("\n\t");
                }
            }
        }

        private ISpanned GetFormattedText(string stringValue)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(stringValue, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                return Html.FromHtml(stringValue);
            }
        }
    }
}