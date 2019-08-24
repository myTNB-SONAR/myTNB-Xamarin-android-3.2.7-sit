using System;
using UIKit;
using myTNB.Dashboard.SelectAccounts;
using CoreGraphics;
using myTNB.Home.Dashboard.SelectAccounts;
using System.Collections.Generic;

namespace myTNB
{
    public partial class SelectAccountTableViewController : CustomUIViewController
    {
        public SelectAccountTableViewController(IntPtr handle) : base(handle) { }

        public bool IsFromSSMR;
        public bool IsFromUsage;
        public bool IsRoot;
        public int CurrentSelectedIndex = -1;
        public Action<int> OnSelect;

        public override void ViewDidLoad()
        {
            PageName = SelectAccountConstants.PageName;
            base.ViewDidLoad();
            AddBackButton();
            nfloat navBarHeigt = NavigationController == null ? 0 : NavigationController.NavigationBar.Frame.Height;
            accountRecordsTableView.Frame = new CGRect(0, 0, View.Frame.Width
                , View.Frame.Height - UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom
                - UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Top
                - navBarHeigt);
            accountRecordsTableView.Source = new SelectAccountsDataSource(this);
            accountRecordsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            if (IsFromSSMR) { AddMissingAccountFooter(); }
            accountRecordsTableView.ReloadData();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        private void AddBackButton()
        {
            Title = GetI18NValue(SelectAccountConstants.I18N_NavTitle);
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (IsRoot)
                { NavigationController.PopViewController(true); }
                else
                { DismissViewController(true, null); }
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        #region SSMR Footer
        private void AddMissingAccountFooter()
        {
            CustomUIView view = new CustomUIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(72)))
            {
                BackgroundColor = UIColor.White
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMargin
                , GetScaledHeight(24), BaseMarginedWidth, GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(SelectAccountConstants.IMG_Info)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), view.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(SelectAccountConstants.I18N_AccountsMissing)
            };
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            view.AddSubview(viewInfo);
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Model.PopupModel popUpContent = SSMRAccounts.GetPopupDetailsByType(SelectAccountConstants.Popup_NoSSMRCA);
                DisplayCustomAlert(popUpContent.Title, popUpContent.Description
                    , new Dictionary<string, Action> { { popUpContent.CTA, null } }
                    , false);
            }));
            accountRecordsTableView.TableFooterView = view;
        }
        #endregion
    }
}