using Android.Graphics;
using Android.Media;
using System;

namespace myTNB_Android.Src.Utils
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

    }
}