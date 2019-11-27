using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V13.App;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;
using Java.Lang;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Adapter
{
    public class RewardsTabAdapter : FragmentStatePagerAdapter
    {
        private List<Fragment> mFragmentList = new List<Fragment>();
        private List<string> mFragmentTitleList = new List<string>();
        private Context context;

        public RewardsTabAdapter(FragmentManager fm, Context context) : base(fm)
        {
            this.context = context;
            mFragmentList = new List<Fragment>();
            mFragmentTitleList = new List<string>();
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

        public void AddFragment(Fragment fragment, string title)
        {
            mFragmentList.Add(fragment);
            mFragmentTitleList.Add(title);
        }

        public View GetTabView(int position)
        {
            View view = LayoutInflater.From(context).Inflate(Resource.Layout.CustomTabLayout, null);
            TextView tabTextView = view.FindViewById<TextView>(Resource.Id.tabTextView);
            ShimmerFrameLayout tabTextShimmer = view.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerTabTextView);
            TextViewUtil.

            if (string.IsNullOrEmpty(mFragmentTitleList[position]))
            {
                tabTextView.Visibility = ViewStates.Gone;
                try
                {
                    var shimmerBuilder = ShimmerUtils.ShimmerBuilderConfig();
                    if (shimmerBuilder != null)
                    {
                        vh.myServiceShimmerImg.SetShimmer(shimmerBuilder?.Build());
                        vh.myServiceShimmerText.SetShimmer(shimmerBuilder?.Build());
                        vh.myServiceShimmerTextTwo.SetShimmer(shimmerBuilder?.Build());
                    }
                    vh.myServiceShimmerImg.StartShimmer();
                    vh.myServiceShimmerText.StartShimmer();
                    vh.myServiceShimmerTextTwo.StartShimmer();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            else
            {
                tabTextView.Visibility = ViewStates.Visible;
            }
            return view;
        }
        public View GetSelectedTabView(int position)
        {
            View view = LayoutInflater.from(context).inflate(R.layout.custom_tab, null);
            TextView tabTextView = view.findViewById(R.id.tabTextView);
            tabTextView.setText(mFragmentTitleList.get(position));
            tabTextView.setTextColor(ContextCompat.getColor(context, R.color.yellow));
            ImageView tabImageView = view.findViewById(R.id.tabImageView);
            tabImageView.setImageResource(mFragmentIconList.get(position));
            tabImageView.setColorFilter(ContextCompat.getColor(context, R.color.yellow), PorterDuff.Mode.SRC_ATOP);
            return view;
        }

    }
}
