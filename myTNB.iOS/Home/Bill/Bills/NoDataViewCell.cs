using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Bill
{
    public class NoDataViewCell : UITableViewCell
    {
        private UIView _view;
        private UIImageView _imgIcon;
        private UILabel _lblDescription;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;

        public NoDataViewCell(IntPtr handle) : base(handle)
        {
            _view = new UIView(new CGRect(0, 0, _cellWidth, ScaleUtility.GetScaledHeight(170)));
            nfloat imgWidth = ScaleUtility.GetWidthByScreenSize(71);
            nfloat heightFactor = 0.887F;
            _imgIcon = new UIImageView(new CGRect(ScaleUtility.GetWidthByScreenSize(131), ScaleUtility.GetScaledHeight(24)
                , imgWidth, imgWidth * heightFactor));
            _lblDescription = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(32), ScaleUtility.GetYLocationFromFrame(_imgIcon.Frame, 7)
                , _cellWidth - ScaleUtility.GetScaledWidth(64), 60))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.Grey,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            _view.AddSubviews(new UIView[] { _imgIcon, _lblDescription });
            AddSubview(_view);
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public string Image
        {
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }

                _imgIcon.Image = UIImage.FromBundle(value);
            }
        }

        public string Message
        {
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
                _lblDescription.Text = value;
                nfloat height = _lblDescription.GetLabelHeight(1000);
                _lblDescription.Frame = new CGRect(_lblDescription.Frame.Location, new CGSize(_lblDescription.Frame.Width, height));
                nfloat viewHeight = ScaleUtility.GetScaledHeight(170);
                if (_lblDescription.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16) > viewHeight)
                {
                    viewHeight = _lblDescription.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16);
                }
                _view.Frame = new CGRect(_view.Frame.Location, new CGSize(_view.Frame.Width, viewHeight));
            }
        }
    }
}