//using Facebook.Shimmer;

using UIKit;

namespace myTNB
{
    public class CustomShimmerView : UIView//FBShimmeringView
    {
        public void SetValues()
        {
            /*ShimmeringSpeed = 2000;
            ShimmeringBeginFadeDuration = 0.3;
            ShimmeringEndFadeDuration = 0.3;
            ShimmeringPauseDuration = 1;*/
        }

        public UIView ContentView { set; get; }
        public bool Shimmering { set; get; }
    }
}
