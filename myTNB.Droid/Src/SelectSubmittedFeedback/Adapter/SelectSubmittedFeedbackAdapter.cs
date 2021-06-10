using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace myTNB_Android.Src.SelectSubmittedFeedback.Adapter
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
                string title = "Bill";
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

                //  vh.txtFeedbackTitle.Text = !string.IsNullOrEmpty(item.FeedbackNameInListView) ? item.FeedbackNameInListView : item.FeedbackCategoryName;

                // vh.txtFeedbackTitle.SetPadding(0, 24, 0, 0);  //inject padding;
                // vh.txtFeedbackDate.SetPadding(0, 24, 0, 0);
                // vh.txtFeedbackContent.Text = item.FeedbackMessage;
                // vh.txtFeedbackContent.Visibility = ViewStates.Gone;

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

                //statusCode color 
                if (item.StatusCode.Equals("CL01"))
                {
                    vh.txtSRstatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.tunagrey)));
                }
                else if (item.StatusCode.Equals("CL02"))
                {

                }
                else if (item.StatusCode.Equals("CL03"))
                {
                    vh.txtSRstatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.completedColor)));
                }
                else if (item.StatusCode.Equals("CL04"))
                {
                    vh.txtSRstatus.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.completedColor)));
                }
                else if (item.StatusCode.Equals("CL06"))
                {

                }

                if (item.FeedbackCategoryId.Equals("1"))
                {
                    // vh.imgFeedback.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.general_enquiry));
                    vh.txtFeedbackTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "generalEnquiryTitle");

                }
                else if (item.FeedbackCategoryId.Equals("2"))
                {
                    // vh.imgFeedback.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_feedback_submitted_streetlamp));
                }
                else if (item.FeedbackCategoryId.Equals("3"))
                {
                    //   vh.imgFeedback.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_feedback_submitted_others));
                }
                else if (item.FeedbackCategoryId.Equals("4"))
                {
                    //  vh.imgFeedback.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.update_personal_details));
                    vh.txtFeedbackTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "updatePersonalDetTitle");
                    //  vh.txtFeedbackTitle.SetPadding(0, 24, 7, 0);
                }
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
            }
        }
    }
}