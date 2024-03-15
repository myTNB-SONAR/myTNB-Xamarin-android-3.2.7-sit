
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.AndroidApp.Src.QuickActionArrange.Model;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.QuickActionArrange.Adapter
{
    public class QuickActionLockedAndExtraAdapter : RecyclerView.Adapter
    {
        private Android.App.Activity mActivity;
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> IconClick;
        private List<Feature> IconList = new List<Feature>();
        private IItemClickListener listener;

        public override int ItemCount => IconList.Count;

        public void Clear()
        {
            this.IconList.Clear();
            this.NotifyDataSetChanged();
        }

        public void Add(Feature newData)
        {
            this.IconList.Add(newData);
            this.NotifyItemInserted(IconList.IndexOf(newData));
        }

        public void AddAll(List<Feature> allData)
        {
            this.IconList.AddRange(allData);
            this.NotifyDataSetChanged();
        }

        public QuickActionLockedAndExtraAdapter(Android.App.Activity activity, List<Feature> data)
        {
            this.mActivity = activity;
            this.IconList = data;
            //this.IconList.AddRange(data);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            QuickActionLockedAndExtraViewHolder vh = holder as QuickActionLockedAndExtraViewHolder;

            Feature item = IconList[position];
            try
            {
                vh.IconName.Text = filterServiceName(item.ServiceName);

                if (item.isLocked)
                {
                    vh.Icon.SetImageResource(Resource.Drawable.ic_locked_white);
                }
                else if (!item.isAvailable)
                {
                    vh.Icon.SetImageResource(Resource.Drawable.ic_plusround_black);
                    vh.Icon.Clickable = true;
                    vh.Icon.Click += (sender, e) =>
                    {
                        listener?.OnItemClick(position);
                    };
                }
                try
                {
                    switch (item.ServiceId)
                    {
                        case "1001":
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.submit_meter);
                            break;
                        case "1003":
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.feedback);
                            break;
                        case "1004":
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.bills);
                            break;
                        case "1005":
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.pdf_bill);
                            break;
                        case "1006":
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.check_status);
                            break;
                        case "1007":
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.Check_Status_Icon);
                            break;
                        case "1008":
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.Icon_Quick_Access_MyHome);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }


            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string filterServiceName(string name)
        {
            string inputString = name;
            string resultString = inputString.Replace("<br>", " ");
            return resultString;
        }

        public void SetItemClickListener(IItemClickListener clickListener)
        {
            this.listener = clickListener;
        }

        public interface IItemClickListener
        {
            void OnItemClick(int position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.QuickActionOtherItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new QuickActionLockedAndExtraViewHolder(itemView);
        }

        public class QuickActionLockedAndExtraViewHolder : RecyclerView.ViewHolder
        {
            public TextView IconName { get; private set; }
            public ImageView Icon { get; private set; }
            public ImageView QuickActionIcon { get; private set; }
            public LinearLayout ItemLayout { get; private set; }
            private Context context;

            private readonly string EG_ACCOUNT_LABEL = "";

            private Feature item = null;

            public QuickActionLockedAndExtraViewHolder(View itemView) : base(itemView)
            {
                Icon = itemView.FindViewById<ImageView>(Resource.Id.imageActionIcon);
                QuickActionIcon = itemView.FindViewById<ImageView>(Resource.Id.imageIcon);
                IconName = itemView.FindViewById<TextView>(Resource.Id.txtAccountName);
                ItemLayout = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                TextViewUtils.SetMuseoSans300Typeface(IconName);
                context = itemView.Context;

                TextViewUtils.SetTextSize13(IconName);
            }
        }
    }
}

