using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.More.FindUs.LocationDetails
{
    public class LocationDetailsDataSource : UITableViewSource
    {
        LocationDetailsViewController _controller;
        AnnotationModel _annotation;
        Dictionary<string, string> _dataDictionary = new Dictionary<string, string>();

        public LocationDetailsDataSource(LocationDetailsViewController controller, AnnotationModel annotation)
        {
            _controller = controller;
            _annotation = annotation;
            PopulateDictionary();
        }

        void PopulateDictionary()
        {
            _dataDictionary.Clear();
            _dataDictionary.Add("TITLE", _annotation.Title);
            _dataDictionary.Add("ADDRESS", _annotation.Subtitle);
            if (_annotation.is7E)
            {
                _dataDictionary.Add("PHONE", _annotation.ConvinientStoreItem.PhoneNumber);
            }
            else
            {
                int phoneCount = 0;
                if (_annotation.KTItem.Phones.Count > 1)
                {
                    foreach (var item in _annotation.KTItem.Phones)
                    {
                        phoneCount++;
                        _dataDictionary.Add(string.Format("PHONE {0}", phoneCount), item.PhoneNumber);
                    }
                }
                else
                {
                    _dataDictionary.Add("PHONE", _annotation.KTItem.Phones[0].PhoneNumber);
                }

            }
            _dataDictionary.Add("OPENING HOURS", "");
            _dataDictionary.Add("SERVICES", "");
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _dataDictionary.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            string key = _dataDictionary.Keys.ElementAt(indexPath.Row);

            if (key.ToLower().Equals("title"))
            {
                var cell = tableView.DequeueReusableCell("LocationNameViewCell", indexPath) as LocationNameViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblName.Text = _annotation.Title;
                CGSize newSize = GetTitleLabelSize(_annotation.Title);
                cell.lblName.Frame = new CGRect(cell.lblName.Frame.X, cell.lblName.Frame.Y
                                                , cell.lblName.Frame.Width, newSize.Height);
                return cell;
            }
            else if (key.ToLower().Equals("address"))
            {
                var cell = tableView.DequeueReusableCell("AddressViewCell", indexPath) as AddressViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = "ADDRESS";
                cell.lblValue.Text = _annotation.Subtitle;
                CGSize newSize = GetLabelSize(cell.lblValue, cell.lblValue.Frame.Width, 1000);
                cell.lblValue.Frame = new CGRect(cell.lblValue.Frame.X, cell.lblValue.Frame.Y
                                                 , cell.lblValue.Frame.Width, newSize.Height);
                cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    _controller.OpenDirections(_annotation);
                }));

                cell.viewDirections.Frame = new CGRect(cell.viewDirections.Frame.X
                                                       , 30 + ((newSize.Height - cell.viewDirections.Frame.Height) / 2)
                                                       , cell.viewDirections.Frame.Width
                                                       , cell.viewDirections.Frame.Height);
                return cell;
            }
            else if (key.ToLower().Contains("phone"))
            {
                var cell = tableView.DequeueReusableCell("ContactUsViewCell", indexPath) as ContactUsViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = key;
                string number = string.Empty;
                if (!_annotation.is7E)
                {
                    number = _dataDictionary[key];
                }
                else
                {
                    number = _annotation.ConvinientStoreItem.PhoneNumber;
                    if (string.IsNullOrEmpty(number) || string.IsNullOrWhiteSpace(number))
                    {
                        cell.viewCall.Hidden = true;
                    }
                }
                cell.lblValue.Text = number;
                cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (!string.IsNullOrEmpty(number) && !string.IsNullOrWhiteSpace(number))
                    {
                        _controller.CallNumber(number);
                    }
                }));
                return cell;
            }
            else if (key.ToLower().Equals("opening hours"))
            {
                var cell = tableView.DequeueReusableCell("OpeningHoursViewCell", indexPath) as OpeningHoursViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = "OPENING HOURS";
                if (_annotation.is7E)
                {
                    cell.lbl7EOperation.Text = "Open 24 hours";
                }
                else
                {
                    string day = string.Empty;
                    string time = string.Empty;
                    GetOperatingHours(ref day, ref time);
                    cell.lblDay.Text = day;
                    cell.lblTime.Text = time;
                    CGSize newSizeDay = GetLabelSize(cell.lblDay, cell.lblDay.Frame.Width, 100);
                    CGSize newSizeTime = GetLabelSize(cell.lblTime, cell.lblTime.Frame.Width, 100);
                    cell.lblDay.Frame = new CGRect(cell.lblDay.Frame.X, cell.lblDay.Frame.Y
                                                   , cell.lblDay.Frame.Width, newSizeDay.Height);
                    cell.lblTime.Frame = new CGRect(cell.lblTime.Frame.X, cell.lblTime.Frame.Y
                                                    , cell.lblTime.Frame.Width, newSizeTime.Height);
                    return cell;
                }
                cell.lblDay.Hidden = _annotation.is7E;
                cell.lblTime.Hidden = _annotation.is7E;
                cell.lbl7EOperation.Hidden = !_annotation.is7E;
                return cell;
            }
            else if (key.ToLower().Equals("services"))
            {
                var cell = tableView.DequeueReusableCell("ServicesViewCell", indexPath) as ServicesViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = "SERVICES";
                string services = string.Empty;
                GetServices(ref services);
                cell.lblValue.Text = services;
                CGSize newSize = GetLabelSize(cell.lblValue, cell.lblValue.Frame.Width, 1000);
                cell.lblValue.Frame = new CGRect(cell.lblValue.Frame.X, cell.lblValue.Frame.Y
                                                 , cell.lblValue.Frame.Width, newSize.Height);
                return cell;
            }
            return null;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            string key = _dataDictionary.Keys.ElementAt(indexPath.Row);
            nfloat rowHeight = 62F;
            if (key.ToLower().Equals("title"))
            {
                CGSize newSize = GetTitleLabelSize(_annotation.Title);
                rowHeight = 36 + newSize.Height;
            }
            else if (key.ToLower().Equals("address"))
            {
                CGSize newSize = GetLabelSize(_annotation.Subtitle, true);
                rowHeight = 46 + newSize.Height;
            }
            else if (key.ToLower().Contains("phone"))
            {
                rowHeight = 62F;
            }
            else if (key.ToLower().Equals("opening hours"))
            {
                if (_annotation.is7E)
                {
                    rowHeight = 62F;
                }
                else
                {
                    string day = string.Empty;
                    string time = string.Empty;
                    GetOperatingHours(ref day, ref time);
                    CGSize newDaySize = GetLabelSize(day, false);
                    CGSize newTimeSize = GetLabelSize(day, false);
                    nfloat height = newDaySize.Height > newTimeSize.Height ? newDaySize.Height : newTimeSize.Height;
                    rowHeight = 50 + height;
                }
            }
            else if (key.ToLower().Equals("services"))
            {
                string services = string.Empty;
                if (_annotation.is7E)
                {
                    services += "•\tPayment of electricity bills and other utility bills";
                }
                else
                {
                    for (int i = 0; i < _annotation.KTItem.Services.Count; i++)
                    {
                        services += "•\t" + _annotation.KTItem.Services[i].Title;
                        int index = i;
                        if (index < _annotation.KTItem.Services.Count - 1)
                        {
                            services += "\r\n";
                        }
                    }
                }
                CGSize newSize = GetLabelSize(services, false);
                rowHeight = newSize.Height + 60;
            }
            return rowHeight;
        }

        void GetOperatingHours(ref string day, ref string time)
        {
            for (int i = 0; i < _annotation.KTItem.OpeningHours.Count; i++)
            {
                day += _annotation.KTItem.OpeningHours[i].Title;
                time += _annotation.KTItem.OpeningHours[i].Description;
                int index = i;
                if (index < _annotation.KTItem.OpeningHours.Count - 1)
                {
                    day += "\r\n";
                    time += "\r\n";
                }
            }
        }

        void GetServices(ref string services)
        {
            if (_annotation.is7E)
            {
                services = "•\tPayment of electricity bills and other utility bills";
            }
            else
            {
                for (int i = 0; i < _annotation.KTItem.Services.Count; i++)
                {
                    services += "•\t" + _annotation.KTItem.Services[i].Title;
                    int index = i;
                    if (index < _annotation.KTItem.Services.Count - 1)
                    {
                        services += "\r\n";
                    }
                }
            }
        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        CGSize GetLabelSize(string text, bool hasRightIcon)
        {
            float widthInset = hasRightIcon ? 78F : 36F;
            UILabel label = new UILabel(new CGRect(0, 0, UIApplication.SharedApplication.KeyWindow.Frame.Width - widthInset, 1000));
            label.Font = myTNBFont.MuseoSans12();
            label.Text = text;
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }

        CGSize GetTitleLabelSize(string text)
        {
            UILabel label = new UILabel(new CGRect(0, 0, UIApplication.SharedApplication.KeyWindow.Frame.Width - 36, 1000));
            label.Font = myTNBFont.MuseoSans16();
            label.Lines = 0;
            label.LineBreakMode = UILineBreakMode.WordWrap;
            label.Text = text;
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }
    }
}