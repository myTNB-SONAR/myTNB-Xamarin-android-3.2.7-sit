using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class TitleBarComponent
    {
        UIView _parentView, _viewTitleBar, _viewPrimaryAction, _viewSecondaryAction, _viewBack;
        UILabel _lblTitle;
        UIImageView _imgViewPrimaryAction, _imgViewSecondaryAction, _imgViewBack;

        const int HEIGHT = 24;

        public TitleBarComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            int yLocation = 26;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = 50;
            }

            _viewTitleBar = new UIView(new CGRect(0, yLocation, _parentView.Frame.Width, HEIGHT));

            _viewBack = new UIView(new CGRect(18, 0, 24, HEIGHT))
            {
                Hidden = true
            };
            _imgViewBack = new UIImageView(new CGRect(0, 0, 24, HEIGHT))
            {
                Image = UIImage.FromBundle("Back-White")
            };
            _viewBack.AddSubview(_imgViewBack);
            _viewTitleBar.AddSubview(_viewBack);

            _lblTitle = new UILabel(new CGRect(58, 0, _parentView.Frame.Width - 116, HEIGHT))
            {
                Font = MyTNBFont.MuseoSans16_500
            };
            ;
            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.TextColor = UIColor.White;
            _viewTitleBar.AddSubview(_lblTitle);

            _viewSecondaryAction = new UIView(new CGRect(_parentView.Frame.Width - 48, 0, 24, HEIGHT));
            _imgViewSecondaryAction = new UIImageView(new CGRect(0, 0, 24, HEIGHT))
            {
                Image = UIImage.FromBundle("Notification")
            };
            _viewSecondaryAction.AddSubview(_imgViewSecondaryAction);
            _viewTitleBar.AddSubview(_viewSecondaryAction);

            _viewPrimaryAction = new UIView(new CGRect(_parentView.Frame.Width - 80, 0, 24, HEIGHT))
            {
                Hidden = true
            };
            _imgViewPrimaryAction = new UIImageView(new CGRect(0, 0, 24, HEIGHT))
            {
                Image = UIImage.FromBundle("Notification")
            };
            _viewPrimaryAction.AddSubview(_imgViewPrimaryAction);
            _viewTitleBar.AddSubview(_viewPrimaryAction);

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

        public void SetNotificationAction(UITapGestureRecognizer tapGesture)
        {
            _viewSecondaryAction.AddGestureRecognizer(tapGesture);
        }

        public void SetNotificationVisibility(bool isHidden)
        {
            _viewSecondaryAction.Hidden = isHidden;
        }

        public void SetNotificationImage(string img)
        {
            _imgViewSecondaryAction.Image = UIImage.FromBundle(img);
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