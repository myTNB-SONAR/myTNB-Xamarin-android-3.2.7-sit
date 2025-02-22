﻿using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class RearrangeCell : CustomUITableViewCell
    {
        public UILabel lblAccountName;
        public UIImageView reIconView;
        public UIView viewLine;
        private nfloat _imgWidth = ScaleUtility.GetScaledWidth(20F);
        public RearrangeCell(IntPtr handle) : base(handle)
        {
            BackgroundColor = UIColor.White;
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            lblAccountName = new UILabel(new CGRect(BaseMarginWidth16, GetScaledHeight(18F), GetNicknameWidth(), GetScaledHeight(24F)))
            {
                LineBreakMode = UILineBreakMode.TailTruncation,
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey
            };

            reIconView = new UIImageView(new CGRect(0, GetScaledHeight(20F), _imgWidth, _imgWidth));
            viewLine = GenericLine.GetLine(new CGRect(0, GetScaledHeight(60F), cellWidth, GetScaledHeight(1F)));
            viewLine.Hidden = false;
            AddSubviews(new UIView[] { lblAccountName, reIconView, viewLine });
        }

        public string ImageIcon
        {
            set
            {
                if (reIconView != null &&
                    lblAccountName != null &&
                    !string.IsNullOrEmpty(value) &&
                    !string.IsNullOrWhiteSpace(value))
                {
                    reIconView.Hidden = false;
                    nfloat width = GetNicknameWidth() - GetScaledWidth(28F);
                    reIconView.Image = UIImage.FromBundle(value);
                    CGSize nameSize = lblAccountName.SizeThatFits(new CGSize(width, GetScaledHeight(16F)));
                    ViewHelper.AdjustFrameSetWidth(lblAccountName, nameSize.Width <= width ? nameSize.Width : width);
                    nfloat addtl = nameSize.Width <= width ? GetScaledWidth(4F) : 0;
                    ViewHelper.AdjustFrameSetX(reIconView, lblAccountName.Frame.GetMaxX() + addtl);
                }
                else
                {
                    if (reIconView != null && lblAccountName != null)
                    {
                        reIconView.Hidden = true;
                        ViewHelper.AdjustFrameSetWidth(lblAccountName, GetNicknameWidth());
                    }
                }
            }
        }

        private nfloat GetNicknameWidth()
        {
            nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat width = cellWidth - (BaseMarginWidth16 * 2) - GetScaledWidth(24F);
            return width;
        }
    }
}
