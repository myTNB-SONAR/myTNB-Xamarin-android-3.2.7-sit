using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class TitleBarComponent
    {
        UIView _parentView;
        UIView _viewTitleBar;
        UILabel _lblTitle;
        UIView _viewNotification;
        UIImageView _imgViewNotification;
        UIView _viewBack;
        UIImageView _imgViewBack;

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

            _viewBack = new UIView(new CGRect(18, 0, 24, HEIGHT));
            _viewBack.Hidden = true;
            _imgViewBack = new UIImageView(new CGRect(0, 0, 24, HEIGHT));
            _imgViewBack.Image = UIImage.FromBundle("Back-White");
            _viewBack.AddSubview(_imgViewBack);
            _viewTitleBar.AddSubview(_viewBack);

            _lblTitle = new UILabel(new CGRect(58, 0, _parentView.Frame.Width - 116, HEIGHT));
            _lblTitle.Font = MyTNBFont.MuseoSans16_500; ;
            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.TextColor = UIColor.White;
            _viewTitleBar.AddSubview(_lblTitle);

            _viewNotification = new UIView(new CGRect(_parentView.Frame.Width - 48, 0, 24, HEIGHT));
            _imgViewNotification = new UIImageView(new CGRect(0, 0, 24, HEIGHT));
            _imgViewNotification.Image = UIImage.FromBundle("Notification");
            _viewNotification.AddSubview(_imgViewNotification);
            _viewTitleBar.AddSubview(_viewNotification);

            _parentView.AddSubview(_viewTitleBar);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewTitleBar;
        }

        public void SetTitle(string title)
        {
            _lblTitle.Text = title;
        }

        public void SetNotificationImage(string img)
        {
            _imgViewNotification.Image = UIImage.FromBundle(img);
        }

        public void SetNotificationAction(UITapGestureRecognizer tapGesture)
        {
            _viewNotification.AddGestureRecognizer(tapGesture);
        }

        public void SetNotificationVisibility(bool isHidden)
        {
            _viewNotification.Hidden = isHidden;
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