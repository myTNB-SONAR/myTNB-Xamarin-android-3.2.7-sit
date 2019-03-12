using Foundation;
using System;
using UIKit;
using myTNB.Home.More.FindUs;
using CoreGraphics;
using myTNB.Home.More.FindUs.LocationDetails;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Home.Components;
using CoreLocation;
using System.Collections.Generic;
using System.Diagnostics;

namespace myTNB
{
    public partial class LocationDetailsViewController : UIViewController
    {
        public LocationDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public string NavigationTitle = string.Empty;
        public AnnotationModel Annotation;

        ActivityIndicatorComponent _activityIndicator;
        UIImageView _imgLocation;

        public override void ViewDidLoad()
        {
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
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.IsSameStoreType = true;
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        void SetTableHeader()
        {
            UIView viewHeader = new UIView(new CGRect(0, 0, View.Frame.Width, 180));
            _imgLocation = new UIImageView(new CGRect(0, 0, View.Frame.Width, 180));
            viewHeader.AddSubview(_imgLocation);
            _activityIndicator = new ActivityIndicatorComponent(viewHeader);
            _activityIndicator.Show();
            locationDetailsTableView.TableHeaderView = viewHeader;
        }

        void AddLocationImage()
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

        void SetTableView()
        {
            SetTableHeader();
            locationDetailsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            locationDetailsTableView.Frame = new CGRect(0, 64, View.Frame.Width, View.Frame.Height - 64);
            locationDetailsTableView.Source = new LocationDetailsDataSource(this, Annotation);
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
                Console.WriteLine(e.Message);
            }
        }

        internal void OpenDirections(AnnotationModel annotation)
        {
            Dictionary<string, string> schemaDictionary = new Dictionary<string, string>();
            string appleMapsSchema = string.Format("maps://?saddr=&daddr={0},{1}&directionsmode=driving"
                                                   , annotation.Coordinate.Latitude
                                                   , annotation.Coordinate.Longitude);
            string googleMapsSchema = string.Format("comgooglemaps://?saddr=&daddr={0},{1}&directionsmode=driving"
                                                   , annotation.Coordinate.Latitude
                                                   , annotation.Coordinate.Longitude);
            string wazeSchema = string.Format("waze://?ll={0},{1}&navigate=yes"
                                                   , annotation.Coordinate.Latitude
                                                   , annotation.Coordinate.Longitude);
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("maps://")))
            {
                schemaDictionary.Add("Maps", appleMapsSchema);
            }
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("comgooglemaps://")))
            {
                schemaDictionary.Add("Google Maps", googleMapsSchema);
            }
            if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("waze://")))
            {
                schemaDictionary.Add("Waze", wazeSchema);
            }
            if (schemaDictionary.Count > 0)
            {
                var mapAlert = UIAlertController.Create("Map Selection", "Select navigation application.", UIAlertControllerStyle.ActionSheet);
                foreach (var schema in schemaDictionary)
                {
                    var action = UIAlertAction.Create("Open in " + schema.Key, UIAlertActionStyle.Default, (obj) =>
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(schema.Value));
                    });
                    mapAlert.AddAction(action);
                }
                var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
                mapAlert.AddAction(cancelAction);
                PresentViewController(mapAlert, animated: true, completionHandler: null);
            }
            else
            {
                var alert = UIAlertController.Create("Warning", "No supported map application is installed.", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, animated: true, completionHandler: null);
            }
        }
    }
}