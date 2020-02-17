using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class GenericSelectorCell : CustomUITableViewCell
    {
        public UILabel LblAccountName;
        public UIImageView ImgViewTick;
        public UIView LineView;

        public GenericSelectorCell(IntPtr handle) : base(handle)
        {
            BackgroundColor = UIColor.White;
            SelectionStyle = UITableViewCellSelectionStyle.None;

            LblAccountName = new UILabel(new CGRect(BaseMarginWidth16, GetScaledHeight(16F), _cellWidth - (BaseMarginWidth16 * 2) - GetScaledWidth(24F), GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            AddSubview(LblAccountName);

            ImgViewTick = new UIImageView(new CGRect(0, 0, GetScaledWidth(24F), GetScaledHeight(24F)))
            {
                Image = UIImage.FromBundle("Table-Tick")
            };

            LineView = new UIView(new CGRect(0, _cellHeight - GetScaledHeight(1F), _cellWidth, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            AddSubview(LineView);
        }

        public void SetText(string text)
        {
            if (text.IsValid())
            {
                LblAccountName.Text = text;
                CGSize size = LblAccountName.SizeThatFits(new CGSize(LblAccountName.Frame.Width, 1000F));
                ViewHelper.AdjustFrameSetHeight(LblAccountName, size.Height);
                ViewHelper.AdjustFrameSetY(LineView, GetYLocationFromFrame(LblAccountName.Frame, 15F));
            }
        }
    }
}
