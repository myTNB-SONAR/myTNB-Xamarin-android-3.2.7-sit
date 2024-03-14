using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using System;

namespace myTNB.Android.Src.FindUs.Maps
{
    public class MapLocationListener : Java.Lang.Object, ILocationListener
    {

        protected LocationManager locationManager;
        Location location;
        Location gpsLocation, nwLocation;
        public static double lonI, latI;
        private static long MIN_DISTANCE_FOR_UPDATE = 10;
        private static long MIN_TIME_FOR_UPDATE = 1000 * 60 * 2;
        private static String TAG = "LocationListner";
        Context context;



        public MapLocationListener()
        {
            locationManager = Application.Context.GetSystemService(Context.LocationService) as LocationManager;
        }


        public Location getLocation(String provider)
        {
            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;

            var locationProvider = locationManager.GetBestProvider(locationCriteria, true);

            if (locationProvider != null && locationManager.IsProviderEnabled(locationProvider))
            {
                locationManager.RequestLocationUpdates(provider, MIN_TIME_FOR_UPDATE, MIN_DISTANCE_FOR_UPDATE, this);
                if (locationManager != null)
                {
                    location = locationManager.GetLastKnownLocation(provider);
                    return location;
                }
            }
            return null;
        }

        public void OnLocationChanged(Location location)
        {
            if (location != null)
            {
                latI = location.Latitude;
                lonI = location.Longitude;
                //String longitude = "Longitude: " + location.Longitude;
                //Log.Debug(TAG, longitude);
                //String latitude = "Latitude: " + location.Latitude;
                //Log.Debug(TAG, latitude);
            }
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }
    }
}