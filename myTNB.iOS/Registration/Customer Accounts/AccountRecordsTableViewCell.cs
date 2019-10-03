using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Registration.CustomerAccounts
{
    public partial class AccountRecordsTableViewCell : UITableViewCell
    {
        public AccountRecordsTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public UIButton DeleteButton
        {
            get
            {
                return btnDelete;
            }
        }

        public string NickNameTitle
        {
            set
            {
                lblNickNameTitle.Text = value;
            }
            get
            {
                return lblNickNameTitle.Text;
            }
        }

        public UILabel NickNameTitleLabel
        {
            get
            {
                return lblNickNameTitle;
            }
        }

        public string NicknameError
        {
            set
            {
                lblNicknameError.Text = value;
            }
            get
            {
                return lblNicknameError.Text;
            }
        }
        public UILabel NickNameErrorLabel
        {
            get
            {
                return lblNicknameError;
            }
        }

        public string AccountNumber
        {
            set
            {
                lblAccountNo.Text = value;
            }
            get
            {
                return lblAccountNo.Text;
            }
        }

        public string Address
        {
            set
            {
                lblAddress.Text = value;
            }
            get
            {
                return lblAddress.Text;
            }
        }

        public UITextField NicknameTextField
        {
            get
            {
                return txtFieldNickname;
            }
        }

        public UIView LineView
        {
            get
            {
                return lineView;
            }
        }

        public UIView SeparatorView
        {
            get
            {
                return viewSeparator;
            }
        }

        public void SetScale()
        {
            lblNickNameTitle.Font = TNBFont.MuseoSans_9_300;
            lblNickNameTitle.TextColor = MyTNBColor.SilverChalice;
            ViewHelper.AdjustFrameSetX(lblNickNameTitle, ScaleUtility.GetScaledWidth(18));
            ViewHelper.AdjustFrameSetY(lblNickNameTitle, ScaleUtility.GetScaledHeight(16));
            ViewHelper.AdjustFrameSetWidth(lblNickNameTitle, ScaleUtility.GetScaledWidth(236));
            ViewHelper.AdjustFrameSetHeight(lblNickNameTitle, ScaleUtility.GetScaledHeight(12));

            txtFieldNickname.Font = TNBFont.MuseoSans_16_300;
            ViewHelper.AdjustFrameSetX(txtFieldNickname, ScaleUtility.GetScaledWidth(18));
            ViewHelper.AdjustFrameSetY(txtFieldNickname, lblNickNameTitle.Frame.GetMaxY());
            ViewHelper.AdjustFrameSetWidth(txtFieldNickname, ScaleUtility.GetScaledWidth(236));
            ViewHelper.AdjustFrameSetHeight(txtFieldNickname, ScaleUtility.GetScaledHeight(24));

            ViewHelper.AdjustFrameSetX(lineView, ScaleUtility.GetScaledWidth(18));
            ViewHelper.AdjustFrameSetY(lineView, txtFieldNickname.Frame.GetMaxY());
            ViewHelper.AdjustFrameSetWidth(lineView, ScaleUtility.GetScaledWidth(236));
            ViewHelper.AdjustFrameSetHeight(lineView, ScaleUtility.GetScaledHeight(1));

            lblNicknameError.Font = TNBFont.MuseoSans_9_300;
            ViewHelper.AdjustFrameSetY(lblNicknameError, lineView.Frame.GetMaxY());
            ViewHelper.AdjustFrameSetWidth(lblNicknameError, ScaleUtility.GetScaledWidth(236));
            ViewHelper.AdjustFrameSetHeight(lblNicknameError, ScaleUtility.GetScaledHeight(14));

            lblAccountNo.Font = TNBFont.MuseoSans_14_500;
            lblAccountNo.TextColor = MyTNBColor.TunaGrey();
            ViewHelper.AdjustFrameSetX(lblAccountNo, ScaleUtility.GetScaledWidth(18));
            ViewHelper.AdjustFrameSetY(lblAccountNo, lineView.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(18));
            ViewHelper.AdjustFrameSetWidth(lblAccountNo, ScaleUtility.GetScaledWidth(236));
            ViewHelper.AdjustFrameSetHeight(lblAccountNo, ScaleUtility.GetScaledHeight(18));

            lblAddress.Font = TNBFont.MuseoSans_12_300;
            lblAddress.TextColor = MyTNBColor.TunaGrey();
            CGSize size = lblAddress.SizeThatFits(new CGSize(lblAddress.Frame.Width, 1000f));
            ViewHelper.AdjustFrameSetHeight(lblAddress, size.Height);
            ViewHelper.AdjustFrameSetWidth(lblAddress, ScaleUtility.GetScaledWidth(236));
            ViewHelper.AdjustFrameSetX(lblAddress, ScaleUtility.GetScaledWidth(18));
            ViewHelper.AdjustFrameSetY(lblAddress, lblAccountNo.Frame.GetMaxY());

            ViewHelper.AdjustFrameSetHeight(btnDelete, ScaleUtility.GetScaledHeight(24));
            ViewHelper.AdjustFrameSetWidth(btnDelete, ScaleUtility.GetScaledWidth(24));
            ViewHelper.AdjustFrameSetX(btnDelete, Frame.Width - ScaleUtility.GetScaledWidth(24) - ScaleUtility.GetScaledWidth(18));
            ViewHelper.AdjustFrameSetY(btnDelete, ScaleUtility.GetYLocationToCenterObject(ScaleUtility.GetScaledHeight(24), this));
        }
    }
}