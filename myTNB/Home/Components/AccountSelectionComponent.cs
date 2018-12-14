using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class AccountSelectionComponent
    {
        UIView _parentView;
        UIView _viewAccountSelection;
        UILabel _lblAccountName;
        UIView _viewDropDown;
        UIImageView _imgDropDown;
        UIImageView _imgLeaf;
        int yLocation = 56;
        public AccountSelectionComponent(UIView view)
        {
            _parentView = view;
            if (DeviceHelper.IsIphoneX())
            {
                yLocation = 80;
            }
        }

        internal void CreateComponent()
        {
            _viewAccountSelection = new UIView(new CGRect(0, yLocation, _parentView.Frame.Width, 16));

            _imgLeaf = new UIImageView(new CGRect(0, 0, 16, 16));
            _imgLeaf.Image = UIImage.FromBundle("IC-RE-Leaf-White");
            _imgLeaf.Hidden = true;

            _lblAccountName = new UILabel(new CGRect(58, 0, _parentView.Frame.Width - 116, 16));
            _lblAccountName.Font = myTNBFont.MuseoSans12();
            _lblAccountName.TextAlignment = UITextAlignment.Center;
            _lblAccountName.TextColor = UIColor.White;
            _lblAccountName.Text = "- - -";

            _viewDropDown = new UIView(new CGRect(0, 0, 16, 16));
            _imgDropDown = new UIImageView(new CGRect(0, -1, 16, 16));
            _imgDropDown.Image = UIImage.FromBundle("Dropdown");
            _viewDropDown.AddSubview(_imgDropDown);

            _viewAccountSelection.AddSubviews(new UIView[] { _imgLeaf, _lblAccountName, _viewDropDown });
            AdjustFrames();
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewAccountSelection;
        }

        public void SetAccountName(string accountName)
        {
            _lblAccountName.Text = accountName;
            AdjustFrames();
        }

        public void SetDropdownVisibility(bool isHidden)
        {
            _viewDropDown.Hidden = isHidden;
            if (isHidden)
            {
                _viewAccountSelection.Frame = new CGRect(_parentView.Center.X - (_lblAccountName.Frame.Width / 2)
                                                         , yLocation
                                                         , _lblAccountName.Frame.Width
                                                         , 16);
            }
        }

        public void SetLeafVisibility(bool isHidden)
        {
            _imgLeaf.Hidden = isHidden;
            if (isHidden)
            {
                _lblAccountName.Frame = new CGRect(0, 0, _lblAccountName.Frame.Width, _lblAccountName.Frame.Height);
                _viewDropDown.Frame = new CGRect(_lblAccountName.Frame.Width + 5, 0, 16, 16);
                _viewAccountSelection.Frame = new CGRect(_parentView.Center.X - (_lblAccountName.Frame.Width + 21) / 2
                                                         , yLocation, _lblAccountName.Frame.Width + 21
                                                         , 16);
            }
        }

        public void SetSelectAccountEvent(UITapGestureRecognizer tapGesture)
        {
            _viewAccountSelection.AddGestureRecognizer(tapGesture);
        }

        internal CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        internal void AdjustFrames()
        {
            CGSize newSize = GetLabelSize(_lblAccountName
                                          , _parentView.Frame.Width - 116
                                          , _lblAccountName.Frame.Height);
            double newWidth = Math.Ceiling(newSize.Width);
            _lblAccountName.Frame = new CGRect(21, 0, newWidth, _lblAccountName.Frame.Height);
            _viewDropDown.Frame = new CGRect(newWidth + 5 + 21, 0, 16, 16);
            _viewAccountSelection.Frame = new CGRect(_parentView.Center.X - (newWidth + 21 + 21) / 2
                                                     , yLocation, newWidth + 21 + 21
                                                     , 16);
        }
    }
}