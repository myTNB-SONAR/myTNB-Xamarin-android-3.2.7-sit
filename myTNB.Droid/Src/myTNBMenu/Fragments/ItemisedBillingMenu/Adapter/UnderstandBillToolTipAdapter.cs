﻿using System;
using System.Collections.Generic;
using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.ItemisedBillingMenu.Adapter
{
    public class UnderstandBillToolTipAdapter : RecyclerView.Adapter
    {
        List<UnderstandTooltipModel> mTooltipModelList;
        ViewGroup mParent;
        bool mIsCAEligibleToBR;
        public UnderstandBillToolTipAdapter(List<UnderstandTooltipModel> tooltipModelList, bool isCAEligibleToBR)
        {
            mTooltipModelList = tooltipModelList;
            mIsCAEligibleToBR = isCAEligibleToBR;
        }

        public override int ItemCount => mTooltipModelList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            UnderstandBillViewHolder vh = holder as UnderstandBillViewHolder;
            UnderstandTooltipModel model = mTooltipModelList[position];
            if (model.TooltipImage == null)
            {
                if (position == 0)
                {
                    vh.imageToolTip.SetImageResource(mIsCAEligibleToBR ? Resource.Drawable.Banner_Bill_Redesign_Tooltip_1 : Resource.Drawable.understand_bill_tooltip_1);
                }
                else
                {
                    vh.imageToolTip.SetImageResource(mIsCAEligibleToBR ? Resource.Drawable.Banner_Bill_Redesign_Tooltip_2 : Resource.Drawable.understand_bill_tooltip_2);
                }
            }
            else
            {
                vh.imageToolTip.SetImageBitmap(model.TooltipImage);
            }
            vh.txtToolTipTitle.Text = model.Title;

            LinearLayout linearLayout;
            for (int i = 0; i < model.ItemList.Count; i++)
            {
                linearLayout = (LinearLayout)LayoutInflater.From(this.mParent.Context).Inflate(Resource.Layout.UnderstandBillTooltipItem, this.mParent, false);
                TextView labelNumber = linearLayout.FindViewById<TextView>(Resource.Id.txtBillItemNumber);
                TextView labelTitle = linearLayout.FindViewById<TextView>(Resource.Id.txtBillItemLabel);
                TextViewUtils.SetMuseoSans300Typeface(labelTitle);
                TextViewUtils.SetMuseoSans500Typeface(labelNumber);
                TextViewUtils.SetTextSize12(labelNumber);
                TextViewUtils.SetTextSize14(labelTitle);

                labelTitle.Text = model.ItemList[i];
                labelNumber.Text = (i + 1).ToString();
                vh.tooltipItemsContent.AddView(linearLayout);
            }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.UnderstandBillTooltipItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
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

            TextViewUtils.SetMuseoSans500Typeface(txtToolTipTitle);
            TextViewUtils.SetTextSize14(txtToolTipTitle);
        }
    }
}
