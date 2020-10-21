using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
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
        #region Fields

        private const string _externalStorageAuthority = "com.android.externalstorage.documents";
        private const string _downloadsAuthority = "com.android.providers.downloads.documents";
        private const string _mediaAuthority = "com.android.providers.media.documents";
        private const string _photoAuthority = "com.google.android.apps.photos.content";
        private const string _diskAuthority = "com.google.android.apps.docs.storage";
        private const string _diskLegacyAuthority = "com.google.android.apps.docs.storage.legacy";

        #endregion


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

        public static Task<string> SaveAsyncPDF(Context context, byte[] fileArray, string pFolder, string pFileName)
        {
            return Task.Run<string>(() =>
            {
            string rootPath = context.FilesDir.AbsolutePath;
            string path = "";

            if (IsExternalStorageReadable() && IsExternalStorageWritable())
            {
                rootPath = context.GetExternalFilesDir(null).AbsolutePath;
            }

            var directory = System.IO.Path.Combine(rootPath, "pdf");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = pFileName;

            path = System.IO.Path.Combine(directory, filename);

            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }


            }


            FileStream fileStream = new FileStream(path, FileMode.Create);
                {
                    // Write the data to the file, byte by byte.
                    for (int i = 0; i < fileArray.Length; i++)
                    {
                        fileStream.WriteByte(fileArray[i]);
                    }

                    // Set the stream position to the beginning of the file.
                    fileStream.Seek(0, SeekOrigin.Begin);

                    // Read and verify the data.
                    for (int i = 0; i < fileStream.Length; i++)
                    {
                        if (fileArray[i] != fileStream.ReadByte())
                        {
                            Console.WriteLine("Error writing data.");

                            return "";
                        }
                    }
                    Console.WriteLine("The data was written to {0} " +
                        "and verified.", fileStream.Name);
                }
                fileStream.Close();
                return path;
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

        public static byte[] GetCompress(Context context, Bitmap bitmap)
        {
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
                bitmapData = stream.ToArray();
            }

            return bitmapData;
        }

        public static byte[] GetPDFByte(Context context, string path)
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

        /// <summary>
        /// Main feature. Return actual path for file from uri. 
        /// </summary>
        /// <param name="uri">File's uri</param>
        /// <param name="context">Current context</param>
        /// <returns>Actual path</returns>
        public static string GetActualPathForFile(global::Android.Net.Uri uri, Context context)
        {
            bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

            if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                        return global::Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                }
                // Google Drive
                else if (IsDiskContentUri(uri))
                    return GetDriveFileAbsolutePath(context, uri);
                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                {
                    try
                    {
                        string id = DocumentsContract.GetDocumentId(uri);

                        if (!string.IsNullOrEmpty(id))
                        {
                            if (id.StartsWith("raw:"))
                                return id.Replace("raw:", "");

                            string[] contentUriPrefixesToTry = new string[]{
                                    "content://downloads/public_downloads",
                                    "content://downloads/my_downloads",
                                    "content://downloads/all_downloads"
                            };

                            string path = null;

                            foreach (string contentUriPrefix in contentUriPrefixesToTry)
                            {
                                global::Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                        global::Android.Net.Uri.Parse(contentUriPrefix), long.Parse(id));

                                path = GetDataColumn(context, contentUri, null, null);

                                if (!string.IsNullOrEmpty(path))
                                    return path;
                            }

                            // path could not be retrieved using ContentResolver, therefore copy file to accessible cache using streams
                            string fileName = GetFileName(context, uri);
                            Java.IO.File cacheDir = GetDocumentCacheDir(context);
                            Java.IO.File file = GenerateFileName(fileName, cacheDir);

                            if (file != null)
                            {
                                path = file.AbsolutePath;
                                SaveFileFromUri(context, uri, path);
                            }

                            // last try
                            if (string.IsNullOrEmpty(path))
                                return global::Android.OS.Environment.ExternalStorageDirectory.ToString() + "/Download/" + GetFileName(context, uri);

                            return path;
                        }
                    }
                    catch
                    {
                        return global::Android.OS.Environment.ExternalStorageDirectory.ToString() + "/Download/" + GetFileName(context, uri);
                    }
                }
                // MediaProvider
                else if (IsMediaDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);

                    string type = split[0];

                    global::Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    else if ("video".Equals(type))
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    else if ("audio".Equals(type))
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;

                    string selection = "_id=?";
                    string[] selectionArgs = new string[] { split[1] };

                    return GetDataColumn(context, contentUri, selection, selectionArgs);
                }

            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                // Return the remote address
                if (IsGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                // Google Disk document .legacy
                if (IsDiskLegacyContentUri(uri))
                    return GetDriveFileAbsolutePath(context, uri);
                return GetDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }
               

            return null;
        }

        /// <summary>
        /// Create file in current directory with unique name
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="directory">Current directory</param>
        /// <returns>Created file</returns>
        public static Java.IO.File GenerateFileName(string name, Java.IO.File directory)
        {
            if (name == null) return null;

            Java.IO.File file = new Java.IO.File(directory, name);

            if (file.Exists())
            {
                string fileName = name;
                string extension = string.Empty;
                int dotIndex = name.LastIndexOf('.');
                if (dotIndex > 0)
                {
                    fileName = name.Substring(0, dotIndex);
                    extension = name.Substring(dotIndex);

                    int index = 0;

                    while (file.Exists())
                    {
                        index++;
                        name = $"{fileName}({index}){extension}";
                        file = new Java.IO.File(directory, name);
                    }
                }
            }

            try
            {
                if (!file.CreateNewFile())
                    return null;
            }
            catch (Exception ex)
            {
           
                return null;
            }

            return file;
        }

        /// <summary>
        /// Return file path for specified uri using CacheDir
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">Specified uri</param>
        /// <returns>Drive File absolute path</returns>
        private static string GetDriveFileAbsolutePath(Context context, global::Android.Net.Uri uri)
        {
            ICursor cursor = null;
            Java.IO.FileInputStream input = null;
            Java.IO.FileOutputStream output = null;

            try
            {
                cursor = context.ContentResolver.Query(uri, new string[] { OpenableColumns.DisplayName }, null, null, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndexOrThrow(OpenableColumns.DisplayName);
                    var fileName = cursor.GetString(column_index);

                    if (uri == null) return null;
                    ContentResolver resolver = context.ContentResolver;

                    string outputFilePath = new Java.IO.File(context.CacheDir, fileName).AbsolutePath;
                    ParcelFileDescriptor pfd = resolver.OpenFileDescriptor(uri, "r");
                    Java.IO.FileDescriptor fd = pfd.FileDescriptor;
                    input = new Java.IO.FileInputStream(fd);
                    output = new Java.IO.FileOutputStream(outputFilePath);
                    int read = 0;
                    byte[] bytes = new byte[4096];
                    while ((read = input.Read(bytes)) != -1)
                    {
                        output.Write(bytes, 0, read);
                    }

                    return new Java.IO.File(outputFilePath).AbsolutePath;
                }
            }
            catch (Java.IO.IOException ignored)
            {
                // nothing we can do
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();

                input.Close();
                output.Close();
            }

            return string.Empty;
        }

        /// <summary>
        /// Return filename for specified uri
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">Specified uri</param>
        /// <returns>Filename</returns>
        private static string GetFileName(Context context, global::Android.Net.Uri uri)
        {
            string result = string.Empty;

            if (uri.Scheme.Equals("content"))
            {
                var cursor = context.ContentResolver.Query(uri, null, null, null, null);
                try
                {
                    if (cursor != null && cursor.MoveToFirst())
                        result = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
                }
                finally
                {
                    cursor.Close();
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                result = uri.Path;
                int cut = result.LastIndexOf('/');

                if (cut != -1)
                    result = result.Substring(cut + 1);
            }

            return result;
        }

        /// <summary>
        /// Return app cache directory
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Cache directory</returns>
        private static Java.IO.File GetDocumentCacheDir(Context context)
        {
            Java.IO.File dir = new Java.IO.File(context.CacheDir, "documents");

            if (!dir.Exists())
                dir.Mkdirs();

            return dir;
        }

        /// <summary>
        /// Save file from URI to destination path
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">File URI</param>
        /// <param name="destinationPath">Destination path</param>
        /// <returns>Task for await</returns>
        private async static Task SaveFileFromUri(Context context, global::Android.Net.Uri uri, string destinationPath)
        {
            Stream stream = context.ContentResolver.OpenInputStream(uri);
            Java.IO.BufferedOutputStream bos = null;

            try
            {
                bos = new Java.IO.BufferedOutputStream(System.IO.File.OpenWrite(destinationPath));

                int bufferSize = 1024 * 4;
                byte[] buffer = new byte[bufferSize];

                while (true)
                {
                    int len = await stream.ReadAsync(buffer, 0, bufferSize);
                    if (len == 0)
                        break;
                    await bos.WriteAsync(buffer, 0, len);
                }

            }
            catch (Exception ex)
            {
               ;
            }
            finally
            {
                try
                {
                    if (stream != null) stream.Close();
                    if (bos != null) bos.Close();
                }
                catch (Exception ex)
                {
                   
                }
            }
        }

        /// <summary>
        /// Return data for specified uri
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="uri">Current uri</param>
        /// <param name="selection">Args names</param>
        /// <param name="selectionArgs">Args values</param>
        /// <returns>Data</returns>
        private static string GetDataColumn(Context context, global::Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            ICursor cursor = null;
            string column = "_data";
            string[] projection = { column };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            catch { }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }

        //Whether the Uri authority is ExternalStorageProvider.
        private static bool IsExternalStorageDocument(global::Android.Net.Uri uri) => _externalStorageAuthority.Equals(uri.Authority);

        //Whether the Uri authority is DownloadsProvider.
        private static bool IsDownloadsDocument(global::Android.Net.Uri uri) => _downloadsAuthority.Equals(uri.Authority);

        //Whether the Uri authority is MediaProvider.
        private static bool IsMediaDocument(global::Android.Net.Uri uri) => _mediaAuthority.Equals(uri.Authority);

        //Whether the Uri authority is Google Photos.
        private static bool IsGooglePhotosUri(global::Android.Net.Uri uri) => _photoAuthority.Equals(uri.Authority);

        //Whether the Uri authority is Google Disk.
        private static bool IsDiskContentUri(global::Android.Net.Uri uri) => _diskAuthority.Equals(uri.Authority);

        //Whether the Uri authority is Google Disk Legacy.
        private static bool IsDiskLegacyContentUri(global::Android.Net.Uri uri) => _diskLegacyAuthority.Equals(uri.Authority);
    }
}