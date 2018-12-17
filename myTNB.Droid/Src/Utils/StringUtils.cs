﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace myTNB_Android.Src.Utils
{
    public static class StringUtils
    {
        public static string ReverseString(this string data)
        {
            char[] charArray = data.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}