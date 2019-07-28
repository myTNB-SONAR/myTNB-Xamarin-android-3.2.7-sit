using CoreGraphics;
using myTNB.Model;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class SSMRReadingHistoryViewController : CustomUIViewController
    {
        SSMRReadingHistoryHeaderComponent _ssmrHeaderComponent;
        UITableView _readingHistoryTableView;
        UIView _headerView;
        MeterReadingHistoryModel _meterReadingHistory;
        List<MeterReadingHistoryItemModel> _readingHistoryList;

        nfloat _headerHeight;
        nfloat _maxHeaderHeight;
        nfloat _minHeaderHeight = 0.1f;
        nfloat _previousScrollOffset;

        public SSMRReadingHistoryViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _meterReadingHistory = DataManager.DataManager.SharedInstance.MeterReadingHistory;
            _readingHistoryList = DataManager.DataManager.SharedInstance.ReadingHistoryList;
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
            _ssmrHeaderComponent.SetTitle(_meterReadingHistory.HistoryViewTitle);
            _ssmrHeaderComponent.SetDescription(_meterReadingHistory.HistoryViewMessage);
            AdjustHeader();
        }

        private void AdjustHeader()
        {
            _headerView.Frame = new CGRect(0, 0, _ssmrHeaderComponent.GetView().Frame.Width, _ssmrHeaderComponent.GetView().Frame.Height);
            _headerHeight = _headerView.Frame.Height;
            _maxHeaderHeight = _headerView.Frame.Height;
        }

        private void AddTableView()
        {
            _readingHistoryTableView = new UITableView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            _readingHistoryTableView.RegisterClassForCellReuse(typeof(SSMRReadingHistoryCell), SSMRConstants.Cell_ReadingHistory);
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(this, OnTableViewScrolled, _readingHistoryList);
            _readingHistoryTableView.BackgroundColor = UIColor.Clear;
            _readingHistoryTableView.RowHeight = UITableView.AutomaticDimension;
            _readingHistoryTableView.EstimatedRowHeight = 67f;
            _readingHistoryTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _readingHistoryTableView.Bounces = false;
            _readingHistoryTableView.SectionFooterHeight = 0;
            _readingHistoryTableView.TableHeaderView = _headerView;
            View.AddSubview(_readingHistoryTableView);
        }

        public void OnTableViewScrolled(object sender, EventArgs e)
        {
            var scrollDiff = _readingHistoryTableView.ContentOffset.Y - _previousScrollOffset;
            var isScrollingDown = scrollDiff > 0;
            var isScrollingUp = scrollDiff < 0;

            var newHeight = _headerHeight;

            if (_readingHistoryTableView.ContentOffset.Y == 0)
            {
                newHeight = _maxHeaderHeight;
            }
            else if (isScrollingDown)
            {
                newHeight = (float)Math.Max(_minHeaderHeight, _headerHeight - Math.Abs(scrollDiff));
            }
            else if (isScrollingUp)
            {
                newHeight = (float)Math.Min(_maxHeaderHeight, _headerHeight + Math.Abs(scrollDiff));
            }

            if (newHeight != _headerHeight)
            {
                _headerHeight = newHeight;
                ViewHelper.AdjustFrameSetHeight(_headerView, _headerHeight);
                _readingHistoryTableView.TableHeaderView = _headerView;
            }

            _previousScrollOffset = _readingHistoryTableView.ContentOffset.Y;
        }
    }
}