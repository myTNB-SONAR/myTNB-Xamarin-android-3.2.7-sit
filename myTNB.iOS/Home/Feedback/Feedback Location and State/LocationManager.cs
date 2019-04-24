using System;
using CoreLocation;

namespace Location
{
    public class LocationManager
    {
        protected CLLocationManager locMgr;
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        public LocationManager()
        {
            this.locMgr = new CLLocationManager
            {
                PausesLocationUpdatesAutomatically = false
            };

            locMgr.RequestWhenInUseAuthorization();
            locMgr.AllowsBackgroundLocationUpdates = false;

        }
        public CLLocationManager LocMgr
        {
            get { return this.locMgr; }
        }

        public void StartLocationUpdates()
        {
            // We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
            // the popover when the app is first launched, or by changing the permissions for the app in Settings
            if (CLLocationManager.LocationServicesEnabled)
            {

                //set the desired accuracy, in meters
                LocMgr.DesiredAccuracy = 1;

                LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                };

                LocMgr.StartUpdatingLocation();
            }
        }
    }
}