using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.AddAccount.Adapter;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.RegistrationForm.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.XDetailRegistrationForm.Adapter;
using myTNB_Android.Src.XDetailRegistrationForm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.XDetailRegistrationForm.Activity
{
    [Activity(Label = "Select Account Type"
              , Icon = "@drawable/ic_launcher"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]
    public class SelectIdentificationTypeActivity : BaseActivityCustom
    {

        ListView listView;
        private IdentificationType selectedIdentificationType;
        private string PAGE_ID = "Register";

        private IdentificationTypeAdapter identificationType;
        private List<IdentificationType> IdTypes = new List<IdentificationType>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey("selectedIdentificationType"))
                {
                    selectedIdentificationType = DeSerialze<IdentificationType>(extras.GetString("selectedIdentificationType"));
                }
            }

            SetToolBarTitle(GetLabelByLanguage("SelectedIdentificationType"));
            IdentificationType Mykad = new IdentificationType();
            Mykad.Id = "1";
            Mykad.Type = GetLabelByLanguage("mykad");

            IdentificationType Armyid = new IdentificationType();
            Armyid.Id = "2";
            Armyid.Type = GetLabelByLanguage("armyid");

            IdentificationType Passport = new IdentificationType();
            Passport.Id = "3";
            Passport.Type = GetLabelByLanguage("passport");

            if (selectedIdentificationType != null)
            {
                if (selectedIdentificationType.Id.Equals("1"))
                {
                    Mykad.IsSelected = true;
                    Armyid.IsSelected = false;
                    Passport.IsSelected = false;
                }
                else if (selectedIdentificationType.Id.Equals("2"))
                {
                    Mykad.IsSelected = false;
                    Armyid.IsSelected = true;
                    Passport.IsSelected = false;
                }
                else if (selectedIdentificationType.Id.Equals("3"))
                {
                    Mykad.IsSelected = false;
                    Armyid.IsSelected = false;
                    Passport.IsSelected = true;
                }
            }
            else
            {
                Mykad.IsSelected = true;
                Armyid.IsSelected = false;
                Passport.IsSelected = false;
            }
            IdTypes.Add(Mykad);
            IdTypes.Add(Armyid);
            IdTypes.Add(Passport);

            identificationType = new IdentificationTypeAdapter(this, IdTypes);
            listView = FindViewById<ListView>(Resource.Id.list_view);
            listView.Adapter = identificationType;

            listView.ItemClick += OnItemClick;
        }

        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                selectedIdentificationType = identificationType.GetItemObject(e.Position);
                selectedIdentificationType.IsSelected = true;
                Intent link_activity = new Intent(this, typeof(DetailRegistrationFormActivity));
                link_activity.PutExtra("selectedIdentificationType", JsonConvert.SerializeObject(selectedIdentificationType));
                SetResult(Result.Ok, link_activity);
                Finish();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Register -> Register Detail");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override int ResourceId()
        {
            return Resource.Layout.SelectAccountTypeView;
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
    }
}