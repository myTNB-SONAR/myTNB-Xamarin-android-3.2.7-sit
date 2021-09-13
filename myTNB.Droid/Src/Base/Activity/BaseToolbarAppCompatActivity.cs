using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace myTNB_Android.Src.Base.Activity
{
    /// <summary>
    /// The class that abstracts the implementation of the resourceId , handling of permissions and the toolbar customizations.
    /// </summary>
    public abstract class BaseToolbarAppCompatActivity : BaseAppCompatActivity, IExceptionView
    {
        [BindView(Resource.Id.toolbar)]
        protected Toolbar toolbar;

        protected Snackbar mErrorMessageSnackBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // To work-around the issue 
            // Add else to manually find the view of toolbar which is using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(ShowBackArrowIndicator());
                SupportActionBar.SetDisplayShowHomeEnabled(true);
                if (ShowCustomToolbarTitle())
                {
                    TextView title = toolbar?.FindViewById<TextView>(Resource.Id.toolbar_title);
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
                    TextView title = toolbar?.FindViewById<TextView>(Resource.Id.toolbar_title);
                    TextViewUtils.SetMuseoSans500Typeface(title);
                    title.Text = ToolbarTitle();
                    SupportActionBar.SetDisplayShowTitleEnabled(false);
                }
            }
            UserSessions.SetSharedPreference(PreferenceManager.GetDefaultSharedPreferences(this));
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
            return toolbar?.Title;
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

        public virtual void SetToolBarTitle(string title)
        {
            if (toolbar != null)
            {
                TextView txtTitle = toolbar?.FindViewById<TextView>(Resource.Id.toolbar_title);
                txtTitle.Text = title;
            }
            else
            {
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                TextView txtTitle = toolbar?.FindViewById<TextView>(Resource.Id.toolbar_title);
                txtTitle.Text = title;
            }
        }

        public virtual void SetToolbarGradientBackground()
        {
            Drawable drawable = Resources.GetDrawable(Resource.Drawable.GradientToolBar);
            if (toolbar != null)
            {
                toolbar?.SetBackgroundDrawable(drawable);
            }
            else
            {
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                toolbar?.SetBackgroundDrawable(drawable);
            }
        }

        public virtual void SetToolbarBackground(int resId)
        {
            Drawable drawable = Resources.GetDrawable(resId);
            if (toolbar != null)
            {
                toolbar?.SetBackgroundDrawable(drawable);
            }
            else
            {
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                toolbar?.SetBackgroundDrawable(drawable);
            }
        }

        public virtual void RemoveToolbarBackground()
        {
            if (toolbar != null)
            {
                toolbar?.SetBackgroundResource(0);
            }
            else
            {
                toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                toolbar?.SetBackgroundResource(0);
            }
        }

        public virtual void SetStatusBarGradientBackground()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Drawable drawable = Resources.GetDrawable(Resource.Drawable.GradientStatusBar);
                this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                this.Window.SetBackgroundDrawable(drawable);
            }
        }

        public virtual void SetStatusBarBackground(int resId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Drawable drawable = Resources.GetDrawable(resId);
                this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                this.Window.SetBackgroundDrawable(drawable);
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

        public int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        public int GetDeviceVerticalScaleInPixel(float percentageValue)
        {
            var deviceHeight = Resources.DisplayMetrics.HeightPixels;
            return GetScaleInPixel(deviceHeight, percentageValue);
        }

        public int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
        }

        public ISpanned GetFormattedText(string stringValue)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(stringValue, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                return Html.FromHtml(stringValue);
            }
        }

        public ISpanned GetFormattedTextNoExtraSpacing(string stringValue)
        {
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(stringValue, FromHtmlOptions.ModeCompact);
            }
            else
            {
                return Html.FromHtml(stringValue);
            }
        }

        public void ShowGenericSnackbarException()
        {
            //if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
            //{
            //    mErrorMessageSnackBar.Dismiss();
            //}

            //mErrorMessageSnackBar = Snackbar.Make(rootView, "Something went wrong! Please try again later", Snackbar.LengthIndefinite)
            //.SetAction("Close", delegate { mErrorMessageSnackBar.Dismiss(); }
            //);
            //View v = mErrorMessageSnackBar.View;
            //TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            //tv.SetMaxLines(5);
            //Button btn = (Button)v.FindViewById<Button>(Resource.Id.snackbar_action);
            //btn.SetTextColor(Android.Graphics.Color.Yellow);
            //mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }
    }
}