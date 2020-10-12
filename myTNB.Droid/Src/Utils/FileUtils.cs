using Android.Content;
using Android.Graphics;
using Android.Util;
using myTNB_Android.Src.Base.Activity;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Utils
{
    public class FileUtils
    {
        internal const string TEMP_IMAGE_FOLDER = "tmpImages";
        internal const string IMAGE_FOLDER = "Images";

        internal const string PROMO_IMAGE_FOLDER = "PromoImages";
        public static string GetImagesPath(Context context, string pFolder)
        {
            string path = context.FilesDir.AbsolutePath;
            string filePath = System.IO.Path.Combine(path, pFolder);
            Console.WriteLine(string.Format("Folder Personal {0} File Path {1}", path, filePath));
            if (IsExternalStorageReadable() && IsExternalStorageWritable())
            {
                string externalPath = context.GetExternalFilesDir(null).AbsolutePath;
                filePath = System.IO.Path.Combine(externalPath, pFolder);
                Console.WriteLine(string.Format("Folder External {0} File Path {1}", path, filePath));
            }
            return filePath;
        }

        public static string GetTemporaryImageFilePath(Context context, string pFolder, string tmpName)
        {
            string path = context.FilesDir.AbsolutePath;
            string filePath = System.IO.Path.Combine(path, pFolder, tmpName);
            if (IsExternalStorageReadable() && IsExternalStorageWritable())
            {
                string externalPath = context.GetExternalFilesDir(null).AbsolutePath;
                filePath = System.IO.Path.Combine(externalPath, pFolder, tmpName);
                Console.WriteLine(string.Format("Folder External {0} File Path {1}", path, filePath));
            }
            return filePath;
        }

        public static void Remove(Context context, string pFolder, string pFileName)
        {
            string path = context.FilesDir.AbsolutePath;

            string filePath = System.IO.Path.Combine(path, pFolder, pFileName);
            Console.WriteLine(string.Format("Folder Personal {0} File Path {1}", path, filePath));
            if (IsExternalStorageReadable() && IsExternalStorageWritable())
            {
                string externalPath = context.GetExternalFilesDir(null).AbsolutePath;
                filePath = System.IO.Path.Combine(externalPath, pFolder, pFileName);
                Console.WriteLine(string.Format("Folder External {0} File Path {1}", path, filePath));
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }

        public static string Save(Context context, Bitmap bitmap, string pFolder, string pFileName)
        {

            string path = context.FilesDir.AbsolutePath;

            string filePath = System.IO.Path.Combine(path, pFolder, pFileName);
            Console.WriteLine(string.Format("Folder Personal {0} File Path {1}", path, filePath));
            if (IsExternalStorageReadable() && IsExternalStorageWritable())
            {
                string externalPath = context.GetExternalFilesDir(null).AbsolutePath;
                filePath = System.IO.Path.Combine(externalPath, pFolder, pFileName);
                Console.WriteLine(string.Format("Folder External {0} File Path {1}", path, filePath));
            }

            if (!Directory.Exists(System.IO.Path.Combine(path, pFolder)))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(path, pFolder));
            }

            var stream = new FileStream(filePath, FileMode.Create);

            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            stream.Close();


            return filePath;
        }

        public static Task<string> SaveAsync(Context context, Bitmap bitmap, string pFolder, string pFileName)
        {
            return Task.Run<string>(() =>
            {
                string path = context.FilesDir.AbsolutePath;

                string filePath = System.IO.Path.Combine(path, pFolder, pFileName);
                if (!Directory.Exists(System.IO.Path.Combine(path, pFolder)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(path, pFolder));
                }

                var stream = new FileStream(filePath, FileMode.Create);

                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                stream.Close();

                return filePath;
            });
        }


        public static byte[] Get(Context context, Bitmap bitmap)
        {

            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, Constants.APP_CONFIG.MAX_IMAGE_QUALITY_IN_PERCENT, stream);
                bitmapData = stream.ToArray();
            }
            return bitmapData;
        }

        public static byte[]GetCompress(Context context, Bitmap bitmap)
        {
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
                bitmapData = stream.ToArray();
            }

            return bitmapData;
        }

        public static byte[] GetPDFByte(Context context ,string path)
        {
            byte[] pdfdata = File.ReadAllBytes(path);
            return pdfdata;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static Bitmap GetImageFromHex(string hex, int fileSize)
        {
            byte[] imageBytes = StringToByteArray(hex);
            return BitmapFactory.DecodeByteArray(imageBytes, 0, Math.Min(imageBytes.Length, fileSize));
        }

        public static Task<Bitmap> GetImageFromHexAsync(string hex, int fileSize)
        {
            return Task.Run<Bitmap>(() =>
            {
                byte[] imageBytes = StringToByteArray(hex);
                return BitmapFactory.DecodeByteArray(imageBytes, 0, Math.Min(imageBytes.Length, fileSize));
            });

        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2).ToLower(), 16);
            return bytes;
        }


        /// <summary>
        /// Checks if external storage is available for read and write
        /// </summary>
        /// <returns>true if external storage is writable</returns>
        public static bool IsExternalStorageWritable()
        {
            String state = Android.OS.Environment.ExternalStorageState;
            if (Android.OS.Environment.MediaMounted.Equals(state))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if external storage is available to at least read 
        /// </summary>
        /// <returns>true if storage is readable</returns>
        public static bool IsExternalStorageReadable()
        {
            String state = Android.OS.Environment.ExternalStorageState;
            if (Android.OS.Environment.MediaMounted.Equals(state) ||
                Android.OS.Environment.MediaMountedReadOnly.Equals(state))
            {
                return true;
            }
            return false;
        }

        internal static bool DirectoryExists(Context context, string pTempFolder)
        {
            string path = context.FilesDir.AbsolutePath;


            if (IsExternalStorageReadable() && IsExternalStorageWritable())
            {
                path = context.GetExternalFilesDir(null).AbsolutePath;
            }
            return Directory.Exists(System.IO.Path.Combine(path, pTempFolder));
        }

        internal static void CreateDirectory(Context context, string pTempFolder)
        {
            if (!DirectoryExists(context, pTempFolder))
            {
                string path = context.FilesDir.AbsolutePath;


                if (IsExternalStorageReadable() && IsExternalStorageWritable())
                {
                    path = context.GetExternalFilesDir(null).AbsolutePath;
                }

                Directory.CreateDirectory(System.IO.Path.Combine(path, pTempFolder));
            }
        }

        internal static string ProcessGalleryImage(Context context, Android.Net.Uri selectedImage, string pTempImagePath, string pFileName)
        {
            DisplayMetrics metrics = new DisplayMetrics();
            int srcWidth = 200;
            if (context is BaseAppCompatActivity)
            {
                var activity = context as BaseAppCompatActivity;
                activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
                srcWidth = Math.Max(200, metrics.WidthPixels);
            }

            using (var imageStream = context.ContentResolver.OpenInputStream(selectedImage))
            {

                BitmapFactory.Options sourceBMOptions = new BitmapFactory.Options()
                {
                    InJustDecodeBounds = true
                };
                Bitmap bitmapSizeOnly = BitmapFactory.DecodeStream(imageStream, null, sourceBMOptions);
                srcWidth = sourceBMOptions.OutWidth;


                if (bitmapSizeOnly != null && !bitmapSizeOnly.IsRecycled)
                {
                    bitmapSizeOnly.Recycle();
                }
            }

            using (var imageStream = context.ContentResolver.OpenInputStream(selectedImage))
            {
                BitmapFactory.Options actualBMOptions = new BitmapFactory.Options()
                {
                    InJustDecodeBounds = false,
                    InScaled = true,
                    InSampleSize = Constants.APP_CONFIG.IN_SAMPLE_SIZE,
                    InDensity = srcWidth

                };
                actualBMOptions.InTargetDensity = (srcWidth / Constants.APP_CONFIG.IN_SAMPLE_SIZE) * actualBMOptions.InSampleSize;

                //Bitmap bitmap = BitmapFactory.DecodeStream(imageStream, null, actualBMOptions);
                Rect rect = new Rect();
                rect.SetEmpty();
                Bitmap bitmap = BitmapFactory.DecodeStream(imageStream, rect, actualBMOptions);


                //Bitmap correctOrientationBitmap = bitmap.CorrectOrientation(imageStream);
                string filePath = Save(context, bitmap, pTempImagePath, pFileName);
                Bitmap correctOrientationBitmap = bitmap.CorrectOrientation(filePath);
                string filePath1 = Save(context, correctOrientationBitmap, pTempImagePath, pFileName);

                if (bitmap != null && !bitmap.IsRecycled)
                {
                    bitmap.Recycle();
                }
                if (correctOrientationBitmap != null && !correctOrientationBitmap.IsRecycled)
                {
                    correctOrientationBitmap.Recycle();
                }
                if (imageStream != null)
                {
                    imageStream.Close();
                }

                return filePath1;
            }
        }

        internal static string ProcessCameraImage(Context context, string tempImagePath, string fileName)
        {
            DisplayMetrics metrics = new DisplayMetrics();
            int srcWidth = 200;
            if (context is BaseAppCompatActivity)
            {
                var activity = context as BaseAppCompatActivity;
                activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
                srcWidth = Math.Max(200, metrics.WidthPixels);
            }
            BitmapFactory.Options sourceBMOptions = new BitmapFactory.Options()
            {
                InJustDecodeBounds = true
            };
            Bitmap bitmapSizeOnly = BitmapFactory.DecodeFile(tempImagePath, sourceBMOptions);
            srcWidth = sourceBMOptions.OutWidth;

            if (bitmapSizeOnly != null && !bitmapSizeOnly.IsRecycled)
            {
                bitmapSizeOnly.Recycle();
            }

            BitmapFactory.Options actualBMOptions = new BitmapFactory.Options()
            {
                InJustDecodeBounds = false,
                InScaled = true,
                InSampleSize = Constants.APP_CONFIG.IN_SAMPLE_SIZE,
                InDensity = srcWidth

            };
            actualBMOptions.InTargetDensity = (srcWidth / Constants.APP_CONFIG.IN_SAMPLE_SIZE) * actualBMOptions.InSampleSize;

            Bitmap bitmap = BitmapFactory.DecodeFile(tempImagePath, actualBMOptions);

            Bitmap correctOrientationBitmap = bitmap.CorrectOrientation(tempImagePath);
            string filePath = FileUtils.Save(context, correctOrientationBitmap, FileUtils.TEMP_IMAGE_FOLDER, fileName);

            FileUtils.Remove(context, FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage"));



            if (bitmap != null && !bitmap.IsRecycled)
            {
                bitmap.Recycle();
            }

            if (correctOrientationBitmap != null && !correctOrientationBitmap.IsRecycled)
            {
                correctOrientationBitmap.Recycle();
            }
            return filePath;
        }
    }
}