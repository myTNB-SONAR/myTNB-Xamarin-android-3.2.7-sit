using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using myTNB_Android.Src.Utils;
using System.Runtime;

namespace myTNB_Android.Src.Base.Activity
{
    /// <summary>
    /// The class that abstracts the implementation of the resourceId , handling of permissions and the toolbar customizations.
    /// </summary>
    public abstract class BaseToolbarAppCompatActivity : BaseAppCompatActivity
    {
        /// <summary>
        /// Binds the Android.Support.V7.Widget.Toolbar
        /// </summary>
        [BindView(Resource.Id.toolbar)]
        protected Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // To work-around the issue 
            // Add else to manually find the view of toolbar which is using Toolbar = Android.Support.V7.Widget.Toolbar;
            if (toolbar != null)
            {
                
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(ShowBackArrowIndicator());
                SupportActionBar.SetDisplayShowHomeEnabled(true);
                if (ShowCustomToolbarTitle())
                {
                    TextView title = toolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
                    TextViewUtils.SetMuseoSans500Typeface(title);
                    title.Text = ToolbarTitle();
                    SupportActionBar.SetDisplayShowTitleEnabled(false);
                }
                    

            }
            else
            {
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(ShowBackArrowIndicator());
                SupportActionBar.SetDisplayShowHomeEnabled(true);
                if (ShowCustomToolbarTitle())
                {

                    TextView title = toolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
                    TextViewUtils.SetMuseoSans500Typeface(title);
                    title.Text = ToolbarTitle();
                    SupportActionBar.SetDisplayShowTitleEnabled(false);
                }
            }
        }

        /// <summary>
        /// Whether if we use a custom toolbar title or we use the default one
        /// If true then we will implement a custom title o
        /// </summary>
        /// <returns>Boolean value</returns>
        public abstract Boolean ShowCustomToolbarTitle();
        
        /// <summary>
        /// The activity title
        /// </summary>
        /// <returns></returns>
        public virtual string ToolbarTitle()
        {
            return toolbar.Title;
        }

        /// <summary>
        /// The back arrow button or home as up indicator
        /// </summary>
        /// <returns></returns>
        public virtual bool ShowBackArrowIndicator()
        {
            return true;
        }

        /// <summary>
        /// Allows user to go back using back button in toolbar
        /// </summary>
        /// <returns></returns>
        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        public virtual void SetToolBarTitle(string title){
            if(toolbar != null)
            {
                TextView txtTitle = toolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
                txtTitle.Text = title;
            }else{
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                TextView txtTitle = toolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
                txtTitle.Text = title;
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
    }
}