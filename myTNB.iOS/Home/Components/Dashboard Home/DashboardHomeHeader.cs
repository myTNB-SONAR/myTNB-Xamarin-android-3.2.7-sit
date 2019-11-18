﻿using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class DashboardHomeHeader
    {
        private readonly UIView _parentView;
        public UIView _accountHeaderView, _greetingView, _notificationView;
        UILabel _greetingLabel, _accountName;
        UIImageView _notificationIcon;
        DashboardHomeViewController _controller;
        string _strGreeting, _strName;

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
            nfloat imageWidth = ScaleUtility.GetScaledWidth(24f);

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
                UserInteractionEnabled = true
            };
            _notificationIcon = new UIImageView(new CGRect(0, 0, imageWidth, imageWidth))
            {
                Image = UIImage.FromBundle("Notification")
            };
            _notificationView.AddSubview(_notificationIcon);

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
    }
}