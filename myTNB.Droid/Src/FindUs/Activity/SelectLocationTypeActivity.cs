﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using myTNB_Android.Src.FindUs.Models;
using myTNB_Android.Src.FindUs.Adapter;
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.FindUs.Activity
{
    [Activity(Label = "Store Type"
       , ScreenOrientation = ScreenOrientation.Portrait
       , Theme = "@style/Theme.FindUs")]
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
            selectedLocationType = JsonConvert.DeserializeObject<LocationType>(Intent.Extras.GetString("selectedLocationType"));

            if(selectedLocationType != null)
            {
                SetToolBarTitle(selectedLocationType.Description);
                if(LocationTypesEntity.HasRecord())
                {
                    foreach(LocationType type in LocationTypesEntity.GetLocationTypes())
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

        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            selectedLocationType = locationTypeAdapter.GetItemObject(e.Position);
            selectedLocationType.IsSelected = true;
            Intent map_activity = new Intent(this, typeof(MapActivity));
            map_activity.PutExtra("selectedLocationType", JsonConvert.SerializeObject(selectedLocationType));
            SetResult(Result.Ok, map_activity);
            Finish();
        }
    }
}