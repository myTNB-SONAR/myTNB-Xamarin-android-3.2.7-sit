using Android.Graphics;
using Android.Util;
using Java.IO;
using System;
using System.IO;
using System.Net;

namespace myTNB_Android.Src.Utils
{
    public class ImageUtils
    {

        /** return bitmap object from image URL **/
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }

        public static string GetBase64FromBitmap(Bitmap bitmap)
        {
            string base64String = "";

            //MemoryStream stream = new MemoryStream();
            //bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
            //byte[] byteArray = stream.ToArray();
            //int length = byteArray.Length;
            //base64String = Base64.EncodeToString(byteArray, Base64Flags.Default);
            //return base64String;

            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 40, stream);

                var byteArray = stream.ToArray();
                int length = byteArray.Length;
                base64String = Convert.ToBase64String(byteArray);
            }

            return base64String;
        }
    }
}