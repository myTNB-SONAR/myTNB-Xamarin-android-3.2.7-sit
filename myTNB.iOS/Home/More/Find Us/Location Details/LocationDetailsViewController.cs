using Foundation;
using System;
using UIKit;
using myTNB.Home.More.FindUs;
using CoreGraphics;
using myTNB.Home.More.FindUs.LocationDetails;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Home.Components;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.FindUs;

namespace myTNB
{
    public partial class LocationDetailsViewController : CustomUIViewController
    {
        public LocationDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public string NavigationTitle = string.Empty;
        public AnnotationModel Annotation;

        private ActivityIndicatorComponent _activityIndicator;
        private UIImageView _imgLocation;

        public override void ViewDidLoad()
        {
            PageName = FindUsConstants.Pagename_LocationDetails;
            base.ViewDidLoad();
            ActivityIndicator.Show();
            SetNavigationBar();
            SetTableView();
            ActivityIndicator.Hide();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AddLocationImage();
        }

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(NavigationTitle);
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.IsSameStoreType = true;
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        private void SetTableHeader()
        {
            UIView viewHeader = new UIView(new CGRect(0, 0, View.Frame.Width, 180));
            _imgLocation = new UIImageView(new CGRect(0, 0, View.Frame.Width, 180));
            viewHeader.AddSubview(_imgLocation);
            _activityIndicator = new ActivityIndicatorComponent(viewHeader);
            _activityIndicator.Show();
            locationDetailsTableView.TableHeaderView = viewHeader;
        }

        private void AddLocationImage()
        {
            string imgPath = string.Empty;
            if ((bool)Annotation?.is7E)
            {
                if (DataManager.DataManager.SharedInstance.LocationTypes != null && DataManager.DataManager.SharedInstance.LocationTypes?.Count > 0)
                {
                    int index = DataManager.DataManager.SharedInstance.LocationTypes.FindIndex(x => x.Title.Equals("7E"));
                    if (index > -1)
                    {
                        if (!string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.LocationTypes[index]?.ImagePath)
                           && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.LocationTypes[index]?.ImagePath))
                        {
                            imgPath = DataManager.DataManager.SharedInstance.LocationTypes[index].ImagePath;
                        }
                    }
                }
            }
            else
            {
                if (Annotation?.KTItem != null
                   && !string.IsNullOrEmpty(Annotation?.KTItem.ImagePath)
                   && !string.IsNullOrWhiteSpace(Annotation?.KTItem.ImagePath))
                {
                    imgPath = Annotation?.KTItem.ImagePath;
                }
                else
                {
                    if (DataManager.DataManager.SharedInstance.LocationTypes != null && DataManager.DataManager.SharedInstance.LocationTypes?.Count > 0)
                    {
                        int index = DataManager.DataManager.SharedInstance.LocationTypes.FindIndex(x => x.Title.Equals("KT"));
                        if (index > -1)
                        {
                            if (!string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.LocationTypes[index]?.ImagePath)
                            && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.LocationTypes[index]?.ImagePath))
                            {
                                imgPath = DataManager.DataManager.SharedInstance.LocationTypes[index].ImagePath;
                            }
                        }
                    }
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(imgPath))
                {
                    NSUrl url = new NSUrl(imgPath);
                    NSUrlSession session = NSUrlSession
                        .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                    NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                    {
                        if (error == null && response != null && data != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                _imgLocation.Image = UIImage.LoadFromData(data);
                                _activityIndicator.Hide();
                            });

                        }
                    });
                    dataTask.Resume();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine("Error: " + err.Message);
            }
        }

        private void SetTableView()
        {
            SetTableHeader();
            locationDetailsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            locationDetailsTableView.Frame = new CGRect(0, 64, View.Frame.Width, View.Frame.Height - 64);
            locationDetailsTableView.Source = new LocationDetailsDataSource(this, Annotation, GetI18NValue);
            locationDetailsTableView.ReloadData();
        }

        internal void CallNumber(string number)
        {
            try
            {
                if (!string.IsNullOrEmpty(number))
                {
                    NSUrl url = new NSUrl(new Uri("tel:" + number).AbsoluteUri);
                    UIApplication.SharedApplication.OpenUrl(url);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        internal void OpenDirections(AnnotationModel annotation)
        {
            Dictionary<string, string> schemaDictionary = new Dictionary<string, string>();
            string appleMapsSchema = string.Format(FindUsConstants.Schema_Apple
                , annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            string googleMapsSchema = string.Format(FindUsConstants.Schema_Google
                , annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            string wazeSchema = string.Format(FindUsConstants.Schema_Waze
                , annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(FindUsConstants.URL_Apple)))
            {
                schemaDictionary.Add("Maps", appleMapsSchema);
            }
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(FindUsConstants.URL_Google)))
            {
                schemaDictionary.Add("Google Maps", googleMapsSchema);
            }
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(FindUsConstants.URL_Waze)))
            {
                schemaDictionary.Add("Waze", wazeSchema);
            }
            if (schemaDictionary.Count > 0)
            {
                UIAlertController mapAlert = UIAlertController.Create(GetI18NValue(FindUsConstants.I18N_MapSelection)
                    , GetI18NValue(FindUsConstants.I18N_SelectApplication), UIAlertControllerStyle.ActionSheet);
                foreach (KeyValuePair<string, string> schema in schemaDictionary)
                {
                    UIAlertAction action = UIAlertAction.Create(string.Format("{0} {1}", GetI18NValue(FindUsConstants.I18N_OpenIn), schema.Key)
                        , UIAlertActionStyle.Default, (obj) =>
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(schema.Value));
                    });
                    mapAlert.AddAction(action);
                }
                UIAlertAction cancelAction = UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, null);
                mapAlert.AddAction(cancelAction);
                mapAlert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(mapAlert, animated: true, completionHandler: null);
            }
            else
            {
                DisplayGenericAlert(GetErrorI18NValue(Constants.Error_DefaultErrorTitle), GetI18NValue(FindUsConstants.I18N_NoSupportedApp));
            }
        }
    }
}