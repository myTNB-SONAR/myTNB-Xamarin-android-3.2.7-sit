using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.ManageBillDelivery.ManageBillDeliveryEmailList.Adapter
{
    
    public class ManageBillDeliveryEmailListAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        //private List<ApplicationStatusColorCodeModel> mStatusCodeColorList = new List<ApplicationStatusColorCodeModel>();
        private List<EmailModel> emailModel = new List<EmailModel>();
        public event EventHandler<int> ItemClick;


        public void Clear()
        {
            this.emailModel.Clear();
            this.NotifyDataSetChanged();
        }

        public List<EmailModel> GetApplicationStatusList()
        {
            return emailModel;
        }

        public void UpdateAddList(List<EmailModel> data)
        {
            this.emailModel.AddRange(data);
            this.NotifyDataSetChanged();
        }

        public ManageBillDeliveryEmailListAdapter(BaseActivityCustom activity, List<EmailModel> data)
        {
            this.mActicity = activity;
            this.emailModel.AddRange(data);
            //this.mStatusCodeColorList.AddRange(color);
        }

        public override int ItemCount => emailModel.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ManageBillDeliveryEmailViewHolder vh = holder as ManageBillDeliveryEmailViewHolder;

            EmailModel item = emailModel[position];
            //ApplicationStatusColorCodeModel color = mStatusCodeColorList.Find(x => x.Code == item.StatusCode);
            vh.PopulateData(item);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ManageBillDeliveryEmailGroupItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ManageBillDeliveryEmailViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }

    public class ManageBillDeliveryEmailViewHolder : RecyclerView.ViewHolder
    {
        public TextView deliveryEmail { get; private set; }
        public TextView deliveryUserName { get; private set; }
        public LinearLayout ManageBillDeliveryEmailListLayout { get; private set; }
        private Context context;

        private EmailModel item = null;

        public ManageBillDeliveryEmailViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            context = itemView.Context;
            ManageBillDeliveryEmailListLayout = itemView.FindViewById<LinearLayout>(Resource.Id.ManageBillDeliveryEmailListLayout);
            deliveryEmail = itemView.FindViewById<TextView>(Resource.Id.deliveryEmail);
            deliveryUserName = itemView.FindViewById<TextView>(Resource.Id.deliveryUserName);
            deliveryEmail.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.tunaGrey)));
            deliveryUserName.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.receipt_note_text)));
            TextViewUtils.SetMuseoSans300Typeface(deliveryUserName);
            TextViewUtils.SetMuseoSans500Typeface(deliveryEmail);
        }

        public void PopulateData(EmailModel item)
        {
            this.item = item;
            try
            {
                UserEntity user = UserEntity.GetActive();
                deliveryEmail.Text = this.item.Email;
                deliveryUserName.Text = item.IsOwner ? user.DisplayName +" "+ Utility.GetLocalizedLabel("ManageDigitalBillLanding", "you") : item.Name;
                TextViewUtils.SetTextSize12(deliveryUserName);
                TextViewUtils.SetTextSize14(deliveryEmail);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}