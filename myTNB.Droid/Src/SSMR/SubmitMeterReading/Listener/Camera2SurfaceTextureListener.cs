using Android.Graphics;
using Android.Views;
using myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.MVP;
using System;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Listener
{
    public class Camera2SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        private readonly SubmitMeterTakePhotoFragment owner;

        public Camera2SurfaceTextureListener(SubmitMeterTakePhotoFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            owner.OpenCamera(width, height);
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            throw new NotImplementedException();
        }

        public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
        {
            return true;
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            throw new NotImplementedException();
        }

        public void OnSurfaceTextureSizeChanged(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.ConfigureTransform(width, height);
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void OnSurfaceTextureUpdated(Android.Graphics.SurfaceTexture surface)
        {

        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
            throw new NotImplementedException();
        }
    }
}
