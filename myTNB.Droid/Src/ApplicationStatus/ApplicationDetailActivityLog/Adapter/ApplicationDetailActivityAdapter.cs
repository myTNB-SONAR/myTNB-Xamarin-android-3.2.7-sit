using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.Adapter
{
    public class ApplicationDetailActivityAdapter : RecyclerView.Adapter
    {
        BaseAppCompatActivity mActivity;
        List<ApplicationActivityLogDetailDisplay> mApplicationActivityLogDetail = new List<ApplicationActivityLogDetailDisplay>();

        public void Clear()
        {
            this.mApplicationActivityLogDetail.Clear();
            this.NotifyDataSetChanged();
        }

        public ApplicationDetailActivityAdapter(BaseAppCompatActivity activity, List<ApplicationActivityLogDetailDisplay> data)
        {
            this.mActivity = activity;
            this.mApplicationActivityLogDetail.AddRange(data);
        }

        public override int ItemCount => mApplicationActivityLogDetail.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ApplicationActivityLogViewHolder vh = holder as ApplicationActivityLogViewHolder;

                ApplicationActivityLogDetailDisplay item = mApplicationActivityLogDetail[position];
                vh.PopulateData(item, mApplicationActivityLogDetail, position);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

            public RecyclerView listview_updated_details { get; private set; }

            public TextView lbl_reason { get; private set; }
            public RecyclerView listview_reason_details { get; private set; }
            public LinearLayout Layout_updated_details { get; private set; }
            public LinearLayout Layout_reason { get; private set; }
            public LinearLayout Layout_attachments { get; private set; }
            public LinearLayout Layout_comment { get; private set; }
            public LinearLayout Layout_submittedby { get; private set; }


            public TextView lbl_attachments { get; private set; }

            public RecyclerView listview_attachments_details { get; private set; }

            private UpdatedDetailsListAdapter updatedDetailsListAdapter;
            RecyclerView.LayoutManager layoutManagerService;

            //public TextView TxtApplicationStatusDetailCTA { get; private set; }

            private Context context;

            private ApplicationActivityLogDetailDisplay item = null;

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
                listview_updated_details = itemView.FindViewById<RecyclerView>(Resource.Id.listview_updated_details);
                lbl_reason = itemView.FindViewById<TextView>(Resource.Id.lbl_reason);
                listview_reason_details = itemView.FindViewById<RecyclerView>(Resource.Id.listview_reason_details);
                lbl_attachments = itemView.FindViewById<TextView>(Resource.Id.lbl_attachments);
                listview_attachments_details = itemView.FindViewById<RecyclerView>(Resource.Id.listview_attachments_details);

                Layout_updated_details = itemView.FindViewById<LinearLayout>(Resource.Id.Layout_updated_details);
                Layout_reason = itemView.FindViewById<LinearLayout>(Resource.Id.Layout_reason);
                Layout_attachments = itemView.FindViewById<LinearLayout>(Resource.Id.Layout_attachments);
                Layout_comment = itemView.FindViewById<LinearLayout>(Resource.Id.Layout_comment);
                Layout_submittedby = itemView.FindViewById<LinearLayout>(Resource.Id.Layout_submittedby);

                TextViewUtils.SetTextSize10(lbl_submittedby, lbl_comment
                    , lbl_attachments, lbl_reason, lbl_details, lbl_date_text);
                TextViewUtils.SetTextSize14(lbl_submittedby_email, lbl_comment_text);
                TextViewUtils.SetTextSize16(lbl_Status, text_status);
            }

            public void PopulateData(ApplicationActivityLogDetailDisplay item, List<ApplicationActivityLogDetailDisplay> mApplicationActivityLogDetail, int position)
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
                    TextViewUtils.SetMuseoSans300Typeface(lbl_Status);

                    TextViewUtils.SetMuseoSans300Typeface(lbl_reason);
                    TextViewUtils.SetMuseoSans300Typeface(lbl_attachments);

                    lbl_Status.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "status");
                    lbl_comment.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "comment").ToUpper();
                    lbl_submittedby.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "submittedBy").ToUpper();
                    lbl_details.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "updatedDetails").ToUpper();
                    lbl_reason.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "reasons").ToUpper();
                    lbl_attachments.Text = Utility.GetLocalizedLabel("ApplicationStatusActivityLog", "attachments").ToUpper();

                    text_status.Text = item.StatusDescription;

                    CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());
                    string date = item.CreatedDate != null && item.CreatedDate.Value != null
                        ? item.CreatedDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                        : string.Empty;

                    lbl_date_text.Text = date;


                    if (item.IsAwaitingApproval)
                    {
                        imgstatus_Approval.Visibility = ViewStates.Visible;
                        imgstatus_required.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        imgstatus_Approval.Visibility = ViewStates.Gone;
                        imgstatus_required.Visibility = ViewStates.Visible;
                    }


                    if (item.Comment != null && item.Comment != string.Empty)
                    {
                        Layout_comment.Visibility = ViewStates.Visible;
                        lbl_comment_text.Text = item.Comment;
                    }
                    else
                    {
                        Layout_comment.Visibility = ViewStates.Gone;
                    }
                    if (item.CreatedBy != null && item.CreatedBy != string.Empty)
                    {
                        Layout_submittedby.Visibility = ViewStates.Visible;
                        lbl_submittedby_email.Text = item.CreatedBy;
                    }
                    else
                    {
                        Layout_submittedby.Visibility = ViewStates.Gone;
                    }



                    if (item.DetailsUpdateList != null && item.DetailsUpdateList.Count != 0)
                    {
                        Layout_updated_details.Visibility = ViewStates.Visible;
                        updatedDetailsListAdapter = new UpdatedDetailsListAdapter(item.DetailsUpdateList);
                        layoutManagerService = new LinearLayoutManager(context, LinearLayoutManager.Vertical, false);
                        listview_updated_details.SetLayoutManager(layoutManagerService);
                        listview_updated_details.SetAdapter(updatedDetailsListAdapter);
                        updatedDetailsListAdapter.NotifyDataSetChanged();
                    }
                    else
                    {
                        Layout_updated_details.Visibility = ViewStates.Gone;
                    }

                    if (item.Reasons != null && item.Reasons.Count != 0)
                    {
                        Layout_reason.Visibility = ViewStates.Visible;
                        updatedDetailsListAdapter = new UpdatedDetailsListAdapter(item.Reasons);
                        layoutManagerService = new LinearLayoutManager(context, LinearLayoutManager.Vertical, false);
                        listview_reason_details.SetLayoutManager(layoutManagerService);
                        listview_reason_details.SetAdapter(updatedDetailsListAdapter);
                        updatedDetailsListAdapter.NotifyDataSetChanged();

                    }
                    else
                    {
                        Layout_reason.Visibility = ViewStates.Gone;
                    }
                    if (item.DocumentsUpdateList != null && item.DocumentsUpdateList.Count != 0)
                    {
                        Layout_attachments.Visibility = ViewStates.Visible;
                        updatedDetailsListAdapter = new UpdatedDetailsListAdapter(item.DocumentsUpdateList);
                        layoutManagerService = new LinearLayoutManager(context, LinearLayoutManager.Vertical, false);
                        listview_attachments_details.SetLayoutManager(layoutManagerService);
                        listview_attachments_details.SetAdapter(updatedDetailsListAdapter);
                        updatedDetailsListAdapter.NotifyDataSetChanged();
                    }
                    else
                    {
                        Layout_attachments.Visibility = ViewStates.Gone;
                    }


                    if (mApplicationActivityLogDetail.Count > 0 && mApplicationActivityLogDetail.Count == position + 1)
                    {
                        activityLogLine.Visibility = ViewStates.Gone;

                    }

                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }
    }
}