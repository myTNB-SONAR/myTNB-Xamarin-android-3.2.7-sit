using Android.Graphics;
using Android.Widget;
using Google.Android.Material.TextField;

namespace myTNB_Android.Src.Utils
{
    public sealed class TextViewUtils
    {

        public static string MuseoSans300 = "MuseoSans_300.otf";
        public static string MuseoSans500 = "MuseoSans_500.otf";


        public static void SetTypeface(string family, params EditText[] editTexts)
        {
            foreach (var editText in editTexts)
            {
                editText.Typeface = Typeface.CreateFromAsset(editText.Context.Assets, "fonts/" + family);
            }
        }

        public static void SetMuseoSans300Typeface(params EditText[] editTexts)
        {
            SetTypeface(MuseoSans300, editTexts);
        }

        public static void SetMuseoSans500Typeface(params EditText[] editTexts)
        {
            SetTypeface(MuseoSans500, editTexts);
        }


        public static void SetTypeface(string family, params TextView[] textViews)
        {
            foreach (var textView in textViews)
            {
                textView.Typeface = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/" + family);
            }
        }

        public static void SetMuseoSans300Typeface(params TextView[] textViews)
        {
            SetTypeface(MuseoSans300, textViews);
        }

        public static void SetMuseoSans500Typeface(params TextView[] textViews)
        {
            SetTypeface(MuseoSans500, textViews);
        }


        public static void SetTypeface(string family, params TextInputLayout[] textInputLayouts)
        {
            foreach (var textView in textInputLayouts)
            {
                textView.Typeface = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/" + family);
            }
        }

        public static void SetMuseoSans300Typeface(params TextInputLayout[] textInputLayouts)
        {
            SetTypeface(MuseoSans300, textInputLayouts);
        }

        public static void SetMuseoSans500Typeface(params TextInputLayout[] textInputLayouts)
        {
            SetTypeface(MuseoSans500, textInputLayouts);
        }


        public static void SetTypeface(string family, params Button[] buttons)
        {
            foreach (var textView in buttons)
            {
                textView.Typeface = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/" + family);
            }
        }

        public static void SetMuseoSans300Typeface(params Button[] buttons)
        {
            SetTypeface(MuseoSans300, buttons);
        }

        public static void SetMuseoSans500Typeface(params Button[] buttons)
        {
            SetTypeface(MuseoSans500, buttons);
        }

        public static float GetFontSize(float font)
        {
            return font;
        }
    }
}