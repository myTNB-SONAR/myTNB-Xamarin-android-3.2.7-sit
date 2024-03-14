using System;
using Android.App;
using Android.Hardware.Camera2;
using myTNB.Android.Src.SSMR.SubmitMeterReading.MVP;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.Listener
{
    public class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly SubmitMeterTakePhotoFragment owner;

        public CameraStateListener(SubmitMeterTakePhotoFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            // This method is called when the camera is opened.  We start camera preview here.
            owner.mCameraOpenCloseLock.Release();
            owner.mCameraDevice = cameraDevice;
            owner.CreateCameraPreviewSession();
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
            if (owner == null)
                return;
            Activity activity = owner.Activity;
            if (activity != null)
            {
                activity.Finish();
            }
        }

    }
}
