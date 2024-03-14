using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB.Android.Src.FindUs.Adapter
{
    public class ServiceListAdapter : RecyclerView.Adapter
    {
        BaseAppCompatActivity mActivity;
        List<string> services = new List<string>();

        public override int ItemCount => services.Count;


        public ServiceListAdapter(BaseAppCompatActivity activity, List<string> data)
        {
            this.mActivity = activity;
            this.services.AddRange(data);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ServiceListViewHolder h = holder as ServiceListViewHolder;
            TextViewUtils.SetMuseoSans300Typeface(h.ServiceLabel, h.ServiceBullet);
            h.ServiceLabel.Text = services[position];
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ServiceListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ServiceListViewHolder(itemView);
        }

        public class ServiceListViewHolder : RecyclerView.ViewHolder
        {
            public TextView ServiceLabel { get; private set; }
            public TextView ServiceBullet { get; private set; }

            public ServiceListViewHolder(View itemView) : base(itemView)
            {
                ServiceLabel = itemView.FindViewById<TextView>(Resource.Id.text_service);
                ServiceBullet = itemView.FindViewById<TextView>(Resource.Id.text_bullet);
                TextViewUtils.SetTextSize16(ServiceBullet, ServiceLabel);
            }

        }
    }
}