using Android.Widget;

namespace myTNB.Android.Src.Utils
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