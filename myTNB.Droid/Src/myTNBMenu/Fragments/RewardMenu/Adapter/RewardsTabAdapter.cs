using System.Collections.Generic;
using Android.Content;

using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Facebook.Shimmer;
using Java.Lang;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter
{
    public class RewardsTabAdapter : FragmentStatePagerAdapter
    {
        private FragmentManager mManager;
        private List<RewardItemFragment> mFragmentList = new List<RewardItemFragment>();
        private List<string> mFragmentTitleList = new List<string>();
        private Context context;

        public RewardsTabAdapter(FragmentManager fm, Context context) : base(fm)
        {
            this.context = context;
            mFragmentList = new List<RewardItemFragment>();
            mFragmentTitleList = new List<string>();
            mManager = fm;
        }

        public override int Count => mFragmentList.Count;

        public override Fragment GetItem(int position)
        {
            return mFragmentList[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return null;
        }

        public void AddFragment(RewardItemFragment fragment, string title)
        {
            mFragmentList.Add(fragment);
            mFragmentTitleList.Add(title);
        }

        public void ClearAll()
        {
            try
            {
                mManager.PopBackStack();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            try
            {
                mFragmentList = new List<RewardItemFragment>();
                mFragmentTitleList = new List<string>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public View GetTabView(int position)
        {
            View view = LayoutInflater.From(context).Inflate(Resource.Layout.CustomTabLayout, null);
            TextView tabTextView = view.FindViewById<TextView>(Resource.Id.tabTextView);
            ShimmerFrameLayout tabTextShimmer = view.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerTabTextView);
            TextViewUtils.SetMuseoSans500Typeface(tabTextView);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                tabTextView.SetTextColor(context.Resources.GetColor(Resource.Color.new_grey, null));
            }
            else
            {
                tabTextView.SetTextColor(context.Resources.GetColor(Resource.Color.new_grey));
            }

            if (string.IsNullOrEmpty(mFragmentTitleList[position]))
            {
                tabTextView.Visibility = ViewStates.Gone;
                tabTextShimmer.Visibility = ViewStates.Visible;
                try
                {
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        tabTextShimmer.SetShimmer(shimmerBuilder?.Build());
                    }
                    tabTextShimmer.StartShimmer();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            else
            {
                tabTextView.Visibility = ViewStates.Visible;
                tabTextShimmer.Visibility = ViewStates.Gone;
                if (tabTextShimmer.IsShimmerStarted)
                {
                    tabTextShimmer.StopShimmer();
                }
                tabTextView.Text = mFragmentTitleList[position];
            }
            return view;
        }
        public View GetSelectedTabView(int position)
        {
            View view = LayoutInflater.From(context).Inflate(Resource.Layout.CustomTabLayout, null);
            TextView tabTextView = view.FindViewById<TextView>(Resource.Id.tabTextView);
            ShimmerFrameLayout tabTextShimmer = view.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerTabTextView);
            TextViewUtils.SetMuseoSans500Typeface(tabTextView);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                tabTextView.SetTextColor(context.Resources.GetColor(Resource.Color.powerBlue, null));
            }
            else
            {
                tabTextView.SetTextColor(context.Resources.GetColor(Resource.Color.powerBlue));
            }

            if (string.IsNullOrEmpty(mFragmentTitleList[position]))
            {
                tabTextView.Visibility = ViewStates.Gone;
                tabTextShimmer.Visibility = ViewStates.Visible;
                try
                {
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        tabTextShimmer.SetShimmer(shimmerBuilder?.Build());
                    }
                    tabTextShimmer.StartShimmer();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            else
            {
                tabTextView.Visibility = ViewStates.Visible;
                tabTextShimmer.Visibility = ViewStates.Gone;
                if (tabTextShimmer.IsShimmerStarted)
                {
                    tabTextShimmer.StopShimmer();
                }
                tabTextView.Text = mFragmentTitleList[position];
            }
            return view;
        }

    }
}
