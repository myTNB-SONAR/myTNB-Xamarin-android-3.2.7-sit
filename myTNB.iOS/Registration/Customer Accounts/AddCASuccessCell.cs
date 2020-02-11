using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AddCASuccessCell : UITableViewCell
    {
        private nfloat cellWidth = 0;
        private readonly UIView _view;
        private UILabel _lblName, _lblCANumber, _lblAddress;

        public AddCASuccessCell(IntPtr handle) : base(handle)
        {
            cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width - GetScaledWidth(32);
            _view = new UIView(new CGRect(0, 0, cellWidth, GetScaledHeight(61)))
            {
                ClipsToBounds = true,
                BackgroundColor = UIColor.White
            };

            AddCellSubviews();
            AddSubview(_view);
            BackgroundColor = UIColor.Clear;

            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        private void AddCellSubviews()
        {
            UIView line = GenericLine.GetLine(new CGRect(GetScaledWidth(14), 0, cellWidth - GetScaledWidth(28), GetScaledHeight(1)));
            _lblName = new UILabel(new CGRect(GetScaledWidth(24), line.Frame.GetMaxY() + GetScaledHeight(16)
                , cellWidth - GetScaledWidth(48), GetScaledHeight(18)))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = TNBFont.MuseoSans_14_500
            };
            _lblCANumber = new UILabel(new CGRect(GetScaledWidth(24), _lblName.Frame.GetMaxY() + GetScaledHeight(1)
                , cellWidth - GetScaledWidth(48), GetScaledHeight(14)))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = TNBFont.MuseoSans_12_300
            };
            _lblAddress = new UILabel(new CGRect(GetScaledWidth(24), _lblCANumber.Frame.GetMaxY() + GetScaledHeight(16)
                , cellWidth - GetScaledWidth(48), GetScaledHeight(14)))
            {
                TextColor = MyTNBColor.TunaGrey(),
                Font = TNBFont.MuseoSans_12_300,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            _view.AddSubviews(new UIView[] { line, _lblName, _lblCANumber, _lblAddress });
        }

        private nfloat GetScaledHeight(nfloat val)
        {
            return ScaleUtility.GetScaledHeight(val);
        }

        private nfloat GetScaledWidth(nfloat val)
        {
            return ScaleUtility.GetScaledWidth(val);
        }

        public string Name
        {
            set
            {
                if (value.IsValid())
                {
                    _lblName.Text = value;
                }
                else
                {
                    _lblName.Text = string.Empty;
                }
            }
        }

        public string CANumber
        {
            set
            {
                if (value.IsValid())
                {
                    _lblCANumber.Text = value;
                }
                else
                {
                    _lblCANumber.Text = string.Empty;
                }
            }
        }

        public string Address
        {
            set
            {
                if (value.IsValid())
                {
                    _lblAddress.Text = value;
                }
                else
                {
                    _lblAddress.Text = string.Empty;
                }
                nfloat newHeight = _lblAddress.GetLabelHeight(1000);
                _lblAddress.Frame = new CGRect(_lblAddress.Frame.Location, new CGSize(_lblAddress.Frame.Width, newHeight));
                _view.Frame = new CGRect(_view.Frame.Location, new CGSize(_view.Frame.Width, _lblAddress.Frame.GetMaxY() + GetScaledHeight(16)));
            }
        }
    }
}