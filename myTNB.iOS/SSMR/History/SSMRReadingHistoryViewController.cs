using CoreGraphics;
using myTNB.SSMR;
using System;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class SSMRReadingHistoryViewController : CustomUIViewController
    {
        SSMRReadingHistoryHeaderComponent _ssmrHeaderComponent;
        UITableView _readingHistoryTableView;
        UIView _headerView;

        public SSMRReadingHistoryViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigation();
            PrepareHeaderView();
            AddTableView();
        }

        private void SetNavigation()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIImage btnRightImg = UIImage.FromBundle("SSMRPrimaryIcon");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            UIBarButtonItem btnRight = new UIBarButtonItem(btnRightImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Debug.WriteLine("btnRight tapped");
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnRight;
            Title = "Self Meter Reading";
        }

        private void PrepareHeaderView()
        {
            _headerView = new UIView
            {
                ClipsToBounds = true
            };
            _ssmrHeaderComponent = new SSMRReadingHistoryHeaderComponent(View);
            _headerView.AddSubview(_ssmrHeaderComponent.GetUI());
            _ssmrHeaderComponent.SetTitle("Looks like you missed your reading.");
            _ssmrHeaderComponent.SetDescription("We will be estimating your Aug 2019 bill. Your next reading period for Sep 2019 will be 08 - 10 Oct 2019.");
            AdjustHeader();
        }

        private void AdjustHeader()
        {
            _headerView.Frame = new CGRect(0, 0, _ssmrHeaderComponent.GetView().Frame.Width, _ssmrHeaderComponent.GetView().Frame.Height);
        }

        private void AddTableView()
        {
            _readingHistoryTableView = new UITableView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            _readingHistoryTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _readingHistoryTableView.RegisterClassForCellReuse(typeof(SSMRReadingHistoryCell), SSMRConstants.Cell_ReadingHistory);
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(this);
            _readingHistoryTableView.BackgroundColor = UIColor.Clear;
            _readingHistoryTableView.TableHeaderView = _headerView;
            View.AddSubview(_readingHistoryTableView);
        }
    }
}