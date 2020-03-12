using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.ProfileUpdates.UpdateMobileNumber.SelectCountry
{
    public class SelectCountryDataSource : UITableViewSource
    {
        private Dictionary<string, List<CountryModel>> _countryDictionary;
        private List<string> _keys;

        public Action<CountryModel> OnSelect;

        public SelectCountryDataSource(Dictionary<string, List<CountryModel>> countryDictionary)
        {
            _countryDictionary = countryDictionary;
            _keys = _countryDictionary.Keys.ToList();
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            string key = _keys[(int)section];
            UIView sectionView = new UIView(new CGRect(0, 0, tableView.Bounds.Width, ScaleUtility.GetScaledHeight(44)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblTitle = new UILabel
            {
                Frame = new CGRect(ScaleUtility.GetScaledWidth(16), ScaleUtility.GetScaledWidth(16), sectionView.Frame.Width, ScaleUtility.GetScaledWidth(20)),
                Text = key,
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue
            };
            sectionView.AddSubviews(new UIView[] { lblTitle });
            return sectionView;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return ScaleUtility.GetScaledHeight(44);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SelectCountryCell cell = tableView.DequeueReusableCell(SelectCountryConstants.Cell_SelectCountryCell) as SelectCountryCell;

            string key = _keys[indexPath.Section];
            List<CountryModel> countryList = _countryDictionary[key];
            CountryModel countryInfo = countryList[indexPath.Row];

            cell.Flag = countryInfo.CountryCode;
            cell.Code = countryInfo.CountryISDCode;
            cell.Country = countryInfo.CountryName;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            string key = _keys[(int)section];
            return _countryDictionary[key].Count;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _countryDictionary.Count;
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            string[] keys = _countryDictionary.Keys.ToArray();
            return keys;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (OnSelect != null)
            {
                string key = _keys[indexPath.Section];
                List<CountryModel> countryList = _countryDictionary[key];
                CountryModel countryInfo = countryList[indexPath.Row];
                OnSelect.Invoke(countryInfo);
            }
        }
    }
}