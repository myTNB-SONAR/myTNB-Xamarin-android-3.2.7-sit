﻿using Foundation;
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
            if (Type == FeedbackCategory.LoginBillRelated)
            {
                _loginBillFeedbackController.AddImage(image, DashedLineView);
            }
            else if (Type == FeedbackCategory.LoginFaultyStreetLamp)
            {
                _loginFaultyLampFeedbackController.AddImage(image, DashedLineView);
            }
            else if (Type == FeedbackCategory.LoginOthers)
            {
                _loginOthersFeedbackController.AddImage(image, DashedLineView);
            }
            else if (Type == FeedbackCategory.NonLoginBillRelated)
            {
                _nonLoginBillFeedbackController.AddImage(image, DashedLineView);
            }
            else if (Type == FeedbackCategory.NonLoginFaultyStreetLamp)
            {
                _nonloginFaultyLampFeedbackController.AddImage(image, DashedLineView);
            }
            else if (Type == FeedbackCategory.NonLoginOthers)
            {
                _nonLoginOthersFeedbackViewController.AddImage(image, DashedLineView);
            }
        }

        void DismissScreen()
        {
            if (Type == FeedbackCategory.LoginBillRelated)
            {
                _loginBillFeedbackController.DismissViewController(true, null);
            }
            else if (Type == FeedbackCategory.LoginFaultyStreetLamp)
            {
                _loginFaultyLampFeedbackController.DismissViewController(true, null);
            }
            else if (Type == FeedbackCategory.LoginOthers)
            {
                _loginOthersFeedbackController.DismissViewController(true, null);
            }
            else if (Type == FeedbackCategory.NonLoginBillRelated)
            {
                _nonLoginBillFeedbackController.DismissViewController(true, null);
            }
            else if (Type == FeedbackCategory.NonLoginFaultyStreetLamp)
            {
                _nonloginFaultyLampFeedbackController.DismissViewController(true, null);
            }
            else if (Type == FeedbackCategory.NonLoginOthers)
            {
                _nonLoginOthersFeedbackViewController.DismissViewController(true, null);
            }
        }
    }
}