using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.Customs;
using myTNB.Feedback;
using myTNB.Feedback.Enquiry.GeneralEnquiry;
using myTNB.Feedback.FeedbackImage;
using myTNB.Home.Bill;
using myTNB.Home.Feedback;
using myTNB.Home.Feedback.FeedbackEntry;
using myTNB.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class GeneralEnquiryViewController : CustomUIViewController
    {
        public GeneralEnquiryViewController(IntPtr handle) : base(handle) { }

        public string FeedbackID = string.Empty;
        public bool IsLoggedIn;
        public bool isMobileNumberAvailable;

        const string ANY_PATTERN = @".*";
        const int MAX_IMAGE = 2;
        const float TXTVIEW_DEFAULT_MARGIN = 24f;
        const float UNIVERSAL_MARGIN = 7f;

        private int imageWidth = 0;
        private int capturedImageCount = 0;
        private int imageCount = 0;

        internal MobileNumberComponent _mobileNumberComponent;
        private UIView _viewPhotoContainer;

        //Widgets
        private UIView _btnSubmitContainer, _viewUploadPhoto, _viewFeedback
             , _viewLineFeedback;
        private UILabel _lblPhotoTitle, _lblFeedbackTitle, _lblFeedbackSubTitle, _lblFeedbackError;
        private UIImageView _iconFeedback;
        private UIButton _btnSubmit;
        private UIScrollView _svContainer, imageContainerScroll;
        private UITapGestureRecognizer _tapImage;

        private FeedbackTextView _feedbackTextView;

        //syahmi
        private UIView _viewTitleSection, _viewTitleSection2;

        //for header
        public nfloat _previousScrollOffset;

        internal CustomUIView _navbarContainer;
        private UILabel _navbarTitle;
        private UIView _viewCommentSection;
        private CAGradientLayer _gradientLayer;
        private nfloat _navBarHeight;
        private UIView _navbarView;
        private UIImageView _bgImageView;
        private nfloat titleBarHeight;

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;

            base.ViewDidLoad();

            SetHeader();
            AddScrollView();
            AddCTA();
            AddSectionTitle();
            CreateCommentSection();
            AddSectionTitle2();
            //Should be the last to add
            CreatePhotoUploadWidget();
            UpdateContentSize();
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
            Title = GetI18NValue(EnquiryConstants.generalEnquiryTitle);
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
            _btnSubmit.SetTitle(GetCommonI18NValue("next"), UIControlState.Normal);
            _btnSubmit.Font = MyTNBFont.MuseoSans18_300;
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryMessage = _feedbackTextView.Text;
                NavigateToPage("Enquiry", "GeneralEnquiry2ViewController");
            };
            _btnSubmitContainer.AddSubview(_btnSubmit);
            View.AddSubview(_btnSubmitContainer);
        }

        private void AddSectionTitle()
        {
            if (_viewTitleSection != null) { return; }
            _viewTitleSection = new UIView(new CGRect(0, 0, View.Frame.Width, GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.enquiryAboutTitle)
            };

            _viewTitleSection.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection);
        }

        private void AddSectionTitle2()
        {
            if (_viewTitleSection2 != null) { return; }
            _viewTitleSection2 = new UIView(new CGRect(0, _viewFeedback.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.uploadDocTitle)

            };

            _viewTitleSection2.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection2);
        }
        private void CreatePhotoUploadWidget()
        {
            //Photo View
            _viewPhotoContainer = new UIView((new CGRect(0, _viewTitleSection2.Frame.GetMaxY(), View.Frame.Width, 180)))
            {
                BackgroundColor = UIColor.White
            };
            _viewUploadPhoto = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 164)))
            {
                BackgroundColor = UIColor.White
            };

            //Photo/s Title
            _lblPhotoTitle = new UILabel(new CGRect(0, 0, View.Frame.Width - 36, 16))
            {
                Text = GetI18NValue(EnquiryConstants.attachTitle),
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans14_300
            };
            _viewUploadPhoto.AddSubview(_lblPhotoTitle);

            UILabel lblPhotoSubTitle = new UILabel(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + 108 + 9 + 6
                , View.Frame.Width - 36, 14))
            {
                Text = GetI18NValue(EnquiryConstants.attachDescription),
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans11_300
            };
            _viewUploadPhoto.AddSubviews(new UIView[] { _lblPhotoTitle, lblPhotoSubTitle });
            _viewPhotoContainer.AddSubview(_viewUploadPhoto);
            _svContainer.AddSubview(_viewPhotoContainer);
            AddImageContainer();
        }

        private void AddImageContainer()
        {
            if (capturedImageCount < MAX_IMAGE)
            {
                if (imageContainerScroll == null)
                {
                    imageContainerScroll = new UIScrollView(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + UNIVERSAL_MARGIN
                    , View.Frame.Width - 36, 120 + 12))
                    {
                        ScrollEnabled = true,
                        Bounces = false,
                    };
                    _viewUploadPhoto.AddSubview(imageContainerScroll);
                }

                UIViewWithDashedLinerBorder dashedLineView = new UIViewWithDashedLinerBorder
                {
                    Frame = new CGRect(0, imageWidth, View.Frame.Width - 36, 48),
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
                    if (imageCount >= MAX_IMAGE)
                    {
                        return;
                    }

                    UIImagePickerController imgPicker = new UIImagePickerController();
                    GeneralEnquiryImagePickerDelegate imgPickerDelegate = new GeneralEnquiryImagePickerDelegate(this)
                    {
                        DashedLineView = dashedLineView
                    };
                    imgPicker.Delegate = imgPickerDelegate;

                    UIAlertController alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(EnquiryConstants.camera), UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.Camera;
                        imgPicker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(imgPicker, true, null);
                    }));

                    alert.AddAction(UIAlertAction.Create(GetI18NValue(EnquiryConstants.cameraRoll), UIAlertActionStyle.Default, (obj) =>
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
                imageContainerScroll.AddSubview(dashedLineView);
                imageWidth += 18 + 48; 
                imageContainerScroll.ContentSize = new CGRect(18, 165, 94, imageWidth).Size;
                capturedImageCount++;
            }
        }

        private void RepositionImageContent()
        {
            imageWidth = 0;
            foreach (UIView view in imageContainerScroll.Subviews)
            {

                if (view.Tag == 10)
                {
                    view.Frame = new CGRect(0, imageWidth, View.Frame.Width - 36, 48);
                    imageWidth += 18 + 48; 
                }

            }
            imageContainerScroll.ContentSize = new CGRect(18, 165, 94, imageWidth).Size;

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

            view.AddSubviews(new UIView[] { capturedImageView, imgView , imgViewNameBox});
            view.RemoveGestureRecognizer(_tapImage);
            AddImageContainer();

            ActivityIndicator.Hide();
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight()).Size;
        }

        private void CreateCommentSection()
        {
            _viewCommentSection = new UIView((new CGRect(0, _viewTitleSection.Frame.GetMaxY(), View.Frame.Width, 135)))
            {
                BackgroundColor = UIColor.White
            };

            _viewFeedback = new UIView((new CGRect(18, GetCommentSectionYCoordinate() + 16, View.Frame.Width - 36, 119)))
            {
                BackgroundColor = UIColor.White
            };

            _lblFeedbackTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedback.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.messageHint) //GetI18NValue("enquiryAboutTitle")
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = false
            };

            _feedbackTextView = new FeedbackTextView
            {
                Frame = new CGRect(TXTVIEW_DEFAULT_MARGIN, 12, View.Frame.Width - 60, 36),
                Editable = true,
                Font = MyTNBFont.MuseoSans18_300,
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.TunaGrey(),
                BackgroundColor = UIColor.Clear,
                EnablesReturnKeyAutomatically = true,
                TranslatesAutoresizingMaskIntoConstraints = true,
                ScrollEnabled = true
            };

            _iconFeedback = new UIImageView(new CGRect(0, _feedbackTextView.Frame.Height / 2, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Feedback")
            };

            _feedbackTextView.SetPlaceholder(GetI18NValue(EnquiryConstants.messageHint)); //GetI18NValue("messageHint")
            _feedbackTextView.CreateDoneButton();

            _feedbackTextView.ShouldChangeText = (txtView, range, replacementString) => //check textfield length
            {
                nint newLength = txtView.Text.Length + replacementString.Length - range.Length;
                return newLength <= 75;
            };

            _lblFeedbackError = new UILabel
            {
                Frame = new CGRect(0, _feedbackTextView.Frame.Height, _feedbackTextView.Frame.Width - 36, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetI18NValue(FeedbackConstants.I18N_InvalidFeedback)
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _viewLineFeedback = GenericLine.GetLine(new CGRect(0, _feedbackTextView.Frame.GetMaxY() + 3f, _viewFeedback.Frame.Width, 1));

            _lblFeedbackSubTitle = new UILabel(new CGRect(0, _viewLineFeedback.Frame.GetMaxY(), _feedbackTextView.Frame.Width - 36, 14))
            {
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans11_300
            };

            _viewFeedback.AddSubviews(new UIView[] { _lblFeedbackTitle, _feedbackTextView
                , _iconFeedback, _lblFeedbackError, _viewLineFeedback, _lblFeedbackSubTitle});
            SetTextViewEvents(_feedbackTextView, _lblFeedbackTitle, _lblFeedbackError,
                 _viewLineFeedback, null, ANY_PATTERN);

            _svContainer.AddSubview(_viewCommentSection);
            _svContainer.AddSubview(_viewFeedback);
        }

        private void SetTextViewEvents(FeedbackTextView textView, UILabel lblTitle
            , UILabel lblError, UIView viewLine, UILabel lblHint, string pattern)
        {
            if (lblHint == null)
            {
                lblHint = new UILabel();
            }
            _feedbackTextView.SetKeyboard();
            textView.Changed += (sender, e) =>
            {
                FeedbackTextView txtView = sender as FeedbackTextView;
                if (txtView == _feedbackTextView)
                {
                    HandleFeedbackTextViewChange();
                    CGRect frame = new CGRect();
                    frame = _feedbackTextView.Frame; 
                    frame.Height = _feedbackTextView.ContentSize.Height <= TNBGlobal.FEEDBACK_FIELD_MAX_HEIGHT
                        ? _feedbackTextView.ContentSize.Height : TNBGlobal.FEEDBACK_FIELD_MAX_HEIGHT;
                    _feedbackTextView.Frame = frame;
                    _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight() + _feedbackTextView.Frame.Height - 38f).Size; 
                    _viewFeedback.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)GetCommentSectionYCoordinate(), 16f
                        , _viewFeedback, (float)(51f + _feedbackTextView.Frame.Height)); 
                    _viewLineFeedback.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)_feedbackTextView.Frame.GetMaxY()
                        , 3f, _viewLineFeedback, (float)_viewLineFeedback.Frame.Height); 
                    _lblFeedbackSubTitle.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)_viewLineFeedback.Frame.GetMaxY()
                        , 3f, _lblFeedbackSubTitle, (float)_lblFeedbackSubTitle.Frame.Height);
                    _feedbackTextView.SetPlaceholderHidden(txtView.Text.Length > 0);
                }
                lblHint.Hidden = !lblError.Hidden || _feedbackTextView.Text.Length == 0;
                lblTitle.Hidden = _feedbackTextView.Text.Length == 0;
                SetButtonEnable();
            };
            textView.ShouldBeginEditing = (sender) =>
            {
                CGRect frame = new CGRect();
                frame = _feedbackTextView.Frame;
                _iconFeedback.Hidden = true;
                frame.X = 0.0f;
                _feedbackTextView.Frame = frame;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                lblError.Hidden = true;
                _lblFeedbackSubTitle.Hidden = false;
                textView.TextColor = MyTNBColor.TunaGrey();
                return true;
            };
            textView.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textView.Text.Length == 0;
                bool isValid = _feedbackTextView.ValidateTextView(textView.Text, pattern);

                lblError.Hidden = isValid || textView.Text.Length == 0;
                _lblFeedbackSubTitle.Hidden = !lblError.Hidden;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isValid || textView.Text.Length == 0 ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textView.TextColor = isValid || textView.Text.Length == 0 ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;

                if (textView.Text.Length == 0)
                {
                    CGRect frame = new CGRect();
                    frame = _feedbackTextView.Frame;
                    frame.X = 24f;
                    _feedbackTextView.Frame = frame;
                    _iconFeedback.Hidden = false;
                    _feedbackTextView.SetPlaceholderHidden(false);
                }

                return true;
            };
            textView.ShouldChangeText += (txtView, range, replacementString) =>
            {
                if (txtView == _feedbackTextView)
                {
                    nint newLength = textView.Text.Length + replacementString.Length - range.Length;
                    return newLength <= TNBGlobal.FeedbackMaxCharCount;
                }
                return true;
            };
        }

        private void HandleFeedbackTextViewChange()
        {
            int charCount = TNBGlobal.FeedbackMaxCharCount - _feedbackTextView.Text.Length;
            _lblFeedbackSubTitle.Text = string.Format(GetCommonI18NValue(charCount > 1
                ? Constants.Common_CharactersLeft : Constants.Common_CharacterLeft), charCount);
        }

        private nfloat GetCommentSectionYCoordinate()
        {
            return _viewTitleSection.Frame.GetMaxY();
        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_viewUploadPhoto.Frame.GetMaxY() + (_btnSubmitContainer.Frame.Height + 50f)));
        }

        internal void SetButtonEnable()
        {
            bool isValidFeedback = _feedbackTextView.ValidateTextView(_feedbackTextView.Text, ANY_PATTERN) && _feedbackTextView.Text.Length != 0;

            bool isValid = isValidFeedback;
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }


        private List<ImageDataEnquiryModel> GetImageList()
        {
            List<ImageDataEnquiryModel> capturedImageList = new List<ImageDataEnquiryModel>();
            UIImageHelper _imageHelper = new UIImageHelper();
            ImageDataEnquiryModel imgData;
            UIImageView imgView;
            UIImage resizedImage;

            foreach (UIView view in imageContainerScroll.Subviews)
            {
                if (view.Tag == 10)
                {
                    for (int i = 0; i < view.Subviews.Length; i++)
                    {
                        if (view.Subviews[i].Tag == 1)
                        {
                            imgData = new ImageDataEnquiryModel();
                            imgView = view.Subviews[i] as UIImageView;
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

        private void NavigateToPage(string storyboardName, string viewControllerName)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
            GeneralEnquiry2ViewController viewController = storyBoard.InstantiateViewController("GeneralEnquiry2ViewController") as GeneralEnquiry2ViewController;
            viewController.Items = GetImageList();
            NavigationController.PushViewController(viewController, true);
        }
    }
}