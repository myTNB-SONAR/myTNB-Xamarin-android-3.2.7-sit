using System;
using Foundation;
using UIKit;

namespace myTNB.Feedback.Enquiry.UpdatePersonalDetail
{
    public class UpdatePersonalDetailImagePickerDelegate : UIImagePickerControllerDelegate
    {
        UpdatePersonalDetail2ViewController _feedbackEntryViewController;

        public UIViewWithDashedLinerBorder DashedLineView;
        //public FeedbackCategory Type;

        public UpdatePersonalDetailImagePickerDelegate(UpdatePersonalDetail2ViewController controller)
        {
            _feedbackEntryViewController = controller;
        }

        public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
        {
            var image = info.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
            AddImageToView(image);
            DismissScreen();
        }

        public override void Canceled(UIImagePickerController picker)
        {
            DismissScreen();
        }

        void AddImageToView(UIImage image)
        {
            _feedbackEntryViewController.AddImage(image, DashedLineView);
        }

        void DismissScreen()
        {
            _feedbackEntryViewController.DismissViewController(true, null);
        }
    }
}
