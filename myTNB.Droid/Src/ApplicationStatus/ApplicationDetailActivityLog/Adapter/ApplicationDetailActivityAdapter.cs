using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;


namespace myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.Adapter
{
    public class ApplicationDetailActivityAdapter : RecyclerView.Adapter
    {
        BaseAppCompatActivity mActivity;
        List<ApplicationActivityLogDetail> mApplicationActivityLogDetail = new List<ApplicationActivityLogDetail>();

     

        public void Clear()
        {
            this.mApplicationActivityLogDetail.Clear();
            this.NotifyDataSetChanged();
        }

        public ApplicationDetailActivityAdapter(BaseAppCompatActivity activity, List<ApplicationActivityLogDetail> data)
        {
            this.mActivity = activity;
            this.mApplicationActivityLogDetail.AddRange(data);
        }

        public override int ItemCount => mApplicationActivityLogDetail.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationActivityLogViewHolder vh = holder as ApplicationActivityLogViewHolder;

            ApplicationActivityLogDetail item = mApplicationActivityLogDetail[position];
            vh.PopulateData(item, mApplicationActivityLogDetail, position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationActivityLog;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            
            return new ApplicationActivityLogViewHolder(itemView);
        }

        public class ApplicationActivityLogViewHolder : RecyclerView.ViewHolder
        {
            public ImageView imgstatus_required { get; private set; }
            public ImageView imgstatus_Approval { get; private set; }
            public TextView text_status { get; private set; }
            public TextView lbl_date_text { get; private set; }
            public TextView lbl_comment_text { get; private set; }
            public TextView lbl_submittedby_email { get; private set; }
            public View activityLogLine { get; private set; }

            
            public TextView lbl_Status { get; private set; }
            public TextView lbl_details { get; private set; }
            public TextView lbl_comment { get; private set; }
            public TextView lbl_submittedby { get; private set; }

            public ListView listview_updated_details { get; private set; }

         

            //public TextView TxtApplicationStatusDetailCTA { get; private set; }

            private Context context;

            private ApplicationActivityLogDetail item = null;

            public ApplicationActivityLogViewHolder(View itemView) : base(itemView)
            {
                context = itemView.Context;
                imgstatus_required = itemView.FindViewById<ImageView>(Resource.Id.imgstatus_required);
                imgstatus_Approval = itemView.FindViewById<ImageView>(Resource.Id.imgstatus_Approval);
                text_status = itemView.FindViewById<TextView>(Resource.Id.text_status);
                lbl_date_text = itemView.FindViewById<TextView>(Resource.Id.lbl_date_text);

                lbl_comment_text = itemView.FindViewById<TextView>(Resource.Id.lbl_comment_text);
                lbl_submittedby_email = itemView.FindViewById<TextView>(Resource.Id.lbl_submittedby_email);
                activityLogLine = itemView.FindViewById<View>(Resource.Id.activityLogLine);

                lbl_Status = itemView.FindViewById<TextView>(Resource.Id.lbl_Status);
                lbl_details = itemView.FindViewById<TextView>(Resource.Id.lbl_details);
                lbl_comment = itemView.FindViewById<TextView>(Resource.Id.lbl_comment);
                lbl_submittedby = itemView.FindViewById<TextView>(Resource.Id.lbl_submittedby);

                listview_updated_details = itemView.FindViewById<ListView>(Resource.Id.listview_updated_details);

               
                //TxtApplicationStatusDetailCTA = itemView.FindViewById<TextView>(Resource.Id.txtApplicationStatusDetailCTA);
                //TxtApplicationStatusDetailCTA.Clickable = true;
                //TxtApplicationStatusDetailCTA.Click += (sender, e) => listener(base.LayoutPosition);
            }


            public void PopulateData(ApplicationActivityLogDetail item, List<ApplicationActivityLogDetail> mApplicationActivityLogDetail, int position)
            {
                this.item = item;
                try
                {
                 
                    TextViewUtils.SetMuseoSans300Typeface(text_status);
                    TextViewUtils.SetMuseoSans300Typeface(lbl_date_text);
                    TextViewUtils.SetMuseoSans300Typeface(lbl_comment_text);
                    TextViewUtils.SetMuseoSans300Typeface(lbl_submittedby_email);

                    TextViewUtils.SetMuseoSans300Typeface(lbl_details);
                    TextViewUtils.SetMuseoSans300Typeface(lbl_comment);
                    TextViewUtils.SetMuseoSans300Typeface(lbl_submittedby);
                    TextViewUtils.SetMuseoSans300Typeface(lbl_Status);


                    lbl_Status.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "status");
                    lbl_comment.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "comment");
                    lbl_submittedby.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "submittedBy");
                    lbl_details.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "updatedDetails");



                    text_status.Text = item.StatusDescription;

                    CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());
                    string date = item.CreatedDate != null && item.CreatedDate.Value != null
                        ? item.CreatedDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                        : string.Empty;

                    lbl_date_text.Text = date;
                    if (item.Comment != string.Empty)
                    {
                        lbl_comment_text.Text = item.Comment;
                    }
                    else
                    {
                        lbl_comment_text.Visibility = ViewStates.Gone;
                    }
                    if(item.CreatedBy != string.Empty)
                    {
                        lbl_submittedby_email.Text = item.CreatedBy;
                    }
                   else
                    {

                    }

                    if(mApplicationActivityLogDetail.Count > 0 && mApplicationActivityLogDetail.Count == position +1)
                    {
                        activityLogLine.Visibility = ViewStates.Gone;
                    }
                  
                    //listview_updated_details.SetAdapter(new ArrayAdapter<String>(
                    //                    this, R.layout.list_item, listCountry));
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

        }
    }
}
