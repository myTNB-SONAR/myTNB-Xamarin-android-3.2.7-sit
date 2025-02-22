﻿using System;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Listener
{
    public class CameraPreview : SurfaceView, ISurfaceHolderCallback
    {
        Android.Hardware.Camera _camera;
        static bool _stopped;

        public CameraPreview(Context context, Android.Hardware.Camera camera) : base(context)
        {
            _camera = camera;
            _camera.SetDisplayOrientation(90);

            //Surface holder callback is set so theat SurfaceChanged, Created, destroy... 
            //Could be called from here.
            Holder.AddCallback(this);
            // deprecated but required on Android versions less than 3.0
            Holder.SetType(SurfaceType.PushBuffers);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            if (Holder.Surface == null)
            {
                return;
            }

            try
            {
                _camera.StopPreview();
            }
            catch (Exception)
            {
                // ignore: tried to stop a non-existent preview
            }

            try
            {
                // start preview with new settings
                _camera.SetPreviewDisplay(Holder);
                _camera.StartPreview();
            }
            catch (Exception e)
            {
                Log.Debug("", "Error starting camera preview: " + e.Message);
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                _camera.SetPreviewDisplay(holder);
                _camera.StartPreview();
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            //throw new NotImplementedException();
        }
    }
}
