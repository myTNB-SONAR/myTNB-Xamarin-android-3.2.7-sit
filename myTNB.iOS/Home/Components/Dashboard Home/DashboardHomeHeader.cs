using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class DashboardHomeHeader
    {
        private readonly UIView _parentView;
        public UIView _accountHeaderView, _greetingView, _notificationView, _badgeView;
        private UILabel _greetingLabel, _accountName, _lblBadge;
        private UIImageView _notificationIcon;
        private DashboardHomeViewController _controller;
        private string _strGreeting, _strName;

        public DashboardHomeHeader(UIView view, DashboardHomeViewController controller)
        {
            _parentView = view;
            _controller = controller;
        }

        private void CreateComponent()
        {
            nfloat parentHeight = _parentView.Frame.Height;
            nfloat parentWidth = _parentView.Frame.Width;
            nfloat padding = ScaleUtility.BaseMarginWidth16;
            nfloat headerHeight = ScaleUtility.GetScaledHeight(68f);
            nfloat labelHeight = ScaleUtility.GetScaledHeight(24f);
            nfloat imageWidth = ScaleUtility.GetScaledWidth(28f);

            _accountHeaderView = new UIView(new CGRect(0, 0, parentWidth, headerHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            _accountHeaderView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (_controller != null)
                {
                    _controller.DismissActiveKeyboard();
                }
            }));

            _greetingView = new UIView(new CGRect(0, 0, _accountHeaderView.Frame.Width, labelHeight * 2 + ScaleUtility.GetScaledHeight(20f)));
            _greetingView.BackgroundColor = UIColor.Clear;

            _greetingLabel = new UILabel(new CGRect(padding, ScaleUtility.GetScaledHeight(20f), _greetingView.Frame.Width * 0.60F, labelHeight))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.SunGlow,
                Text = _strGreeting
            };

            _accountName = new UILabel(new CGRect(padding, _greetingLabel.Frame.GetMaxY(), _greetingView.Frame.Width - (imageWidth + (padding * 3))
                , labelHeight))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.SunGlow,
                LineBreakMode = UILineBreakMode.MiddleTruncation,
                Text = _strName
            };
            _notificationView = new UIView(new CGRect(parentWidth - (imageWidth + padding), padding + labelHeight / 2, imageWidth, imageWidth))
            {
                UserInteractionEnabled = true,
                ClipsToBounds = false
            };
            _badgeView = new UIView(new CGRect(ScaleUtility.GetScaledWidth(12), 0, ScaleUtility.GetScaledWidth(16), ScaleUtility.GetScaledWidth(16)))
            {
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _badgeView.Layer.CornerRadius = ScaleUtility.GetScaledWidth(_badgeView.Frame.Height / 2);

            _lblBadge = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(3), 0, 0, _badgeView.Frame.Height))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = TNBFont.MuseoSans_10_500
            };
            _badgeView.AddSubview(_lblBadge);

            _notificationIcon = new UIImageView(new CGRect(0, 0, imageWidth, imageWidth))
            {
                Image = UIImage.FromBundle("Notification")
            };

            _notificationView.AddSubviews(new UIView[] { _notificationIcon, _badgeView });

            _greetingView.AddSubview(_greetingLabel);
            _greetingView.AddSubview(_accountName);
            _greetingView.AddSubview(_notificationView);

            _accountHeaderView.AddSubview(_greetingView);
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
            if (_notificationIcon != null)
            {
                _notificationIcon.Image = UIImage.FromBundle(image);
            }
        }

        public void SetNotificationActionRecognizer(UITapGestureRecognizer recognizer)
        {
            if (_notificationView != null)
            {
                _notificationView.AddGestureRecognizer(recognizer);
            }
        }

        public int BadgeValue
        {
            set
            {
                _badgeView.Hidden = value == 0;
                if (value > 0)
                {
                    string badgeContent = value > 99 ? Constants.Value_99 : value.ToString();
                    _lblBadge.Text = badgeContent;
                    nfloat width = _lblBadge.GetLabelWidth(ScaleUtility.GetScaledWidth(28f));
                    nfloat containerWidth = width + ScaleUtility.GetScaledWidth(6);
                    if (containerWidth < ScaleUtility.GetScaledWidth(16))
                    {
                        containerWidth = ScaleUtility.GetScaledWidth(16);
                    }
                    _badgeView.Frame = new CGRect(ScaleUtility.GetScaledWidth(14), _badgeView.Frame.Y
                        , containerWidth, _badgeView.Frame.Height);
                    _badgeView.Layer.CornerRadius = ScaleUtility.GetScaledWidth(_badgeView.Frame.Height / 2);

                    _lblBadge.Frame = new CGRect(ScaleUtility.GetXLocationToCenterObject(width, _badgeView)
                        , ScaleUtility.GetYLocationToCenterObject(_lblBadge.Frame.Height, _badgeView)
                        , width, _lblBadge.Frame.Height);
                }
            }
        }
    }
}