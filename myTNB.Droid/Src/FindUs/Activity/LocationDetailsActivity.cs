using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Java.Net;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.FindUs.Adapter;
using myTNB.AndroidApp.Src.FindUs.Models;
using myTNB.AndroidApp.Src.FindUs.MVP;
using myTNB.AndroidApp.Src.FindUs.Response;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.FindUs.Activity
{
    [Activity(Label = "Store Type"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class LocationDetailsActivity : BaseToolbarAppCompatActivity, LocationDetailsContract.IView
    {

        private LocationDetailsPresenter mPresenter;
        private LocationDetailsContract.IUserActionsListener userActionsListener;

        LocationData locationData = null;
        GoogleApiResult googleApiResult = null;
        string GoogelApiKey;
        CancellationTokenSource cts;
        Bitmap imageBitmap = null;

        private double mLat;
        private double mLng;
        private string mImagePath;

        private readonly string NOT_AVAILABLE = "Not Available";

        private PhoneListAdapter phoneListAdapter;
        RecyclerView.LayoutManager layoutManager;

        private ServiceListAdapter serviceListAdapter;
        RecyclerView.LayoutManager layoutManagerService;

        private OpeningHoursAdapter openingHoursListAdapter;
        RecyclerView.LayoutManager layoutManagerOpeningHours;

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.icon)]
        ImageView icon;

        [BindView(Resource.Id.btnOpenMap)]
        ImageButton btnOpenMap;

        [BindView(Resource.Id.text_title)]
        TextView txtTitle;

        [BindView(Resource.Id.lbl_address)]
        TextView lblAddress;

        [BindView(Resource.Id.text_address)]
        TextView txtAddress;

        [BindView(Resource.Id.lbl_phone)]
        TextView lblPhone;

        [BindView(Resource.Id.layout_phone)]
        RecyclerView layoutPhone;

        [BindView(Resource.Id.lbl_opening_hours)]
        TextView lblOpeningHours;

        [BindView(Resource.Id.layout_opening_hours)]
        RecyclerView layoutOpeningHours;

        [BindView(Resource.Id.lbl_services)]
        TextView lblServices;

        [BindView(Resource.Id.layout_services)]
        RecyclerView layoutServices;

        [BindView(Resource.Id.imageProgressBar)]
        public ProgressBar mImageProgressBar;

        public override int ResourceId()
        {

            return Resource.Layout.LocationDetailsView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new LocationDetailsPresenter(this);
                GoogelApiKey = GetString(Resource.String.google_maps_search_api_key);
                cts = new CancellationTokenSource();

                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                layoutManagerService = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                layoutManagerOpeningHours = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                layoutPhone.NestedScrollingEnabled = false;
                layoutOpeningHours.NestedScrollingEnabled = false;
                layoutServices.NestedScrollingEnabled = false;

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    string KT = extras.GetString("KT");
                    string store = extras.GetString("store");
                    string title = extras.GetString("Title");
                    mImagePath = extras.GetString("imagePath");

                    if (!String.IsNullOrEmpty(KT))
                    {
                        //locationData = JsonConvert.DeserializeObject<LocationData>(KT);
                        locationData = DeSerialze<LocationData>(KT);
                        RunOnUiThread(() =>
                        {
                            InitView(locationData);
                        });
                    }
                    if (!String.IsNullOrEmpty(store))
                    {
                        //googleApiResult = JsonConvert.DeserializeObject<GoogleApiResult>(store);
                        googleApiResult = DeSerialze<GoogleApiResult>(store);
                        RunOnUiThread(() =>
                        {
                            InitView(googleApiResult);
                        });
                    }
                    if (!String.IsNullOrEmpty(title))
                    {
                        SetToolBarTitle(title);
                    }

                }

                TextViewUtils.SetMuseoSans500Typeface(txtTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtAddress);
                TextViewUtils.SetMuseoSans300Typeface(lblAddress, lblPhone, lblOpeningHours, lblServices);
                TextViewUtils.SetTextSize14(lblAddress, lblPhone, lblOpeningHours, lblServices);
                TextViewUtils.SetTextSize16(txtAddress);
                TextViewUtils.SetTextSize18(txtTitle);

                lblAddress.Text = Utility.GetLocalizedLabel("LocationDetails", "address").ToUpper();
                lblPhone.Text = Utility.GetLocalizedLabel("LocationDetails", "phone").ToUpper();
                lblOpeningHours.Text = Utility.GetLocalizedLabel("LocationDetails", "openingHours").ToUpper();
                lblServices.Text = Utility.GetLocalizedLabel("LocationDetails", "services").ToUpper();

                rootView.RequestFocus();

                btnOpenMap.Click += delegate
                {
                    LaunchMapIntent(mLat, mLng);
                };
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }
        }

        public void InitView(Object obj)
        {
            try
            {
                if (obj is LocationData)
                {
                    LocationData data = (LocationData)obj;
                    mLat = data.Latitude;
                    mLng = data.Longitude;
                    Bitmap imageBitmap = null;
                    if (!string.IsNullOrEmpty(data.ImagePath))
                    {
                        GetImageAsync(icon, data.ImagePath);
                    }
                    else
                    {
                        GetImageAsync(icon, mImagePath);
                    }

                    txtTitle.Text = data.Title;
                    txtAddress.Text = data.Address;

                    List<string> phones = new List<string>();
                    if (data.PhoneList.Count > 0)
                    {
                        foreach (Phones phone in data.PhoneList)
                        {
                            phones.Add(phone.PhoneNumber);
                        }
                    }
                    else
                    {
                        phones.Add(NOT_AVAILABLE);
                    }
                    phoneListAdapter = new PhoneListAdapter(this, phones);
                    layoutPhone.SetLayoutManager(layoutManager);
                    layoutPhone.SetAdapter(phoneListAdapter);
                    phoneListAdapter.NotifyDataSetChanged();

                    List<OpeningHours> openingHours = new List<OpeningHours>();
                    if (data.OpeningHourList != null && data.OpeningHourList.Count != 0)
                    {
                        openingHours.AddRange(data.OpeningHourList);

                    }
                    else
                    {
                        OpeningHours item = new OpeningHours
                        {
                            Title = NOT_AVAILABLE,
                            Description = "",
                            Id = "",
                            LocationId = ""
                        };
                        openingHours.Add(item);
                    }
                    openingHoursListAdapter = new OpeningHoursAdapter(this, openingHours);
                    layoutOpeningHours.SetLayoutManager(layoutManagerOpeningHours);
                    layoutOpeningHours.SetAdapter(openingHoursListAdapter);
                    openingHoursListAdapter.NotifyDataSetChanged();

                    List<string> services = new List<string>();
                    if (data.ServicesList != null && data.ServicesList.Count != 0)
                    {
                        foreach (Services item in data.ServicesList)
                        {
                            services.Add(item.Title);
                        }
                    }
                    else
                    {
                        services.Add(NOT_AVAILABLE);
                    }
                    serviceListAdapter = new ServiceListAdapter(this, services);
                    layoutServices.SetLayoutManager(layoutManagerService);
                    layoutServices.SetAdapter(serviceListAdapter);
                    serviceListAdapter.NotifyDataSetChanged();

                }
                else if (obj is GoogleApiResult)
                {
                    GoogleApiResult result = (GoogleApiResult)obj;
                    mLat = result.geometry.location.lat;
                    mLng = result.geometry.location.lng;

                    if (!string.IsNullOrEmpty(mImagePath))
                    {
                        GetImageAsync(icon, mImagePath);
                    }

                    txtTitle.Text = result.name;
                    txtAddress.Text = result.vicinity;

                    List<OpeningHours> openingHours = new List<OpeningHours>();
                    OpeningHours item = new OpeningHours();
                    item.Title = NOT_AVAILABLE;
                    item.Description = "";
                    item.Id = "";
                    item.LocationId = "";
                    if (result.openingHours != null)
                    {
                        if (result.openingHours.openNow)
                        {
                            item.Title = "Open now..";
                        }
                        else
                        {
                            item.Title = "Closed !";
                        }
                    }
                    openingHours.Add(item);
                    openingHoursListAdapter = new OpeningHoursAdapter(this, openingHours);
                    layoutOpeningHours.SetLayoutManager(layoutManagerOpeningHours);
                    layoutOpeningHours.SetAdapter(openingHoursListAdapter);
                    openingHoursListAdapter.NotifyDataSetChanged();

                    List<string> services = new List<string>();
                    services.Add("Payment for electricity bills and other utility bill.");
                    serviceListAdapter = new ServiceListAdapter(this, services);
                    layoutServices.SetLayoutManager(layoutManagerService);
                    layoutServices.SetAdapter(serviceListAdapter);
                    serviceListAdapter.NotifyDataSetChanged();

                    List<string> phones = new List<string>();
                    phones.Add(NOT_AVAILABLE);
                    phoneListAdapter = new PhoneListAdapter(this, phones);
                    layoutPhone.SetLayoutManager(layoutManager);
                    layoutPhone.SetAdapter(phoneListAdapter);
                    phoneListAdapter.NotifyDataSetChanged();

                    this.userActionsListener.GetLocationDetailsFromGoogle(result.place_id, GoogelApiKey, cts);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        public async Task GetImageAsync(ImageView icon, string url)
        {
            try
            {
                mImageProgressBar.Visibility = ViewStates.Visible;
                await Task.Run(() =>
                {
                    imageBitmap = GetImageBitmapFromUrl(icon, url);
                }, cts.Token);

                if (imageBitmap != null)
                {
                    icon.SetImageBitmap(imageBitmap);
                }
                mImageProgressBar.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }
        }


        private Bitmap GetImageBitmapFromUrl(ImageView icon, string url)
        {
            Bitmap image = null;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        image = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }
            return image;
        }

        private void LaunchMapIntent(double lat, double lng)
        {
            try
            {
                String query = lat + "," + lng + "(" + locationData.Title + ")";
                String encodedQuery = URLEncoder.Encode(query);

                string geoLcoation = "geo:" + lat + "," + lng + "?q=" + encodedQuery;
                Android.Net.Uri geoUri = Android.Net.Uri.Parse(geoLcoation);
                Intent mapIntent = new Intent(Intent.ActionView, geoUri);
                StartActivity(mapIntent);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void ShowGetGoogleLocationDetailsSucess(GetGoogleLocationDetailsResponse response)
        {
            List<string> phones = new List<string>();
            if (response.result.formatted_phone_number != null)
            {
                phones.Add(response.result.formatted_phone_number);
                phoneListAdapter = new PhoneListAdapter(this, phones);
                layoutPhone.SetLayoutManager(layoutManager);
                layoutPhone.SetAdapter(phoneListAdapter);
                phoneListAdapter.NotifyDataSetChanged();
            }

        }

        public void SetPresenter(LocationDetailsContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return true;
        }


        protected override void OnDestroy()
        {
            if (cts != null && cts.Token.CanBeCanceled)
            {
                cts.Cancel();
            }

            base.OnDestroy();
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}