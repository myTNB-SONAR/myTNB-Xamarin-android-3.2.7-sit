using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class SmartMeterCardComponent : BaseComponent
    {
        CustomUIView _containerView, _toolTipView;
        UILabel _toolTipLabel;
        UIView _parentView;
        List<UsageCostItemModel> _usageCostModel;
        RMkWhEnum _RMkWh;

        public SmartMeterCardComponent(UIView parentView, List<UsageCostItemModel> usageCostModel, RMkWhEnum RMkWh)
        {
            _parentView = parentView;
            _usageCostModel = usageCostModel;
            _RMkWh = RMkWh;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width - (BaseMarginWidth16 * 2);
            nfloat height = _RMkWh == RMkWhEnum.RM ? GetScaledHeight(169F) : GetScaledHeight(129F);
            _containerView = new CustomUIView(new CGRect(BaseMarginWidth16, 0, width, height))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = GetScaledHeight(5F);
            if (_usageCostModel != null && _usageCostModel.Count > 0)
            {
                _containerView.AddSubview(ItemDetailView(_containerView, 0, Constants.IMG_CalendarIcon, _usageCostModel[0]));
            }
            else
            {
                _containerView.AddSubview(ItemDetailView(_containerView, 0, Constants.IMG_CalendarIcon));
            }
            UIView line = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(64F), width - (BaseMarginHeight16 * 2), GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            _containerView.AddSubview(line);
            if (_usageCostModel != null && _usageCostModel.Count > 0)
            {
                _containerView.AddSubview(ItemDetailView(_containerView, line.Frame.GetMaxY(), Constants.IMG_PredictIcon, _usageCostModel[1]));
            }
            else
            {
                _containerView.AddSubview(ItemDetailView(_containerView, line.Frame.GetMaxY(), Constants.IMG_PredictIcon));
            }
            if (_RMkWh == RMkWhEnum.RM)
            {
                _toolTipView = new CustomUIView(new CGRect(BaseMarginWidth16, GetScaledHeight(64F * 2) + GetScaledHeight(1F), width - (BaseMarginHeight16 * 2), GetScaledHeight(24F)))
                {
                    BackgroundColor = MyTNBColor.IceBlue
                };
                _toolTipView.Layer.CornerRadius = GetScaledHeight(12F);
                UIImageView toolTipIcon = new UIImageView(new CGRect(GetScaledWidth(4F), GetScaledHeight(4F), GetScaledWidth(16F), GetScaledHeight(16F)))
                {
                    Image = UIImage.FromBundle(Constants.IMG_InfoBlue)
                };
                _toolTipLabel = new UILabel(new CGRect(toolTipIcon.Frame.GetMaxX() + GetScaledWidth(8F), GetScaledHeight(4F), _toolTipView.Frame.Width * .80F, GetScaledHeight(16F)))
                {
                    Font = TNBFont.MuseoSans_12_500,
                    TextColor = MyTNBColor.WaterBlue,
                    TextAlignment = UITextAlignment.Left
                };
                _toolTipView.AddSubviews(new UIView { _toolTipLabel, toolTipIcon });
                _containerView.AddSubview(_toolTipView);
            }
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private UIView ItemDetailView(UIView parentView, nfloat yPos, string imageName, UsageCostItemModel model = null)
        {
            UIImageView icon;
            UILabel title, dateRange, amount;
            nfloat width = parentView.Frame.Width;
            nfloat height = GetScaledHeight(64F);
            nfloat iconWidth = GetScaledWidth(28F);
            nfloat iconHeight = GetScaledHeight(28F);
            bool isCurrentUsage = model?.UsageCostType == UsageCostEnum.CURRENTUSAGE && _RMkWh == RMkWhEnum.kWh;
            bool isAverageUsage = model?.UsageCostType == UsageCostEnum.AVERAGEUSAGE && _RMkWh == RMkWhEnum.kWh;

            UIView itemView = new UIView(new CGRect(0, yPos, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            icon = new UIImageView(new CGRect(BaseMarginWidth16, GetScaledHeight(18F), iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(!isAverageUsage ? imageName : Constants.IMG_TrendIcon)
            };
            title = new UILabel(new CGRect(icon.Frame.GetMaxX() + GetScaledWidth(12), BaseMarginHeight16, width * .70F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.GreyishBrownTwo,
                TextAlignment = UITextAlignment.Left,
                Text = (model != null) ? model.Title : string.Empty
            };
            dateRange = new UILabel(new CGRect(icon.Frame.GetMaxX() + GetScaledWidth(12), title.Frame.GetMaxY(), width * .70F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.BrownishGrey,
                TextAlignment = UITextAlignment.Left,
                Text = (model != null) ? model.SubTitle : string.Empty
            };
            amount = new UILabel(new CGRect(width * .50F - BaseMarginWidth16, GetScaledHeight(22F), width * .50F, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.GreyishBrownTwo,
                TextAlignment = UITextAlignment.Right
            };
            if (isAverageUsage)
            {
                amount.Text = (model != null) ? model.Value : "--";
                CGSize size = amount.SizeThatFits(new CGSize(width, 1000f));
                ViewHelper.AdjustFrameSetWidth(amount, size.Width);
                ViewHelper.AdjustFrameSetX(amount, width - size.Width - BaseMarginWidth16);
                AddTrendIcon(ref itemView, amount, model);
            }
            else
            {
                string amountStr = (model != null) ? model.Value : "--";
                string valueUnitStr = (model != null) ? model.ValueUnit : "RM";
                amount.AttributedText = TextHelper.CreateValuePairString(amountStr + " "
                        , valueUnitStr + " ", !isCurrentUsage, TNBFont.MuseoSans_16_300
                        , MyTNBColor.GreyishBrownTwo, TNBFont.MuseoSans_10_300, MyTNBColor.GreyishBrownTwo);
                CGSize size = amount.SizeThatFits(new CGSize(width, 1000f));
                ViewHelper.AdjustFrameSetWidth(amount, size.Width);
                ViewHelper.AdjustFrameSetX(amount, width - size.Width - BaseMarginWidth16);
            }
            itemView.AddSubviews(new UIView { icon, title, dateRange, amount });
            return itemView;
        }

        private void AddTrendIcon(ref UIView itemView, UILabel amount, UsageCostItemModel model)
        {
            if (model.TrendType != TrendEnum.None)
            {
                UIImageView trendIcon = new UIImageView(new CGRect(amount.Frame.GetMinX() - GetScaledWidth(6F) - GetScaledWidth(4.5F), amount.Frame.GetMinY() + GetScaledHeight(4F), GetScaledWidth(6F), GetScaledHeight(12F)))
                {
                    Image = UIImage.FromBundle((model.TrendType == TrendEnum.UP) ? Constants.IMG_TrendUpIcon : Constants.IMG_TrendDownIcon)
                };
                itemView.AddSubview(trendIcon);
            }
        }

        public void SetTooltipText(string text)
        {
            if (_toolTipLabel != null)
            {
                _toolTipLabel.Text = text;
            }
        }

        public void SetTooltipTapRecognizer(UITapGestureRecognizer recognizer)
        {
            if (_toolTipView != null)
            {
                _toolTipView.AddGestureRecognizer(recognizer);
            }
        }

        public virtual CustomUIView GetShimmerUI()
        {
            nfloat baseWidth = _parentView.Frame.Width - (BaseMarginWidth16 * 2);
            nfloat baseHeight = GetScaledHeight(169F);

            CustomShimmerView shimmeringView = new CustomShimmerView();
            CustomUIView parentView = new CustomUIView(new CGRect(BaseMarginWidth16, 0
                , baseWidth, baseHeight))
            { BackgroundColor = UIColor.White };
            parentView.Layer.CornerRadius = GetScaledHeight(4f);
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            parentView.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            viewShimmerContent.AddSubview(ItemDetailShimmerView(parentView, 0));
            UIView line = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(64F), parentView.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            viewShimmerContent.AddSubview(line);
            viewShimmerContent.AddSubview(ItemDetailShimmerView(parentView, line.Frame.GetMaxY()));

            UIView tooltipView = new CustomUIView(new CGRect(BaseMarginWidth16, GetScaledHeight(64F * 2) + GetScaledHeight(1F), parentView.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(24F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            viewShimmerContent.AddSubview(tooltipView);

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            return parentView;
        }

        private UIView ItemDetailShimmerView(UIView parentView, nfloat yPos)
        {
            nfloat width = parentView.Frame.Width;
            nfloat height = GetScaledHeight(64F);
            nfloat iconWidth = GetScaledWidth(28F);
            nfloat iconHeight = GetScaledHeight(28F);
            UIView itemView = new UIView(new CGRect(0, yPos, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            UIView iconView = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(18F), iconWidth, iconHeight))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            iconView.Layer.CornerRadius = GetScaledHeight(14f);
            UIView titleView = new UIView(new CGRect(iconView.Frame.GetMaxX() + GetScaledWidth(12), BaseMarginHeight16, width * .30F, GetScaledHeight(16F) * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            titleView.Layer.CornerRadius = GetScaledHeight(2f);
            UIView dateRangeView = new UIView(new CGRect(iconView.Frame.GetMaxX() + GetScaledWidth(12), titleView.Frame.GetMaxY() + GetScaledHeight(5f), width * .20F, GetScaledHeight(16F) * 0.7F))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            dateRangeView.Layer.CornerRadius = GetScaledHeight(2f);
            UIView amountView = new UIView(new CGRect(width * .75F - BaseMarginWidth16, GetScaledHeight(22F), width * .25F, GetScaledHeight(20F) * 0.8F))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            amountView.Layer.CornerRadius = GetScaledHeight(2f);
            itemView.AddSubviews(new UIView { iconView, titleView, dateRangeView, amountView });
            return itemView;
        }
    }
}
