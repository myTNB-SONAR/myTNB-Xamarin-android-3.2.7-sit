using Android.Graphics;
using Android.Media;
using Android.OS;
using Java.IO;
using Java.Lang;
using Java.Nio;
using myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.MVP;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Listener
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        private readonly File file;

        private readonly SubmitMeterTakePhotoFragment owner;
        public ImageAvailableListener(SubmitMeterTakePhotoFragment fragment, File file)
        {
            if (fragment == null)
                throw new System.ArgumentNullException("fragment");
            if (file == null)
                throw new System.ArgumentNullException("file");

            owner = fragment;
            this.file = file;
        }

        //public File File { get; private set; }
        //public Camera2BasicFragment Owner { get; private set; }

        public void OnImageAvailable(ImageReader reader)
        {
            string dir = Environment.ExternalStorageDirectory + "/" + Environment.DirectoryDcim + "/";
            
            owner.mBackgroundHandler.Post(new ImageSaver(reader.AcquireNextImage(), file, owner));
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        private class ImageSaver : Java.Lang.Object, IRunnable
        {
            // The JPEG image
            private Image mImage;

            // The file we save the image into.
            private File mFile;

            private SubmitMeterTakePhotoFragment mOwner;
            public ImageSaver(Image image, File file, SubmitMeterTakePhotoFragment owner)
            {
                if (image == null)
                    throw new System.ArgumentNullException("image");
                if (file == null)
                    throw new System.ArgumentNullException("file");

                mImage = image;
                mFile = file;
                mOwner = owner;
            }

            public void Run()
            {
                ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;
                byte[] bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);

                Bitmap myBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, null);
                mOwner.SetImageGallery(myBitmap);

                using (var output = new FileOutputStream(mFile))
                {
                    try
                    {
                        output.Write(bytes);
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                    finally
                    {
                        mImage.Close();
                    }
                }
            }
        }
    }
}
