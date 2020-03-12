using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountSelectionComponentV2 : BaseComponent
    {
        private readonly UIView _parentView;
        private UIView _viewAccountSelection, _viewDropDown;
        private UILabel _lblAccountName;
        private UIImageView _imgDropDown;
        private nfloat _yLocation;
        private nfloat _iconWidth;
        private nfloat _height;
        public AccountSelectionComponentV2(UIView view, nfloat yLocation)
        {
            _parentView = view;
            _yLocation = yLocation;
            _iconWidth = GetWidthByScreenSize(16);
            _height = GetScaledHeight(16);
        }

        internal void CreateComponent()
        {
            _viewAccountSelection = new UIView(new CGRect(0, _yLocation, _parentView.Frame.Width, _height));
            _lblAccountName = new UILabel(new CGRect(GetScaledWidth(16), 0, _parentView.Frame.Width - GetScaledWidth(56), _height))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Text = "- - -"
            };

            _viewDropDown = new UIView(new CGRect(0, 0, _iconWidth, _iconWidth));
            _imgDropDown = new UIImageView(new CGRect(0, 0, _iconWidth, _iconWidth))
            {
                Image = UIImage.FromBundle("Dropdown")
            };
            _viewDropDown.AddSubview(_imgDropDown);

            _viewAccountSelection.AddSubviews(new UIView[] { _lblAccountName, _viewDropDown });
            AdjustFrames();
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewAccountSelection;
        }

        public UIView GetView()
        {
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
                    , _yLocation, _lblAccountName.Frame.Width, _height);
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
            CGSize newSize = GetLabelSize(_lblAccountName, _parentView.Frame.Width - GetScaledWidth(56), _lblAccountName.Frame.Height);
            double newWidth = newSize.Width;
            _lblAccountName.Frame = new CGRect(0, 0, newWidth, _lblAccountName.Frame.Height);
            _viewDropDown.Frame = new CGRect(_lblAccountName.Frame.GetMaxX() + GetScaledWidth(8), 0, _iconWidth, _iconWidth);

            nfloat totalWidth = _lblAccountName.Frame.Width + _viewDropDown.Frame.Width + GetScaledWidth(8);

            _viewAccountSelection.Frame = new CGRect((_parentView.Frame.Width - totalWidth) / 2
                , _viewAccountSelection.Frame.Y, totalWidth, _height);
        }
    }
}