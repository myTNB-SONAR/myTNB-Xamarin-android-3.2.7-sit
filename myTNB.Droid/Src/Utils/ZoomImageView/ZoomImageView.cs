using Android.Content;
using Android.Graphics;
using Android.Util;
using Com.Davemorrissey.Labs.Subscaleview;

namespace myTNB_Android.Src.Utils.ZoomImageView
{
    public class ZoomImageView : SubsamplingScaleImageView
    {
        private string mPath = "";
        private Bitmap mBitmap = null;
        public Context mContext;

        public ZoomImageView(Context context) : base(context)
        {
            this.mContext = context;
            ZoomEnabled = true;
            SetMinimumScaleType(SubsamplingScaleImageView.ScaleTypeCenterInside);
        }

        public ZoomImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.mContext = context;
            ZoomEnabled = true;
            SetMinimumScaleType(SubsamplingScaleImageView.ScaleTypeCenterInside);
        }


        public ZoomImageView FromFile(string filePath)
        {
            mPath = filePath;
            return this;
        }

        public ZoomImageView FromBitmap(Bitmap bitmap)
        {
            mBitmap = bitmap;
            return this;
        }

        public void Show()
        {
            if (mBitmap != null || !string.IsNullOrEmpty(mPath))
            {
                if (!string.IsNullOrEmpty(mPath))
                {
                    var source = ImageSource.InvokeUri(mPath);
                    SetImage(source);
                }
                else
                {
                    var source = ImageSource.InvokeBitmap(mBitmap);
                    SetImage(source);
                }
            }
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            this.Recycle();
        }

    }
}
