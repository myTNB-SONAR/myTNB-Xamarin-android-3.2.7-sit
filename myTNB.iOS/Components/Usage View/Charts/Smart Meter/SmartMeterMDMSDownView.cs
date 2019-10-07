using System;
using CoreGraphics;
using myTNB.SmartMeterView;
using UIKit;

namespace myTNB
{
    public class SmartMeterMDMSDownView : BaseSmartMeterView
    {
        private nfloat _width = UIScreen.MainScreen.Bounds.Width;

        public override void CreateSegment(ref CustomUIView view)
        {
            base.CreateSegment(ref view);
            view = new CustomUIView(new CGRect(0, ReferenceWidget.GetMaxY(), _width, GetHeightByScreenSize(157)));
            UIImageView icon = new UIImageView(new CGRect(GetXLocationToCenterObject(GetScaledWidth(61), view), GetScaledHeight(24), GetScaledWidth(61), GetScaledHeight(67)))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_MDMSDown),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            view.AddSubview(icon);
            nfloat descWidth = _width - (GetScaledWidth(32) * 2);
            UILabel desc = new UILabel(new CGRect(GetScaledWidth(32), icon.Frame.GetMaxY() + GetScaledHeight(8), descWidth, 0))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = UIColor.White,
                Lines = 0,
                Text = GetI18NValue(UsageConstants.I18N_MDMSUnavailableMsg)
            };
            CGSize descSize = desc.SizeThatFits(new CGSize(descWidth, 1000f));
            ViewHelper.AdjustFrameSetHeight(desc, descSize.Height);
            view.AddSubview(desc);
        }
    }
}
