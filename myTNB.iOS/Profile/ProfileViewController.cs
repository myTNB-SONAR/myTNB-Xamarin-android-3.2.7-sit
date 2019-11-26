using CoreGraphics;
using Foundation;
using myTNB.Dashboard.DashboardComponents;
using myTNB.MyAccount;
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
            SetTableView();
            SetFooterView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //moreTableView.Source = new MoreDataSource(this, GetMoreList());
            //moreTableView.ReloadData();

            ProfileDataSource dataSource = new ProfileDataSource();
            dataSource.ProfileList = GetProfileList();
            _profileTableview.Source = dataSource;
            _profileTableview.ReloadData();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> MORE LanguageDidChange");
            base.LanguageDidChange(notification);
            Title = GetI18NValue(ProfileConstants.I18N_NavTitle);
            _lblAppVersion.Text = Version;
        }

        private Dictionary<string, List<string>> GetProfileList()
        {
            Dictionary<string, List<string>> profileList = new Dictionary<string, List<string>>();
            profileList.Add(GetI18NValue(MyAccountConstants.I18N_DetailSectionTitle), new List<string>());
            profileList.Add(GetI18NValue(ProfileConstants.I18N_Settings), new List<string> {
                GetI18NValue(ProfileConstants.I18N_Notifications)
                , GetI18NValue(ProfileConstants.I18N_SetAppLanguage)
            });
            profileList.Add(GetI18NValue(ProfileConstants.I18N_HelpAndSupport), new List<string>{
                GetI18NValue(ProfileConstants.I18N_FindUs)
                , GetI18NValue(ProfileConstants.I18N_CallUsOutagesAndBreakdown)
                , GetI18NValue(ProfileConstants.I18N_CallUsBilling)
                , GetI18NValue(ProfileConstants.I18N_FAQ)
                , GetI18NValue(ProfileConstants.I18N_TNC)});
            profileList.Add(GetI18NValue(ProfileConstants.I18N_Share), new List<string>{
                GetI18NValue(ProfileConstants.I18N_ShareDescription)
                , GetI18NValue(ProfileConstants.I18N_Rate)});

            return profileList;
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

        private void SetTableView()
        {
            Title = GetI18NValue(ProfileConstants.I18N_NavTitle);
            _profileTableview = new UITableView(new CGRect(0, NavigationController.NavigationBar.Frame.GetMaxY(), View.Frame.Width, ViewHeight));
            _profileTableview.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _profileTableview.RegisterClassForCellReuse(typeof(ProfileCell), ProfileConstants.Cell_Profile);
            View.AddSubview(_profileTableview);
        }

        private void SetFooterView()
        {
            UIView footerView = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(118))) { BackgroundColor = MyTNBColor.LightGrayBG };

            _lblAppVersion = new UILabel(new CGRect(BaseMargin, GetScaledHeight(8), BaseMarginedWidth, GetScaledHeight(14)))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = TNBFont.MuseoSans_9_300,
                Text = Version
            };

            UIView logoutView = new UIView(new CGRect(0, GetScaledHeight(38), ViewWidth, GetScaledHeight(80))) { BackgroundColor = UIColor.White };
            CustomUIButtonV2 btnLogout = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            btnLogout.SetTitle(GetCommonI18NValue(Constants.Common_Logout), UIControlState.Normal);
            btnLogout.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("Logout");
            }));
            logoutView.AddSubview(btnLogout);
            footerView.AddSubviews(new UIView[] { _lblAppVersion, logoutView });
            _profileTableview.TableFooterView = footerView;
        }

        private string Version
        {
            get
            {
                string appVersion = string.Format("{0} {1}", GetI18NValue(ProfileConstants.I18N_AppVersion)
                    , AppVersionHelper.GetAppShortVersion());
                if (!TNBGlobal.IsProduction)
                {
                    appVersion += string.Format("({0})", AppVersionHelper.GetBuildVersion());
                }
                return appVersion;
            }
        }
    }
}