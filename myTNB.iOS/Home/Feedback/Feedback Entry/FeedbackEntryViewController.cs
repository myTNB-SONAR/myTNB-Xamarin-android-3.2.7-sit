using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Home.Feedback;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.Model;
using System.Drawing;
using myTNB.Customs;
using myTNB.Home.Feedback.FeedbackEntry;

namespace myTNB
{
    public partial class FeedbackEntryViewController : UIViewController
    {
        public FeedbackEntryViewController(IntPtr handle) : base(handle)
        {
        }

        public string FeedbackID = string.Empty;
        public bool IsLoggedIn;

        const float VIEW_PHOTO_MARGIN = 20f;
        const float UNIVERSAL_MARGIN = 7f;
        const int MAX_IMAGE = 2;

        int imageWidth = 0;
        int capturedImageCount = 0;
        int imageCount = 0;

        //Widgets
        UIView _btnSubmitContainer, _viewUploadPhoto, _nonLoginWidgets;
        UILabel _lblPhotoTitle;
        UIButton _btnSubmit;
        UIScrollView _svContainer, imageContainerScroll;
        UITapGestureRecognizer _tapImage;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetHeader();
            AddScrollView();
            AddCTA();
            if (!IsLoggedIn)
            {
                AddNonLoginCommonWidgets();
            }
            //Should be the last to add
            CreatePhotoUploadWidget();
            UpdateContentSize();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        void SetHeader()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.DismissViewController(true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
            this.Title = DataManager.DataManager.SharedInstance.FeedbackCategory?.Find(x => x?.FeedbackCategoryId == FeedbackID)?.FeedbackCategoryName; ;
        }

        void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_svContainer);
        }

        void AddCTA()
        {
            _btnSubmitContainer = new UIView(new CGRect(0, (View.Frame.Height - DeviceHelper.GetScaledHeight(145))
                , View.Frame.Width, DeviceHelper.GetScaledHeight(100)))
            {
                BackgroundColor = UIColor.White
            };

            _btnSubmit = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, DeviceHelper.GetScaledHeight(18), _btnSubmitContainer.Frame.Width - 36, 48)
            };
            _btnSubmit.SetTitle("Common_Submit".Translate(), UIControlState.Normal);
            _btnSubmit.Font = myTNBFont.MuseoSans18_300();
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = myTNBColor.SilverChalice();
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                //ExecuteSubmitFeedback();
            };
            _btnSubmitContainer.AddSubview(_btnSubmit);
            View.AddSubview(_btnSubmitContainer);
        }

        void CreatePhotoUploadWidget()
        {
            //Photo View
            _viewUploadPhoto = new UIView((new CGRect(18, (nfloat)(GetNonLoginWidgetHeight() + VIEW_PHOTO_MARGIN)
                , View.Frame.Width - 36, 180)))
            {
                BackgroundColor = UIColor.Clear
            };

            //Photo/s Title
            _lblPhotoTitle = new UILabel(new CGRect(0, 0, View.Frame.Width - 36, 14))
            {
                Text = "Feedback_AttachPhotoMessage".Translate(),
                TextColor = myTNBColor.SilverChalice(),
                Font = myTNBFont.MuseoSans11_300()
            };
            _viewUploadPhoto.AddSubview(_lblPhotoTitle);

            var lblPhotoSubTitle = new UILabel(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + 108
                , View.Frame.Width - 36, 14))
            {
                Text = "Feedback_MaxFiles".Translate(),
                TextColor = myTNBColor.SilverChalice(),
                Font = myTNBFont.MuseoSans11_300()
            };
            _viewUploadPhoto.AddSubviews(new UIView[] { _lblPhotoTitle, lblPhotoSubTitle });
            _svContainer.AddSubview(_viewUploadPhoto);
            AddImageContainer();
        }

        void AddImageContainer()
        {
            if (capturedImageCount < MAX_IMAGE)
            {
                if (imageContainerScroll == null)
                {
                    imageContainerScroll = new UIScrollView(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + UNIVERSAL_MARGIN
                        , View.Frame.Width - 36, 94))
                    {
                        ScrollEnabled = true,
                        Bounces = false,
                        DirectionalLockEnabled = true
                    };
                    _viewUploadPhoto.AddSubview(imageContainerScroll);
                }

                UIViewWithDashedLinerBorder dashedLineView = new UIViewWithDashedLinerBorder();
                dashedLineView.Frame = new CGRect(imageWidth, 0, 94, 94);
                dashedLineView.BackgroundColor = UIColor.White;
                dashedLineView.Layer.CornerRadius = 5.0f;
                dashedLineView.Tag = 10;

                UIImageView imgViewAdd = new UIImageView(new CGRect(35, 35, 24, 24));
                imgViewAdd.Image = UIImage.FromBundle("IC-Action-Add-Card");
                imgViewAdd.Tag = 0;

                dashedLineView.AddSubview(imgViewAdd);

                _tapImage = new UITapGestureRecognizer(() =>
                {
                    if (imageCount >= MAX_IMAGE)
                    {
                        return;
                    }

                    UIImagePickerController imgPicker = new UIImagePickerController();
                    ImagePickerDelegate imgPickerDelegate = new ImagePickerDelegate(this);
                    //imgPickerDelegate.Type = Enums.FeedbackCategory.LoginOthers;
                    imgPickerDelegate.DashedLineView = dashedLineView;
                    imgPicker.Delegate = imgPickerDelegate;

                    var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create("Feedback_Camera".Translate(), UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.Camera;
                        PresentViewController(imgPicker, true, null);
                    }));

                    alert.AddAction(UIAlertAction.Create("Feedback_CameraRoll".Translate(), UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                        PresentViewController(imgPicker, true, null);
                    }));

                    var cancelAction = UIAlertAction.Create("Common_Cancel".Translate(), UIAlertActionStyle.Cancel, null);
                    alert.AddAction(cancelAction);
                    PresentViewController(alert, animated: true, completionHandler: null);
                });

                dashedLineView.AddGestureRecognizer(_tapImage);
                imageContainerScroll.AddSubview(dashedLineView);
                imageWidth += 18 + 94;
                imageContainerScroll.ContentSize = new CGRect(18, 165, imageWidth, 94).Size;
                capturedImageCount++;
            }
        }

        void RepositionImageContent()
        {
            imageWidth = 0;
            foreach (UIView view in imageContainerScroll.Subviews)
            {
                if (view.Tag == 10)
                {
                    view.Frame = new CGRect(imageWidth, 0, 94, 94);
                    imageWidth += 18 + 94;
                }
            }
            imageContainerScroll.ContentSize = new CGRect(18, 165, imageWidth, 94).Size;
        }

        internal void AddImage(UIImage image, UIViewWithDashedLinerBorder view)
        {
            ActivityIndicator.Show();
            UIImageView capturedImageView = new UIImageView(new CGRect(0, 0, view.Frame.Width, view.Frame.Height))
            {
                Image = image,
                Tag = 1
            };
            UIView imgView = new UIView(new CGRect(65, 0, 29, 29))
            {
                BackgroundColor = UIColor.Clear
            };
            UIImageView imgDelete = new UIImageView(new CGRect(2, 2, 24, 24))
            {
                Image = UIImage.FromBundle("Delete")
            };
            imageCount++;
            imgView.AddSubview(imgDelete);
            imgView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                view.RemoveFromSuperview();
                RepositionImageContent();
                capturedImageCount--;
                imageCount--;
                if (imageCount > 0)
                {
                    AddImageContainer();
                }
            }));

            view.AddSubviews(new UIView[] { capturedImageView, imgView });
            view.RemoveGestureRecognizer(_tapImage);
            AddImageContainer();

            ActivityIndicator.Hide();
        }

        void AddNonLoginCommonWidgets()
        {
            NonLoginCommonWidget _nonLoginCommonWidgets = new NonLoginCommonWidget(View);
            _nonLoginWidgets = _nonLoginCommonWidgets.GetCommonWidgets();
            _svContainer.AddSubview(_nonLoginWidgets);
        }

        void UpdateContentSize()
        {
            float scrollViewHeight = (float)((float)_viewUploadPhoto?.Frame.Height + GetNonLoginWidgetHeight());
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, scrollViewHeight).Size;
        }

        nfloat GetNonLoginWidgetHeight()
        {
            return _nonLoginWidgets?.Frame.Height ?? 0;
        }
    }
}