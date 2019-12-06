using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ProfileMenu
{
    public class ProfileMenuFragment : BaseFragmentCustom
	{
        [BindView(Resource.Id.profileMenuItemsContainer)]
        LinearLayout profileMenuItemsContainer;

        const string PAGE_ID = "Profile";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnAttach(Context context)
        {

            try
            {
                if (context is DashboardHomeActivity)
                {
                    var activity = context as DashboardHomeActivity;
                    // SETS THE WINDOW BACKGROUND TO HORIZONTAL GRADIENT AS PER UI ALIGNMENT
                    activity.Window.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.HorizontalGradientBackground));
                    activity.UnsetToolbarBackground();
                    ((DashboardHomeActivity)Activity).SetToolBarTitle(GetLabelByLanguage("title"));
                }
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Profile");
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            base.OnAttach(context);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            try
            {
                Context context = Activity.ApplicationContext;
                var name = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
                var code = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
                if (name != null)
                {
                    //txt_app_version.Text = GetLabelByLanguage("appVersion") + " " + name;
                }

                ((DashboardHomeActivity)Activity).SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                ((DashboardHomeActivity)Activity).SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);

                ProfileMenuItemComponent myTNBAccountItem = GetMyTNBAccountItems();
                myTNBAccountItem.SetHeaderTitle("myTNB Account");
                profileMenuItemsContainer.AddView(myTNBAccountItem);

                ProfileMenuItemComponent item2 = new ProfileMenuItemComponent(Context);
                item2.SetHeaderTitle("Settings");
                profileMenuItemsContainer.AddView(item2);

                ProfileMenuItemComponent item3 = new ProfileMenuItemComponent(Context);
                item3.SetHeaderTitle("Help & Support");
                profileMenuItemsContainer.AddView(item3);

                ProfileMenuItemComponent item4 = new ProfileMenuItemComponent(Context);
                item4.SetHeaderTitle("Share");
                profileMenuItemsContainer.AddView(item4);
            }
            catch (System.Exception e)
            {
                Log.Debug("Package Manager", e.StackTrace);
                //txt_app_version.Visibility = ViewStates.Gone;
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Show();
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            ((DashboardHomeActivity)Activity).ReloadProfileMenu();
        }

        public override int ResourceId()
        {
            return Resource.Layout.ProfileMenuFragmentLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        private ProfileMenuItemComponent GetMyTNBAccountItems()
        {
            ProfileMenuItemComponent myTNBAccountItem = new ProfileMenuItemComponent(Context);

            List<View> myTNBAccountItems = new List<View>();

            ProfileMenuItemContentComponent fullName = new ProfileMenuItemContentComponent(Context);
            fullName.SetTitle("FULL NAME");
            fullName.SetValue("Alia Izzah Bte Abdul Rahman");
            //fullName.SetItemActionVisibility(false);
            //myTNBAccountItem.AddComponentView(fullName);
            //myTNBAccountItem.AddSeparator();
            myTNBAccountItems.Add(fullName);

            ProfileMenuItemContentComponent referenceNumber = new ProfileMenuItemContentComponent(Context);
            referenceNumber.SetTitle("IC / ROC / PASSPORT NUMBER.");
            referenceNumber.SetValue("•••••• ••  5214");
            referenceNumber.SetItemActionVisibility(false);
            //myTNBAccountItem.AddComponentView(referenceNumber);
            myTNBAccountItems.Add(referenceNumber);

            ProfileMenuItemContentComponent email = new ProfileMenuItemContentComponent(Context);
            email.SetTitle("EMAIL");
            email.SetValue("alia.izzah@email.com");
            email.SetItemActionVisibility(false);
            //myTNBAccountItem.AddComponentView(email);
            myTNBAccountItems.Add(email);

            ProfileMenuItemContentComponent mobileNumber = new ProfileMenuItemContentComponent(Context);
            mobileNumber.SetTitle("MOBILE NUMBER");
            mobileNumber.SetValue("+60 12-345 6789");
            mobileNumber.SetItemActionVisibility(true);
            mobileNumber.SetItemActionTitle("Update");
            //myTNBAccountItem.AddComponentView(mobileNumber);
            myTNBAccountItems.Add(mobileNumber);

            ProfileMenuItemContentComponent password = new ProfileMenuItemContentComponent(Context);
            password.SetTitle("PASSWORD");
            password.SetValue("••••••••••••••••");
            password.SetItemActionVisibility(true);
            password.SetItemActionTitle("Update");
            //myTNBAccountItem.AddComponentView(password);
            myTNBAccountItems.Add(password);

            ProfileMenuItemContentComponent cards = new ProfileMenuItemContentComponent(Context);
            cards.SetTitle("CREDIT / DEBIT CARDS");
            cards.SetValue("3");
            cards.SetItemActionVisibility(true);
            cards.SetItemActionTitle("Manage");
            //myTNBAccountItem.AddComponentView(cards);
            myTNBAccountItems.Add(cards);

            ProfileMenuItemContentComponent electricityAccount = new ProfileMenuItemContentComponent(Context);
            electricityAccount.SetTitle("ELECTRICITY ACCOUNTS");
            electricityAccount.SetValue("3");
            electricityAccount.SetItemActionVisibility(true);
            electricityAccount.SetItemActionTitle("Manage");
            //myTNBAccountItem.AddComponentView(electricityAccount);
            myTNBAccountItems.Add(electricityAccount);

            myTNBAccountItem.AddComponentView(myTNBAccountItems);
            return myTNBAccountItem;
        }
    }
}
