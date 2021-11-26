using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Enquiry.GSL.Fragment
{
    public class GSLInfoItemListComponent : LinearLayout
    {
        TextView txtGSLInfoItemTitle, txtGSLInfoItemDesc;
        LinearLayout gslInfoContentList;

        private readonly BaseAppCompatActivity mActivity;
        private readonly Context mContext;

        public GSLInfoItemListComponent(Context context, BaseAppCompatActivity activity) : base(context)
        {
            mContext = context;
            mActivity = activity;
            Init(mContext);
        }

        public GSLInfoItemListComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init(mContext);
        }

        public GSLInfoItemListComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init(mContext);
        }

        public void Init(Context context)
        {
            Inflate(context, Resource.Layout.GSLInfoItemListView, this);
            txtGSLInfoItemTitle = FindViewById<TextView>(Resource.Id.txtGSLInfoItemTitle);
            txtGSLInfoItemDesc = FindViewById<TextView>(Resource.Id.txtGSLInfoItemDesc);
            gslInfoContentList = FindViewById<LinearLayout>(Resource.Id.gslInfoContentList);

            SetUpViews();
        }

        private void SetUpViews()
        {
            TextViewUtils.SetMuseoSans500Typeface(txtGSLInfoItemTitle);
            TextViewUtils.SetTextSize16(txtGSLInfoItemTitle);

            TextViewUtils.SetMuseoSans300Typeface(txtGSLInfoItemDesc);
            TextViewUtils.SetTextSize14(txtGSLInfoItemDesc);
        }

        public void SetTitle(string title)
        {
            txtGSLInfoItemTitle.Text = title;
        }

        public void SetDescription(string desc)
        {
            txtGSLInfoItemDesc.Text = desc;

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                txtGSLInfoItemDesc.TextFormatted = Html.FromHtml(desc, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                txtGSLInfoItemDesc.TextFormatted = Html.FromHtml(desc);
            }

            txtGSLInfoItemDesc = LinkRedirectionUtils
                .Create(this.mActivity, Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_TITLE))
                .SetTextView(txtGSLInfoItemDesc)
                .SetMessage(desc)
                .Build()
                .GetProcessedTextView();
        }

        public void AddContent(LinearLayout layout)
        {
            gslInfoContentList.AddView(layout);
        }
    }
}