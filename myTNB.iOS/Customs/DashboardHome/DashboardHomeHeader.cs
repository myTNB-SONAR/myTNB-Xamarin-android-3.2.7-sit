using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class DashboardHomeHeader
    {
        private readonly UIView _parentView;
        UIView _accountHeaderView, _greetingView, _searchView, _notificationView;
        UILabel _greetingLabel, _accountName, _headerTitle;
        UIImageView _notificationIcon, _addAccountIcon, _searchIcon;
        string _strGreeting, _strName;

        public DashboardHomeHeader(UIView view)
        {
            _parentView = view;
        }

        private void CreateComponent()
        {
            nfloat parentHeight = _parentView.Frame.Height;
            nfloat parentWidth = _parentView.Frame.Width;
            nfloat padding = 16f;
            nfloat headerHeight = 110f;
            nfloat labelHeight = 24f;
            nfloat imageWidth = 24f;
            nfloat imageHeight = 24f;

            _accountHeaderView = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight(), parentWidth, headerHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _greetingView = new UIView(new CGRect(0, 0, _accountHeaderView.Frame.Width, labelHeight * 2 + padding));
            _greetingView.BackgroundColor = UIColor.Clear;

            _greetingLabel = new UILabel(new CGRect(padding, padding, _greetingView.Frame.Width * 0.60F, labelHeight))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.SunGlow,
                Text = _strGreeting
            };

            _accountName = new UILabel(new CGRect(padding, _greetingLabel.Frame.GetMaxY(), _greetingView.Frame.Width - (imageWidth + (padding * 3))
                , labelHeight))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.SunGlow,
                LineBreakMode = UILineBreakMode.MiddleTruncation,
                Text = _strName
            };
            _notificationView = new UIView(new CGRect(parentWidth - (imageWidth + padding), padding + labelHeight / 2, imageWidth, imageHeight));
            _notificationIcon = new UIImageView(new CGRect(0, 0, imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle("Notification")
            };
            _notificationView.AddSubview(_notificationIcon);
            _greetingView.AddSubviews(new UIView { _greetingLabel, _accountName, _notificationView });
            _searchView = new UIView(new CGRect(0, _greetingView.Frame.GetMaxY() + padding, _accountHeaderView.Frame.Width, 24f));
            _searchView.BackgroundColor = UIColor.Clear;

            _headerTitle = new UILabel(new CGRect(padding, 0, 186f, labelHeight))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = UIColor.White,
                Text = @"My Accounts"
            };

            _searchView.AddSubviews(new UIView { _headerTitle });

            _accountHeaderView.AddSubviews(new UIView { _greetingView, _searchView });
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _accountHeaderView;
        }

        public UIView GetView()
        {
            return _accountHeaderView;
        }

        private nfloat GetCenterYForIcon(nfloat parentHeight, nfloat iconHeight)
        {
            return parentHeight / 2 - iconHeight / 2;
        }

        public void SetGreetingText(string text)
        {
            _strGreeting = text ?? string.Empty;
        }

        public void SetNameText(string text)
        {
            _strName = text ?? string.Empty;
        }

        public void SetNotificationImage(string image)
        {
            _notificationIcon.Image = UIImage.FromBundle(image);
        }

        public void AddNotificationAction(Action action)
        {
            _notificationView.AddGestureRecognizer(new UITapGestureRecognizer(action));
        }
    }
}