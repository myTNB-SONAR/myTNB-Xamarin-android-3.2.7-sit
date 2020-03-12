using System;
using Foundation;
using UIKit;

namespace myTNB.SSMR.MeterReading
{
    public class ImagePickerDelegate : UIImagePickerControllerDelegate
    {
        public Action OnDismiss;
        public Action<UIImage> OnSelect;

        public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
        {
            UIImage image = info.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
            if (image != null && OnSelect != null)
            {
                OnSelect.Invoke(image);
            }
            if (OnDismiss != null)
            {
                OnDismiss.Invoke();
            }
        }

        public override void Canceled(UIImagePickerController picker)
        {
            if (OnDismiss != null)
            {
                OnDismiss.Invoke();
            }
        }
    }
}