﻿using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB.Home.Components.UsageView
{
    public class TariffLegendComponent : BaseComponent
    {
        UIView _parentView, _containerView;
        List<LegendItemModel> _tariffLegendList = new List<LegendItemModel>();
        nfloat _totalHeight;

        public TariffLegendComponent(UIView parentView, List<LegendItemModel> tariffLegendList)
        {
            _parentView = parentView;
            _tariffLegendList = tariffLegendList;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat height = _parentView.Frame.Height;
            _containerView = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            for (int i = 0; i < _tariffLegendList.Count; i++)
            {
                _containerView.AddSubview(LegendItemView(i));
            }
            AdjustContainerHeight();
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private UIView LegendItemView(int index)
        {
            nfloat viewHeight = GetScaledHeight(14f) + GetScaledHeight(11f);
            nfloat viewXPos = GetScaledWidth(24f);
            nfloat viewYPos = viewHeight * index;
            nfloat viewWidth = _parentView.Frame.Width;

            UIView view = new UIView(new CGRect(viewXPos, viewYPos, viewWidth, viewHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            nfloat coulourViewWidth = GetScaledWidth(14f);
            nfloat coulourViewHeight = GetScaledWidth(14f);
            UIView colourView = new UIView(new CGRect(0, 0, coulourViewWidth, coulourViewHeight))
            {
                BackgroundColor = _tariffLegendList[index].Colour
            };
            colourView.Layer.CornerRadius = GetScaledHeight(7f);
            view.AddSubview(colourView);

            nfloat labelViewMargin = GetScaledWidth(24f);
            nfloat labelViewXPos = colourView.Frame.GetMaxX() + GetScaledWidth(12f);
            nfloat labelViewWidth = viewWidth - viewXPos - labelViewXPos - labelViewMargin;
            nfloat labelWidth = labelViewWidth / 2;
            nfloat labelHeight = GetScaledHeight(14f);
            UIView labelView = new UIView(new CGRect(labelViewXPos, 0, labelViewWidth, labelHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            view.AddSubview(labelView);

            UILabel rangeLabel = new UILabel(new CGRect(0, 0, labelWidth, labelHeight))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Text = _tariffLegendList[index].BlockRange
            };
            labelView.AddSubview(rangeLabel);

            UILabel priceLabel = new UILabel(new CGRect(labelWidth, 0, labelWidth, labelHeight))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right,
                Text = _tariffLegendList[index].BlockPrice
            };
            labelView.AddSubview(priceLabel);
            _totalHeight = view.Frame.GetMaxY();
            return view;
        }

        private void AdjustContainerHeight()
        {
            ViewHelper.AdjustFrameSetHeight(_containerView, _totalHeight);
        }
    }
}
