using CoreAnimation;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
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
        GradientViewComponent _gradientViewComponent;
        TitleBarComponent _titleBarComponent;
        UITableView _readingHistoryTableView;
        UIView _headerView, _navbarView;
        MeterReadingHistoryModel _meterReadingHistory;
        List<MeterReadingHistoryItemModel> _readingHistoryList;
        CAGradientLayer _gradientLayer;

        nfloat _headerHeight;
        nfloat _maxHeaderHeight;
        nfloat _minHeaderHeight = 0.1f;
        nfloat _tableViewOffset = 64f;
        nfloat _previousScrollOffset;
        nfloat btnWidth = 24f;
        nfloat titleBarHeight = 24f;

        public SSMRReadingHistoryViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = "SSMRReadingHistory";
            base.ViewDidLoad();
            _meterReadingHistory = DataManager.DataManager.SharedInstance.MeterReadingHistory;
            _readingHistoryList = DataManager.DataManager.SharedInstance.ReadingHistoryList;
            SetNavigation();
            PrepareHeaderView();
            AddTableView();
        }

        private void SetNavigation()
        {
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
            }

            int yLocation = 26;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = 50;
            }

            _navbarView = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };

            UIImageView bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, 190f))
            {
                Image = UIImage.FromBundle("SMR-History-BG")
            };

            View.AddSubview(bgImageView);

            UIView viewTitleBar = new UIView(new CGRect(0, yLocation, _navbarView.Frame.Width, titleBarHeight));

            UIView viewBack = new UIView(new CGRect(18, 0, 24, titleBarHeight));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, 24, titleBarHeight))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_BackIcon)
            };
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);

            UILabel lblTitle = new UILabel(new CGRect(58, 0, _navbarView.Frame.Width - 116, titleBarHeight))
            {
                Font = MyTNBFont.MuseoSans16_500,
                Text = GetI18NValue(SSMRConstants.I18N_NavTitle)
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);

            UIView viewRightBtn = new UIView(new CGRect(_navbarView.Frame.Width - 40, 0, 24, titleBarHeight));
            UIImageView imgViewRightBtn = new UIImageView(new CGRect(0, 0, 24, titleBarHeight))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_PrimaryIcon)
            };
            viewRightBtn.AddSubview(imgViewRightBtn);
            viewTitleBar.AddSubview(viewRightBtn);

            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));

            viewRightBtn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("btnRight tapped");
            }));

            _navbarView.AddSubview(viewTitleBar);

            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            _gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            _gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            _gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            _gradientLayer.Frame = _navbarView.Bounds;
            _gradientLayer.Opacity = 0f;
            _navbarView.Layer.InsertSublayer(_gradientLayer, 0);
            View.AddSubview(_navbarView);
        }

        private void AddViewWithOpacity(float opacity)
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            gradientLayer.Frame = _navbarView.Bounds;
            gradientLayer.Opacity = opacity;
            _navbarView.Layer.ReplaceSublayer(_gradientLayer, gradientLayer);
            _gradientLayer = gradientLayer;
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
            _readingHistoryTableView = new UITableView(new CGRect(0, _navbarView.Frame.GetMaxY(), ViewWidth, ViewHeight));
            _readingHistoryTableView.RegisterClassForCellReuse(typeof(SSMRReadingHistoryCell), SSMRConstants.Cell_ReadingHistory);
            _readingHistoryTableView.Source = new SSMRReadingHistoryDataSource(OnTableViewScrolled, _readingHistoryList);
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
            var opac = _previousScrollOffset / _tableViewOffset;
            var absOpacity = Math.Abs((float)opac);
            AddViewWithOpacity(absOpacity);
        }
    }
}