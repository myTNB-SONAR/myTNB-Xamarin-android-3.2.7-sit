using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.SelectSubmittedFeedback.Adapter
{
    public class SelectSubmittedFeedbackAdapter : BaseCustomAdapter<SubmittedFeedback>
    {


        public SelectSubmittedFeedbackAdapter(Context context) : base(context)
        {
        }

        public SelectSubmittedFeedbackAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public SelectSubmittedFeedbackAdapter(Context context, List<SubmittedFeedback> itemList) : base(context, itemList)
        {
        }

        public SelectSubmittedFeedbackAdapter(Context context, List<SubmittedFeedback> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            SimpleDateFormat simpleDateParser = new SimpleDateFormat("dd/MM/yyyy", LocaleUtils.GetDefaultLocale());
            SimpleDateFormat simpleDateFormat = new SimpleDateFormat("dd MMM yyyy", LocaleUtils.GetCurrentLocale());
            SubmitFeedbackViewHolder vh = null;

            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.FeedbackSubmittedRowNew, parent, false);
                vh = new SubmitFeedbackViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as SubmitFeedbackViewHolder;
            }

            try
            {
                SubmittedFeedback item = GetItemObject(position);

                Date d = null;
                try
                {
                    d = simpleDateParser.Parse(item.DateCreated);
                    vh.txtFeedbackDate.Text = simpleDateFormat.Format(d);
                }
                catch (Java.Text.ParseException e)
                {
                    vh.txtFeedbackDate.Text = "NA";
                    Utility.LoggingNonFatalError(e);
                }

                vh.txtSRstatus.Text = item.StatusDesc;
                vh.txtSRNumber.Text = "SR: " + item.FeedbackId;


                if (!item.IsRead.Equals("true"))
                {
                    vh.completeIndicator.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.completeIndicator.Visibility = ViewStates.Gone;
                }

                int statusColor = item.FeedbackCategoryId.Equals("9") ? item.GSLStatusColor : item.StatusColor;

                vh.txtSRstatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, statusColor)));
                vh.txtFeedbackTitle.Text = item.FeedbackCategoryId.Equals("11") ? Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle") : item.FeedbackCategoryName;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }

        public class SubmitFeedbackViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtFeedbackTitle)]
            internal TextView txtFeedbackTitle;

            [BindView(Resource.Id.txtSRstatus)]
            internal TextView txtSRstatus;

            [BindView(Resource.Id.txtSRNumber)]
            internal TextView txtSRNumber;

            [BindView(Resource.Id.txtFeedbackDate)]
            internal TextView txtFeedbackDate;

            [BindView(Resource.Id.completeIndicator)]
            internal ImageView completeIndicator;

            public SubmitFeedbackViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans500Typeface(txtFeedbackTitle, txtSRstatus, txtFeedbackDate);
                TextViewUtils.SetMuseoSans300Typeface(txtSRNumber);
                TextViewUtils.SetTextSize11(txtFeedbackDate);
                TextViewUtils.SetTextSize14(txtFeedbackTitle);
                TextViewUtils.SetTextSize12(txtSRstatus, txtSRNumber);
            }
        }
    }
}
