using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.FindUs.Adapter;
using myTNB.Android.Src.FindUs.Models;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB.Android.Src.FindUs.Activity
{
    [Activity(Label = "Store Type"
       , ScreenOrientation = ScreenOrientation.Portrait
       , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class SelectLocationTypeActivity : BaseToolbarAppCompatActivity
    {

        ListView listView;
        private LocationType selectedLocationType;

        private LocationTypeAdapter locationTypeAdapter;
        private List<LocationType> locationTypes = new List<LocationType>();



        public override int ResourceId()
        {
            return Resource.Layout.SelectLocationTypeView;
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
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey("selectedLocationType"))
                    {
                        //selectedLocationType = JsonConvert.DeserializeObject<LocationType>(Intent.Extras.GetString("selectedLocationType"));
                        selectedLocationType = DeSerialze<LocationType>(extras.GetString("selectedLocationType"));
                    }
                }


                if (selectedLocationType != null)
                {
                    SetToolBarTitle(selectedLocationType.Description);
                    if (LocationTypesEntity.HasRecord())
                    {
                        foreach (LocationType type in LocationTypesEntity.GetLocationTypes())
                        {
                            LocationType newType = new LocationType()
                            {
                                Id = type.Id,
                                Title = type.Title,
                                Description = type.Description,
                                ImagePath = type.ImagePath,
                                IsSelected = selectedLocationType.Id.Equals(type.Id) ? true : false
                            };
                            locationTypes.Add(newType);
                        }
                    }
                }

                locationTypeAdapter = new LocationTypeAdapter(this, locationTypes);
                listView = FindViewById<ListView>(Resource.Id.list_view);
                listView.Adapter = locationTypeAdapter;

                listView.ItemClick += OnItemClick;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }
        }
        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                selectedLocationType = locationTypeAdapter.GetItemObject(e.Position);
                selectedLocationType.IsSelected = true;
                Intent map_activity = new Intent(this, typeof(MapActivity));
                map_activity.PutExtra("selectedLocationType", JsonConvert.SerializeObject(selectedLocationType));
                SetResult(Result.Ok, map_activity);
                Finish();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
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
    }
}