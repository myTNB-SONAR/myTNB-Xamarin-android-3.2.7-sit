using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Utils
{
	public class WhatNewMenuUtils
    {
		private static bool isWhatNewLoading = false;


		public static void OnSetWhatNewLoading(bool flag)
		{
            isWhatNewLoading = flag;
		}

		public static bool GetWhatNewLoading()
		{
			return isWhatNewLoading;
		}

	}
}