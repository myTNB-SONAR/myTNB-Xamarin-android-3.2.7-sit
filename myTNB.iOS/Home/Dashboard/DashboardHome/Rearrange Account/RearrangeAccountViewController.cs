using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public partial class RearrangeAccountViewController : CustomUIViewController
    {
        private UITableView _accountListTableView;
        private UIView _footerView;

        public override void ViewDidLoad()
        {
            PageName = DashboardHomeConstants.PageNameRearrange;
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            View.Frame = new CGRect(0, 0, width, height);
            base.ViewDidLoad();
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = GetI18NValue(DashboardHomeConstants.I18N_RearrangeNavTitle);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(DashboardHomeConstants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            AddSaveButton();
            AddTableView();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_accountListTableView != null)
            {
                _accountListTableView.SetEditing(true, true);
            }
        }

        private void AddTableView()
        {
            var addtl = DeviceHelper.IsIphoneXUpResolution() ? 20F : 0;
            _accountListTableView = new UITableView(new CGRect(0, 0, View.Frame.Width
                , View.Frame.Height - _footerView.Frame.Height - addtl));
            View.AddSubview(_accountListTableView);
            _accountListTableView.Source = new RearrangeDataSource(this);
            _accountListTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _accountListTableView.RegisterClassForCellReuse(typeof(RearrangeCell), DashboardHomeConstants.Cell_RearrangeAccount);
            _accountListTableView.ReloadData();
        }

        private void AddSaveButton()
        {
            var addtl = DeviceHelper.IsIphoneXUpResolution() ? 20F : 0;
            _footerView = new UIView(new CGRect(0, View.Frame.Height - GetScaledHeight(80F) - addtl, ViewWidth, GetScaledHeight(80F)))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_footerView);

            UIButton btnSave = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, GetYLocationToCenterObject(GetScaledHeight(48F), _footerView), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(48F)),
                Font = TNBFont.MuseoSans_16_500,
                BackgroundColor = MyTNBColor.FreshGreen,
                UserInteractionEnabled = true
            };
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.SetTitle(GetI18NValue(DashboardHomeConstants.I18N_RearrangeBtnTitle), UIControlState.Normal);
            btnSave.Layer.CornerRadius = GetScaledHeight(4F);
            btnSave.Layer.BorderColor = UIColor.White.CGColor;
            btnSave.TouchUpInside += (sender, e) =>
            {
                Debug.WriteLine("btnSave on tap!");
            };
            _footerView.AddSubview(btnSave);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}

