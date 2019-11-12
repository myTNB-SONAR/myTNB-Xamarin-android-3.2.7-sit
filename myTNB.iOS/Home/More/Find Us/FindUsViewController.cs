using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using CoreAnimation;
using MapKit;
using CoreLocation;
using System.Collections.Generic;
using System.Linq;
using myTNB.Home.More.FindUs;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.FindUs;

namespace myTNB
{
    public partial class FindUsViewController : CustomUIViewController
    {
        public FindUsViewController(IntPtr handle) : base(handle) { }

        private CLLocationManager _locationManager;
        private TextFieldHelper _textFieldHelper = new TextFieldHelper();
        private UIView _viewLocation;
        private MKMapView _mapView;
        private UILabel _lblType;

        private List<MKMapItem> _convinientStoreList = new List<MKMapItem>();
        private GetLocationsResponseModel _locations = new GetLocationsResponseModel();

        private CLAuthorizationStatus _locStatus = CLAuthorizationStatus.NotDetermined;

        private string _searchLoc = string.Empty;
        private bool _ktSearchDone, _711SearchDone;

        public override void ViewDidLoad()
        {
            PageName = FindUsConstants.Pagename_FindUs;
            base.ViewDidLoad();
            SetNavigationBar();
            AddSubViews();
            SetMapView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!DataManager.DataManager.SharedInstance.isLocationSearch
                && !DataManager.DataManager.SharedInstance.IsSameStoreType)
            {
                if (DataManager.DataManager.SharedInstance.LocationTypes != null
                    && DataManager.DataManager.SharedInstance.LocationTypes?.Count > 0)
                {
                    if (DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex < DataManager.DataManager.SharedInstance.LocationTypes.Count)
                    {
                        _lblType.Text = DataManager.DataManager.SharedInstance.LocationTypes
                        [DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex].Description;
                    }
                }
                else
                {
                    _lblType.Text = TNBGlobal.STORE_TYPE_LIST[DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex];
                }
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ShowStoreLocations();
        }

        private void ShowStoreLocations()
        {
            if (!DataManager.DataManager.SharedInstance.isLocationSearch
                && !DataManager.DataManager.SharedInstance.IsSameStoreType)
            {
                _mapView.RemoveAnnotations(_mapView.Annotations);
                if (_locStatus != CLAuthorizationStatus.Denied
                    && _locStatus != CLAuthorizationStatus.NotDetermined)
                {

                    ActivityIndicator.Show();
                    _711SearchDone = true;
                    DisplayKTLocations(false);
                    /*if (DataManager.DataManager.SharedInstance.SelectedLocationTypeTitle == "All")
                    {
                        ActivityIndicator.Show();
                        DisplayKTLocations(false);
                        DisplayConvinietStoreLocations(false);
                    }
                    else if (DataManager.DataManager.SharedInstance.SelectedLocationTypeTitle == "KT")
                    {
                        ActivityIndicator.Show();
                        _711SearchDone = true;
                        DisplayKTLocations(false);
                    }
                    else if (DataManager.DataManager.SharedInstance.SelectedLocationTypeTitle == "7E")
                    {
                        ActivityIndicator.Show();
                        _ktSearchDone = true;
                        DisplayConvinietStoreLocations(true);
                    }*/
                }
            }
            else
            {
                if (_locStatus == CLAuthorizationStatus.Denied
                    || _locStatus == CLAuthorizationStatus.NotDetermined)
                {
                    _mapView.RemoveAnnotations(_mapView.Annotations);
                }
            }
        }

        private void DisplayKTLocations(bool isSearch)
        {
            GetLocations(isSearch).ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_locations != null && _locations?.d != null && _locations?.d?.data != null)
                    {
                        if (_locations.d.data.Any())
                        {
                            foreach (var item in _locations.d.data)
                            {
                                AnnotationModel annotation = new AnnotationModel(
                                    new CLLocationCoordinate2D((double)item?.Latitude, (double)item?.Longitude)
                                    , item?.Title
                                    , item?.Address
                                )
                                {
                                    is7E = false,
                                    KTItem = item
                                };
                                _mapView.AddAnnotation(annotation);
                            }
                            ReCenterMap(_locations.d.data[0].Latitude, _locations.d.data[0].Longitude);
                        }
                        else
                        {
                            DisplayGenericAlert(GetI18NValue(FindUsConstants.I18N_ZeroLocations), GetI18NValue(FindUsConstants.I18N_NoKTFound));
                        }
                    }
                    else
                    {
                        DisplayGenericAlert(GetI18NValue(FindUsConstants.I18N_ZeroLocations), GetI18NValue(FindUsConstants.I18N_NoKTFound));
                    }
                    _ktSearchDone = true;
                    HideActivityIndicator();
                });
            });
        }

        private void DisplayConvinietStoreLocations(bool isRecenter)
        {
            MKLocalSearchRequest searchRequest = new MKLocalSearchRequest();
            searchRequest.NaturalLanguageQuery = "7 Eleven";
            //searchRequest.Region = new MKCoordinateRegion();
            _convinientStoreList = new List<MKMapItem>();
            MKLocalSearch localSearch = new MKLocalSearch(searchRequest);
            localSearch.Start(delegate (MKLocalSearchResponse response, NSError error)
            {
                if (response != null && error == null)
                {
                    _convinientStoreList = response.MapItems.ToList();
                    foreach (MKMapItem item in _convinientStoreList)
                    {
                        AnnotationModel annotation = new AnnotationModel(
                            new CLLocationCoordinate2D(
                                item.Placemark.Coordinate.Latitude
                                , item.Placemark.Coordinate.Longitude
                            )
                            , item.Name
                            , item.Placemark.Title
                        )
                        {
                            is7E = true,
                            ConvinientStoreItem = item
                        };
                        _mapView.AddAnnotation(annotation);
                    }
                    if (isRecenter && _convinientStoreList != null && _convinientStoreList?.Count > 0)
                    {
                        ReCenterMap((double)_convinientStoreList[0].Placemark.Coordinate.Latitude
                            , (double)_convinientStoreList[0].Placemark.Coordinate.Longitude);
                    }
                }
                else
                {
                    DisplayGenericAlert(GetI18NValue(FindUsConstants.I18N_ZeroLocations), GetI18NValue(FindUsConstants.I18N_No711Found));
                }
                _711SearchDone = true;
                HideActivityIndicator();
            });
        }

        private void HideActivityIndicator()
        {
            if (_ktSearchDone && _711SearchDone)
            {
                _ktSearchDone = false;
                _711SearchDone = false;
                ActivityIndicator.Hide();
            }
        }

        private void AddSubViews()
        {
            UIView viewSearch = new UIView(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 51));
            UITextField txtFieldSearch = new UITextField
            {
                Frame = new CGRect(0, 12, viewSearch.Frame.Width, 24)
                ,
                AttributedPlaceholder = new NSAttributedString(
                    GetI18NValue(FindUsConstants.I18N_SearchPlaceholder)
                    , font: MyTNBFont.MuseoSans16
                    , foregroundColor: MyTNBColor.SilverChalice
                    , strokeWidth: 0
                )
                ,
                TextColor = MyTNBColor.TunaGrey()
            };
            _textFieldHelper.CreateTextFieldLeftView(txtFieldSearch, "IC-Field-Search");
            _textFieldHelper.SetKeyboard(txtFieldSearch);
            txtFieldSearch.ReturnKeyType = UIReturnKeyType.Search;
            UIView viewLineSearch = GenericLine.GetLine(new CGRect(0, 36, viewSearch.Frame.Width, 1));

            txtFieldSearch.ShouldReturn += (textField) =>
            {
                ((UITextField)textField).ResignFirstResponder();

                _searchLoc = ((UITextField)textField).Text;
                if (!string.IsNullOrEmpty(_searchLoc) && !string.IsNullOrWhiteSpace(_searchLoc))
                {
                    ExecuteLocationSearch();
                }
                return false;
            };
            txtFieldSearch.EditingDidBegin += (sender, e) =>
            {
                viewLineSearch.BackgroundColor = MyTNBColor.PowerBlue;
                txtFieldSearch.LeftViewMode = UITextFieldViewMode.Never;
            };
            txtFieldSearch.EditingDidEnd += (sender, e) =>
            {
                viewLineSearch.BackgroundColor = MyTNBColor.PlatinumGrey;
                if (txtFieldSearch.Text.Length == 0)
                    txtFieldSearch.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
            viewSearch.AddSubviews(new UIView[] { txtFieldSearch, viewLineSearch });

            UIView viewShow = new UIView(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 160 : 138, View.Frame.Width - 36, 51));
            UILabel lblShow = new UILabel(new CGRect(0, 0, viewShow.Frame.Width, 12))
            {
                Text = GetI18NValue(FindUsConstants.I18N_Show).ToUpper(),
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans9
            };

            _lblType = new UILabel(new CGRect(0, 12, viewShow.Frame.Width, 24))
            {
                Text = GetCommonI18NValue(Constants.Common_All),
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans16
            };

            UIImageView imgDropDown = new UIImageView(new CGRect(viewShow.Frame.Width - 30, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };

            UIView viewLineShow = new UIView(new CGRect(0, 36, viewShow.Frame.Width, 1))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };

            viewShow.AddSubviews(new UIView[] { lblShow, _lblType, imgDropDown, viewLineShow });

            viewShow.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("SelectStoreType", null);
                SelectStoreTypeViewController viewController =
                    storyBoard.InstantiateViewController("SelectStoreTypeViewController") as SelectStoreTypeViewController;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }));

            View.AddSubviews(new UIView[] { viewSearch });
        }

        private bool IsConvinientStoreSearch
        {
            get
            {
                /*foreach (string item in TNBGlobal.CONVINIENT_STORE_LIST)
                {
                    if (_searchLoc.ToLower().Contains(item))
                    {
                        return true;
                    }
                }*/
                return false;
            }
        }

        private void ExecuteLocationSearch()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.isLocationSearch = true;
                        _mapView.RemoveAnnotations(_mapView.Annotations);
                        ActivityIndicator.Show();
                        if (IsConvinientStoreSearch)
                        {
                            _ktSearchDone = true;
                            DisplayConvinietStoreLocations(true);
                        }
                        else
                        {
                            _711SearchDone = true;
                            DisplayKTLocations(true);
                        }
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(FindUsConstants.I18N_NavTitle));
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.isLocationSearch = false;
                DataManager.DataManager.SharedInstance.CurrentStoreTypeIndex = 0;
                DataManager.DataManager.SharedInstance.PreviousStoreTypeIndex = 0;
                DataManager.DataManager.SharedInstance.SelectedLocationTypeID = "all";
                DataManager.DataManager.SharedInstance.SelectedLocationTypeTitle = GetCommonI18NValue(Constants.Common_All);
                DataManager.DataManager.SharedInstance.IsSameStoreType = false;
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        private void SetMapView()
        {
            _mapView = new MKMapView(new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 160
                : 138, View.Frame.Width, View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 162 : 137)))
            {
                ShowsCompass = false,
                GetViewForAnnotation = GetViewForAnnotation
            };
            SetUserLocation();
            CreateLocationIcon();
            View.AddSubview(_mapView);
        }

        private void CreateLocationIcon()
        {
            _viewLocation = new UIView(new CGRect(_mapView.Frame.Width - 67, _mapView.Frame.Height - 75, 50, 50));

            UIImageView imgLocation = new UIImageView(new CGRect(13, 13, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Button-Map-Center")
            };

            UIBezierPath path = new UIBezierPath();
            path.AddArc(new CGPoint(25, 25), 24, 0, (nfloat)Math.PI * 2, true);
            CAShapeLayer shapeLayer = new CAShapeLayer
            {
                Path = path.CGPath,
                FillColor = UIColor.White.CGColor
            };

            _viewLocation.Layer.AddSublayer(shapeLayer);
            _viewLocation.AddSubview(imgLocation);
            _viewLocation.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                SetUserLocation();
            }));
            _mapView.AddSubview(_viewLocation);
        }

        private void ReCenterMap(double lat, double lon)
        {
            CLLocationCoordinate2D coords = new CLLocationCoordinate2D(lat, lon);
            MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(1), MilesToLongitudeDegrees(1, coords.Latitude));
            _mapView.Region = new MKCoordinateRegion(coords, span);
        }

        private void SetUserLocation()
        {
            _mapView.ShowsUserLocation = true;
            _locationManager = new CLLocationManager();
            _locationManager.AuthorizationChanged += (sender, e) =>
            {
                _locStatus = e.Status;
                if (e.Status == CLAuthorizationStatus.Authorized
                   || e.Status == CLAuthorizationStatus.AuthorizedWhenInUse
                   || e.Status == CLAuthorizationStatus.AuthorizedAlways)
                {
                    if (_locationManager != null && _locationManager?.Location != null)
                    {
                        CLLocationCoordinate2D coords = new CLLocationCoordinate2D(_locationManager.Location.Coordinate.Latitude, _locationManager.Location.Coordinate.Longitude);
                        MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(1), MilesToLongitudeDegrees(1, coords.Latitude));
                        _mapView.Region = new MKCoordinateRegion(coords, span);
                    }
                }
                else if (e.Status == CLAuthorizationStatus.NotDetermined
                        || e.Status == CLAuthorizationStatus.Denied)
                {
                    _locationManager.RequestWhenInUseAuthorization();
                }
            };
        }

        private double MilesToLatitudeDegrees(double miles)
        {
            double earthRadius = 3960.0; // in miles
            double radiansToDegrees = 180.0 / Math.PI;
            return (miles / earthRadius) * radiansToDegrees;
        }

        private double MilesToLongitudeDegrees(double miles, double atLatitude)
        {
            double earthRadius = 3960.0; // in miles
            double degreesToRadians = Math.PI / 180.0;
            double radiansToDegrees = 180.0 / Math.PI;
            // derive the earth's radius at that point in latitude
            double radiusAtLatitude = earthRadius * Math.Cos(atLatitude * degreesToRadians);
            return (miles / radiusAtLatitude) * radiansToDegrees;
        }

        private MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;
            if (annotation is MKUserLocation)
            {
                return null;
            }
            if (annotation is AnnotationModel)
            {
                annotationView = mapView.DequeueReusableAnnotation("annotationIdentifier");
                if (annotationView == null)
                {
                    annotationView = new MKAnnotationView(annotation, "annotationIdentifier");
                }
                annotationView.Image = UIImage.FromBundle(((AnnotationModel)annotation).is7E
                    ? "Map-Convenient-Store" : "Map-Kedai-Tenaga");
                annotationView.CanShowCallout = false;
                string title = ((AnnotationModel)annotation).is7E ? "7-Eleven" : "Kedai Tenaga";
                annotationView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("LocationDetails", null);
                    LocationDetailsViewController viewController =
                        storyBoard.InstantiateViewController("LocationDetailsViewController") as LocationDetailsViewController;
                    viewController.NavigationTitle = title;
                    viewController.Annotation = (AnnotationModel)annotation;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }));
            }
            return annotationView;
        }

        private Task GetLocations(bool isSearch)
        {
            return Task.Factory.StartNew(() =>
            {
                string locType = "KT";
                string latt = _locationManager?.Location.Coordinate.Latitude.ToString() ?? string.Empty;
                string longt = _locationManager?.Location.Coordinate.Longitude.ToString() ?? string.Empty;

                if (!isSearch)
                {
                    locType = DataManager.DataManager.SharedInstance.SelectedLocationTypeTitle == "All"
                        ? "KT" : DataManager.DataManager.SharedInstance.SelectedLocationTypeTitle;
                }
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    latitude = latt,//"3.1365952399077304",//
                    longitude = longt,//"101.69228553771973",/
                    locationType = locType,
                    keyword = isSearch ? _searchLoc : string.Empty
                };
                _locations = serviceManager.OnExecuteAPIV6<GetLocationsResponseModel>(isSearch
                    ? FindUsConstants.Service_GetLocationsByKeyword : FindUsConstants.Service_GetLocations, requestParameter);
            });
        }
    }
}