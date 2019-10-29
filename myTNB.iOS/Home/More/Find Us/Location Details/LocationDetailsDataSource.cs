using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using myTNB.FindUs;
using UIKit;

namespace myTNB.Home.More.FindUs.LocationDetails
{
    public class LocationDetailsDataSource : UITableViewSource
    {
        private LocationDetailsViewController _controller;
        private AnnotationModel _annotation;
        private Dictionary<string, string> _dataDictionary = new Dictionary<string, string>();
        private Func<string, string> GetI18NValue;

        public LocationDetailsDataSource(LocationDetailsViewController controller, AnnotationModel annotation, Func<string, string> getI18NFunc)
        {
            _controller = controller;
            _annotation = annotation;
            GetI18NValue = getI18NFunc;
            PopulateDictionary();
        }

        private void PopulateDictionary()
        {
            _dataDictionary.Clear();
            _dataDictionary.Add("TITLE", _annotation.Title);
            _dataDictionary.Add("ADDRESS", _annotation.Subtitle);
            if (_annotation.is7E)
            {
                _dataDictionary.Add(GetI18NValue(FindUsConstants.I18N_Phone).ToUpper(), _annotation.ConvinientStoreItem.PhoneNumber);
            }
            else
            {
                int phoneCount = 0;
                if (_annotation.KTItem.Phones.Count > 1)
                {
                    foreach (Model.PhoneModel item in _annotation.KTItem.Phones)
                    {
                        phoneCount++;
                        _dataDictionary.Add(string.Format("{0} {1}", GetI18NValue(FindUsConstants.I18N_Phone).ToUpper(), phoneCount), item.PhoneNumber);
                    }
                }
                else
                {
                    _dataDictionary.Add(GetI18NValue(FindUsConstants.I18N_Phone).ToUpper(), _annotation.KTItem.Phones[0].PhoneNumber);
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
                LocationNameViewCell cell = tableView.DequeueReusableCell("LocationNameViewCell", indexPath) as LocationNameViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblName.Text = _annotation.Title;
                CGSize newSize = GetTitleLabelSize(_annotation.Title);
                cell.lblName.Frame = new CGRect(cell.lblName.Frame.X, cell.lblName.Frame.Y
                                                , cell.lblName.Frame.Width, newSize.Height);
                return cell;
            }
            else if (key.ToLower().Equals("address"))
            {
                AddressViewCell cell = tableView.DequeueReusableCell("AddressViewCell", indexPath) as AddressViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = GetI18NValue(FindUsConstants.I18N_Address);
                cell.lblValue.Text = _annotation.Subtitle;
                CGSize newSize = GetLabelSize(cell.lblValue, cell.lblValue.Frame.Width, 1000);
                cell.lblValue.Frame = new CGRect(cell.lblValue.Frame.X, cell.lblValue.Frame.Y
                                                 , cell.lblValue.Frame.Width, newSize.Height);
                cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    _controller.OpenDirections(_annotation);
                }));

                cell.viewDirections.Frame = new CGRect(cell.viewDirections.Frame.X
                    , 30 + ((newSize.Height - cell.viewDirections.Frame.Height) / 2)                    , cell.viewDirections.Frame.Width
                    , cell.viewDirections.Frame.Height);
                return cell;
            }
            else if (key.ToLower().Contains(GetI18NValue(FindUsConstants.I18N_Phone).ToLower()))
            {
                ContactUsViewCell cell = tableView.DequeueReusableCell("ContactUsViewCell", indexPath) as ContactUsViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = GetI18NValue(FindUsConstants.I18N_Phone).ToUpper();
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
                OpeningHoursViewCell cell = tableView.DequeueReusableCell("OpeningHoursViewCell", indexPath) as OpeningHoursViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = GetI18NValue(FindUsConstants.I18N_OpeningHours);
                if (_annotation.is7E)
                {
                    cell.lbl7EOperation.Text = GetI18NValue(FindUsConstants.I18N_OperationHours);
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
                ServicesViewCell cell = tableView.DequeueReusableCell("ServicesViewCell", indexPath) as ServicesViewCell;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.lblTitle.Text = GetI18NValue(FindUsConstants.I18N_Services);
                cell.AddSubview(RenderServicesListCell());
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
                    services += GetI18NValue(FindUsConstants.I18N_ServiceDescription);
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
        /// <summary>
        /// Renders the services list cell.
        /// </summary>
        /// <returns>The services list cell.</returns>
        public UIView RenderServicesListCell()
        {
            nfloat lblYPos = 0f;
            nfloat lblHeight = 14.4f;
            nfloat lblXPos = 10f;
            UIView viewContent = new UIView(new CGRect(18, 30, 282, 300));

            for (int i = 0; i < _annotation.KTItem.Services.Count; i++)
            {
                string str = _annotation.KTItem.Services[i].Title;
                CGSize strSize = GetLabelSize(str, false);

                UIView innerView = new UIView(new CGRect(0, lblYPos, 282, strSize.Height));

                UILabel lblDot = new UILabel(new CGRect(lblXPos, 0, 5, lblHeight));
                lblDot.TextColor = MyTNBColor.TunaGrey();
                lblDot.Font = MyTNBFont.MuseoSans12_300;
                lblDot.Text = "•";

                UILabel lblVal = new UILabel(new CGRect(lblXPos + 15, 0, innerView.Frame.Width, strSize.Height));
                lblVal.TextColor = MyTNBColor.TunaGrey();
                lblVal.Font = MyTNBFont.MuseoSans12_300;
                lblVal.LineBreakMode = UILineBreakMode.WordWrap;
                lblVal.Lines = 0;
                lblVal.Text = str;

                innerView.AddSubviews(lblDot, lblVal);
                viewContent.AddSubview(innerView);

                lblYPos += strSize.Height;
            }

            return viewContent;
        }

        private void GetOperatingHours(ref string day, ref string time)
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

        private void GetServices(ref string services)
        {
            if (_annotation.is7E)
            {
                services = GetI18NValue(FindUsConstants.I18N_ServiceDescription);
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

        private CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        private CGSize GetLabelSize(string text, bool hasRightIcon)
        {
            float widthInset = hasRightIcon ? 78F : 36F;
            UILabel label = new UILabel(new CGRect(0, 0, UIApplication.SharedApplication.KeyWindow.Frame.Width - widthInset, 1000));
            label.Font = MyTNBFont.MuseoSans12;
            label.Text = text;
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }

        private CGSize GetTitleLabelSize(string text)
        {
            UILabel label = new UILabel(new CGRect(0, 0, UIApplication.SharedApplication.KeyWindow.Frame.Width - 36, 1000));
            label.Font = MyTNBFont.MuseoSans16;
            label.Lines = 0;
            label.LineBreakMode = UILineBreakMode.WordWrap;
            label.Text = text;
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }
    }
}