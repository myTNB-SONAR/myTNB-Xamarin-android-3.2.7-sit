using Android.App;




using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.BottomNavigation;
using System;

namespace myTNB_Android.Src.Utils
{
    public static class BottomNavigationViewUtils
    {
        /// <summary>
        /// Enable or disable shift mode on bottom navigation view
        /// </summary>
        /// <param name="bottomNavigationView"></param>
        /// <param name="enabled"></param>
        public static void SetShiftMode(this BottomNavigationView bottomNavigationView, bool enableShiftMode, bool enableItemShiftMode)
        {
            try
            {
                var menuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
                if (menuView == null)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to find BottomNavigationMenuView");
                    return;
                }

                bottomNavigationView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityLabeled;

                for (int i = 0; i < menuView.ChildCount; i++)
                {
                    var item = menuView.GetChildAt(i) as BottomNavigationItemView;
                    if (item == null)
                        continue;

                    item.SetLabelVisibilityMode(LabelVisibilityMode.LabelVisibilityLabeled);
                    item.SetChecked(item.ItemData.IsChecked);

                }

                menuView.UpdateMenuView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to set shift mode: {ex}");
            }
        }

        /// <summary>
        /// Changes the size of all images in the bottom menu view 
        /// which is hardcoded in https://android.googlesource.com/platform/frameworks/support/+/master/design/res/layout/design_bottom_navigation_item.xml
        /// </summary>
        /// <param name="bottomNavigationView"></param>
        /// <param name="sizeInDp"></param>
        /// <param name="topPadding"></param>
        public static void SetImageFontSize(this BottomNavigationView bottomNavigationView, Activity mActivity, int sizeInDp, int topPadding, float textSize)
        {
            DisplayMetrics displayMetrics = Application.Context.Resources.DisplayMetrics;
            var bottomNavigationMenuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
            ViewGroup.LayoutParams bottomViewParams = bottomNavigationMenuView.LayoutParameters;
            for (int i = 0; i < bottomNavigationMenuView.ChildCount; i++)
            {
                View iconView = bottomNavigationMenuView.GetChildAt(i).FindViewById(Resource.Id.icon);
                ViewGroup.LayoutParams layoutParams = iconView.LayoutParameters;
                var paddingTop = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, topPadding, displayMetrics);
                iconView.SetPadding(0, paddingTop, 0, 0);
                if (bottomNavigationMenuView.GetChildAt(i).Id == Resource.Id.menu_promotion || bottomNavigationMenuView.GetChildAt(i).Id == Resource.Id.menu_reward)
                {
                    layoutParams.Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 28f, displayMetrics);
                    layoutParams.Width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 60f, displayMetrics);
                }
                else
                {
                    layoutParams.Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, sizeInDp, displayMetrics);
                    layoutParams.Width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, sizeInDp, displayMetrics);
                }

                iconView.LayoutParameters = layoutParams;

                TextView selectedTextView = bottomNavigationMenuView.GetChildAt(i).FindViewById<TextView>(Resource.Id.largeLabel);
                TextView unselectedTextView = bottomNavigationMenuView.GetChildAt(i).FindViewById<TextView>(Resource.Id.smallLabel);

                selectedTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mActivity, Resource.Color.powerBlue)));
                unselectedTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(mActivity, Resource.Color.charcoalGrey)));
                selectedTextView.SetTextSize(ComplexUnitType.Dip, textSize);
                unselectedTextView.SetTextSize(ComplexUnitType.Dip, textSize);
                selectedTextView.SetPadding(0, 0, 0, paddingTop);
                unselectedTextView.SetPadding(0, 0, 0, paddingTop);

                TextViewUtils.SetMuseoSans500Typeface(selectedTextView);
                TextViewUtils.SetMuseoSans300Typeface(unselectedTextView);

            }
        }
    }
}