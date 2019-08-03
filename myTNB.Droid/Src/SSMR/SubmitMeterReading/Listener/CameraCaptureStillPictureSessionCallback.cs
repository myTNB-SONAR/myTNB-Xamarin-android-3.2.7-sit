using System;
using Android.Hardware.Camera2;
using Android.Util;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.Listener
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly SubmitMeterTakePhotoFragment owner;

        public CameraCaptureStillPictureSessionCallback(SubmitMeterTakePhotoFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            // If something goes wrong with the save (or the handler isn't even 
            // registered, this code will toast a success message regardless...)
            owner.ShowToast("Saved: " + owner.mFile);
            //Log.Debug(TAG, owner.mFile.ToString());
            owner.UnlockFocus();
        }
    }
}
