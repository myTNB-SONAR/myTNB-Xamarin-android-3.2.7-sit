﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB.SmartMeterView
{
    public class SmartMeterDayView : BaseSmartMeterView
    {
        private nfloat _width = UIScreen.MainScreen.Bounds.Width;

        public override void CreateSegment(ref CustomUIView view)
        {
            view = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(ReferenceWidget, 24)
               , _width, GetHeightByScreenSize(149)));

            nfloat height = view.Frame.Height;
            nfloat width = GetWidthByScreenSize(12);
            nfloat segmentMargin = GetWidthByScreenSize(4);
            nfloat baseMargin = GetWidthByScreenSize(25);
            nfloat xLoc = baseMargin;
            nfloat maxBarHeight = GetHeightByScreenSize(117);
            nfloat segmentWidth = GetWidthByScreenSize(5);
            nfloat barMargin = GetWidthByScreenSize(5);
            nfloat missingReadingBarMargin = GetHeightByScreenSize(10);

            List<DayItemModel> usageData = AccountUsageSmartCache.FlatDays;
            List<string> valueList = usageData.Select(x => x.Amount).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;
            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                DayItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapNormalBar"
                };
                view.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                double.TryParse(item.Amount, out double value);
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = maxBarHeight - barHeight;

                CustomUIView viewBar = new CustomUIView(new CGRect(0, maxBarHeight, segmentWidth, 0))
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = 1001,
                    ClipsToBounds = true
                };
                viewBar.Layer.CornerRadius = segmentWidth / 2;

                UIImageView imgMissingReading = null;
                if (item.IsMissingReading || index == 20)//For Testing
                {
                    imgMissingReading = new UIImageView(new CGRect(0, maxBarHeight - missingReadingBarMargin, segmentWidth, segmentWidth))
                    {
                        Image = UIImage.FromBundle(SmartMeterConstants.IMG_MissingReading),
                        Tag = 3001
                    };
                    segment.AddSubview(imgMissingReading);
                }

                UIView viewCover = new UIView(new CGRect(new CGPoint(0, 0), new CGSize(viewBar.Frame.Width, barHeight)))
                {
                    BackgroundColor = UIColor.White,
                    Tag = 2001,
                    Hidden = IsTariffView
                };
                viewBar.AddSubview(viewCover);
                if (AddTariffBlocks != null)
                {
                    AddTariffBlocks.Invoke(viewBar, item.tariffBlocks, value, true, viewCover.Frame.Size, false);
                }
                segment.AddSubview(viewBar);

                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                    , () =>
                    {
                        viewBar.Frame = new CGRect(viewBar.Frame.X, yLoc, viewBar.Frame.Width, barHeight);
                        if (imgMissingReading != null)
                        {
                            imgMissingReading.Frame = new CGRect(new CGPoint(imgMissingReading.Frame.X, yLoc - missingReadingBarMargin)
                                , imgMissingReading.Frame.Size);
                        }
                    }
                    , () => { }
                );
            }

            AddDateGuide(ref view);
        }

        private void AddDateGuide(ref CustomUIView view)
        {
            nfloat height = view.Frame.Height;
            nfloat lblHeight = GetHeightByScreenSize(14);

            UILabel lblStartDate = new UILabel(new CGRect(GetWidthByScreenSize(16), height - lblHeight, 0, lblHeight))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White,
                Text = "22 Jul"
            };
            nfloat lblStartDateWidth = lblStartDate.GetLabelWidth(GetWidthByScreenSize(100));
            lblStartDate.Frame = new CGRect(lblStartDate.Frame.Location, new CGSize(lblStartDateWidth, lblHeight));

            UILabel lblMidDate = new UILabel(new CGRect(GetWidthByScreenSize(16), height - lblHeight, 0, lblHeight))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White,
                Text = "10 Aug"
            };
            nfloat lblMidDateWidth = lblMidDate.GetLabelWidth(GetWidthByScreenSize(100));
            lblMidDate.Frame = new CGRect(new CGPoint((_width - lblMidDateWidth) / 2, lblMidDate.Frame.Y), new CGSize(lblMidDateWidth, lblHeight));

            UILabel lblEndDate = new UILabel(new CGRect(GetWidthByScreenSize(16), height - lblHeight, 0, lblHeight))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White,
                Text = "21 Aug"
            };
            nfloat lblEndDateWidth = lblEndDate.GetLabelWidth(GetWidthByScreenSize(100));
            lblEndDate.Frame = new CGRect(new CGPoint(_width - GetWidthByScreenSize(16) - lblEndDateWidth
                , lblEndDate.Frame.Y), new CGSize(lblEndDateWidth, lblHeight));

            view.AddSubviews(new UIView[] { lblStartDate, lblMidDate, lblEndDate });
        }
    }
}