using System;
using CoreGraphics;
using Foundation;
using myTNB.SmartMeterView;
using UIKit;

namespace myTNB
{
    public class SmartMeterMDMSDownView : BaseSmartMeterView
    {
        public Action OnMDMSRefresh { set; private get; }

        private nfloat _width = UIScreen.MainScreen.Bounds.Width;

        public override void CreateSegment(ref CustomUIView view)
        {
            nfloat margin = AccountUsageSmartCache.IsUnplannedMDMSDown ? GetScaledHeight(10) : GetScaledHeight(16);
            view = new CustomUIView(new CGRect(0, ReferenceWidget.GetMaxY(), _width, GetHeightByScreenSize(157)));

            string mdmsImageString = UsageConstants.IMG_MDMSDownUnplanned;
            string descriptionString = AccountUsageSmartCache.ErrorMessage
                ?? LanguageUtility.GetCommonI18NValue(Constants.Common_RefreshMessage);
            nfloat imgWidth = GetScaledWidth(68);
            nfloat imgHeight = GetScaledWidth(63);
            if (AccountUsageSmartCache.IsPlannedMDMSDown)
            {
                mdmsImageString = UsageConstants.IMG_MDMSDownPlanned;
                imgWidth = GetScaledWidth(70);
                imgHeight = GetScaledWidth(72);
            }

            UIImageView icon = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(68), view), 0, imgWidth, imgHeight))
            {
                Image = UIImage.FromBundle(mdmsImageString),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            view.AddSubview(icon);
            nfloat descWidth = _width - (GetScaledWidth(32) * 2);

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(descriptionString
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, 14f);
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            }, new NSRange(0, htmlBody.Length));

            UITextView txtViewDescription = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                BackgroundColor = UIColor.Clear
            };
            txtViewDescription.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            CGSize size = txtViewDescription.SizeThatFits(new CGSize(descWidth, 1000f));
            txtViewDescription.Frame = new CGRect(GetScaledWidth(32), icon.Frame.GetMaxY() + margin, descWidth, size.Height);
            txtViewDescription.TextAlignment = UITextAlignment.Center;
            view.AddSubview(txtViewDescription);

            if (AccountUsageSmartCache.IsUnplannedMDMSDown)
            {
                CustomUIButtonV2 btnRefresh = new CustomUIButtonV2(true)
                {
                    Frame = new CGRect(GetScaledWidth(16), txtViewDescription.Frame.GetMaxY() + margin
                        , view.Frame.Width - GetScaledWidth(32), GetScaledHeight(48))
                };
                btnRefresh.SetTitle(LanguageUtility.GetCommonI18NValue(Constants.Common_RefreshNow), UIControlState.Normal);
                btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (OnMDMSRefresh != null)
                    {
                        OnMDMSRefresh.Invoke();
                    }
                }));
                view.AddSubview(btnRefresh);
            }
        }
    }
}