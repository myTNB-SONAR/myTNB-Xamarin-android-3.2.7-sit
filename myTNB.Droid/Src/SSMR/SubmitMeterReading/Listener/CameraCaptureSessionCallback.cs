﻿using System;
using Android.Hardware.Camera2;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.Listener
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly SubmitMeterTakePhotoFragment owner;

        public CameraCaptureSessionCallback(SubmitMeterTakePhotoFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            //owner.ShowToast("Failed");
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (null == owner.mCameraDevice)
            {
                return;
            }

            // When the session is ready, we start displaying the preview.
            owner.mCaptureSession = session;
            try
            {
                // Auto focus should be continuous for camera preview.
                owner.mPreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                // Flash is automatically enabled when necessary.
                //owner.SetAutoFlash(owner.mPreviewRequestBuilder);

                // Finally, we start displaying the camera preview.
                owner.mPreviewRequest = owner.mPreviewRequestBuilder.Build();
                owner.mCaptureSession.SetRepeatingRequest(owner.mPreviewRequest,
                        owner.mCaptureCallback, owner.mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}
