﻿using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class SystemDownComponent
    {
        UIView _parentView;
        UIView _baseView;
        UITextView _txtView;
        bool _isHeaderMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:myTNB.Dashboard.DashboardComponents.SystemDownComponent"/> class.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="isHeaderMode">If set to <c>true</c> is header mode.</param>
        public SystemDownComponent(UIView view, bool isHeaderMode)
        {
            _parentView = view;
            _isHeaderMode = isHeaderMode;
        }

        internal void CreateComponent()
        {
            int mainYLocation = 102;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                mainYLocation = 136;
            }

            var baseWidth = _parentView.Frame.Width;
            _baseView = new UIView();

            int yLocation = _isHeaderMode ? 33 : 36;

            // ratio = height / width
            var origImgHeightRatio = _isHeaderMode ? 92.0f / 114.0f : 132.0f / 164.0f;
            var origImgWidthRatio = _isHeaderMode ? (114.0f / 320.0f) : (164.0f / 320.0f);
            var imgWidth = baseWidth * origImgWidthRatio;
            var imgHeight = imgWidth * origImgHeightRatio;

            var xLocation = baseWidth / 2.0f - imgWidth / 2.0f;
            UIImageView imgGraphic = new UIImageView(new CGRect(xLocation, yLocation, imgWidth, imgHeight));
            imgGraphic.Image = UIImage.FromBundle("Down-BCRM");
            imgGraphic.ContentMode = UIViewContentMode.ScaleAspectFit;
            _baseView.AddSubview(imgGraphic);

            var horizontalMargin = 34.0f;
            var msgWidth = baseWidth - horizontalMargin * 2.0f;
            var msgXLoc = baseWidth / 2.0f - msgWidth / 2.0f;

            var bcrm = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == Enums.SystemEnum.BCRM);
            var bcrmMsg = bcrm?.DowntimeMessage ?? "BRCM will be down for maintenance purpose";

            NSError htmlError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(bcrmMsg, ref htmlError, myTNBFont.FONTNAME_300, 12f);
            NSMutableAttributedString mutableDowntime = new NSMutableAttributedString(htmlBody);

            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle();
            msgParagraphStyle.Alignment = UITextAlignment.Center;

            UIStringAttributes msgAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                ParagraphStyle = msgParagraphStyle
            };

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                Font = myTNBFont.MuseoSans12_300(),
                ForegroundColor = UIColor.White,
                UnderlineStyle = NSUnderlineStyle.Single,
                BackgroundColor = UIColor.Clear
            };

            mutableDowntime.AddAttributes(msgAttributes, new NSRange(0, htmlBody.Length));

            _txtView = new UITextView(new CGRect(msgXLoc, imgGraphic.Frame.GetMaxY() + 31f, msgWidth, 90f));
            _txtView.BackgroundColor = UIColor.Clear;
            _txtView.Editable = false;
            _txtView.ScrollEnabled = false;
            _txtView.Selectable = false;
            _txtView.AttributedText = mutableDowntime;
            _txtView.WeakLinkTextAttributes = linkAttributes.Dictionary;
            _txtView.UserInteractionEnabled = true;
            _txtView.AddGestureRecognizer(
                new UITapGestureRecognizer(() => {
                    string str = bcrmMsg.ToString();
                    if (str.Contains("faqid"))
                    {
                        var startStr = str.Substring(str.IndexOf('{'));
                        string faqId = startStr?.Split('"')[0];
                        if (!string.IsNullOrEmpty(faqId))
                        {
                            ViewHelper.GoToFAQScreenWithId(faqId);
                        }
                    }
                    else if (str.Contains("http") || str.Contains("https"))
                    {
                        var startStr = str.Substring(str.IndexOf('"') + 1);
                        string url = startStr?.Split('"')[0];
                        if (!string.IsNullOrEmpty(url))
                        {
                            ViewHelper.OpenBrowserWithUrl(url);
                        }
                    }
                }));

            _baseView.AddSubview(_txtView);

            if(_isHeaderMode)
            {
                _baseView.Frame = new CGRect(0, 0, baseWidth, _txtView.Frame.GetMaxY() + 29f);
            }
            else
            {
                _baseView.Frame = new CGRect(0, mainYLocation, baseWidth, DeviceHelper.GetScaledHeight(254));
            }
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _baseView;
        }

    }
}
