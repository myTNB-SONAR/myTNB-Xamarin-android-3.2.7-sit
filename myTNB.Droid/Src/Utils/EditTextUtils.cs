using System;
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
    public static class EditTextUtils
    {
        public static void EnableClick(this EditText e)
        {
            e.Focusable = false;
            e.Clickable = true;
            e.SetCursorVisible(false);
        }
    }
}