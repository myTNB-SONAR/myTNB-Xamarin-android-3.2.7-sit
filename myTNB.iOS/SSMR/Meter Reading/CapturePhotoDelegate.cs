using System;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using UIKit;

namespace myTNB.SSMR.MeterReading
{
    public class CapturePhotoDelegate : AVCapturePhotoCaptureDelegate
    {
        public Action<UIImage> OnCapturePhoto;

        public override void DidFinishProcessingPhoto(AVCapturePhotoOutput captureOutput
            , CMSampleBuffer photoSampleBuffer, CMSampleBuffer previewPhotoSampleBuffer
            , AVCaptureResolvedPhotoSettings resolvedSettings, AVCaptureBracketedStillImageSettings bracketSettings
            , NSError error)
        {
            if (photoSampleBuffer == null)
            {
                return;
            }
            NSData imgData = AVCapturePhotoOutput.GetJpegPhotoDataRepresentation(photoSampleBuffer, previewPhotoSampleBuffer);
            UIImage capturedImage = UIImage.LoadFromData(imgData, 1.0F);
            if (OnCapturePhoto != null)
            {
                OnCapturePhoto.Invoke(GetPortraitImage(capturedImage));
            }
        }

        private UIImage GetPortraitImage(UIImage img)
        {
            UIGraphics.BeginImageContextWithOptions(img.Size, false, img.CurrentScale);
            img.Draw(new CGRect(new CGPoint(0, 0), img.Size));
            UIImage newImg = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return newImg != null ? newImg : img;
        }
    }
}