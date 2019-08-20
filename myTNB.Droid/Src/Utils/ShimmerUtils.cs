﻿using System;
using Android.Graphics;
using Facebook.Shimmer;

namespace myTNB_Android.Src.Utils
{
	public class ShimmerUtils
	{
		public static Shimmer.AlphaHighlightBuilder ShimmerBuilderConfig()
		{
            Shimmer.AlphaHighlightBuilder shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            shimmerBuilder.SetAutoStart(false);
            shimmerBuilder.SetBaseAlpha(1f);
            shimmerBuilder.SetHighlightAlpha(0.1f);
            shimmerBuilder.SetDropoff(1);
            shimmerBuilder.SetDuration(1500);
            return shimmerBuilder;
		}

        public static Shimmer.AlphaHighlightBuilder InvertedShimmerBuilderConfig()
        {
            Shimmer.AlphaHighlightBuilder shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            shimmerBuilder.SetAutoStart(false);
            shimmerBuilder.SetBaseAlpha(0.1f);
            shimmerBuilder.SetHighlightAlpha(0.4f);
            shimmerBuilder.SetDropoff(1);
            shimmerBuilder.SetDuration(1500);
            return shimmerBuilder;
        }

        public static Shimmer.ColorHighlightBuilder ColorShimmerBuilderConfig()
        {
            Shimmer.ColorHighlightBuilder shimmerBuilder = new Shimmer.ColorHighlightBuilder();
            shimmerBuilder.SetAutoStart(false);
            shimmerBuilder.SetHighlightColor(Color.ParseColor("#bed9f1"));
            shimmerBuilder.SetDropoff(1);
            shimmerBuilder.SetDuration(1500);
            return shimmerBuilder;
        }
    }
}
