using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.Customs;
using myTNB.Feedback;
using myTNB.Feedback.Enquiry.GeneralEnquiry;
using myTNB.Feedback.Enquiry.UpdatePersonalDetail;
using myTNB.Feedback.FeedbackImage;
using myTNB.Home.Bill;
using myTNB.Home.Feedback;
using myTNB.Model;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class UpdatePersonalDetail2ViewController : CustomUIViewController
    {
        public UpdatePersonalDetail2ViewController (IntPtr handle) : base (handle)
        {
        }

        public bool IsOwner;
        public string FeedbackID = string.Empty;
        public bool IsLoggedIn;
        public bool isMobileNumberAvailable;
        public List<FeedbackUpdateDetailsModel> feedbackUpdateDetailsList;

        const string ANY_PATTERN = @".*";
        const int MAX_IMAGE = 2;
        const float TXTVIEW_DEFAULT_MARGIN = 24f;
        const float UNIVERSAL_MARGIN = 7f;
        const float VIEW_PHOTO_MARGIN = 20f;

        private int imageWidth = 0;
        private int capturedImageCount = 0;
        private int imageCount = 0;

        internal MobileNumberComponent _mobileNumberComponent;
        private UIView _viewPhotoContainer;

        //Widgets
        private UIView _btnSubmitContainer, _viewUploadPhoto, _feedbackCategoryView, _viewFeedback
             , _viewLineFeedback;
        private UILabel _lblPhotoTitle, _lblFeedbackTitle, _lblFeedbackSubTitle, _lblFeedbackError;
        private UIImageView _iconFeedback;
        private UIButton _btnSubmit;
        private UIScrollView _svContainer, imageContainerScroll;
        private UITapGestureRecognizer _tapImage, _tapImage2, _tapImage3;

        private FeedbackTextView _feedbackTextView;
        //private OtherFeedbackComponent _otherFeedbackComponent;
        private UpdatePersonalDetailComponent _billRelatedFeedbackComponent;
        //public StreetLampFeedbackComponent _streetLampRelatedFeedbackComponent;

        private SubmitFeedbackResponseModel _submitFeedback;

        //syahmi
        private UIView _viewTitleSection, _viewTitleSection2;

        //for header
        private UIView _headerViewContainer, _headerView, _navbarView
        , _shimmerView, _viewRefreshContainer, _refreshView;
        private UILabel _lblNavTitle, _lblNavStep;
        private UIImageView _bgImageView, _imgFilter;
        private CAGradientLayer _gradientLayer;
        private CustomUIView _accountSelectorContainer, _viewFilter;
        private nfloat _navBarHeight;
        public nfloat _previousScrollOffset;
        private nfloat _tableViewOffset;
        private CGRect DefaultBannerRect;

        private CustomUIView _identificationToolTipsView;
        private CustomUIView _proofConsentToolTipsView;

        private UIView imageContainer;
        private UIView imageContainer2;
        private UIView imageContainer3;

        private bool imageTapped, imageTapped2, imageTapped3;
        private UIView _viewUploadPhoto2;
        private UILabel _lblPhotoTitle2;
        private UIView _viewUploadPhoto3;
        private UILabel _lblPhotoTitle3;

        public int imageContainerSelected = 0;

        UIViewWithDashedLinerBorder dashedLineView, dashedLineView2, dashedLineView3;

        public override void ViewDidLoad()
        {
            PageName = FeedbackConstants.Pagename_FeedbackForm;

            base.ViewDidLoad();
            //_otherFeedbackComponent = new OtherFeedbackComponent(this);
            _billRelatedFeedbackComponent = new UpdatePersonalDetailComponent(this);
            //_streetLampRelatedFeedbackComponent = new StreetLampFeedbackComponent(this);
            SetHeader();
            AddScrollView();
            AddCTA();
            AddSectionTitle2();
            //Should be the last to add
            if (IsOwner)
            {
                CreatePhotoUploadWidget();
                CreateIdentifcationToolTip();
                UpdateContentSize();
            }
            else
            {
                CreatePhotoUploadWidget();
                CreatePhotoUploadWidget2();
                CreateIdentifcationToolTip();
                CreatePhotoUploadWidget3();
                UpdateContentSize();
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }

        private void SetHeader()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController?.PopViewController(true);
            });
            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = btnBack;
            }

            //Title = DataManager.DataManager.SharedInstance.FeedbackCategory?.Find(x => x?.FeedbackCategoryId == FeedbackID)?.FeedbackCategoryName;
            Title = "Update Personal Detail";
        }

        private void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height)) //y:0
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            View.AddSubview(_svContainer);
        }

        private void AddCTA()
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
            _btnSubmit.SetTitle("Next", UIControlState.Normal);
            _btnSubmit.Font = MyTNBFont.MuseoSans18_300;
            _btnSubmit.Layer.CornerRadius = 5.0f;
            //_btnSubmit.Enabled = true; //false;
            //_btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;

            _btnSubmit.Enabled = true;//Temporary
            _btnSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnSubmit.BackgroundColor = MyTNBColor.FreshGreen;

            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                //DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryMessage = _feedbackTextView.Text ? string.Empty;
                NavigateToPage();
            };
            _btnSubmitContainer.AddSubview(_btnSubmit);
            View.AddSubview(_btnSubmitContainer);
        }

        private void AddSectionTitle2()
        {
            _viewTitleSection2 = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "Upload supporting documents:"

            };

            _viewTitleSection2.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection2);
        }
        private void CreatePhotoUploadWidget()
        {
            //Create All Photo Container
            _viewPhotoContainer = new UIView((new CGRect(0, _viewTitleSection2.Frame.GetMaxY(), View.Frame.Width, IsOwner ? 166 : 364))) //364 166
            {
                BackgroundColor = UIColor.White,
            };

            _viewUploadPhoto = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 89))) //164
            {
                BackgroundColor = UIColor.Clear
            };

            //Photo/s Title
            _lblPhotoTitle = new UILabel(new CGRect(0, 0, View.Frame.Width - 36, 16))
            {
                Text = "Copy of owner's identification (IC/Passport)",
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_300
            };
            _viewUploadPhoto.AddSubview(_lblPhotoTitle);

            imageContainer = new UIView(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + UNIVERSAL_MARGIN
            , View.Frame.Width - 36, 48));

            _viewUploadPhoto.AddSubview(imageContainer);

            AddImageContainer();
  
            UILabel lblPhotoSubTitle = new UILabel(new CGRect(0, imageContainer.Frame.GetMaxY() + 4
                , View.Frame.Width - 36, 14))
            {
                Text = "Maximum document size is 1MB (PDF, JPG & JPEG format only)",
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans11_300
            };
            _viewUploadPhoto.AddSubviews(new UIView[] { _lblPhotoTitle, lblPhotoSubTitle }); //GetIdentificationTooltipView(GetYLocationFromFrame(lblPhotoSubTitle.Frame, 16))
            _viewPhotoContainer.AddSubview(_viewUploadPhoto);
            _svContainer.AddSubview(_viewPhotoContainer);

        }

        private void CreatePhotoUploadWidget2()
        {
            //Photo View2
            _viewUploadPhoto2 = new UIView((new CGRect(18, _viewUploadPhoto.Frame.GetMaxY() + 16, View.Frame.Width - 36, 89))) //164
            {
                BackgroundColor = UIColor.Clear
            };

            //Photo/s Title
            _lblPhotoTitle2 = new UILabel(new CGRect(0, 0, View.Frame.Width - 36, 16))
            {
                Text = "Copy of your identification (IC/Passport)",
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_300
            };
            _viewUploadPhoto2.AddSubview(_lblPhotoTitle2);

            imageContainer2 = new UIView(new CGRect(0, _lblPhotoTitle2.Frame.GetMaxY() + UNIVERSAL_MARGIN
            , View.Frame.Width - 36, 48));

            _viewUploadPhoto2.AddSubview(imageContainer2);

            AddImageContainer2();

            UILabel lblPhotoSubTitle2 = new UILabel(new CGRect(0, imageContainer2.Frame.GetMaxY() + 4
                , View.Frame.Width - 36, 14))
            {
                Text = "Maximum document size is 1MB (PDF, JPG & JPEG format only)",
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans11_300
            };
            _viewUploadPhoto2.AddSubviews(new UIView[] { _lblPhotoTitle2, lblPhotoSubTitle2});
            _viewPhotoContainer.AddSubview(_viewUploadPhoto2);
            //_viewPhotoContainer.AddSubview(GetIdentificationTooltipView(GetYLocationFromFrame(_viewUploadPhoto2.Frame, 16))); tnb

        }

        private void CreateIdentifcationToolTip()
        {
            _viewPhotoContainer.AddSubview(GetIdentificationTooltipView(GetYLocationFromFrame(IsOwner ? _viewUploadPhoto.Frame : _viewUploadPhoto2.Frame, 16))); 

        }

        private void CreatePhotoUploadWidget3()
        {
            //Photo View3
            _viewUploadPhoto3 = new UIView((new CGRect(18, _identificationToolTipsView.Frame.GetMaxY() + 16, View.Frame.Width - 36, 89))) //164
            {
                BackgroundColor = UIColor.Clear
            };

            //Photo/s Title
            _lblPhotoTitle3 = new UILabel(new CGRect(0, 0, View.Frame.Width - 36, 16))
            {
                Text = "Proof of consent",
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_300
            };
            _viewUploadPhoto3.AddSubview(_lblPhotoTitle3);

            imageContainer3 = new UIView(new CGRect(0, _lblPhotoTitle3.Frame.GetMaxY() + UNIVERSAL_MARGIN
            , View.Frame.Width - 36, 48));

            _viewUploadPhoto3.AddSubview(imageContainer3);

            AddImageContainer3();

            UILabel lblPhotoSubTitle3 = new UILabel(new CGRect(0, imageContainer3.Frame.GetMaxY() + 4
                , View.Frame.Width - 36, 14))
            {
                Text = "Maximum document size is 1MB (PDF, JPG & JPEG format only)",
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans11_300
            };
            _viewUploadPhoto3.AddSubviews(new UIView[] { _lblPhotoTitle3, lblPhotoSubTitle3 });
            _viewPhotoContainer.AddSubview(_viewUploadPhoto3);
            _viewPhotoContainer.AddSubview(GetProofConsentTooltipView(GetYLocationFromFrame(_viewUploadPhoto3.Frame, 16)));

            _viewPhotoContainer.Frame = new CGRect(0, _viewTitleSection2.Frame.GetMaxY(), View.Frame.Width, _proofConsentToolTipsView.Frame.GetMaxY() + 16);

        }

        private void AddImageContainer()
        {
            imageContainer = new UIView(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + UNIVERSAL_MARGIN
            , View.Frame.Width - 36, 48));
            _viewUploadPhoto.AddSubview(imageContainer);

             dashedLineView = new UIViewWithDashedLinerBorder
            {
                Frame = new CGRect(0, imageWidth, View.Frame.Width - 36, 48),//94
                BackgroundColor = UIColor.White
            };
            dashedLineView.Layer.CornerRadius = 5.0f;
            dashedLineView.Tag = 10;

            UIImageView imgViewAdd = new UIImageView(new CGRect(dashedLineView.Frame.GetMidX() - 12, 12, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Add-Card"),
                Tag = 0
            };

            dashedLineView.AddSubview(imgViewAdd);

                _tapImage = new UITapGestureRecognizer(() =>
                {
                    imageContainerSelected = 1;

                    UIImagePickerController imgPicker = new UIImagePickerController();
                    UpdatePersonalDetailImagePickerDelegate imgPickerDelegate = new UpdatePersonalDetailImagePickerDelegate(this)
                    {
                        DashedLineView = dashedLineView
                    };
                    imgPicker.Delegate = imgPickerDelegate;

                    UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(FeedbackConstants.I18N_Camera), UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.Camera;
                        imgPicker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(imgPicker, true, null);
                    }));

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(FeedbackConstants.I18N_CameraRoll), UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                        imgPicker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(imgPicker, true, null);
                    }));

                    UIAlertAction cancelAction = UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, null);
                    alert.AddAction(cancelAction);
                    alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(alert, animated: true, completionHandler: null);
                });

              dashedLineView.AddGestureRecognizer(_tapImage);
              imageContainer.AddSubview(dashedLineView);

        }

        private void AddImageContainer2()
        {

            imageContainer2 = new UIView(new CGRect(0, _lblPhotoTitle2.Frame.GetMaxY() + UNIVERSAL_MARGIN
            , View.Frame.Width - 36, 48)); //94

                _viewUploadPhoto2.AddSubview(imageContainer2);
                

                 dashedLineView2 = new UIViewWithDashedLinerBorder
                {
                    Frame = new CGRect(0, 0, View.Frame.Width - 36, 48),//94
                    BackgroundColor = UIColor.White
                };
                dashedLineView2.Layer.CornerRadius = 5.0f;
                dashedLineView2.Tag = 10;

                UIImageView imgViewAdd2 = new UIImageView(new CGRect(dashedLineView2.Frame.GetMidX() - 12, 12, 24, 24))
                {
                    Image = UIImage.FromBundle("IC-Action-Add-Card"),
                    Tag = 0
                };

                dashedLineView2.AddSubview(imgViewAdd2);

                _tapImage2 = new UITapGestureRecognizer(() =>
                {
                    imageContainerSelected = 2;

                    UIImagePickerController imgPicker2 = new UIImagePickerController();
                    UpdatePersonalDetailImagePickerDelegate imgPickerDelegate2 = new UpdatePersonalDetailImagePickerDelegate(this)
                    {
                        DashedLineView = dashedLineView2
                    };
                    imgPicker2.Delegate = imgPickerDelegate2;

                    UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(FeedbackConstants.I18N_Camera), UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker2.SourceType = UIImagePickerControllerSourceType.Camera;
                        imgPicker2.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(imgPicker2, true, null);
                    }));

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(FeedbackConstants.I18N_CameraRoll), UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker2.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                        imgPicker2.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(imgPicker2, true, null);
                    }));

                    UIAlertAction cancelAction = UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, null);
                    alert.AddAction(cancelAction);
                    alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(alert, animated: true, completionHandler: null);
                });

                dashedLineView2.AddGestureRecognizer(_tapImage2);
                imageContainer2.AddSubview(dashedLineView2);

        }
        
        private void AddImageContainer3()
        {
            imageContainer3 = new UIView(new CGRect(0, _lblPhotoTitle2.Frame.GetMaxY() + UNIVERSAL_MARGIN
            , View.Frame.Width - 36, 48)); //94
                
                _viewUploadPhoto3.AddSubview(imageContainer3);
                

                 dashedLineView3 = new UIViewWithDashedLinerBorder
                {
                    Frame = new CGRect(0, 0, View.Frame.Width - 36, 48),//94
                    BackgroundColor = UIColor.White
                };
                dashedLineView3.Layer.CornerRadius = 5.0f;
                dashedLineView3.Tag = 10;

                UIImageView imgViewAdd3 = new UIImageView(new CGRect(dashedLineView3.Frame.GetMidX() - 12, 12, 24, 24))
                {
                    Image = UIImage.FromBundle("IC-Action-Add-Card"),
                    Tag = 0
                };

                dashedLineView3.AddSubview(imgViewAdd3);

                _tapImage3 = new UITapGestureRecognizer(() =>
                {
                    imageContainerSelected = 3;

                    UIImagePickerController imgPicker3 = new UIImagePickerController();
                    UpdatePersonalDetailImagePickerDelegate imgPickerDelegate3 = new UpdatePersonalDetailImagePickerDelegate(this)
                    {
                        DashedLineView = dashedLineView3
                    };
                    imgPicker3.Delegate = imgPickerDelegate3;

                    UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(FeedbackConstants.I18N_Camera), UIAlertActionStyle.Default, (obj) =>
                    {
                        imageTapped3 = true;
                        imgPicker3.SourceType = UIImagePickerControllerSourceType.Camera;
                        imgPicker3.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(imgPicker3, true, null);
                    }));

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(FeedbackConstants.I18N_CameraRoll), UIAlertActionStyle.Default, (obj) =>
                    {
                        imageTapped3 = true;
                        imgPicker3.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                        imgPicker3.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(imgPicker3, true, null);
                    }));

                    UIAlertAction cancelAction = UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, null);
                    alert.AddAction(cancelAction);
                    alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(alert, animated: true, completionHandler: null);
                });

                dashedLineView3.AddGestureRecognizer(_tapImage3);
                imageContainer3.AddSubview(dashedLineView3);
            
        }


        internal void AddImage(UIImage image, UIViewWithDashedLinerBorder view)
        {
            ActivityIndicator.Show();
            UIImageView capturedImageView = new UIImageView(new CGRect(8, 8, 32, 32))
            {
                Image = image,
                Tag = 1,
            };
            capturedImageView.Layer.CornerRadius = 2.0f;
            capturedImageView.Layer.MasksToBounds = true;


            UIView imgView = new UIView(new CGRect(view.Frame.GetMaxX() - 40, 8, 32, 32))
            {
                BackgroundColor = UIColor.Clear
            };
            UIImageView imgDelete = new UIImageView(new CGRect(6, 4, 24, 24))
            {
                Image = UIImage.FromBundle("Delete")
            };
            UIView imgViewNameBox = new UIView(new CGRect(8 + 8 + 32, 8, view.Frame.Width - 32 - 8 - 8 - (8 + 8 + 32), 32))
            {
                BackgroundColor = UIColor.White
            };
            UILabel imgViewName = new UILabel(new CGRect(0, 0, imgViewNameBox.Frame.Width, 32))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.WaterBlue,
                Text = FeedbackFileNameHelper.GenerateFileName(),
            };
            imgViewNameBox.AddSubview(imgViewName);

            imgView.AddSubview(imgDelete);
            imgView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                capturedImageView.RemoveFromSuperview();
                imgView.RemoveFromSuperview();
                imgViewNameBox.RemoveFromSuperview();

            }));

            view.AddSubviews(new UIView[] { capturedImageView, imgView, imgViewNameBox });


            ActivityIndicator.Hide();
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight()).Size;
        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_viewPhotoContainer.Frame.GetMaxY() + (_btnSubmitContainer.Frame.Height + 50f)));
        }

        internal void SetButtonEnable()
        {
            bool isValidFeedback = _feedbackTextView.ValidateTextView(_feedbackTextView.Text, ANY_PATTERN) && _feedbackTextView.Text.Length != 0;
            bool isValidFields;
            //if (string.Compare(FeedbackID, "1") == 0)
            //{
                isValidFields = _billRelatedFeedbackComponent.IsValidEntry();
            //}
            //else if (string.Compare(FeedbackID, "2") == 0)
            //{
            //    isValidFields = _streetLampRelatedFeedbackComponent.IsValidEntry();
            //}
            //else
            //{
            //    isValidFields = _otherFeedbackComponent.IsValidEntry();
            //}
            bool isValid = isValidFields && isValidFeedback;
            _btnSubmit.Enabled = true; //isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }


        public CustomUIView GetIdentificationTooltipView(nfloat yLoc)
        {
            _identificationToolTipsView = new CustomUIView(new CGRect(0, yLoc, View.Frame.Width, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.Clear
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(16, 0, View.Frame.Width - (GetScaledWidth(16) * 2), GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), _identificationToolTipsView.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "How does copy of identification look like?"

            };
            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert("Copy of identification"
                    , "You’ll need a scanned copy of myKad (front and back), Passport or Army/Police ID with ‘For TNB purposes only’ written across the image. "
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null } }
                    , UIImage.FromBundle("imgIcCopy"));
            });
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            _identificationToolTipsView.AddGestureRecognizer(tapInfo);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            _identificationToolTipsView.AddSubview(viewInfo);

            return _identificationToolTipsView;
        }

        public CustomUIView GetProofConsentTooltipView(nfloat yLoc)
        {
            _proofConsentToolTipsView = new CustomUIView(new CGRect(0, yLoc, View.Frame.Width, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.Clear
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(16, 0, View.Frame.Width - (GetScaledWidth(16) * 2), GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), _proofConsentToolTipsView.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "How does proof of consent look like?"

            };
            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                DisplayCustomAlert("Proof of consent"
                    , "Accepted proof of consents can be in the form of emails, SMS, conversation screenshots or letters stating the consent being given to you."
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null } }
                    , UIImage.FromBundle("imgPoc"));
            });
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            _proofConsentToolTipsView.AddGestureRecognizer(tapInfo);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            _proofConsentToolTipsView.AddSubview(viewInfo);

            return _proofConsentToolTipsView;
        }


        private List<ImageDataEnquiryModel> GetImageList()
        {
            List<ImageDataEnquiryModel> capturedImageList = new List<ImageDataEnquiryModel>();
            UIImageHelper _imageHelper = new UIImageHelper();
            ImageDataEnquiryModel imgData;
            UIImageView imgView;
            UIImage resizedImage;

            foreach (UIView view in dashedLineView.Subviews)
            {
                if (view.Tag == 1)
                {
                    imgData = new ImageDataEnquiryModel();
                    imgView = view as UIImageView;
                    if (imgView != null)
                    {
                        resizedImage = _imageHelper.ResizeImage(imgView.Image);
                        imgData.fileType = "jpeg";
                        imgData.fileHex = _imageHelper.ConvertImageToHex(resizedImage);
                        imgData.fileSize = _imageHelper.GetImageFileSize(resizedImage).ToString();
                        imgData.fileName = FeedbackFileNameHelper.GenerateFileName();
                        capturedImageList.Add(imgData);
                    }
                }
            }

            if (dashedLineView2 != null) {
                foreach (UIView view in dashedLineView2.Subviews)
                {
                    if (view.Tag == 1)
                    {
                        imgData = new ImageDataEnquiryModel();
                        imgView = view as UIImageView;
                        if (imgView != null)
                        {
                            resizedImage = _imageHelper.ResizeImage(imgView.Image);
                            imgData.fileType = "jpeg";
                            imgData.fileHex = _imageHelper.ConvertImageToHex(resizedImage);
                            imgData.fileSize = _imageHelper.GetImageFileSize(resizedImage).ToString();
                            imgData.fileName = FeedbackFileNameHelper.GenerateFileName();
                            capturedImageList.Add(imgData);
                        }
                    }
                }
            }
            if (dashedLineView3 != null)
            {
                foreach (UIView view in dashedLineView3.Subviews)
                {
                    if (view.Tag == 1)
                    {
                        imgData = new ImageDataEnquiryModel();
                        imgView = view as UIImageView;
                        if (imgView != null)
                        {
                            resizedImage = _imageHelper.ResizeImage(imgView.Image);
                            imgData.fileType = "jpeg";
                            imgData.fileHex = _imageHelper.ConvertImageToHex(resizedImage);
                            imgData.fileSize = _imageHelper.GetImageFileSize(resizedImage).ToString();
                            imgData.fileName = FeedbackFileNameHelper.GenerateFileName();
                            capturedImageList.Add(imgData);
                        }
                    }

                }
            }
            return capturedImageList;
        }

        private void NavigateToPage()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
            UpdatePersonalDetail3ViewController viewController = storyBoard.InstantiateViewController("UpdatePersonalDetail3ViewController") as UpdatePersonalDetail3ViewController;
            viewController.IsOwner = IsOwner;
            viewController.Items = GetImageList();
            viewController.feedbackUpdateDetailsList = feedbackUpdateDetailsList;
            NavigationController.PushViewController(viewController, true);


        }
    }
}