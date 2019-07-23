using System;
using Facebook.Shimmer;

namespace myTNB
{
    public class CustomShimmerView : FBShimmeringView
    {
        public void SetValues()
        {
            ShimmeringSpeed = 2000;
            ShimmeringBeginFadeDuration = 0.3;
            ShimmeringEndFadeDuration = 0.3;
            ShimmeringPauseDuration = 1;
        }
    }
}
