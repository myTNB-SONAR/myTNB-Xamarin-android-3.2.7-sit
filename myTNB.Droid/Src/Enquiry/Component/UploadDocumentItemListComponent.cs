using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Android.Src.Enquiry.Adapter;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.Enquiry.Component
{
    public class UploadDocumentItemListComponent : LinearLayout
    {
        TextView txtUploadDocumentLabel, txtUploadDocumentHint, txtUploadDocumentToolTip;
        RecyclerView recyclerViewUploadDocument;
        FrameLayout uploadDocumentTooltipLayout;

        private Action OnToolTipTapAction;

        private readonly Context mContext;

        public UploadDocumentItemListComponent(Context context) : base(context)
        {
            mContext = context;
            Init(mContext);
        }

        public UploadDocumentItemListComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init(mContext);
        }

        public UploadDocumentItemListComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init(mContext);
        }

        public void Init(Context context)
        {
            Inflate(context, Resource.Layout.UploadDocumentItemListView, this);
            txtUploadDocumentLabel = FindViewById<TextView>(Resource.Id.txtUploadDocumentLabel);
            txtUploadDocumentHint = FindViewById<TextView>(Resource.Id.txtUploadDocumentHint);
            txtUploadDocumentToolTip = FindViewById<TextView>(Resource.Id.txtUploadDocumentToolTip);

            recyclerViewUploadDocument = FindViewById<RecyclerView>(Resource.Id.recyclerViewUploadDocument);
            uploadDocumentTooltipLayout = FindViewById<FrameLayout>(Resource.Id.uploadDocumentTooltipLayout);

            SetUpViews();
        }

        private void SetUpViews()
        {
            TextViewUtils.SetMuseoSans500Typeface(txtUploadDocumentLabel, txtUploadDocumentToolTip);
            TextViewUtils.SetMuseoSans300Typeface(txtUploadDocumentHint);
            TextViewUtils.SetTextSize16(txtUploadDocumentLabel);
            TextViewUtils.SetTextSize12(txtUploadDocumentToolTip);
            TextViewUtils.SetTextSize9(txtUploadDocumentHint);

            uploadDocumentTooltipLayout.Visibility = Android.Views.ViewStates.Gone;
            uploadDocumentTooltipLayout.Click += delegate
            {
                OnToolTipTapAction?.Invoke();
            };
        }

        public void SetUploadDocumentLabel(string value)
        {
            txtUploadDocumentLabel.Text = value;
        }

        public void SetUploadDocumentHint(string value)
        {
            txtUploadDocumentHint.Text = value;
        }

        public void SetUploadDocumentToolTipLabel(string value)
        {
            txtUploadDocumentToolTip.Text = value;
        }

        public void SetToolTipAction(Action action)
        {
            this.OnToolTipTapAction = action;
        }

        public void SetToolTipToVisible(bool isVisible)
        {
            uploadDocumentTooltipLayout.Visibility = isVisible ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
        }

        public void SetLayoutManager(LinearLayoutManager layoutManager)
        {
            recyclerViewUploadDocument.SetLayoutManager(layoutManager);
        }

        public void SetAdapter(UploadDocumentItemAdapter adapter)
        {
            recyclerViewUploadDocument.SetAdapter(adapter);
        }
    }
}
