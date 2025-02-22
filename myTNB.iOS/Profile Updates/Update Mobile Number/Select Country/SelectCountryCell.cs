﻿using System;
using CoreGraphics;
using UIKit;

namespace myTNB.ProfileUpdates.UpdateMobileNumber.SelectCountry
{
    public class SelectCountryCell : CustomUITableViewCell
    {
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private readonly UIView _view;
        private UIImageView _imgFlag;
        private UILabel _lblCountryCode, _lblCountryName;

        public SelectCountryCell(IntPtr handle) : base(handle)
        {
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
            _imgFlag = new UIImageView(new CGRect(GetScaledWidth(12), GetScaledHeight(20), GetScaledWidth(28), GetScaledHeight(20)))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            _lblCountryCode = new UILabel(new CGRect(_imgFlag.Frame.GetMaxX() + GetScaledWidth(8)
                , GetScaledHeight(18), GetScaledWidth(55), GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey
            };
            _lblCountryName = new UILabel(new CGRect(_lblCountryCode.Frame.GetMaxX() + GetScaledWidth(12)
                , 0, cellWidth - (_lblCountryCode.Frame.GetMaxX() + GetScaledWidth(30)), GetScaledHeight(60)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            UIView viewLine = GenericLine.GetLine(new CGRect(0, _view.Frame.Height - GetScaledHeight(1), cellWidth, GetScaledHeight(1)));
            _view.AddSubviews(new UIView[] { _imgFlag, _lblCountryCode, _lblCountryName, viewLine });
        }

        public string Flag
        {
            set
            {
                if (value.IsValid())
                {
                    UIImage img = UIImage.FromBundle(value.ToUpper());
                    _imgFlag.Image = img ?? UIImage.FromBundle(Constants.IMG_NoFlag);
                }
            }
        }

        public string Code
        {
            set
            {
                if (value.IsValid())
                {
                    _lblCountryCode.Text = value;
                }
            }
        }

        public string Country
        {
            set
            {
                if (value.IsValid())
                {
                    _lblCountryName.Text = value;
                }
            }
        }
    }
}