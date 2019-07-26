﻿using System;
using Facebook.Shimmer;

namespace myTNB_Android.Src.Utils
{
	public class ShimmerUtils
	{
		public static Shimmer.AlphaHighlightBuilder ShimmerBuilderConfig()
		{
            Shimmer.AlphaHighlightBuilder shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            shimmerBuilder.SetAutoStart(false);
            shimmerBuilder.SetBaseAlpha(0.1f);
            shimmerBuilder.SetHighlightAlpha(0.9f);
            shimmerBuilder.SetDropoff(1);
            shimmerBuilder.SetDuration(1500);
            return shimmerBuilder;
		}
	}
}