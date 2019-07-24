using System;
using Facebook.Shimmer;

namespace myTNB_Android.Src.Utils
{
	public class ShimmerUtils
	{
		public static Shimmer.AlphaHighlightBuilder ShimmerBuilderConfig()
		{
            Shimmer.AlphaHighlightBuilder shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            shimmerBuilder.SetAutoStart(false);
            shimmerBuilder.SetBaseAlpha(1);
            shimmerBuilder.SetHighlightAlpha(0.8f);
            shimmerBuilder.SetDropoff(1);
            return shimmerBuilder;
		}
	}
}