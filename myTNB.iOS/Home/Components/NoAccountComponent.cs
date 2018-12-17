using System;
using CoreGraphics;
using UIKit;
using myTNB.Extensions;

namespace myTNB.Dashboard.DashboardComponents
{
    public class NoAccountComponent
    {
        UIView _parentView;
        UIView _viewNoAccount;
        UILabel _lblTitle;
        UILabel _lblSubtitle;
        public UIButton _btnAddAccount;
        public NoAccountComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            float yLocation = DeviceHelper.GetScaledHeight(70);
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = 80;
            }
            _viewNoAccount = new UIView();
            _viewNoAccount.Frame = new CGRect(0, yLocation, _parentView.Frame.Width, _parentView.Frame.Height - 156);//(DeviceHelper.IsIphoneXUpResolution() ? 32 : 56));

            float topSpace = -15;
            if (_parentView.Frame.Width == 414)
            {
                topSpace = -40;
            }

            var imgWidth = _viewNoAccount.Frame.Width * .4;
            var xLocation = _viewNoAccount.Frame.Width / 2 - imgWidth / 2;
            UIImageView imgViewEmpty = new UIImageView(new CGRect(xLocation, topSpace, imgWidth, imgWidth));
            imgViewEmpty.Image = UIImage.FromBundle("Empty-Dashboard");
            _viewNoAccount.AddSubview(imgViewEmpty);

            _lblTitle = new UILabel(new CGRect(10, imgViewEmpty.Frame.GetMaxY() + 20, _viewNoAccount.Frame.Width - 20, 16));
            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.Font = myTNBFont.MuseoSans14_500();
            _lblTitle.Text = "No Electricity Account";
            _lblTitle.TextColor = UIColor.White;
            _viewNoAccount.AddSubview(_lblTitle);

            var subWidth = _viewNoAccount.Frame.Width * .8;
            xLocation = _viewNoAccount.Frame.Width / 2 - subWidth / 2;
            _lblSubtitle = new UILabel(new CGRect(xLocation, _lblTitle.Frame.GetMaxY() + 2, subWidth, 28));
            _lblSubtitle.TextAlignment = UITextAlignment.Center;
            _lblSubtitle.Text = string.Format("DshbrdNoAccountMsg".Translate(), Environment.NewLine);
            _lblSubtitle.Font = myTNBFont.MuseoSans11_300();
            _lblSubtitle.Lines = 2;
            _lblSubtitle.TextColor = UIColor.White;
            _lblSubtitle.LineBreakMode = UILineBreakMode.WordWrap;
            _viewNoAccount.AddSubview(_lblSubtitle);

            _btnAddAccount = new UIButton(UIButtonType.Custom);
            _btnAddAccount.Frame = new CGRect(90, _lblSubtitle.Frame.GetMaxY() + 16, _viewNoAccount.Frame.Width - 180, 48);
            _btnAddAccount.Layer.CornerRadius = 4;
            _btnAddAccount.Layer.BorderColor = UIColor.White.CGColor;
            _btnAddAccount.Layer.BorderWidth = 1;
            _btnAddAccount.SetTitle("AddAcct".Translate(), UIControlState.Normal);
            _btnAddAccount.Font = myTNBFont.MuseoSans18_500();
            _btnAddAccount.SetTitleColor(UIColor.White, UIControlState.Normal);
            _viewNoAccount.AddSubview(_btnAddAccount);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewNoAccount;
        }

        public void SetTitle(string title)
        {
            _lblTitle.Text = title;
        }

        public void SetSubtitle(string subtitle)
        {
            _lblSubtitle.Text = subtitle;
        }

        public void SetAddAccountButtonTitle(string title)
        {
            _btnAddAccount.SetTitle(title, UIControlState.Normal);
        }
    }
}