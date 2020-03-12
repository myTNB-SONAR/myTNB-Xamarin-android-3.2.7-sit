using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Profile
{
    public class ProfileCell : UITableViewCell
    {
        private UILabel _lblTitle, _lblValue, _lblAction;
        private UIView _viewLine;
        private bool _isActionEnabled;

        public ProfileCell(IntPtr handle) : base(handle)
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;

            UIView view = new UIView(new CGRect(0, 0, cellWidth, GetScaledHeight(65))) { BackgroundColor = UIColor.White };

            _lblTitle = new UILabel(new CGRect(GetScaledWidth(16), GetScaledHeight(16), cellWidth - GetScaledWidth(32), GetScaledHeight(12)))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = TNBFont.MuseoSans_10_300,
                TextAlignment = UITextAlignment.Left
            };
            _lblValue = new UILabel(new CGRect(GetScaledWidth(16), _lblTitle.Frame.GetMaxY(), cellWidth - GetScaledWidth(32), GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_300,
                TextAlignment = UITextAlignment.Left
            };
            _lblAction = new UILabel(new CGRect(cellWidth - GetScaledWidth(116), GetScaledHeight(24), GetScaledWidth(100), GetScaledHeight(16)))
            {
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Right,
                Hidden = true
            };
            _viewLine = new UIView(new CGRect(0, GetScaledHeight(64), cellWidth, GetScaledHeight(1))) { BackgroundColor = MyTNBColor.VeryLightPinkThree };

            view.AddSubviews(new UIView[] { _lblTitle, _lblValue, _lblAction, _viewLine });
            AddSubview(view);
        }

        private nfloat GetScaledHeight(nfloat val)
        {
            return ScaleUtility.GetScaledHeight(val);
        }

        private nfloat GetScaledWidth(nfloat val)
        {
            return ScaleUtility.GetScaledWidth(val);
        }

        public string Title
        {
            set
            {
                _lblTitle.Text = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? string.Empty : value.ToUpper();
            }
        }

        public string Value
        {
            set
            {
                _lblValue.Text = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? string.Empty : value;
            }
        }

        public string Action
        {
            set
            {
                _lblAction.Text = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? string.Empty : value;
                _lblAction.Hidden = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
            }
        }

        public bool IsActionEnabled
        {
            set
            {
                _isActionEnabled = value;
                _lblAction.TextColor = value ? MyTNBColor.WaterBlue : MyTNBColor.SilverChalice;
            }
            get
            {
                return _isActionEnabled;
            }
        }

        public bool IsLineHidden
        {
            set
            {
                _viewLine.Hidden = value;
            }
        }
    }
}