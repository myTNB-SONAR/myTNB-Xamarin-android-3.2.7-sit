using Foundation;
using myTNB.Enums;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class ImagePickerDelegate : UIImagePickerControllerDelegate
    {
        LoginBillRelatedFeedbackViewController _loginBillFeedbackController;
        LoginFaultyStreetLampFeedbackViewController _loginFaultyLampFeedbackController;
        LoginOthersFeedbackViewController _loginOthersFeedbackController;

        NonLoginBillRelatedFeedbackViewController _nonLoginBillFeedbackController;
        NonLoginFaultyStreetLampFeedbackViewController _nonloginFaultyLampFeedbackController;
        NonLoginOthersFeedbackViewController _nonLoginOthersFeedbackViewController;

        FeedbackEntryViewController _feedbackEntryViewController;

        public UIViewWithDashedLinerBorder DashedLineView;
        public FeedbackCategory Type;

        public ImagePickerDelegate(LoginBillRelatedFeedbackViewController controller)
        {
            _loginBillFeedbackController = controller;
        }
        public ImagePickerDelegate(LoginFaultyStreetLampFeedbackViewController controller)
        {
            _loginFaultyLampFeedbackController = controller;
        }
        public ImagePickerDelegate(LoginOthersFeedbackViewController controller)
        {
            _loginOthersFeedbackController = controller;
        }
        public ImagePickerDelegate(NonLoginBillRelatedFeedbackViewController controller)
        {
            _nonLoginBillFeedbackController = controller;
        }
        public ImagePickerDelegate(NonLoginFaultyStreetLampFeedbackViewController controller)
        {
            _nonloginFaultyLampFeedbackController = controller;
        }
        public ImagePickerDelegate(NonLoginOthersFeedbackViewController controller)
        {
            _nonLoginOthersFeedbackViewController = controller;
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