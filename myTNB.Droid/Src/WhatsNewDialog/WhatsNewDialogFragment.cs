using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB_Android.Src.WhatsNewDialog
{
    public class WhatsNewDialogFragment : DialogFragment, ViewPager.IOnPageChangeListener
    {

        private Context mContext;
        private DashboardHomeActivity mActivity;
        private LinearLayout container;
        private LinearLayout mainContainer;
        private ViewPager pager;
        private WhatsNewDialogPagerAdapter adapter;
        private List<WhatsNewModel> whatsnew = new List<WhatsNewModel>();
        private LinearLayout indicator;

        public WhatsNewDialogFragment(Context ctx)
        {
            this.mContext = ctx;
            if (this.mContext is DashboardHomeActivity)
            {
                this.mActivity = ((DashboardHomeActivity)this.mContext);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.SetDimAmount(0.75f);
            Dialog.SetCancelable(false);
            Dialog.SetCanceledOnTouchOutside(false);
            Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
            Dialog.Window.Attributes.Gravity = GravityFlags.Center;

            View rootView = inflater.Inflate(Resource.Layout.WhatsNewDialogLayout, container, false);

            try
            {
                container = (LinearLayout)rootView.FindViewById(Resource.Id.subContainer);
                mainContainer = (LinearLayout)rootView.FindViewById(Resource.Id.mainContainer);
                pager = (ViewPager)rootView.FindViewById(Resource.Id.whatsNewPager);
                indicator = (LinearLayout)rootView.FindViewById(Resource.Id.indicatorContainer);
                indicator.Visibility = ViewStates.Gone;

                whatsnew = JsonConvert.DeserializeObject<List<WhatsNewModel>>(Arguments.GetString("whatsnew"));

                adapter = new WhatsNewDialogPagerAdapter(mContext, whatsnew);
                pager.Adapter = adapter;
                adapter.DetailsClicked += OnDetailsClick;
                adapter.CloseClicked += OnCloseClick;
                adapter.RefreshIndicator += OnRefreshIndicator;
                adapter.NotifyDataSetChanged();

                pager.SetClipToPadding(false);
                int topPadding = GetDeviceVerticalScaleInPixel(0.058f);
                int leftRightPadding = GetDeviceHorizontalScaleInPixel(0.096f);

                if (Resources.DisplayMetrics.HeightPixels >= 2200)
                {
                    topPadding = GetDeviceVerticalScaleInPixel(0.138f);
                    leftRightPadding = GetDeviceHorizontalScaleInPixel(0.016f);
                }
                else if (Resources.DisplayMetrics.HeightPixels >= 1920)
                {
                    topPadding = GetDeviceVerticalScaleInPixel(0.138f);
                    leftRightPadding = GetDeviceHorizontalScaleInPixel(0.036f);
                }
                else if (Resources.DisplayMetrics.HeightPixels >= 1080)
                {
                    leftRightPadding = GetDeviceHorizontalScaleInPixel(0.006f);
                }

                if (whatsnew != null && whatsnew.Count > 0 && string.IsNullOrEmpty(whatsnew[0].PortraitImage_PopUp))
                {
                    topPadding = 0;
                }

                pager.SetPadding(leftRightPadding, topPadding, leftRightPadding, 0);
                pager.PageMargin = leftRightPadding;

                int maxHeight = GetDeviceVerticalScaleInPixel(1f);
                pager.LayoutParameters.Height = maxHeight;
                container.RequestLayout();
                pager.RequestLayout();
                mainContainer.RequestLayout();
                indicator.RequestLayout();

                pager.SetOnPageChangeListener(this);


                /*if (adapter != null && adapter.Count > 1)
                {
                    indicator.Visibility = ViewStates.Visible;
                    for (int i = 0; i < adapter.Count; i++)
                    {
                        ImageView image = new ImageView(this.Context);
                        image.Id = i;
                        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                        layoutParams.RightMargin = 8;
                        layoutParams.LeftMargin = 8;
                        image.LayoutParameters = layoutParams;
                        if (i == 0)
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                        indicator.AddView(image, i);
                    }
                }
                else
                {
                    indicator.Visibility = ViewStates.Gone;
                }*/
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
            return rootView;
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

        void OnDetailsClick(object sender, int position)
        {
            try
            {
                if (whatsnew != null && whatsnew.Count > 0 && position >= 0)
                {
                    WhatsNewModel model = whatsnew[position];

                    if (!model.Read)
                    {
                        UpdateWhatsNewRead(model.ID, true);
                    }

                    Intent activity = new Intent(mContext, typeof(WhatsNewDetailActivity));
                    activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, model.ID);
                    activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                    mContext.StartActivity(activity);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void UpdateWhatsNewRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            WhatsNewEntity wtManager = new WhatsNewEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);
        }

        void OnCloseClick(object sender, int position)
        {
            this.Dismiss();
            if (this.mActivity != null)
            {
                this.mActivity.SetIsRootTutorialShown(false);
            }
        }

        void OnRefreshIndicator(object sender, int position)
        {
            /*if (adapter != null && adapter.Count > 1)
            {
                for (int i = 0; i < adapter.Count; i++)
                {
                    ImageView selectedDot = (ImageView)indicator.GetChildAt(i);
                    if (position == i)
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                    }
                    else
                    {
                        selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                    }
                }
            }
            else
            {
                indicator.Visibility = ViewStates.Gone;
            }*/
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public void OnPageScrollStateChanged(int state)
        {

        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public void OnPageSelected(int position)
        {
            try
            {
                /*if (adapter != null && adapter.Count > 1)
                {
                    for (int i = 0; i < adapter.Count; i++)
                    {
                        ImageView selectedDot = (ImageView)indicator.GetChildAt(i);
                        if (position == i)
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}