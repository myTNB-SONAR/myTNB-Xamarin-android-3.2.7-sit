using System;
using Facebook.Shimmer;

namespace myTNB
{
    public class CustomShimmerView : FBShimmeringView
    {
        public override nfloat ShimmeringSpeed
        {
            get
            {
                return 1000;
            }
        }

        public override double ShimmeringBeginFadeDuration
        {
            get
            {
                return 0.3;
            }
        }
    }
}
