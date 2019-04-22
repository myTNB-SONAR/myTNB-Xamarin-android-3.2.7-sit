using Foundation;
using myTNB.Enums;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class ImagePickerDelegate : UIImagePickerControllerDelegate
    {
        FeedbackEntryViewController _feedbackEntryViewController;

        public UIViewWithDashedLinerBorder DashedLineView;
        public FeedbackCategory Type;

        public ImagePickerDelegate(LoginBillRelatedFeedbackViewController controller)
        {
        }
        public ImagePickerDelegate(LoginFaultyStreetLampFeedbackViewController controller)
        {
        }
        public ImagePickerDelegate(LoginOthersFeedbackViewController controller)
        {
        }
        public ImagePickerDelegate(NonLoginBillRelatedFeedbackViewController controller)
        {
        }
        public ImagePickerDelegate(NonLoginFaultyStreetLampFeedbackViewController controller)
        {
        }
        public ImagePickerDelegate(NonLoginOthersFeedbackViewController controller)
        {
        }

        public ImagePickerDelegate(FeedbackEntryViewController controller)
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