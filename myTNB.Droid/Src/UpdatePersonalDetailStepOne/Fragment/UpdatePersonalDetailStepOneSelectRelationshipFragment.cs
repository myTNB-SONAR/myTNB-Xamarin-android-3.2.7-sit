using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Activity;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Adapter;
using myTNB_Android.Src.UpdatePersonalDetailStepOne.Model;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.UpdatePersonalDetailStepOne.Fragment
{

    [Activity(Label = "Relationship with Owner"
          , Icon = "@drawable/ic_launcher"
, ScreenOrientation = ScreenOrientation.Portrait
, Theme = "@style/Theme.Dashboard")]
    public class UpdatePersonalDetailStepOneSelectRelationshipFragment : BaseActivityCustom
    {


     
            private string PAGE_ID = "AddAccount";
            
            ListView listView;

            private SelectRelationshipModel selectedAccountRelationship;

            private UpdatePersonalDetailStepOneSelectRelationshipAdapter relationshipType;

            private List<SelectRelationshipModel> ListrelationshipTypes = new List<SelectRelationshipModel>();

            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey("selectedAccountType"))
                    {
                        selectedAccountRelationship = DeSerialze<SelectRelationshipModel>(extras.GetString("selectedAccountType"));
                    }
                }

                SetToolBarTitle("Relationship with Owner");  // set lang and translation  GetLabelByLanguage("selectAccountType")
                SelectRelationshipModel Child = new SelectRelationshipModel();
                Child.Id = "1";
                Child.Type = "Child";  // translation  GetLabelByLanguage("residential")

                SelectRelationshipModel Tenant = new SelectRelationshipModel();
                Tenant.Id = "2";
                Tenant.Type = "Tenant";  //trnaslation GetLabelByLanguage("commercial")

                SelectRelationshipModel Guardian = new SelectRelationshipModel();
                Guardian.Id = "3";
                Guardian.Type = "Guardian";  //trnaslation GetLabelByLanguage("commercial")

                SelectRelationshipModel Parent = new SelectRelationshipModel();
                Parent.Id = "4";
                Parent.Type = "Parent";  //trnaslation GetLabelByLanguage("commercial")

                SelectRelationshipModel Spouse = new SelectRelationshipModel();
                Spouse.Id = "5";
                Spouse.Type = "Spouse";  //trnaslation GetLabelByLanguage("commercial")

                SelectRelationshipModel Others = new SelectRelationshipModel();
                Others.Id = "6";
                Others.Type = "Others";  //trnaslation GetLabelByLanguage("commercial")

                //AccountType Government = new AccountType();
                //Government.Id = "3";
                //Government.Type = "Government";

                if (selectedAccountRelationship != null)
                {
                    if (selectedAccountRelationship.Id.Equals("1"))
                    {
                        Child.IsSelected = true;
                        Tenant.IsSelected = false;
                        Guardian.IsSelected = false;
                        Parent.IsSelected = false;
                        Spouse.IsSelected = false;
                        Others.IsSelected = false;
                        //Government.IsSelected = false;
                    }
                    else if (selectedAccountRelationship.Id.Equals("2"))
                    {
                        Child.IsSelected = false;
                        Tenant.IsSelected = true;
                        Guardian.IsSelected = false;
                        Parent.IsSelected = false;
                        Spouse.IsSelected = false;
                        Others.IsSelected = false;
                        //Government.IsSelected = false;
                    }
                    else if (selectedAccountRelationship.Id.Equals("3"))
                    {
                        Child.IsSelected = false;
                        Tenant.IsSelected = false;
                        Guardian.IsSelected = true;
                        Parent.IsSelected = false;
                        Spouse.IsSelected = false;
                        Others.IsSelected = false;
                        //Government.IsSelected = true;
                    }
                    else if (selectedAccountRelationship.Id.Equals("4"))
                    {
                        Child.IsSelected = false;
                        Tenant.IsSelected = false;
                        Guardian.IsSelected = false;
                        Parent.IsSelected = true;
                        Spouse.IsSelected = false;
                        Others.IsSelected = false;
                        //Government.IsSelected = true;
                    }
                    else if (selectedAccountRelationship.Id.Equals("5"))
                    {
                        Child.IsSelected = false;
                        Tenant.IsSelected = false;
                        Guardian.IsSelected = false;
                        Parent.IsSelected = false;
                        Spouse.IsSelected = true;
                        Others.IsSelected = false;
                        //Government.IsSelected = true;
                    }
                    else if (selectedAccountRelationship.Id.Equals("6"))
                    {
                        Child.IsSelected = false;
                        Tenant.IsSelected = false;
                        Guardian.IsSelected = false;
                        Parent.IsSelected = false;
                        Spouse.IsSelected = false;
                        Others.IsSelected = true;
                        //Government.IsSelected = true;
                    }
                    //else if (selectedAccountRelationship.Id.Equals("7"))
                    //{
                    //    Child.IsSelected = false;
                    //    Friend.IsSelected = false;
                    //    Guardian.IsSelected = false;
                    //    Parent.IsSelected = false;
                    //    Relative.IsSelected = false;
                    //    Spouse.IsSelected = false;
                    //    Others.IsSelected = true;
                    //    //Government.IsSelected = true;
                    //}
                
                }
                else
                {
                    Child.IsSelected = true;
                    Tenant.IsSelected = false;
                    Guardian.IsSelected = false;
                    Parent.IsSelected = false;
                    Spouse.IsSelected = false;
                    Others.IsSelected = false;
                    //Government.IsSelected = false;
                }
                ListrelationshipTypes.Add(Child);
                ListrelationshipTypes.Add(Tenant);
                ListrelationshipTypes.Add(Guardian);
                ListrelationshipTypes.Add(Parent);
                ListrelationshipTypes.Add(Spouse);
                ListrelationshipTypes.Add(Others);
          
                //acctTypes.Add(Government);

                relationshipType = new UpdatePersonalDetailStepOneSelectRelationshipAdapter(this, ListrelationshipTypes);
                listView = FindViewById<ListView>(Resource.Id.list_view);
                listView.Adapter = relationshipType;

                listView.ItemClick += OnItemClick;
            }

            internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    selectedAccountRelationship = relationshipType.GetItemObject(e.Position);
                    selectedAccountRelationship.IsSelected = true;
                    Intent link_activity = new Intent(this, typeof(UpdatePersonalDetailStepOneActivity));
                    link_activity.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedAccountRelationship));
                    SetResult(Result.Ok, link_activity);
                  
                Finish();
                }
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

            protected override void OnResume()
            {
                base.OnResume();
                try
                {
                    FirebaseAnalyticsUtils.SetScreenName(this, "Update Personal Details -> Select Relationship Type");
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override string GetPageId()
            {
                return PAGE_ID;
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
                return Resource.Layout.SelectAccountTypeView;  //tochange
            }



        
    }
}