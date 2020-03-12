using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.ProfileUpdates.UpdateMobileNumber.SelectCountry;
using UIKit;

namespace myTNB
{
    public class SelectCountryViewController : CustomUIViewController
    {
        private UITableView _tableView;
        public override void ViewDidLoad()
        {
            PageName = SelectCountryConstants.Pagename_SelectCountry;
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            View.BackgroundColor = MyTNBColor.LightGrayBG;
            SetNavigationBar();
            AddSubviews();
            AddSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Dictionary<string, List<CountryModel>> countryDictionary = CountryManager.Instance.GetCountryDictionary();
            _tableView.Source = new SelectCountryDataSource(countryDictionary)
            {
                OnSelect = OnSelect
            };
            _tableView.ReloadData();
        }

        private void SetNavigationBar()
        {
            Title = GetI18NValue(SelectCountryConstants.I18N_Title);
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (NavigationController != null)
                {
                    NavigationController.PopViewController(true);
                }
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void AddSubviews()
        {
            _tableView = new UITableView(new CGRect(0, 0, ViewWidth, ViewHeight))
            {
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                EstimatedRowHeight = GetScaledHeight(61),
                RowHeight = UITableView.AutomaticDimension,
                BackgroundColor = UIColor.Clear
            };
            _tableView.RegisterClassForCellReuse(typeof(SelectCountryCell), SelectCountryConstants.Cell_SelectCountryCell);
            View.AddSubview(_tableView);
        }

        private void OnSelect(CountryModel countryInfo)
        {
            NSString[] keys = new[] { new NSString("CountryCode"), new NSString("CountryName"), new NSString("CountryISDCode") };
            NSString[] values = new[] { new NSString(countryInfo.CountryCode.ToUpper())
                , new NSString(countryInfo.CountryName), new NSString(countryInfo.CountryISDCode) };
            NSDictionary<NSString, NSString> userInfo = new NSDictionary<NSString, NSString>(keys, values);
            NSNotificationCenter.DefaultCenter.PostNotificationName("OnCountrySelected", new NSObject(), userInfo);
            if (NavigationController != null)
            {
                NavigationController.PopViewController(true);
            }
        }
    }
}