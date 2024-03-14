using System;
using Android.Content;
using Android.Widget;
using Android.Views;
using Android.Graphics;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class PhotoContainerBox : RelativeLayout
    {
        Context mContext;
        int mIndicatorId;
        public bool mIsActive;
        public bool mHasPhoto;
        public bool mIsSelected;
        public string mMeterId;
        TextView boxIndicatorView;
        ImageView photoImageView;
        public Bitmap photoBitmap;
        const int INACTIVE_BACKGROUND = Resource.Drawable.meter_capture_holder_inactive;
        const int ACTIVE_BACKGROUND = Resource.Drawable.meter_capture_holder_active;
        const int SELECTED_BACKGROUND = Resource.Drawable.meter_capture_holder_selected;
        public PhotoContainerBox(Context context, int indicator) : base(context)
        {
            mContext = context;
            mIndicatorId = indicator;
            int size = (int)(56 * Resources.DisplayMetrics.Density);
            int margin = (int)(16 * Resources.DisplayMetrics.Density);
            LinearLayout.LayoutParams photoContainerBoxParams = new LinearLayout.LayoutParams(size, size);
            photoContainerBoxParams.LeftMargin = margin;
            photoContainerBoxParams.RightMargin = margin;
            this.LayoutParameters = photoContainerBoxParams;
            this.AddView(CreateBoxIndicator());
        }

        public TextView CreateBoxIndicator()
        {
            boxIndicatorView = new TextView(mContext);
            LinearLayout.LayoutParams indicatorViewParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            boxIndicatorView.LayoutParameters = indicatorViewParams;
            boxIndicatorView.Gravity = GravityFlags.Center;
            boxIndicatorView.Text = mIndicatorId.ToString();
            return boxIndicatorView;
        }

        public PhotoContainerBox Create()
        {
            this.AddView(CreateBoxIndicator());
            return this;
        }

        public void SetActive(bool isActive)
        {
            mIsActive = isActive;
        }

        public void SetMeterId(string meterId)
        {
            mMeterId = meterId;
        }

        public void UpdateBackground()
        {
            if (this.mIsActive)
            {
                SetBackgroundResource(SELECTED_BACKGROUND);
                this.boxIndicatorView.SetTextColor(Color.ParseColor("#1c79ca"));
            }
            else if (this.mHasPhoto)
            {
                SetBackgroundResource(ACTIVE_BACKGROUND);
            }
            else
            {
                SetBackgroundResource(INACTIVE_BACKGROUND);
                this.boxIndicatorView.SetTextColor(Color.ParseColor("#d0d0d0"));
            }
        }

        public void SetPhotoImage(Bitmap bitmap)
        {
            photoImageView = new ImageView(mContext);
            LinearLayout.LayoutParams indicatorViewParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            photoImageView.LayoutParameters = indicatorViewParams;
            photoImageView.SetImageBitmap(bitmap);
            photoImageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            this.AddView(photoImageView);
            this.photoBitmap = bitmap;
            this.mIsActive = false;
            this.mHasPhoto = true;
        }

        public void DeletePhotoImage()
        {
            this.RemoveView(photoImageView);
            this.mIsActive = false;
            this.mHasPhoto = false;
        }
    }
}
