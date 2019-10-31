using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class TitleBarComponentV2 : BaseComponent
    {
        private UIView _parentView, _viewTitleBar, _viewSecondaryAction, _viewPrimaryAction, _viewBack;
        private UILabel _lblTitle;
        private UIImageView _imgViewSecondaryAction, _imgViewPrimaryAction, _imgViewBack;
        private nfloat _height;

        public TitleBarComponentV2(UIView view)
        {
            _parentView = view;
            _height = GetScaledHeight(24);
        }

        internal void CreateComponent()
        {
            nfloat iconWidth = GetWidthByScreenSize(24);

            int yLocation = 26;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = 50;
            }

            _viewTitleBar = new UIView(new CGRect(0, yLocation, _parentView.Frame.Width, _height));

            _viewBack = new UIView(new CGRect(GetWidthByScreenSize(16), 0, iconWidth, iconWidth))
            {
                Hidden = true
            };
            _imgViewBack = new UIImageView(new CGRect(0, 0, iconWidth, iconWidth))
            {
                Image = UIImage.FromBundle(Constants.IMG_Back)
            };
            _viewBack.AddSubview(_imgViewBack);
            _viewTitleBar.AddSubview(_viewBack);

            _lblTitle = new UILabel(new CGRect(58, 0, _parentView.Frame.Width - 116, _height))
            {
                Font = TNBFont.MuseoSans_16_500
            };

            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.TextColor = UIColor.White;
            _viewTitleBar.AddSubview(_lblTitle);

            _viewPrimaryAction = new UIView(new CGRect(_parentView.Frame.Width - GetWidthByScreenSize(40), 0, iconWidth, iconWidth));
            _imgViewPrimaryAction = new UIImageView(new CGRect(0, 0, iconWidth, iconWidth))
            {
                Image = UIImage.FromBundle("Notification")
            };
            _viewPrimaryAction.AddSubview(_imgViewPrimaryAction);
            _viewTitleBar.AddSubview(_viewPrimaryAction);

            _viewSecondaryAction = new UIView(new CGRect(_parentView.Frame.Width - GetWidthByScreenSize(72), 0, iconWidth, iconWidth))
            {
                Hidden = true
            };
            _imgViewSecondaryAction = new UIImageView(new CGRect(0, 0, iconWidth, iconWidth))
            {
                Image = UIImage.FromBundle("Notification")
            };
            _viewSecondaryAction.AddSubview(_imgViewSecondaryAction);
            _viewTitleBar.AddSubview(_viewSecondaryAction);

            _parentView.AddSubview(_viewTitleBar);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewTitleBar;
        }

        public UIView GetView()
        {
            return _viewTitleBar;
        }

        public void SetTitle(string title)
        {
            _lblTitle.Text = title;
        }

        public void SetSecondaryAction(UITapGestureRecognizer tapGesture)
        {
            _viewSecondaryAction.AddGestureRecognizer(tapGesture);
        }

        public void SetSecondaryVisibility(bool isHidden)
        {
            _viewSecondaryAction.Hidden = isHidden;
        }

        public void SetSecondaryImage(string img)
        {
            _imgViewSecondaryAction.Image = UIImage.FromBundle(img);
        }

        public void SetPrimaryAction(UITapGestureRecognizer tapGesture)
        {
            _viewPrimaryAction.AddGestureRecognizer(tapGesture);
        }

        public void SetPrimaryVisibility(bool isHidden)
        {
            _viewPrimaryAction.Hidden = isHidden;
        }

        public void SetPrimaryImage(string img)
        {
            _imgViewPrimaryAction.Image = UIImage.FromBundle(img);
        }

        public void SetBackImage(string img)
        {
            _imgViewBack.Image = UIImage.FromBundle(img);
        }

        public void SetBackAction(UITapGestureRecognizer tapGesture)
        {
            _viewBack.AddGestureRecognizer(tapGesture);
        }

        public void SetBackVisibility(bool isHidden)
        {
            _viewBack.Hidden = isHidden;
        }
    }
}