using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class SmartMeterZOut : BaseChartView
    {
        public SmartMeterZOut()
        {
            ShimmerHeight = GetHeightByScreenSize(189);
        }

        protected override void CreatUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            _baseMargin = GetWidthByScreenSize(16);
            _baseMarginedWidth = _width - (_baseMargin * 2);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, GetHeightByScreenSize(189)));

            _lblDateRange = new UILabel(new CGRect(_baseMargin, 0, _baseMarginedWidth, GetHeightByScreenSize(16)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.ButterScotch,
                Font = TNBFont.MuseoSans_12_500,
                Text = "22 Jul - 21 Aug 2019"
            };

            nfloat zoomSize = GetHeightByScreenSize(24);
            _zoomView = new CustomUIView(new CGRect(GetWidthByScreenSize(280), 0, zoomSize, zoomSize));
            _zoomView.AddSubview(new UIImageView(new CGRect(new CGPoint(0, 0), _zoomView.Frame.Size))
            {
                Image = UIImage.FromBundle("IC-Action-Zoom")
            });

            CreateSegment();

            CustomUIView viewLine = new CustomUIView(new CGRect(_baseMargin, GetYLocationFromFrameScreenSize(_segmentContainer.Frame, 9)
                , _baseMarginedWidth, GetHeightByScreenSize(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };

            _mainView.AddSubviews(new UIView[] { _lblDateRange, viewLine });
            _mainView.AddSubview(_zoomView);
        }

        protected override void CreateSegment()
        {
            _segmentContainer = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(_lblDateRange.Frame, 24)
               , _width, GetHeightByScreenSize(128)));

            _segmentContainer.Layer.BorderColor = UIColor.Red.CGColor;
            _segmentContainer.Layer.BorderWidth = 1F;

            nfloat height = _segmentContainer.Frame.Height;
            nfloat width = GetWidthByScreenSize(5);
            nfloat segmentMargin = GetWidthByScreenSize(4);
            nfloat baseMargin = GetWidthByScreenSize(23);
            nfloat xLoc = baseMargin;
            nfloat lblHeight = GetHeightByScreenSize(14);
            nfloat maxBarHeight = GetHeightByScreenSize(118);
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            nfloat segmentWidth = GetWidthByScreenSize(5);
            nfloat barMargin = 0; //GetWidthByScreenSize(9);

            List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            List<string> valueList = usageData.Select(x => x.UsageTotal).ToList();
            double maxValue = 10;// GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;

            for (int i = 0; i < 31; i++)// usageData.Count; i++)
            {
                int index = i;
                //MonthItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, _segmentContainer.Frame.Height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapNormalBar"
                };

                segment.Layer.BorderColor = UIColor.Blue.CGColor;
                segment.Layer.BorderWidth = 1F;

                _segmentContainer.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                //double.TryParse(item.UsageTotal, out double value);
                double value = 10;
                nfloat barHeight = (nfloat)(divisor * value);
                //  nfloat yLoc = segment.Frame.Height - barHeight - GetHeightByScreenSize(10);

                nfloat yLoc = GetHeightByScreenSize(10);
                CustomUIView viewBar = new CustomUIView(new CGRect(barMargin, segment.Frame.Height, width, 0))
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = 1001,
                    ClipsToBounds = true
                };
                viewBar.Layer.CornerRadius = width / 2;
                UIView viewCover = new UIView(new CGRect(new CGPoint(0, 0), new CGSize(viewBar.Frame.Width, barHeight)))
                {
                    BackgroundColor = UIColor.White,
                    Tag = 2001,
                    Hidden = false
                };
                viewBar.AddSubview(viewCover);
                //AddTariffBlocks(viewBar, item.tariffBlocks, value, index == usageData.Count - 1, viewCover.Frame.Size);


                bool hasMissingReading = true;
                UIImageView imgMissingReading = null;
                if (hasMissingReading)
                {
                    nfloat imgSize = GetWidthByScreenSize(5);
                    imgMissingReading = new UIImageView(new CGRect(0, viewBar.Frame.GetMinY(), imgSize, imgSize))
                    {
                        Image = UIImage.FromBundle("Usage-MissingReading")
                    };
                    segment.Add(imgMissingReading);
                }


                /* nfloat amtYLoc = yLoc - amountBarMargin - lblHeight;
                 UILabel lblAmount = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                     , GetWidthByScreenSize(100), lblHeight))
                 {
                     TextAlignment = UITextAlignment.Center,
                     Font = TNBFont.MuseoSans_10_300,
                     TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                     Text = item.AmountTotal.FormatAmountString(item.Currency),
                     Hidden = index < usageData.Count - 1,
                     Tag = 1002
                 };
                 nfloat lblAmountWidth = lblAmount.GetLabelWidth(GetWidthByScreenSize(100));
                 lblAmount.Frame = new CGRect((segmentWidth - lblAmountWidth) / 2, lblAmount.Frame.Y, lblAmountWidth, lblAmount.Frame.Height);

                 UILabel lblDate = new UILabel(new CGRect(0, segment.Frame.Height - lblHeight
                     , GetWidthByScreenSize(30), lblHeight))
                 {
                     TextAlignment = UITextAlignment.Center,
                     Font = TNBFont.MuseoSans_10_300,
                     TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                     Text = string.IsNullOrEmpty(item.Year) ? item.Month : string.Format(Format_Value, item.Month, item.Year),
                     Tag = 1003
                 };
                 nfloat lblDateWidth = lblDate.GetLabelWidth(GetWidthByScreenSize(30));
                 lblDate.Frame = new CGRect((segmentWidth - lblDateWidth) / 2, lblDate.Frame.Y, lblDateWidth, lblDate.Frame.Height);
                 */
                segment.AddSubviews(new UIView[] { viewBar });//, lblAmount, viewBar,lblDate });

                segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    //OnSegmentTap(index);
                }));

                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                    , () =>
                    {
                        viewBar.Frame = new CGRect(viewBar.Frame.X, yLoc, viewBar.Frame.Width, barHeight);
                        if (imgMissingReading != null)
                        {
                            imgMissingReading.Frame = new CGRect(imgMissingReading.Frame.X, 0
                                , imgMissingReading.Frame.Width, imgMissingReading.Frame.Height);
                        }
                        //lblAmount.Frame = new CGRect(lblAmount.Frame.X, amtYLoc, lblAmount.Frame.Width, lblAmount.Frame.Height);
                    }
                    , () => { }
                );
            }
            _mainView.AddSubview(_segmentContainer);
        }

        protected override void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size)
        {
            if (viewBar == null || tariffList == null || tariffList.Count == 0) { return; }
            nfloat baseHeigt = size.Height;
            nfloat barMaxY = size.Height;
            UIView viewTariffContainer = new UIView(new CGRect(0, 0, size.Width, size.Height))
            {
                Tag = 2002,
                Hidden = true
            };
            for (int i = 0; i < tariffList.Count; i++)
            {
                TariffItemModel item = tariffList[i];
                double.TryParse(item.Usage, out double val);
                nfloat percentage = (nfloat)(val / baseValue);
                nfloat blockHeight = baseHeigt * percentage;
                barMaxY -= blockHeight;
                UIView viewTariffBlock = new UIView(new CGRect(0, barMaxY, size.Width, blockHeight))
                {
                    BackgroundColor = GetTariffBlockColor(item.BlockId, isSelected)
                };
                viewTariffContainer.AddSubview(viewTariffBlock);
                barMaxY -= GetHeightByScreenSize(1);
            }
            viewBar.AddSubview(viewTariffContainer);
        }

        protected override void OnSegmentTap(int index)
        {
            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                bool isSelected = segmentView.Tag == index;
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (bar != null)
                {
                    UIView viewCover = bar.ViewWithTag(2001);
                    if (viewCover != null) { viewCover.BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F); }
                    UIView viewTariff = bar.ViewWithTag(2002);
                    if (viewTariff != null)
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
                }
                /* UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                 if (value != null)
                 {
                     value.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                     value.Hidden = !isSelected;
                 }
                 UILabel date = segmentView.ViewWithTag(1003) as UILabel;
                 if (date != null) { date.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F); }
                 */
            }
        }

        public override void ToggleTariffView(bool isTariffView)
        {
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (bar == null) { continue; }
                UIView viewCover = bar.ViewWithTag(2001);
                if (viewCover != null) { viewCover.Hidden = isTariffView; }
                UIView viewTariff = bar.ViewWithTag(2002);
                if (viewTariff != null) { viewTariff.Hidden = !isTariffView; }
            }
        }

        public override void ToggleRMKWHValues(RMkWhEnum state)
        {
            List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                /*UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                if (value == null) { continue; }
                value.Text = state == RMkWhEnum.RM ? usageData[i].AmountTotal.FormatAmountString(usageData[i].Currency)
                    : string.Format(Format_Value, usageData[i].UsageTotal, usageData[i].UsageUnit);
                nfloat lblAmountWidth = value.GetLabelWidth(GetWidthByScreenSize(200));
                value.Frame = new CGRect((GetWidthByScreenSize(30) - lblAmountWidth) / 2, value.Frame.Y, lblAmountWidth, value.Frame.Height);
                */
            }
        }
    }
}
