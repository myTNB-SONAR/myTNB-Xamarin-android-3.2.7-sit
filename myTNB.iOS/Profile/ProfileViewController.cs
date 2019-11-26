using CoreGraphics;
using Foundation;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class ProfileViewController : CustomUIViewController
    {
        public ProfileViewController(IntPtr handle) : base(handle) { }

        private TitleBarComponent _titleBarComponent;
        private UILabel _lblAppVersion;
        private bool _isSitecoreDone, _isMasterDataDone;
        private GenericSelectorViewController languageViewController;

        private UITableView _profileTableview;

        public override void ViewDidLoad()
        {
            PageName = ProfileConstants.Pagename_Profile;
            // NavigationController.NavigationBarHidden = true;
            base.ViewDidLoad();
            _isSitecoreDone = false;
            _isMasterDataDone = false;
            SetSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //moreTableView.Source = new MoreDataSource(this, GetMoreList());
            //moreTableView.ReloadData();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> MORE LanguageDidChange");
            base.LanguageDidChange(notification);
            _titleBarComponent?.SetTitle(GetI18NValue(ProfileConstants.I18N_NavTitle));
            _lblAppVersion.Text = string.Format("{0} {1}", GetI18NValue(ProfileConstants.I18N_AppVersion), AppVersionHelper.GetAppShortVersion());
            if (!TNBGlobal.IsProduction)
            {
                _lblAppVersion.Text += string.Format("({0})", AppVersionHelper.GetBuildVersion());
            }
        }

        private Dictionary<string, List<string>> GetMoreList()
        {
            Dictionary<string, List<string>> _itemsDictionary = new Dictionary<string, List<string>>(){
                {GetI18NValue(ProfileConstants.I18N_Settings), new List<string>{ GetI18NValue(ProfileConstants.I18N_MyAccount)
                    , GetI18NValue(ProfileConstants.I18N_Notifications)
                    , GetI18NValue(ProfileConstants.I18N_SetAppLanguage)}}
                , {GetI18NValue(ProfileConstants.I18N_HelpAndSupport), new List<string>{ GetI18NValue(ProfileConstants.I18N_FindUs)
                    , GetI18NValue(ProfileConstants.I18N_CallUsOutagesAndBreakdown)
                    ,GetI18NValue(ProfileConstants.I18N_CallUsBilling)
                    ,GetI18NValue(ProfileConstants.I18N_FAQ)
                    , GetI18NValue(ProfileConstants.I18N_TNC)}}
                , {GetI18NValue(ProfileConstants.I18N_Share), new List<string>{ GetI18NValue(ProfileConstants.I18N_ShareDescription)
                    , GetI18NValue(ProfileConstants.I18N_Rate)}}
            };
            if (_itemsDictionary.ContainsKey(GetI18NValue(ProfileConstants.I18N_HelpAndSupport)) && IsValidWeblinks())
            {
                int cloIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbclo"));
                int cleIndex = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcle"));
                if (cloIndex > -1 && cleIndex > -1)
                {
                    List<string> helpAndSupportList = new List<string>
                    {
                        GetI18NValue(ProfileConstants.I18N_FindUs)
                        , DataManager.DataManager.SharedInstance.WebLinks[cloIndex].Title
                        , DataManager.DataManager.SharedInstance.WebLinks[cleIndex].Title
                        , GetI18NValue(ProfileConstants.I18N_FAQ)
                        , GetI18NValue(ProfileConstants.I18N_TNC)
                    };
                    _itemsDictionary[GetI18NValue(ProfileConstants.I18N_HelpAndSupport)] = helpAndSupportList;
                }
            }
            return _itemsDictionary;
        }

        private bool IsValidWeblinks()
        {
            return DataManager.DataManager.SharedInstance.WebLinks != null;
        }

        private void SetSubviews()
        {
            Title = GetI18NValue(ProfileConstants.I18N_NavTitle);
            // ViewHeight = View.Frame.Height - NavigationController.NavigationBar.Frame.GetMaxY() - (View.Frame.Height - TabBarController.TabBar.Frame.GetMinY());
            _profileTableview = new UITableView(new CGRect(0, NavigationController.NavigationBar.Frame.GetMaxY(), View.Frame.Width, ViewHeight));
            _profileTableview.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            _profileTableview.Layer.BorderColor = UIColor.Red.CGColor;
            _profileTableview.Layer.BorderWidth = 1;

            _profileTableview.RegisterClassForCellReuse(typeof(UITableViewCell), "genericViewCell");


            View.AddSubview(_profileTableview);

            _lblAppVersion = new UILabel(new CGRect(18, 16, _profileTableview.Frame.Width - 36, 14))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9_300,
                Text = string.Format("{0} {1}", GetI18NValue(ProfileConstants.I18N_AppVersion), AppVersionHelper.GetAppShortVersion())
            };

            if (!TNBGlobal.IsProduction)
            {
                _lblAppVersion.Text += string.Format("({0})", AppVersionHelper.GetBuildVersion());
            }

            _profileTableview.TableFooterView = _lblAppVersion;
        }
    }
}