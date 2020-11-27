using Android.Graphics;
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
        public static bool isLargeFontVisible = false;

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
                }
            }
            return largeFontModel.Key;
        }
        public static float GetFontSize(float font)
        {

            var Key = SelectedFontSize();

            //if (Key != null && Key == "L")
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


    }
}