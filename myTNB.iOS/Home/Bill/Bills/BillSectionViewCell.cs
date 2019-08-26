using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillSectionViewCell : UITableViewCell
    {
        private UIView _view;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat _baseHMargin = ScaleUtility.GetScaledWidth(16);
        private nfloat _baseVMargin = ScaleUtility.GetScaledHeight(16);

        public BillSectionViewCell(IntPtr handle) : base(handle)
        {
            nfloat scaled16 = ScaleUtility.GetScaledWidth(16);
            _view = new UIView(new CGRect(0, 0, _cellWidth, ScaleUtility.GetScaledHeight(60)));
            _view.BackgroundColor = MyTNBColor.LightGrayBG;
            UILabel lblTitle = new UILabel(new CGRect(scaled16, ScaleUtility.GetScaledHeight(16), _view.Frame.Width, ScaleUtility.GetScaledHeight(24)))
            {
                Text = "My History",
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue
            };

            UIView viewFilter = new UIView(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(20)
                , scaled16, scaled16));
            UIImageView imgFilter = new UIImageView(new CGRect(0, 0, scaled16, scaled16))
            {
                Image = UIImage.FromBundle("IC-Action-Filter")
            };
            viewFilter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("Filter");
            }));
            viewFilter.AddSubview(imgFilter);

            _view.AddSubview(lblTitle);
            _view.AddSubview(viewFilter);

            AddSubview(_view);
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
        }
    }
}