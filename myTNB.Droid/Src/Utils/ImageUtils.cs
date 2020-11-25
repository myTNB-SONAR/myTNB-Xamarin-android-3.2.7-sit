using Android.Graphics;
using System;
using Android.Util;
using Java.IO;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Utils
{
    public class ImageUtils
    {

        /** return bitmap object from image URL **/
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return imageBitmap;
        }

        public static Bitmap GetImageBitmapFromUrlWithTimeOut(string url)
        {
            Bitmap imageBitmap = null;
            try
            {
                using (var webClient = new WebClientWithTimeout())
                {
                   
                    var awaitImage = webClient.DownloadDataTaskAsync(new Uri(url));
                    var imageBytes = awaitImage.Result;


                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                return null;
            }
            return imageBitmap;
        }

        

        public static string GetBase64FromBitmap(Bitmap bitmap, int imageQuality)
        {
            string base64String = "";
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, imageQuality, stream);

                var byteArray = stream.ToArray();
                int length = byteArray.Length;
                base64String = Convert.ToBase64String(byteArray);
            }
            return base64String;
        }
    }

    public class WebClientWithTimeout : WebClient
    {  //custom timeout webclient wrapper
        public int Timeout { get; set; } = Constants.SPLASHSCREEN_DOWNLOAD_TIMEOUT_MILISEC; //4 secs default

        public new async Task<byte[]> DownloadDataTaskAsync(Uri address)
        {
            return await RunWithTimeout(base.DownloadDataTaskAsync(address));
        }
        private async Task<T> RunWithTimeout<T>(Task<T> task)
        {
            if (task == await Task.WhenAny(task, Task.Delay(Timeout)))
                return await task;
            else
            {
                this.CancelAsync();
                throw new TimeoutException();
            }
        }
    }

}