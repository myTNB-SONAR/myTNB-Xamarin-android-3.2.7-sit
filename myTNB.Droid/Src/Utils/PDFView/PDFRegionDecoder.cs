using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Com.Davemorrissey.Labs.Subscaleview;
using Com.Davemorrissey.Labs.Subscaleview.Decoder;
using Java.IO;

namespace myTNB.AndroidApp.Src.Utils.PDFView
{
    public class PDFRegionDecoder : Java.Lang.Object, IImageRegionDecoder
    {
        private ParcelFileDescriptor descriptor;
        private PdfRenderer renderer;
        private int pageWidth = 0;
        private int pageHeight = 0;
        public PDFView mView { get; set; }
        public File mFile { get; set; }
        public float mScale { get; set; }
        public int mBackgroundColorPdf { get; set; }

        public PDFRegionDecoder() : base()
        {

        }

        public PDFRegionDecoder(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {

        }


        bool IImageRegionDecoder.IsReady => (pageWidth > 0 && pageHeight > 0);

        Bitmap IImageRegionDecoder.DecodeRegion(Rect rect, int sampleSize)
        {
            var numPageAtStart = (int) Math.Floor((double) rect.Top / pageHeight);
            var numPageAtEnd = (int) Math.Ceiling((double) rect.Bottom / pageHeight) - 1;
            var bitmap = Bitmap.CreateBitmap(rect.Width() / sampleSize, rect.Height() / sampleSize, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bitmap);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                canvas.DrawColor(mView.mContext.Resources.GetColor(mBackgroundColorPdf, null));
            }
            else
            {
                canvas.DrawColor(mView.mContext.Resources.GetColor(mBackgroundColorPdf));
            }
            canvas.DrawBitmap(bitmap, 0f, 0f, null);

            int iteration = 0;
            for (int pageIndex = numPageAtStart; pageIndex <= numPageAtEnd; pageIndex++)
            {
                lock(renderer)
                {
                    if (pageIndex <= (renderer.PageCount - 1))
                    {
                        var page = renderer.OpenPage(pageIndex);
                        var matrix = new Matrix();
                        matrix.SetScale(mScale / sampleSize, mScale / sampleSize);
                        matrix.PostTranslate(
                            (float)(-rect.Left / sampleSize), -(float)((rect.Top - pageHeight * numPageAtStart) / sampleSize) + ((float)pageHeight / sampleSize) * iteration);
                        page.Render(bitmap, null, matrix, PdfRenderMode.ForDisplay);
                        page.Close();
                        iteration++;
                    }
                }
            }
            return bitmap;
        }

        Point IImageRegionDecoder.Init(Context p0, Android.Net.Uri p1)
        {
            descriptor = ParcelFileDescriptor.Open(mFile, ParcelFileMode.ReadOnly);
            renderer = new PdfRenderer(descriptor);
            var page = renderer.OpenPage(0);
            pageWidth = (int) (page.Width * mScale);
            pageHeight = (int) (page.Height * mScale);
            mView.SetMinimumScaleType(SubsamplingScaleImageView.ScaleTypeCenterInside);
            page.Close();
            return new Point(pageWidth, pageHeight * renderer.PageCount);
        }

        void IImageRegionDecoder.Recycle()
        {
            renderer.Close();
            descriptor.Close();
            pageWidth = 0;
            pageHeight = 0;
        }
    }
}
