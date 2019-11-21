using System;
using System.Collections.Generic;
using CoreGraphics;
using Force.DeepCloner;
using Foundation;
using myTNB.Model;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;
using UIKit;

namespace myTNB
{
    public partial class RearrangeAccountViewController : CustomUIViewController
    {
        private UITableView _accountListTableView;
        private UIView _footerView;
        private List<CustomerAccountRecordModel> _accountList = new List<CustomerAccountRecordModel>();
        public Action<string> OnRearrangeSuccess;
        private bool _hasArrangedAccts;
        private UIButton _btnSave;

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
                if (_hasArrangedAccts)
                {
                    DisplayCustomAlert(GetI18NValue(DashboardHomeConstants.I18N_RearrangeTitle), GetI18NValue(DashboardHomeConstants.I18N_RearrangeMsg)
                        , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_No), () => { DismissViewController(true, null); } }, { GetCommonI18NValue(Constants.Common_Yes), () => { OnSaveAction(); } } }
                        , false);
                }
                else
                {
                    DismissViewController(true, null);
                }
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

            _btnSave = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, GetYLocationToCenterObject(GetScaledHeight(48F), _footerView), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(48F)),
                Font = TNBFont.MuseoSans_16_500,
                BackgroundColor = _hasArrangedAccts ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice,
                Enabled = _hasArrangedAccts,
                UserInteractionEnabled = true
            };
            _btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnSave.SetTitle(GetI18NValue(DashboardHomeConstants.I18N_RearrangeBtnTitle), UIControlState.Normal);
            _btnSave.Layer.CornerRadius = GetScaledHeight(4F);
            _btnSave.Layer.BorderColor = _hasArrangedAccts ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
            _btnSave.TouchUpInside += (sender, e) =>
            {
                OnSaveAction();
            };
            _footerView.AddSubview(_btnSave);
        }

        public void RearrangeAction(List<CustomerAccountRecordModel> acctList)
        {
            _accountList = acctList;
            _hasArrangedAccts = true;
            EnableSaveButton();
        }

        private void OnSaveAction()
        {
            DataManager.DataManager.SharedInstance.AccountRecordsList.d = new List<CustomerAccountRecordModel>();
            DataManager.DataManager.SharedInstance.AccountRecordsList.d = _accountList.DeepClone();
            UserAccountsEntity uaManager = new UserAccountsEntity();
            if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null)
            {
                uaManager.DeleteTable();
                uaManager.CreateTable();
                uaManager.InsertListOfItems(DataManager.DataManager.SharedInstance.AccountRecordsList);

                string acctListData = JsonConvert.SerializeObject(DataManager.DataManager.SharedInstance.AccountRecordsList);
                var userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                      ? DataManager.DataManager.SharedInstance.UserEntity[0]
                      : new UserEntity();
                if (userInfo.email.IsValid() && acctListData.IsValid())
                {
                    APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
                    NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                    userDefaults.SetString(acctListData, string.Format("{0}-{1}", env, userInfo.email));
                    userDefaults.Synchronize();

                    DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = true;

                    if (OnRearrangeSuccess != null)
                    {
                        OnRearrangeSuccess.Invoke(GetI18NValue(DashboardHomeConstants.I18N_RearrangeToastSuccessMsg));
                        DismissViewController(true, null);
                    }
                    else
                    {
                        DisplayToast(GetI18NValue(DashboardHomeConstants.I18N_RearrangeToastFailMsg));
                    }
                }
                else
                {
                    DisplayToast(GetI18NValue(DashboardHomeConstants.I18N_RearrangeToastFailMsg));
                }
            }
            else
            {
                DisplayToast(GetI18NValue(DashboardHomeConstants.I18N_RearrangeToastFailMsg));
            }
        }

        private void EnableSaveButton()
        {
            _btnSave.Enabled = _hasArrangedAccts;
            _btnSave.BackgroundColor = _hasArrangedAccts ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            _btnSave.Layer.BorderColor = _hasArrangedAccts ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}

