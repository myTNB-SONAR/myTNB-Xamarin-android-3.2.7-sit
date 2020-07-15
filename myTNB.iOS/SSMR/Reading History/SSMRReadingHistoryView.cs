using System;
using CoreGraphics;
using UIKit;

namespace myTNB.SSMR.ReadingHistory
{
    internal static class SSMRReadingHistoryView
    {
        #region Set CTA
        internal static void SetStartButton(this SSMRReadingHistoryViewController c)
        {
            nfloat height = c.GetScaledHeight(80) + DeviceHelper.BottomSafeAreaInset;

            c._ctaView = new CustomUIView(new CGRect(0, c.View.Frame.Height - DeviceHelper.BottomSafeAreaInset,
                c.View.Frame.Width, height))
            {
                BackgroundColor = UIColor.White
            };
            c._btnStart = new CustomUIButtonV2
            {
                Frame = new CGRect(c.BaseMargin, c.GetScaledHeight(16), c.BaseMarginedWidth, c.GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.SilverChalice,
                PageName = c.PageName,
                EventName = "StartSMR",
                Enabled = false
            };
            c._btnStart.SetTitle(c.GetI18NValue(SSMRConstants.I18N_EnableSSMRCTA), UIControlState.Normal);
            c._btnStart.SetTitleColor(UIColor.White, UIControlState.Normal);
            c._btnStart.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
            c._btnStart.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                c.OnEnableSSMR();
            }));
            c._ctaView.AddSubviews(new UIView[] { c._btnStart });
            c.View.AddSubview(c._ctaView);
        }

        internal static void SetStartButtonEnable(this SSMRReadingHistoryViewController c, bool enable)
        {
            c._btnStart.Enabled = enable;
            c._btnStart.BackgroundColor = enable ? MyTNBColor.AlgaeGreen : MyTNBColor.SilverChalice;
            c._btnStart.Layer.BorderColor = (enable ? MyTNBColor.AlgaeGreen : MyTNBColor.SilverChalice).CGColor;
        }

        internal static void SetApplyItemsHidden(this SSMRReadingHistoryViewController c, bool hidden)
        {
            if (c._ssmrHeaderComponent != null)
            {
                c._ssmrHeaderComponent.IsApplyHidden = true;
            }
            if (c._ctaView != null)
            {
                c._ctaView.Hidden = hidden;
                c._ctaView.RemoveFromSuperview();
                c._ctaView = null;
            }
        }
        #endregion
    }
}