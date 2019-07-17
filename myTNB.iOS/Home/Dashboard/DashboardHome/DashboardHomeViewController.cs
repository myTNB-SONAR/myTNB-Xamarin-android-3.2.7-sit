using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public partial class DashboardHomeViewController : CustomUIViewController
    {
        public DashboardHomeViewController(IntPtr handle) : base(handle) { }

        private UITableView _homeTableView;
        private UIView _viewHeader;

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }
            PageName = DashboardHomeConstants.PageName;
            IsGradientRequired = true;
            base.ViewDidLoad();

            AddHeader();
            AddTableView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _homeTableView.Source = new DashboardHomeDataSource(this);
            _homeTableView.ReloadData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        private void AddTableView()
        {
            nfloat tabbarHeight = TabBarController.TabBar.Frame.Height + 20.0F;
            _homeTableView = new UITableView(new CGRect(0, _viewHeader.Frame.GetMaxY(), View.Frame.Width
                , View.Frame.Height - _viewHeader.Frame.GetMaxY() - tabbarHeight))
            { BackgroundColor = UIColor.Clear };
            _homeTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _homeTableView.RowHeight = UITableView.AutomaticDimension;
            _homeTableView.EstimatedRowHeight = 600.0F;
            _homeTableView.RegisterClassForCellReuse(typeof(HelpTableViewCell), DashboardHomeConstants.Cell_Help);
            _homeTableView.RegisterClassForCellReuse(typeof(ServicesTableViewCell), DashboardHomeConstants.Cell_Services);
            View.AddSubview(_homeTableView);
        }

        private void AddHeader()
        {
            float yLoc = 36.0F;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLoc += 20.0F;
            }
            _viewHeader = new UIView(new CGRect(0, yLoc, View.Frame.Width, 48.0F))
            { BackgroundColor = UIColor.Clear, ClipsToBounds = true };
            UIView viewNotification = new UIView(new CGRect(View.Frame.Width - (24 + 16), 12, 24, 24));
            UIImageView imgNotification = new UIImageView(new CGRect(0, 0, viewNotification.Frame.Width, viewNotification.Frame.Height))
            { Image = UIImage.FromBundle(DashboardHomeConstants.Img_Notification) };
            viewNotification.AddGestureRecognizer(new UITapGestureRecognizer());
            viewNotification.AddSubview(imgNotification);
            UILabel lblGreeting = new UILabel(new CGRect(16, 8, _viewHeader.Frame.Width * 0.60F, 16))
            {
                TextColor = MyTNBColor.SunGlow,
                Font = MyTNBFont.MuseoSans16_500,
                Text = GetGreeting()
            };
            UILabel lblName = new UILabel(new CGRect(16, 24, _viewHeader.Frame.Width - 40, 16))
            {
                TextColor = MyTNBColor.SunGlow,
                Font = MyTNBFont.MuseoSans16_500,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Text = GetDisplayName()
            };
            _viewHeader.AddSubviews(new UIView[] { lblGreeting, lblName, viewNotification });
            View.AddSubview(_viewHeader);
        }

        private string GetGreeting()
        {
            DateTime now = DateTime.Now;
            string key = DashboardHomeConstants.I18N_Evening;
            if (now.Hour < 12)
            {
                key = DashboardHomeConstants.I18N_Morning;
            }
            else if (now.Hour < 18)
            {
                key = DashboardHomeConstants.I18N_Afternoon;
            }
            return I18NDictionary[key];
        }

        private string GetDisplayName()
        {
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0 && DataManager.DataManager.SharedInstance.UserEntity[0] != null)
            {
                return string.Format("{0}!", DataManager.DataManager.SharedInstance.UserEntity[0]?.displayName);
            }
            return string.Empty;
        }
    }
}