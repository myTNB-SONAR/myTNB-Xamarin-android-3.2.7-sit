using Android.Graphics;
using Android.Util;
using Android.Widget;
using Google.Android.Material.TextField;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Database;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.Utils
{
    public sealed class TextViewUtils
    {

        public static string MuseoSans300 = "MuseoSans_300.otf";
        public static string MuseoSans500 = "MuseoSans_500.otf";
        public static string FontSelected = string.Empty;

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

        public static string SelectedFontSize()
        {
            LargeFontModel largeFontModel = new LargeFontModel();
            LargeFontEntity largeFont = new LargeFontEntity();
            var db = DBHelper.GetSQLiteConnection();


            if (LargeFontEntity.TableExists<LargeFontEntity>(db))
            {
                var selected = db.Query<LargeFontEntity>("select * from LargeFontEntity");
                if (selected.Count > 0)
                {
                    largeFontModel.selected = selected[0].selected;
                    largeFontModel.Key = selected[0].Key;
                    largeFontModel.Value = selected[0].Value;
                    FontSelected = selected[0].Value;
                }
            }
            return largeFontModel.Key;
        }

        public static bool IsLargeFonts
        {
            get
            {
                return SelectedFontSize() == "L";
            }
        }

        private static float GetFontSize(float font)
        {
            var Key = SelectedFontSize();

            if (Key != null && Key == "L")
            {

                return font + 4;
            }

            return font;
        }

        public static void SaveFontSize(Item selectedItem)
        {
            LargeFontEntity largeFontEntity = new LargeFontEntity();
            largeFontEntity.Key = selectedItem.type;
            largeFontEntity.Value = selectedItem.title;
            largeFontEntity.selected = selectedItem.selected;

            largeFontEntity.DeleteTable();
            largeFontEntity.CreateTable();

            largeFontEntity.InsertItem(largeFontEntity);
        }

        internal static void SetTextSize(float size, TextView textView, bool shouldScale = true)
        {
            textView.SetTextSize(ComplexUnitType.Dip, shouldScale ? GetFontSize(size) : size);
        }

        internal static void SetTextSize(float size, EditText editText, bool shouldScale = true)
        {
            editText.SetTextSize(ComplexUnitType.Dip, shouldScale ? GetFontSize(size) : size);
        }

        internal static void SetTextSize(float size, Button button, bool shouldScale = true)
        {
            button.SetTextSize(ComplexUnitType.Dip, shouldScale ? GetFontSize(size) : size);
        }

        private static void SetTextSize(float size, params TextView[] textViews)
        {
            foreach (var textView in textViews)
            {
                textView.SetTextSize(ComplexUnitType.Dip, GetFontSize(size));
            }
        }

        private static void SetTextSize(float size, params EditText[] editTexts)
        {
            foreach (var textView in editTexts)
            {
                textView.SetTextSize(ComplexUnitType.Dip, GetFontSize(size));
            }
        }

        private static void SetTextSize(float size, params Button[] buttons)
        {
            foreach (var textView in buttons)
            {
                textView.SetTextSize(ComplexUnitType.Dip, GetFontSize(size));
            }
        }

        internal static void SetTextSize8(params EditText[] editText)
        {
            SetTextSize(8, editText);
        }

        internal static void SetTextSize9(params EditText[] editText)
        {
            SetTextSize(9, editText);
        }

        internal static void SetTextSize10(params EditText[] editText)
        {
            SetTextSize(10, editText);
        }

        internal static void SetTextSize11(params EditText[] editText)
        {
            SetTextSize(11, editText);
        }

        internal static void SetTextSize12(params EditText[] editText)
        {
            SetTextSize(12, editText);
        }

        internal static void SetTextSize13(params EditText[] editText)
        {
            SetTextSize(13, editText);
        }

        internal static void SetTextSize14(params EditText[] editText)
        {
            SetTextSize(14, editText);
        }

        internal static void SetTextSize15(params EditText[] editText)
        {
            SetTextSize(15, editText);
        }

        internal static void SetTextSize16(params EditText[] editText)
        {
            SetTextSize(16, editText);
        }

        internal static void SetTextSize17(params EditText[] editText)
        {
            SetTextSize(17, editText);
        }

        internal static void SetTextSize18(params EditText[] editText)
        {
            SetTextSize(18, editText);
        }

        internal static void SetTextSize20(params EditText[] editText)
        {
            SetTextSize(20, editText);
        }

        internal static void SetTextSize22(params EditText[] editText)
        {
            SetTextSize(22, editText);
        }

        internal static void SetTextSize24(params EditText[] editText)
        {
            SetTextSize(24, editText);
        }

        internal static void SetTextSize25(params EditText[] editText)
        {
            SetTextSize(25, editText);
        }

        internal static void SetTextSize36(params EditText[] editText)
        {
            SetTextSize(36, editText);
        }

        internal static void SetTextSize8(params TextView[] textViews)
        {
            SetTextSize(8, textViews);
        }

        internal static void SetTextSize9(params TextView[] textViews)
        {
            SetTextSize(9, textViews);
        }

        internal static void SetTextSize10(params TextView[] textViews)
        {
            SetTextSize(10, textViews);
        }

        internal static void SetTextSize11(params TextView[] textViews)
        {
            SetTextSize(11, textViews);
        }

        internal static void SetTextSize12(params TextView[] textViews)
        {
            SetTextSize(12, textViews);
        }

        internal static void SetTextSize13(params TextView[] textViews)
        {
            SetTextSize(13, textViews);
        }

        internal static void SetTextSize14(params TextView[] textViews)
        {
            SetTextSize(14, textViews);
        }

        internal static void SetTextSize15(params TextView[] textViews)
        {
            SetTextSize(15, textViews);
        }

        internal static void SetTextSize16(params TextView[] textViews)
        {
            SetTextSize(16, textViews);
        }

        internal static void SetTextSize17(params TextView[] textViews)
        {
            SetTextSize(17, textViews);
        }

        internal static void SetTextSize18(params TextView[] textViews)
        {
            SetTextSize(18, textViews);
        }

        internal static void SetTextSize20(params TextView[] textViews)
        {
            SetTextSize(20, textViews);
        }

        internal static void SetTextSize22(params TextView[] textViews)
        {
            SetTextSize(22, textViews);
        }

        internal static void SetTextSize24(params TextView[] textViews)
        {
            SetTextSize(24, textViews);
        }

        internal static void SetTextSize25(params TextView[] textViews)
        {
            SetTextSize(25, textViews);
        }

        internal static void SetTextSize36(params TextView[] textViews)
        {
            SetTextSize(36, textViews);
        }

        internal static void SetTextSize8(params Button[] button)
        {
            SetTextSize(8, button);
        }

        internal static void SetTextSize9(params Button[] button)
        {
            SetTextSize(9, button);
        }

        internal static void SetTextSize10(params Button[] button)
        {
            SetTextSize(10, button);
        }

        internal static void SetTextSize11(params Button[] button)
        {
            SetTextSize(11, button);
        }

        internal static void SetTextSize12(params Button[] button)
        {
            SetTextSize(12, button);
        }

        internal static void SetTextSize13(params Button[] button)
        {
            SetTextSize(13, button);
        }

        internal static void SetTextSize14(params Button[] button)
        {
            SetTextSize(14, button);
        }

        internal static void SetTextSize15(params Button[] button)
        {
            SetTextSize(15, button);
        }

        internal static void SetTextSize16(params Button[] button)
        {
            SetTextSize(16, button);
        }

        internal static void SetTextSize17(params Button[] button)
        {
            SetTextSize(17, button);
        }

        internal static void SetTextSize18(params Button[] button)
        {
            SetTextSize(18, button);
        }

        internal static void SetTextSize20(params Button[] button)
        {
            SetTextSize(20, button);
        }

        internal static void SetTextSize22(params Button[] button)
        {
            SetTextSize(22, button);
        }

        internal static void SetTextSize24(params Button[] button)
        {
            SetTextSize(24, button);
        }

        internal static void SetTextSize25(params Button[] button)
        {
            SetTextSize(25, button);
        }

        internal static void SetTextSize36(params Button[] button)
        {
            SetTextSize(36, button);
        }
    }
}