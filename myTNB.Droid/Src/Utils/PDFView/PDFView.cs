using System;
using Android.Content;
using Android.Util;
using Com.Davemorrissey.Labs.Subscaleview;
using Com.Davemorrissey.Labs.Subscaleview.Decoder;
using Java.IO;

namespace myTNB_Android.Src.Utils.PDFView
{
    public class PDFView : SubsamplingScaleImageView
    {
        private File mFile = null;
        private float mScale = 8f;
        public Context mContext;

        public PDFView(Context context) : base(context)
        {
            this.mContext = context;
            init();
        }

        public PDFView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.mContext = context;
            init();
        }

        private void init()
        {
            SetMinimumTileDpi(120);
            SetMinimumScaleType(SubsamplingScaleImageView.ScaleTypeCenterInside);
        }

        public PDFView FromFile(File file)
        {
            mFile = file;
            return this;
        }

        public PDFView FromFile(string filePath)
        {
            mFile = new File(filePath);
            return this;
        }

        public PDFView Scale(float scale)
        {
            mScale = scale;
            return this;
        }

        public void Show()
        {
            if (mFile != null && !string.IsNullOrEmpty(mFile.Path))
            {
                var source = ImageSource.InvokeUri(mFile!.Path);
                PDFDecodeFactory mPDFDecodeFactory = new PDFDecodeFactory()
                {
                    mView = this,
                    mFile = this.mFile,
                    mScale = this.mScale,
                    mBackgroundColorPdf = Resource.Color.white
                };
                SetRegionDecoderFactory(mPDFDecodeFactory);
                SetImage(source);
            }
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            this.Recycle();
        }

    }
}
