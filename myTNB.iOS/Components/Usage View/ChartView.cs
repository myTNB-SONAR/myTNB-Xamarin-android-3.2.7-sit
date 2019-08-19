using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Force.DeepCloner;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class ChartView : BaseComponent
    {
        public ChartView()
        {
        }

        private CustomUIView _mainView, _segmentContainer;
        private nfloat _width, _baseMargin, _baseMarginedWidth;
        private UILabel _lblDateRange;

        private void CreatUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            _baseMargin = GetScaledWidth(16);
            _baseMarginedWidth = _width - (_baseMargin * 2);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, GetScaledHeight(189)));

            _lblDateRange = new UILabel(new CGRect(_baseMargin, 0, _baseMarginedWidth, GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.ButterScotch,
                Font = TNBFont.MuseoSans_12_500,
                Text = AccountUsageCache.ByMonthDateRange
            };

            CustomUIView viewLine = new CustomUIView(new CGRect(_baseMargin, GetYLocationFromFrame(_lblDateRange.Frame, 150)//GetScaledHeight(182)
                , _baseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F)
            };

            _mainView.AddSubviews(new UIView[] { _lblDateRange, viewLine });
            CreateSegment();
        }

        private void CreateSegment()
        {
            _segmentContainer = new CustomUIView(new CGRect(0, GetYLocationFromFrame(_lblDateRange.Frame, 16)
               , _width, GetScaledHeight(157)));
            //_segmentContainer.Layer.BorderWidth = 1;
            //_segmentContainer.Layer.BorderColor = UIColor.Red.CGColor;

            nfloat height = _segmentContainer.Frame.Height;
            nfloat width = GetScaledWidth(12);
            nfloat barMargin = GetScaledWidth(36);
            nfloat baseMargin = GetScaledWidth(34);
            nfloat xLoc = baseMargin;
            nfloat lblHeight = GetScaledHeight(14);
            nfloat maxBarHeight = GetScaledHeight(108);// height * 0.688F;
            nfloat amountBarMargin = GetScaledHeight(4);

            List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            List<string> valueList = usageData.Select(x => x.UsageTotal).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;

            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                MonthItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, width, height)) { Tag = index };
                // { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.5F) };
                _segmentContainer.AddSubview(segment);
                xLoc += width + barMargin;

                double.TryParse(item.UsageTotal, out double value);
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = lblHeight + amountBarMargin + (maxBarHeight - barHeight);

                CustomUIView viewBar = new CustomUIView(new CGRect(0, yLoc, width, barHeight))
                {
                    BackgroundColor = UIColor.Clear,// i < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Tag = 1001,
                    ClipsToBounds = true
                };
                viewBar.Layer.CornerRadius = width / 2;
                AddTariffBlocks(viewBar, item.tariffBlocks, value, index == usageData.Count - 1);
                UIView viewCover = new UIView(new CGRect(new CGPoint(0, 0), viewBar.Frame.Size))
                {
                    BackgroundColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Tag = 2001,
                    Hidden = !false
                };
                viewBar.AddSubview(viewCover);

                UILabel lblAmount = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                    , GetScaledWidth(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = string.Format("{0} {1}", item.Currency, item.AmountTotal),
                    Hidden = index < usageData.Count - 1,
                    Tag = 1002
                };
                nfloat lblAmountWidth = lblAmount.GetLabelWidth(GetScaledWidth(100));
                lblAmount.Frame = new CGRect((width - lblAmountWidth) / 2, lblAmount.Frame.Y, lblAmountWidth, lblAmount.Frame.Height);

                UILabel lblDate = new UILabel(new CGRect(0, segment.Frame.Height - lblHeight
                    , GetScaledWidth(30), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = string.IsNullOrEmpty(item.Year) ? item.Month : string.Format("{0} {1}", item.Month, item.Year),
                    Tag = 1003
                };
                nfloat lblDateWidth = lblDate.GetLabelWidth(GetScaledWidth(30));
                lblDate.Frame = new CGRect((width - lblDateWidth) / 2, lblDate.Frame.Y, lblDateWidth, lblDate.Frame.Height);

                segment.AddSubviews(new UIView[] { lblAmount, viewBar, lblDate });

                segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSegmentTap(index);
                }));
            }

            _mainView.AddSubview(_segmentContainer);
            //_mainView.SendSubviewToBack(_segmentContainer);
        }

        private double GetMaxValue(RMkWhEnum view, List<string> value)
        {
            double maxValue = 0;
            switch (view)
            {
                case RMkWhEnum.kWh:
                    {
                        maxValue = value.Max(x => Math.Abs(TextHelper.ParseStringToDouble(x)));
                        break;
                    }
                case RMkWhEnum.RM:
                    {
                        maxValue = value.Max(x => Math.Abs(TextHelper.ParseStringToDouble(x)));
                        break;
                    }
                default:
                    {
                        maxValue = 0;
                        break;
                    }
            }
            return maxValue;
        }

        private void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariff, double baseValue, bool isSelected)
        {
            nfloat baseHeigt = viewBar.Frame.Height;
            nfloat barMaxY = viewBar.Frame.Height;
            UIView viewTariffContainer = new UIView(new CGRect(0, 0, viewBar.Frame.Width, viewBar.Frame.Height))
            {
                Tag = 2002,
                Hidden = !true
            };
            for (int i = 0; i < tariff.Count; i++)
            {
                TariffItemModel item = tariff[i];
                double.TryParse(item.Usage, out double val);
                nfloat percentage = (nfloat)(val / baseValue);
                nfloat blockHeight = baseHeigt * percentage;
                barMaxY -= blockHeight;
                UIView viewTariffBlock = new UIView(new CGRect(0, barMaxY, viewBar.Frame.Width, blockHeight))
                {
                    BackgroundColor = GetTariffBlockColor(item.BlockId, isSelected)
                };
                viewTariffContainer.AddSubview(viewTariffBlock);
                barMaxY -= GetScaledHeight(1);
            }
            viewBar.AddSubview(viewTariffContainer);
        }

        private UIColor GetTariffBlockColor(string blockID, bool isSelected)
        {
            List<LegendItemModel> legend = AccountUsageCache.GetTariffLegendList();
            LegendItemModel item = legend.Find(x => x.BlockId == blockID);
            if (item != null)
            {
                return new UIColor((nfloat)item.RGB.R / 255F, (nfloat)item.RGB.G / 255F, (nfloat)item.RGB.B / 255F, isSelected ? 1F : 0.5F);
            }
            return UIColor.White;
        }

        private void OnSegmentTap(int index)
        {
            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                bool isSelected = segmentView.Tag == index;
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                UIView viewCover = bar.ViewWithTag(2001);
                viewCover.BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                UIView viewTariff = bar.ViewWithTag(2002);
                if (!viewTariff.Hidden)
                {
                    for (int j = 0; j < viewTariff.Subviews.Count(); j++)
                    {
                        UIView tBlock = viewTariff.Subviews[j];
                        UIColor tColor = tBlock.BackgroundColor;
                        nint componentCount = tColor.CGColor.NumberOfComponents;
                        if (componentCount == 4)
                        {
                            nfloat[] components = tColor.CGColor.Components;
                            nfloat alpha = isSelected ? 1F : 0.5F;
                            tBlock.BackgroundColor = new UIColor(components[0], components[1], components[2], alpha);
                        }
                    }
                }
                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                value.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                value.Hidden = !isSelected;
                UILabel date = segmentView.ViewWithTag(1003) as UILabel;
                date.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
            }
        }

        public CustomUIView GetUI()
        {
            CreatUI();
            return _mainView;
        }
    }
}
