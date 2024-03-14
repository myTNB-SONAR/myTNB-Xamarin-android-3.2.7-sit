using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Facebook.Shimmer;

namespace myTNB.Android.Src.Utils
{
    public class ShimmerLoadingLayout
    {
        private static ShimmerLoadingLayout instance;

        private Shimmer.AlphaHighlightBuilder shimmerBuilder;
        private List<ShimmerViewContainer> shimmerViewList;
        private StopShimmerCondition mStopShimmerCondition;

        private const string SHIMMER_ID = "shimmerContainer-";
        private ShimmerLoadingLayout()
        {
          shimmerBuilder = ShimmerUtils.InvertedShimmerBuilderConfig();
            shimmerViewList = new List<ShimmerViewContainer>();
        }
        public static ShimmerLoadingLayout GetInstance()
        {
            if (instance == null)
            {
                instance = new ShimmerLoadingLayout();
            }
            return instance;
        }

        //public void AddViewWithShimmer(Context context, ViewGroup parentContainer, ViewGroup baseLayout, ViewGroup shimmerLayout)
        //{
        //    ShimmerFrameLayout shimmerFrameLayout = new ShimmerFrameLayout(context);
        //    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
        //    shimmerFrameLayout.LayoutParameters = layoutParams;

        //    ViewGroup shimmerChildContainer = (ViewGroup)shimmerLayout.FindViewById(Resource.Id.accountShimmerContainer);
        //    int childIndex = shimmerLayout.IndexOfChild(shimmerChildContainer);
        //    shimmerFrameLayout.AddView(shimmerChildContainer);
        //    shimmerLayout.AddView(shimmerFrameLayout,childIndex);
        //    parentContainer.AddView(shimmerLayout);
        //    shimmerFrameLayout.SetShimmer(this.shimmerBuilder.Build());
        //    string shimmerId = SHIMMER_ID + parentContainer.Id;
        //    shimmerViewList.Add(new ShimmerViewContainer(shimmerId, baseLayout, shimmerFrameLayout));
        //    shimmerFrameLayout.StartShimmer();
        //}

        public void AddViewWithShimmer(Context context, ViewGroup parentContainer, ViewGroup baseLayout, ViewGroup shimmerLayout, StopShimmerCondition stopShimmerCondition)
        {
            if (stopShimmerCondition())
            {
                parentContainer.AddView(baseLayout);
            }
            else
            {
                ShimmerFrameLayout shimmerFrameLayout = shimmerLayout.FindViewById(Resource.Id.shimmerConstainer) as ShimmerFrameLayout;
                parentContainer.AddView(shimmerLayout);
                shimmerFrameLayout.SetShimmer(this.shimmerBuilder.Build());
                string shimmerId = SHIMMER_ID + parentContainer.Id;
                shimmerViewList.Add(new ShimmerViewContainer(shimmerId, baseLayout, shimmerFrameLayout));
                shimmerFrameLayout.StartShimmer();
            }
        }

        public void StopShimmer(String id)
        {
            ShimmerFrameLayout shimmerLayout = shimmerViewList.Find((shimmer) =>
            {
                return shimmer.GetId() == SHIMMER_ID + id;
            }).GetShimmer();
            if (shimmerLayout != null)
            {
                shimmerLayout.StopShimmer();
            }
        }

        public void RegisterStopShimmerCondition(StopShimmerCondition stopShimmerCondition)
        {
            mStopShimmerCondition = stopShimmerCondition;
        }

        public delegate bool StopShimmerCondition();
    }

    class ShimmerViewContainer
    {
        private string shimmerId;
        private ShimmerFrameLayout viewShimmerLayout;
        private ViewGroup mBaseLayout;

        public ShimmerViewContainer(string id, ViewGroup baseLayout, ShimmerFrameLayout layout)
        {
            shimmerId = id;
            viewShimmerLayout = layout;
            mBaseLayout = baseLayout;
        }

        public string GetId()
        {
            return shimmerId;
        }

        public ShimmerFrameLayout GetShimmer()
        {
            return viewShimmerLayout;
        }

        public ViewGroup GetBaseLayout()
        {
            return mBaseLayout;
        }
    }
}
