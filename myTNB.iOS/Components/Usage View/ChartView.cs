using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class ChartView : BaseComponent
    {
        public ChartView()
        {
        }

        private CustomUIView _mainView;
        private nfloat _width, _baseMargin, _baseMarginedWidth;
        private UILabel _lblDateRange;

        private void CreatUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            _baseMargin = GetScaledWidth(16);
            _baseMarginedWidth = _width - (_baseMargin * 2);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, GetScaledHeight(206)));

            _lblDateRange = new UILabel(new CGRect(_baseMargin, 0, _baseMarginedWidth, GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.ButterScotch,
                Font = TNBFont.MuseoSans_12_500,
                Text = "Mar 2019 - Aug 2019"
            };

            CustomUIView viewLine = new CustomUIView(new CGRect(_baseMargin, GetScaledHeight(182)
                , _baseMarginedWidth, GetScaledHeight(1)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F)
            };

            _mainView.AddSubviews(new UIView[] { _lblDateRange, viewLine });
            CreateSegment();
        }

        private void CreateSegment()
        {
            CustomUIView segmentContainer = new CustomUIView(new CGRect(0, GetPercentHeightValue(_mainView.Frame, 23)
                , _width, GetPercentHeightValue(_mainView.Frame, 77)));
            //{ BackgroundColor = UIColor.Brown };

            nfloat height = segmentContainer.Frame.Height;
            nfloat width = GetScaledWidth(12);
            nfloat barMargin = GetScaledWidth(36);
            nfloat baseMargin = GetScaledWidth(34);
            nfloat xLoc = baseMargin;
            nfloat lblHeight = GetScaledHeight(14);
            nfloat maxBarHeight = height * 0.688F;
            nfloat amountBarMargin = GetScaledHeight(4);

            for (int i = 0; i < 6; i++)
            {
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, width, height));
                // { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.5F) };
                segmentContainer.AddSubview(segment);
                xLoc += width + barMargin;

                UILabel lblAmount = new UILabel(new CGRect(0, 0, GetScaledWidth(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = UIColor.FromWhiteAlpha(1, 0.50F),
                    Text = "RM 220.20"
                };
                nfloat lblAmountWidth = lblAmount.GetLabelWidth(GetScaledWidth(100));
                lblAmount.Frame = new CGRect((width - lblAmountWidth) / 2, lblAmount.Frame.Y, lblAmountWidth, lblAmount.Frame.Height);

                CustomUIView viewBar = new CustomUIView(new CGRect(0, lblAmount.Frame.GetMaxY() + amountBarMargin
                    , width, maxBarHeight))
                { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.50F) };
                viewBar.Layer.CornerRadius = width / 2;

                UILabel lblDate = new UILabel(new CGRect(0, segment.Frame.Height - lblHeight
                    , GetScaledWidth(30), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = UIColor.FromWhiteAlpha(1, 0.50F),
                    Text = "Mar"
                };
                nfloat lblDateWidth = lblDate.GetLabelWidth(GetScaledWidth(30));
                lblDate.Frame = new CGRect((width - lblDateWidth) / 2, lblDate.Frame.Y, lblDateWidth, lblDate.Frame.Height);

                segment.AddSubviews(new UIView[] { lblAmount, viewBar, lblDate });
            }

            _mainView.AddSubview(segmentContainer);
            _mainView.SendSubviewToBack(segmentContainer);
        }

        public CustomUIView GetUI()
        {
            CreatUI();
            return _mainView;
        }
    }
}