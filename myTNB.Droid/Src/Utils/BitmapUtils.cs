using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Util;
using System;

namespace myTNB.Android.Src.Utils
{
    public static class BitmapUtils
    {

        public static Bitmap CorrectOrientation(this Bitmap bitmap, String filePath)
        {
            ExifInterface ei = new ExifInterface(filePath);
            int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

            switch (orientation)
            {
                case (int)Android.Media.Orientation.Rotate90:
                    return Rotate(bitmap, 90);

                case (int)Android.Media.Orientation.Rotate180:
                    return Rotate(bitmap, 180);

                case (int)Android.Media.Orientation.Rotate270:
                    return Rotate(bitmap, 270);

                case (int)Android.Media.Orientation.FlipHorizontal:
                    return Flip(bitmap, true, false);

                case (int)Android.Media.Orientation.FlipVertical:
                    return Flip(bitmap, false, true);

                default:
                    return bitmap;
            }
        }

        public static Bitmap CorrectOrientation(this Bitmap bitmap, System.IO.Stream inputStream)
        {
            ExifInterface ei = new ExifInterface(inputStream);
            int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

            switch (orientation)
            {
                case (int)Android.Media.Orientation.Rotate90:
                    return Rotate(bitmap, 90);

                case (int)Android.Media.Orientation.Rotate180:
                    return Rotate(bitmap, 180);

                case (int)Android.Media.Orientation.Rotate270:
                    return Rotate(bitmap, 270);

                case (int)Android.Media.Orientation.FlipHorizontal:
                    return Flip(bitmap, true, false);

                case (int)Android.Media.Orientation.FlipVertical:
                    return Flip(bitmap, false, true);

                default:
                    return bitmap;
            }
        }

        //public static Bitmap CorrectOrientation(this Bitmap bitmap, FileDescriptor fileDescriptor)
        //{
        //    ExifInterface ei = new ExifInterface(fileDescriptor);
        //    int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

        //    switch (orientation)
        //    {
        //        case (int)Android.Media.Orientation.Rotate90:
        //            return Rotate(bitmap, 90);

        //        case (int)Android.Media.Orientation.Rotate180:
        //            return Rotate(bitmap, 180);

        //        case (int)Android.Media.Orientation.Rotate270:
        //            return Rotate(bitmap, 270);

        //        case (int)Android.Media.Orientation.FlipHorizontal:
        //            return Flip(bitmap, true, false);

        //        case (int)Android.Media.Orientation.FlipVertical:
        //            return Flip(bitmap, false, true);

        //        default:
        //            return bitmap;
        //    }
        //}



        public static Bitmap Rotate(Bitmap bitmap, float degrees)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(degrees);
            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }

        public static Bitmap Flip(Bitmap bitmap, bool horizontal, bool vertical)
        {
            Matrix matrix = new Matrix();
            matrix.PreScale(horizontal ? -1 : 1, vertical ? -1 : 1);
            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }

        public static Bitmap RoundCornerImage(Bitmap raw, float round, int r, int g, int b)
        {
            int width = raw.Width;
            int height = raw.Height;
            Bitmap result = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(result);
            canvas.DrawARGB(255, r, g, b);
            Paint paint = new Paint();
            paint.AntiAlias = true;
            paint.Color = Color.ParseColor("#000000");
            Rect rect = new Rect(0, 0, width, height);
            RectF rectF = new RectF(rect);
            canvas.DrawRoundRect(rectF, round, round, paint);
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(raw, rect, rect, paint);
            return result;
        }

        public static Bitmap ScaledImage(Bitmap originalBitmap, int widthInPx, int heightInPx)
        {
            Bitmap background = Bitmap.CreateBitmap(widthInPx, heightInPx, Bitmap.Config.Argb8888);

            int originalWidth = originalBitmap.Width;
            int originalHeight = originalBitmap.Height;

            Canvas canvas = new Canvas(background);

            float scale = widthInPx / originalWidth;

            float xTranslation = 0.0f;
            float yTranslation = (heightInPx - originalHeight * scale) / 2.0f;

            Matrix transformation = new Matrix();
            transformation.PostTranslate(xTranslation, yTranslation);
            transformation.PreScale(scale, scale);

            Paint paint = new Paint();
            paint.FilterBitmap = true;

            canvas.DrawBitmap(originalBitmap, transformation, paint);

            return background;
        }

        public static Bitmap CreateBitmapFromDrawable(Drawable originalDrawable, int widthInPx = 0, int heightInPx = 0)
        {
            if (widthInPx == 0)
            {
                widthInPx = originalDrawable.IntrinsicWidth;
            }

            if (heightInPx == 0)
            {
                heightInPx = originalDrawable.IntrinsicHeight;
            }

            Bitmap bitmap = Bitmap.CreateBitmap(widthInPx, heightInPx, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            originalDrawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            originalDrawable.Draw(canvas);

            return bitmap;
        }

        public static Bitmap ScaledImageFromDrawable(int id, int widthInPx, int heightInPx, Activity mActivity)
        {
            BitmapFactory.Options opt = new BitmapFactory.Options();
            opt.InMutable = true;
            Bitmap originalBitmap = BitmapFactory.DecodeResource(mActivity.Resources, id, opt);

            Bitmap background = Bitmap.CreateBitmap(widthInPx, heightInPx, Bitmap.Config.Argb8888);

            int originalWidth = originalBitmap.Width;
            int originalHeight = originalBitmap.Height;

            Canvas canvas = new Canvas(background);

            float scale = widthInPx / originalWidth;

            float xTranslation = 0.0f;
            float yTranslation = (heightInPx - originalHeight * scale) / 2.0f;

            Matrix transformation = new Matrix();
            transformation.PostTranslate(xTranslation, yTranslation);
            transformation.PreScale(scale, scale);

            Paint paint = new Paint();
            paint.FilterBitmap = true;

            canvas.DrawBitmap(originalBitmap, transformation, paint);

            return background;
        }

    }
}