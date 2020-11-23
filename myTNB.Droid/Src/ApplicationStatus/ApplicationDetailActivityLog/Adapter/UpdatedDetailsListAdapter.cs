using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.Adapter.ApplicationDetailActivityAdapter;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.Adapter
{
    public class UpdatedDetailsListAdapter : RecyclerView.Adapter
    {
       
        List<string> services = new List<string>();

        public override int ItemCount => services.Count;
        public UpdatedDetailsListAdapter(List<string> data)
        {
            
            this.services.AddRange(data);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ServiceListViewHolder h = holder as ServiceListViewHolder;
            TextViewUtils.SetMuseoSans300Typeface(h.ServiceLabel, h.ServiceBullet);
            h.ServiceLabel.Text = services[position];
            h.ServiceLabel.TextSize = TextViewUtils.GetFontSize(16);
            h.ServiceBullet.TextSize = TextViewUtils.GetFontSize(16);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationUpdatedDetailsVIew;
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
                TextViewUtils.SetMuseoSans300Typeface(ServiceLabel);
            }

        }
    }
}

