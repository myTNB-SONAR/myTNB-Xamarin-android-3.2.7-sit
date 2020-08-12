using Android.Icu.Text;

using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Adapter.BillsMenu
{
    public class ItemizedBillingDetailsAdapter : RecyclerView.Adapter
    {

        private Android.App.Activity mActicity;
        private List<ItemizedBillingDetails> itemizedBillingDetails = new List<ItemizedBillingDetails>();

        DecimalFormat decimalFormatter = new DecimalFormat("###,###,###,###,##0.00");

        public ItemizedBillingDetailsAdapter(Android.App.Activity activity, List<ItemizedBillingDetails> data)
        {
            this.mActicity = activity;
            this.itemizedBillingDetails.AddRange(data);
        }

        public override int ItemCount => itemizedBillingDetails.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ItemizedBillingDetailsViewHolder vh = holder as ItemizedBillingDetailsViewHolder;

            ItemizedBillingDetails model = itemizedBillingDetails[position];
            vh.Title.Text = model.NonConsumpChargeName;
            vh.AmountContent.Text = decimalFormatter.Format(model.NonConsumpChargeValue);

            TextViewUtils.SetMuseoSans500Typeface(vh.Title, vh.AmountContent);
            TextViewUtils.SetMuseoSans300Typeface(vh.AmountCurrency);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.MandatoryPaymentsListView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ItemizedBillingDetailsViewHolder(itemView);
        }

        public class ItemizedBillingDetailsViewHolder : RecyclerView.ViewHolder
        {
            public RelativeLayout RootView { get; private set; }
            public TextView Title { get; private set; }
            public TextView AmountCurrency { get; private set; }
            public TextView AmountContent { get; private set; }

            public ItemizedBillingDetailsViewHolder(View itemView) : base(itemView)
            {
                RootView = itemView.FindViewById<RelativeLayout>(Resource.Id.rootView);
                Title = itemView.FindViewById<TextView>(Resource.Id.txtMandatoryPaymentTitle);
                AmountCurrency = itemView.FindViewById<TextView>(Resource.Id.txtMandatoryPaymentRM);
                AmountContent = itemView.FindViewById<TextView>(Resource.Id.txtMandatoryPaymentContent);
            }
        }
    }
}