using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.Adapter
{
    public class UnderstandBillToolTipAdapter : RecyclerView.Adapter
    {
        List<UnderstandTooltipModel> mTooltipModelList;
        ViewGroup mParent;
        public UnderstandBillToolTipAdapter(List<UnderstandTooltipModel> tooltipModelList)
        {
            mTooltipModelList = tooltipModelList;
        }

        public override int ItemCount => mTooltipModelList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            UnderstandBillViewHolder vh = holder as UnderstandBillViewHolder;
            UnderstandTooltipModel model = mTooltipModelList[position];
            if (model.TooltipImage == null)
            {
                vh.imageToolTip.SetImageResource(Resource.Drawable.understand_bill_tooltip_1);
            }
            else
            {
                vh.imageToolTip.SetImageBitmap(model.TooltipImage);
            }
            vh.txtToolTipTitle.Text = model.Title;

            LinearLayout linearLayout;
            for (int i=0; i < model.ItemList.Count; i++)
            {
                linearLayout = (LinearLayout)LayoutInflater.From(this.mParent.Context).Inflate(Resource.Layout.UnderstandBillTooltipItem, this.mParent, false);
                TextView labelTitle = linearLayout.FindViewById<TextView>(Resource.Id.txtBillItemLabel);
                labelTitle.Text = model.ItemList[i];
                vh.tooltipItemsContent.AddView(linearLayout);
            }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.UnderstandBillTooltipItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id,parent,false);
            mParent = parent;
            return new UnderstandBillViewHolder(itemView);
        }
    }

    public class UnderstandBillViewHolder : RecyclerView.ViewHolder
    {
        //[BindView(Resource.Id.txtToolTipTitle)]
        public TextView txtToolTipTitle;
        public ImageView imageToolTip;
        public LinearLayout tooltipItemsContent;

        public UnderstandBillViewHolder(View itemView) : base(itemView)
        {
            imageToolTip = itemView.FindViewById<ImageView>(Resource.Id.tooltipImageHeader);
            txtToolTipTitle = itemView.FindViewById<TextView>(Resource.Id.txtToolTipTitle);
            tooltipItemsContent = itemView.FindViewById<LinearLayout>(Resource.Id.tooltipItemsContent);
        }
    }
}
