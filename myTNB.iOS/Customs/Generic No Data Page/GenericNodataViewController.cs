using CoreGraphics;
using System;
using UIKit;

namespace myTNB
{
    public partial class GenericNodataViewController : CustomUIViewController
    {
        public GenericNodataViewController(IntPtr handle) : base(handle)
        {
        }

        public string NavTitle;
        public bool IsRootPage;
        public string Image;
        public string Message;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.SetNavigationBarHidden(true, animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (NavigationController != null)
            {
                NavigationController.SetNavigationBarHidden(false, animated);
            }
        }

        private void SetNavigationBar()
        {
            nfloat navHeight = NavigationController.NavigationBar.Frame.Height;
            UIView viewBack = new UIView(new CGRect(16, DeviceHelper.GetStatusBarHeight() + ((navHeight - 24) / 2), 24, 24));
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (NavigationController != null && IsRootPage)
                {
                    NavigationController.PopViewController(true);
                }
                else
                {
                    DismissViewController(true, null);
                }
            }));

            UIImageView imgBack = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Back-White")
            };
            viewBack.AddSubview(imgBack);
            UILabel lblTitle = new UILabel(new CGRect(50, viewBack.Frame.Y, ViewWidth - 100, 24))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = MyTNBFont.MuseoSans16_500,
                Text = NavTitle ?? string.Empty
            };
            View.AddSubviews(new UIView[] { viewBack, lblTitle });
        }

        private void SetSubviews()
        {
            UIImageView imgView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(Image ?? string.Empty)
            };
            UILabel lblMessage = new UILabel(new CGRect(GetScaledWidth(32), GetYLocationFromFrame(imgView.Frame, 24)
                , ViewWidth - GetScaledWidth(64), GetScaledHeight(40)))
            {
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.Grey,
                Text = Message ?? string.Empty
            };
            CGSize size = GetLabelSize(lblMessage, ViewWidth - GetScaledWidth(64), ViewHeight / 2);
            lblMessage.Frame = new CGRect(lblMessage.Frame.X, lblMessage.Frame.Y, lblMessage.Frame.Width, size.Height);
            View.AddSubviews(new UIView[] { imgView, lblMessage });
            View.SendSubviewToBack(imgView);
        }
    }
}