using Foundation;
using System;
using UIKit;
using CoreAnimation;
using myTNB.Registration;
using CoreGraphics;
using myTNB.Model;
using System.Collections.Generic;
using System.Linq;

namespace myTNB
{
    public partial class AddAccountSuccessViewController : CustomUIViewController
    {
        public int AccountsAddedCount = 0;
        public bool IsDashboardFlow = false;
        const float TopPadding = 48f;
        const float RowHeight = 115f;
        const float HeaderViewHeight = 131f;
        CustomerAccountRecordListModel GetStartedList = new CustomerAccountRecordListModel();

        public AddAccountSuccessViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = AddAccountConstants.PageName;
            base.ViewDidLoad();
            SetupSuperViewBackground();

            AccountsTableView.Frame = new CGRect(BaseMarginWidth16, TopPadding, ViewWidth - GetScaledWidth(32), GetScaledHeight(HeaderViewHeight));
            AccountsTableView.RegisterClassForCellReuse(typeof(AddCASuccessCell), "AddCASuccessCell");
            AccountsTableView.Source = new AddAccountSuccessDataSource(GetStartedList);
            AccountsTableView.Layer.CornerRadius = GetScaledHeight(4f);
            AccountsTableView.RowHeight = RowHeight;
            AccountsTableView.BackgroundColor = UIColor.White;
            AccountsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            AccountsTableView.Bounces = false;
            AccountsTableView.EstimatedRowHeight = GetScaledHeight(61);
            AccountsTableView.RowHeight = UITableView.AutomaticDimension;

            UIView headerView = new UIView((new CGRect(0, 0, ViewWidth - GetScaledWidth(32), GetScaledHeight(HeaderViewHeight))))
            {
                BackgroundColor = UIColor.White
            };

            nfloat imgWidth = GetScaledWidth(64);
            nfloat imgXLoc = (headerView.Frame.Width - imgWidth) / 2;
            UIImageView imgViewSuccess = new UIImageView(new CGRect(imgXLoc, GetScaledHeight(24), imgWidth, imgWidth))
            {
                Image = UIImage.FromBundle(AddAccountConstants.IMG_CircleGreen)
            };

            UILabel lblPasswordSuccess = new UILabel
            {
                Frame = new CGRect(GetScaledWidth(18), imgViewSuccess.Frame.GetMaxY() + GetScaledHeight(10), headerView.Frame.Width - GetScaledWidth(36), GetScaledHeight(18)),
                AttributedText = new NSAttributedString(GetI18NValue(AddAccountConstants.I18N_AddAcctSuccessMsg)
                    , font: TNBFont.MuseoSans_18_500
                    , foregroundColor: MyTNBColor.PowerBlue
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Center,
            };

            headerView.AddSubview(imgViewSuccess);
            headerView.AddSubview(lblPasswordSuccess);

            AccountsTableView.TableHeaderView = headerView;

            UIButton btnStart = new UIButton(new CGRect(GetScaledWidth(18), View.Frame.Height - GetScaledHeight(48) - GetScaledHeight(24), ViewWidth - (GetScaledWidth(18) * 2), GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.FreshGreen,
                Font = TNBFont.MuseoSans_16_500
            };
            btnStart.Layer.CornerRadius = GetScaledHeight(5.0f);
            btnStart.SetTitle(GetCommonI18NValue(AddAccountConstants.I18N_Done), UIControlState.Normal);
            btnStart.SetTitleColor(UIColor.White, UIControlState.Normal);

            btnStart.TouchUpInside += (object sender, EventArgs e) =>
            {
                GetStarted();
            };

            View.AddSubview(btnStart);

            float maxTableHeight = (float)(View.Frame.Height - btnStart.Frame.Height - DeviceHelper.GetScaledHeight(TopPadding * 2));
            float tableHeight = (float)AccountsTableView.Frame.Height;
            if (GetStartedList != null && GetStartedList?.d != null && GetStartedList?.d?.Count > 0)
            {
                tableHeight += (RowHeight * GetStartedList.d.Count);
                if (tableHeight > maxTableHeight)
                {
                    tableHeight = maxTableHeight;
                }
                ViewHelper.AdjustFrameSetHeight(AccountsTableView, tableHeight);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }

        /// <summary>
        /// Creates the newly added list.
        /// </summary>
        public void CreateNewlyAddedList()
        {
            GetStartedList.d = new List<CustomerAccountRecordModel>();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
               && DataManager.DataManager.SharedInstance.AccountsToBeAddedList?.d != null)
            {
                foreach (var item in DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d)
                {
                    int index = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == item.accNum);
                    if (index == -1)
                    {
                        GetStartedList.d.Add(item);
                    }
                }
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.AccountsToBeAddedList != null
                   && DataManager.DataManager.SharedInstance.AccountsToBeAddedList.d != null)
                {
                    GetStartedList = DataManager.DataManager.SharedInstance.AccountsToBeAddedList;
                }
            }
        }

        private void GetStarted()
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ExecuteGetCustomerRecordsCall();
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }


        private void ExecuteGetCustomerRecordsCall()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
               && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null)
            {
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0
                    ? DataManager.DataManager.SharedInstance.AccountRecordsList.d.First()
                    : new CustomerAccountRecordModel();
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex = 0;
                DataManager.DataManager.SharedInstance.PreviousSelectedAccountIndex = 0;
                DataManager.DataManager.SharedInstance.AccountsAddedCount = AccountsAddedCount;

                if (IsDashboardFlow)
                {
                    ViewHelper.DismissControllersAndSelectTab(this, 0, true, true);
                }
                else
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                    UIViewController homeVc = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
                    homeVc.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(homeVc, true, null);
                }
                ActivityIndicator.Hide();
            }
            else
            {
                AlertHandler.DisplayServiceError(this, string.Empty);
            }
            ActivityIndicator.Hide();
        }

        private void SetupSuperViewBackground()
        {
            UIColor startColor = MyTNBColor.GradientPurpleDarkElement;
            UIColor endColor = MyTNBColor.GradientPurpleLightElement;
            CAGradientLayer gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = View.Bounds;
            View.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}
