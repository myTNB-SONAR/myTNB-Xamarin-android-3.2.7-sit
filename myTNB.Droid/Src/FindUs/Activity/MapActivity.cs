using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Tasks;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Runtime;


using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using Java.Lang;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FindUs.Models;
using myTNB_Android.Src.FindUs.MVP;
using myTNB_Android.Src.FindUs.Response;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace myTNB_Android.Src.FindUs.Activity
{
    [Activity(Label = "Find Us"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class MapActivity : BaseActivityCustom, IOnMapReadyCallback, GoogleMap.IOnMyLocationButtonClickListener, FindUsContract.IView, Android.Gms.Tasks.IOnSuccessListener, Android.Gms.Tasks.IOnFailureListener, Android.Locations.ILocationListener
    {
        private readonly string TAG = "FindUSActivity";
        private readonly int SELECT_LOCATION_TYPE_CODE = 3410;
        private readonly string KEDAI_TENAGA = "Kedai Tenaga";

        private string GoogelApiKey;
        private FindUsPresenter mPresenter;
        private FindUsContract.IUserActionsListener userActionsListener;

        bool alreadyAskedPermission = false;

        private GoogleMap mMap;
        private MapFragment mapFragment;
        //private double mCurrentLat;
        //private double mCurrentLng;
        private string mLocationDescription = "7-Eleven";

        private List<LocationType> locationTypes = new List<LocationType>();
        private LocationType selectedLocationType;

        private LinearLayout rootView;
        private AlertDialog mNoLocationsDialog;
        private Snackbar mErrorMessageSnackBar;

        private LocationManager _locationManager;
        private string _locationProvider;
        private Location _currentLocation;

        // flag for GPS Status
        bool isGPSEnabled = false;

        // flag for network status
        bool isNetworkEnabled = false;

        bool canGetLocation = false;

        // The minimum distance to change updates in metters
        private static readonly long MIN_DISTANCE_CHANGE_FOR_UPDATES = 0;

        // The minimum time beetwen updates in milliseconds
        private static readonly long MIN_TIME_BW_UPDATES = 0;

        private AlertDialog mLocationDialog;

        private List<GoogleApiResult> googleLocationsResponse = new List<GoogleApiResult>();
        private List<LocationData> locationsResponse = new List<LocationData>();
        private List<MarkerOptions> mMarkers = new List<MarkerOptions>();

        [BindView(Resource.Id.selector_location_type)]
        TextView selectorLocationType;

        [BindView(Resource.Id.search_layout)]
        TextInputLayout txtSearch;

        [BindView(Resource.Id.search_edittext)]
        EditText edtSearch;

        [BindView(Resource.Id.progressBar)]
        ProgressBar progressBar;

        const string PAGE_ID = "FindUs";

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;

            if (_currentLocation != null)
            {
                SetCurrentLoation(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude));
                userActionsListener.GetLocations(Constants.APP_CONFIG.API_KEY_ID, GoogelApiKey, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString(), "ALL", mLocationDescription);
            }

            if (mMap != null)
            {
                mMap.MarkerClick += GMapMarkerClick;
                mMap.InfoWindowClick += GMapInfoWindowClick;
            }

        }

        public override int ResourceId()
        {
            return Resource.Layout.FIndUsView;
        }

        public void SetPresenter(FindUsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowGetLocationsDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowGetLocationsError(string message)
        {
            if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            {
                mErrorMessageSnackBar.Dismiss();
            }

            mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mErrorMessageSnackBar.Dismiss(); }
            );
            View v = mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mErrorMessageSnackBar.Show();
        }

        public void ShowGetLocationsSuccess(GetLocationsResponse response, GetGoogleLocationsResponse results)
        {
            if (mMap != null)
            {
                mMap.Clear();
                mMarkers.Clear();
                mMap.MarkerClick += null;
                mMap.InfoWindowClick += null;
            }
            locationsResponse.Clear();
            googleLocationsResponse.Clear();
            if (response.D != null && response.D.LocationList != null)
            {
                locationsResponse = response.D.LocationList;
                if (response.D.LocationList.Count > 0)
                {
                    foreach (LocationData item in response.D.LocationList)
                    {
                        BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_button_map_pin_kedai_tenaga);
                        MarkerOptions markerOptions = new MarkerOptions();
                        LatLng location = new LatLng(item.Latitude, item.Longitude);
                        markerOptions.SetPosition(location);
                        markerOptions.SetTitle(item.Title);
                        markerOptions.SetIcon(icon);

                        mMap.AddMarker(markerOptions);
                        mMarkers.Add(markerOptions);
                    }
                }
                else
                {
                    if (selectedLocationType.Title.Equals("KI"))
                    {
                        mNoLocationsDialog.Show();
                    }
                }
            }

            //if(results.results != null)
            //{
            //    googleLocationsResponse = results.results;
            //    if (results.results.Count > 0)
            //    {
            //        foreach (GoogleApiResult item in results.results)
            //        {
            //            BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_button_map_pin_convenient_store);
            //            MarkerOptions markerOptions = new MarkerOptions();
            //            LatLng location = new LatLng(item.geometry.location.lat, item.geometry.location.lng);
            //            markerOptions.SetPosition(location);
            //            markerOptions.SetTitle(item.name);
            //            markerOptions.SetIcon(icon);


            //            mMap.AddMarker(markerOptions);
            //            mMarkers.Add(markerOptions);
            //        }
            //    }
            //    else
            //    {
            //        if (selectedLocationType.Title.Equals("7E"))
            //        {
            //            mNoLocationsDialog.Show();
            //        }
            //    }
            //}

            //Recenter map to first searched location
            if (locationsResponse.Count == 0 && googleLocationsResponse.Count == 0)
            {
                mNoLocationsDialog.Show();
            }
            else
            {
                if (locationsResponse.Count > 0)
                {
                    LocationData data = locationsResponse[0];
                    LatLng newLocation = new LatLng(data.Latitude, data.Longitude);
                    SetCurrentLoation(newLocation);
                }
                //else if (googleLocationsResponse.Count > 0)
                //{
                //    GoogleApiResult result = googleLocationsResponse[0];
                //    LatLng newLocation = new LatLng(result.geometry.location.lat, result.geometry.location.lng);
                //    SetCurrentLoation(newLocation);
                //}
            }

        }


        public void SetCurrentLoation(LatLng currentLocation)
        {

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(currentLocation);
            builder.Zoom(16);
            //builder.Bearing(155);
            //builder.Tilt(65);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);


            mMap.UiSettings.ZoomControlsEnabled = false;
            mMap.UiSettings.CompassEnabled = true;
            mMap.UiSettings.MyLocationButtonEnabled = true;
            View locationButton = mapFragment.View.FindViewById(2);
            if (locationButton != null)
            {
                // and next place it, for exemple, on bottom right (as Google Maps app)
                RelativeLayout.LayoutParams rlp = (RelativeLayout.LayoutParams)locationButton.LayoutParameters;
                // position on right bottom
                rlp.AddRule(LayoutRules.AlignParentTop, 0);
                rlp.AddRule(LayoutRules.AlignParentBottom, 1);
                rlp.SetMargins(0, 0, 30, 30);
            }

            mMap.MyLocationEnabled = true;
            mMap.MoveCamera(cameraUpdate);
            mMap.SetOnMyLocationButtonClickListener(this);

        }

        public void HideGetLocationsDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            mPresenter = new FindUsPresenter(this);
            GoogelApiKey = GetString(Resource.String.google_maps_search_api_key);

            rootView = (LinearLayout)FindViewById(Resource.Id.rootView);
            mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map_view);

            mapFragment.GetMapAsync(this);

            //mGetLocationsDialog = new AlertDialog.Builder(this)
            //   .SetTitle("Loading..")
            //   .SetMessage("Please wait while we are fetching location details")
            //   .SetCancelable(false)
            //   .Create();

            mNoLocationsDialog = new AlertDialog.Builder(this)
               .SetTitle(GetLabelByLanguage("zeroLocations"))
               .SetMessage(GetLabelByLanguage("noKTFound"))
               .SetCancelable(true)
                .SetPositiveButton(Utility.GetLocalizedCommonLabel("ok"), (senderAlert, args) =>
                {
                    try
                    {
                        LoadingOverlayUtils.OnStopLoadingAnimation(this);
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                })
               .Create();

            _locationManager = (LocationManager)GetSystemService(LocationService);

            TextViewUtils.SetMuseoSans300Typeface(selectorLocationType);
            TextViewUtils.SetMuseoSans300Typeface(edtSearch);
            TextViewUtils.SetMuseoSans300Typeface(txtSearch);
            TextViewUtils.SetTextSize12(edtSearch);
            TextViewUtils.SetTextSize16(selectorLocationType);
            txtSearch.Hint = GetLabelByLanguage("searchPlaceholder");
            txtSearch.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayout_TextAppearance_Large
                : Resource.Style.TextInputLayout_TextAppearance_Small);
            if (LocationTypesEntity.HasRecord())
            {
                locationTypes = LocationTypesEntity.GetLocationTypes();
                if (locationTypes.Count > 0)
                {
                    int index = 0;
                    foreach (LocationType type in locationTypes)
                    {
                        if (type.Id.Equals("1"))
                        {
                            selectorLocationType.Text = type.Description;
                            selectedLocationType = type;
                        }
                    }
                }
            }

            selectorLocationType.Click += delegate
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    Intent accountType = new Intent(this, typeof(SelectLocationTypeActivity));
                    accountType.PutExtra("selectedLocationType", JsonConvert.SerializeObject(selectedLocationType));
                    StartActivityForResult(accountType, SELECT_LOCATION_TYPE_CODE);
                }
            };

            edtSearch.EditorAction += (sender, e) =>
            {
                e.Handled = false;
                if (e.ActionId == ImeAction.Search || e.ActionId == ImeAction.Done)
                {
                    if (_currentLocation != null)
                    {
                        string s = edtSearch.Text;
                        this.userActionsListener.GetLocationsByKeyword(Constants.APP_CONFIG.API_KEY_ID, GoogelApiKey, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString(), selectedLocationType.Title, selectedLocationType.Description, s);
                    }

                    InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);

                    inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

                    e.Handled = true;
                }
            };
        }

        /// <summary>
        /// Initialize location manager
        /// detect last location of a user : get best location provider to detect correct location
        /// </summary>
        public void OnLoad()
        {
            if (IsNetworkAvailable())
            {
                try
                {
                    Android.Gms.Location.LocationRequest locationRequest = Android.Gms.Location.LocationRequest.Create();
                    locationRequest.SetPriority(Android.Gms.Location.LocationRequest.PriorityHighAccuracy);
                    LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder()
                                  .AddLocationRequest(locationRequest);

                    LocationServices.GetSettingsClient(this).CheckLocationSettings(builder.Build())
                        .AddOnFailureListener(this)
                        .AddOnSuccessListener(this);

                }
                catch (System.Exception e)
                {
                    // e.printStackTrace();
                    Log.Error("Error : Location",
                            "Impossible to connect to LocationManager", e);
                    Utility.LoggingNonFatalError(e);
                }


            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Locations");
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        private void GMapMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            Marker marker = e.Marker;
            //marker.ShowInfoWindow();
            if (marker != null)
            {
                FindClickedItem(new LatLng(marker.Position.Latitude, marker.Position.Longitude));
            }
        }

        private void GMapInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {

        }

        /// <summary>
        /// Find clicked location from the map : detect weather user has clicked KT location or any other location
        /// </summary>
        public void FindClickedItem(LatLng clickedItem)
        {
            try
            {
                LocationData locationData = null;
                GoogleApiResult googleApiResult = null;

                locationData = locationsResponse.Find(x => x.Latitude == clickedItem.Latitude);
                googleApiResult = googleLocationsResponse.Find(y => y.geometry.location.lat == clickedItem.Latitude);
                if (locationData != null)
                {
                    Intent detailsView = new Intent(this, typeof(LocationDetailsActivity));
                    detailsView.PutExtra("Title", KEDAI_TENAGA);
                    detailsView.PutExtra("KT", JsonConvert.SerializeObject(locationData));
                    detailsView.PutExtra("imagePath", selectedLocationType.ImagePath);
                    StartActivity(detailsView);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }
            //else if (googleApiResult != null)
            //{
            //    Intent detailsView = new Intent(this, typeof(LocationDetailsActivity));
            //    LocationTypesEntity entity = LocationTypesEntity.GetById("2");
            //    detailsView.PutExtra("Title", entity.Description);
            //    detailsView.PutExtra("store", JsonConvert.SerializeObject(googleApiResult));
            //    detailsView.PutExtra("imagePath", entity.ImagePath);
            //    StartActivity(detailsView);
            //}

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == Android.Gms.Location.LocationRequest.PriorityHighAccuracy)
                {
                    if (resultCode == Result.Ok)
                    {
                        alreadyAskedPermission = true;

                        _ = System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
                        {
                            RunOnUiThread(() =>
                            {
                                OnRequestCheckLocation();
                            });

                            _ = System.Threading.Tasks.Task.Delay(100).ContinueWith((run) =>
                            {
                                RunOnUiThread(() =>
                                {
                                    if (_currentLocation != null)
                                    {
                                        SetCurrentLoation(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude));
                                        userActionsListener.GetLocations(Constants.APP_CONFIG.API_KEY_ID, GoogelApiKey, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString(), "ALL", mLocationDescription);
                                    }
                                });
                            });
                        });
                    }
                }
                else if (resultCode == Result.Ok)
                {
                    if (requestCode == SELECT_LOCATION_TYPE_CODE)
                    {
                        selectedLocationType = JsonConvert.DeserializeObject<LocationType>(data.GetStringExtra("selectedLocationType"));
                        if (selectedLocationType != null)
                        {
                            if (_currentLocation != null)
                            {
                                selectorLocationType.Text = selectedLocationType.Description;
                                userActionsListener.GetLocations(Constants.APP_CONFIG.API_KEY_ID, GoogelApiKey, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString(), selectedLocationType.Title, selectedLocationType.Description);
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }
        }

        /// <summary>
        /// Check internet connection
        /// </summary>
        public bool IsNetworkAvailable()
        {
            ConnectivityManager connectivity = (ConnectivityManager)(Application.Context.ApplicationContext).GetSystemService(Context.ConnectivityService);
            if (connectivity != null)
            {
                NetworkInfo[] info = connectivity.GetAllNetworkInfo();
                if (info != null)
                    for (int i = 0; i < info.Length; i++)
                        if (info[i].GetState() == NetworkInfo.State.Connected)
                        {
                            return true;
                        }

            }
            return false;
        }


        public bool OnMyLocationButtonClick()
        {
            SetCurrentLoation(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude));
            return false;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Granted)
                    {
                        alreadyAskedPermission = true;

                        RunOnUiThread(() =>
                        {
                            OnLoad();
                        });

                        if (_currentLocation != null)
                        {
                            SetCurrentLoation(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude));
                            userActionsListener.GetLocations(Constants.APP_CONFIG.API_KEY_ID, GoogelApiKey, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString(), "ALL", mLocationDescription);
                        }
                    }
                    else
                    {
                        this.Finish();
                    }
                }
            }
        }

        /// <summary>
        /// Check app permission weather it is allowed to access location service or not!
        /// </summary>
        public override void Ready()
        {
            if (!alreadyAskedPermission)
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
                {


                    if (ShouldShowRequestPermissionRationale(Manifest.Permission.AccessFineLocation) || ShouldShowRequestPermissionRationale(Manifest.Permission.AccessCoarseLocation))
                    {
                        ShowRationale(LocationTitleRationale(), LocationContentRationale(), Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE);

                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE);
                    }
                    return;
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        OnLoad();
                    });
                    alreadyAskedPermission = true;
                }
            }
        }

        public async System.Threading.Tasks.Task<string> GetUserLocationAsync()
        {
            System.Text.StringBuilder userAddress = new System.Text.StringBuilder();
            try
            {
                if (_currentLocation == null)
                {
                    return string.Empty;
                }

                Geocoder geocoder = new Geocoder(Application.Context.ApplicationContext);
                IList<Address> addressList = await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

                Address address = addressList.FirstOrDefault();
                if (address != null)
                {

                    for (int i = 0; i < address.MaxAddressLineIndex; i++)
                    {
                        userAddress.Append(address.GetAddressLine(i)).AppendLine(",");
                    }
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                Utility.LoggingNonFatalError(exception);
                return "Location is disabled in user Phone Settings";

            }
            catch (System.Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return ex.Message;

            }
            return userAddress.ToString();
        }

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void ShowZeroLocationFoundDialog()
        {
            mNoLocationsDialog.Show();
        }

        void IOnSuccessListener.OnSuccess(Java.Lang.Object result)
        {
            RunOnUiThread(() =>
            {
                OnRequestCheckLocation();
            });

            if (_currentLocation != null)
            {
                SetCurrentLoation(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude));
                userActionsListener.GetLocations(Constants.APP_CONFIG.API_KEY_ID, GoogelApiKey, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString(), "ALL", mLocationDescription);
            }
        }

        void IOnFailureListener.OnFailure(Java.Lang.Exception e)
        {
            ApiException mException = e.JavaCast<ApiException>();
            switch (mException.StatusCode)
            {
                case (int)LocationSettingsStatusCodes.ResolutionRequired:
                    try
                    {
                        ResolvableApiException resolvable = (ResolvableApiException)mException;
                        resolvable.StartResolutionForResult(
                                        this,
                                        Android.Gms.Location.LocationRequest.PriorityHighAccuracy);
                    }
                    catch (IntentSender.SendIntentException ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                        this.Finish();
                    }
                    catch (ClassCastException ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                        this.Finish();
                    }
                    catch (System.Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                        this.Finish();
                    }
                    break;
                default:
                    this.Finish();
                    break;
            }
        }


        public void OnRequestCheckLocation()
        {
            if (IsNetworkAvailable())
            {

                //_locationManager = (LocationManager)(Application.Context.ApplicationContext).GetSystemService(Context.LocationService);
                //Criteria criteria = new Criteria();
                //_locationProvider = _locationManager.GetBestProvider(criteria, true);
                //OnLocationChanged(_locationManager.GetLastKnownLocation(_locationProvider));

                try
                {
                    _locationManager = (LocationManager)Application.Context.ApplicationContext
                            .GetSystemService(Context.LocationService);

                    // getting GPS status
                    isGPSEnabled = _locationManager
                            .IsProviderEnabled(LocationManager.GpsProvider);

                    // getting network status
                    isNetworkEnabled = _locationManager
                            .IsProviderEnabled(LocationManager.NetworkProvider);

                    if (!isGPSEnabled && !isNetworkEnabled)
                    {
                        // location service disabled
                        mLocationDialog = new AlertDialog.Builder(this)
                                .SetTitle("Location!")
                                .SetMessage("Location service disabled. Please enable location from phone settings.")
                                .SetPositiveButton(Utility.GetLocalizedCommonLabel("ok"), (senderAlert, args) =>
                                {
                                    mLocationDialog.Dismiss();
                                })
                                .SetCancelable(false)
                                .Create();
                        if (!mLocationDialog.IsShowing)
                        {
                            mLocationDialog.Show();
                        }
                    }
                    else
                    {
                        this.canGetLocation = true;

                        // if GPS Enabled get lat/long using GPS Services

                        if (isGPSEnabled)
                        {
                            _locationManager.RequestLocationUpdates(
                                    LocationManager.GpsProvider, MIN_TIME_BW_UPDATES,
                                    MIN_DISTANCE_CHANGE_FOR_UPDATES, this);

                            Log.Debug("GPS Enabled", "GPS Enabled");

                            if (_locationManager != null)
                            {
                                _currentLocation = _locationManager
                                        .GetLastKnownLocation(LocationManager.GpsProvider);
                                //updateGPSCoordinates();
                            }
                        }

                        // First get location from Network Provider
                        if (isNetworkEnabled)
                        {
                            if (_currentLocation == null)
                            {
                                _locationManager.RequestLocationUpdates(
                                        LocationManager.NetworkProvider,
                                        MIN_TIME_BW_UPDATES,
                                        MIN_DISTANCE_CHANGE_FOR_UPDATES, this);

                                Log.Debug("Network", "Network");

                                if (_locationManager != null)
                                {
                                    _currentLocation = _locationManager
                                            .GetLastKnownLocation(LocationManager.NetworkProvider);
                                    //updateGPSCoordinates();
                                }
                            }

                        }
                        OnLocationChanged(_currentLocation);
                    }

                }
                catch (System.Exception e)
                {
                    // e.printStackTrace();
                    Log.Error("Error : Location",
                            "Impossible to connect to LocationManager", e);
                    Utility.LoggingNonFatalError(e);
                }


            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }
    }

}
