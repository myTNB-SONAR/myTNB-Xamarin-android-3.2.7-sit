using Android.Content;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

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
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.FeedbackSubmittedRow, parent, false);
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

                vh.txtFeedbackTitle.Text = !string.IsNullOrEmpty(item.FeedbackNameInListView) ? item.FeedbackNameInListView : item.FeedbackCategoryName;
                vh.txtFeedbackContent.Text = item.FeedbackMessage;

                if (item.FeedbackCategoryId.Equals("1"))
                {
                    vh.imgFeedback.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.general_enquiry));
                }
                else if (item.FeedbackCategoryId.Equals("2"))
                {
                    vh.imgFeedback.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_feedback_submitted_streetlamp));
                }
                else if (item.FeedbackCategoryId.Equals("3"))
                {
                    vh.imgFeedback.SetImageDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.ic_feedback_submitted_others));
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

            [BindView(Resource.Id.txtFeedbackContent)]
            internal TextView txtFeedbackContent;

            [BindView(Resource.Id.txtFeedbackDate)]
            internal TextView txtFeedbackDate;

            [BindView(Resource.Id.imgFeedback)]
            internal ImageView imgFeedback;

            public SubmitFeedbackViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans500Typeface(txtFeedbackTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtFeedbackContent, txtFeedbackDate);
            }
        }
    }
}