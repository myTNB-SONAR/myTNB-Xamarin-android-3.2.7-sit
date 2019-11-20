using System;
using CoreGraphics;
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
            view = new CustomUIView(new CGRect(0, ReferenceWidget.GetMaxY(), _width, GetHeightByScreenSize(157)));

            UIImageView icon = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(68), view), 0, GetScaledWidth(68), GetScaledHeight(63)))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_MDMSDown),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            view.AddSubview(icon);
            nfloat descWidth = _width - (GetScaledWidth(32) * 2);
            UILabel desc = new UILabel(new CGRect(GetScaledWidth(32), icon.Frame.GetMaxY() + GetScaledHeight(17), descWidth, 0))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = UIColor.White,
                Lines = 0,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_RefreshMessage)
            };
            CGSize descSize = desc.SizeThatFits(new CGSize(descWidth, 1000f));
            ViewHelper.AdjustFrameSetHeight(desc, descSize.Height);
            view.AddSubview(desc);

            CustomUIButtonV2 btnRefresh = new CustomUIButtonV2(true)
            {
                Frame = new CGRect(GetScaledWidth(16), desc.Frame.GetMaxY() + GetScaledHeight(16)
                    , view.Frame.Width - GetScaledWidth(32), GetScaledHeight(48))
            };
            btnRefresh.SetTitle(AccountUsageSmartCache.ErrorCTA ?? LanguageUtility.GetCommonI18NValue(Constants.Common_RefreshNow)
                , UIControlState.Normal);
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