using System;
using UIKit;
using CoreGraphics;
using myTNB.Home.Feedback;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.Customs;
using myTNB.Home.Feedback.FeedbackEntry;
using myTNB.Feedback;
using Foundation;

namespace myTNB
{
    public partial class FeedbackEntryViewController : CustomUIViewController
    {
        public FeedbackEntryViewController(IntPtr handle) : base(handle) { }

        public string FeedbackID = string.Empty;
        public bool IsLoggedIn;
        public bool isMobileNumberAvailable;

        const string ANY_PATTERN = @".*";
        const int MAX_IMAGE = 2;
        const float TXTVIEW_DEFAULT_MARGIN = 24f;
        const float UNIVERSAL_MARGIN = 7f;
        const float VIEW_PHOTO_MARGIN = 20f;

        private int imageWidth = 0;
        private int capturedImageCount = 0;
        private int imageCount = 0;

        internal MobileNumberComponent _mobileNumberComponent;

        //Widgets
        private UIView _btnSubmitContainer, _viewUploadPhoto, _feedbackCategoryView, _viewFeedback
             , _viewLineFeedback;
        private UILabel _lblPhotoTitle, _lblFeedbackTitle, _lblFeedbackSubTitle, _lblFeedbackError;
        private UIImageView _iconFeedback;
        private UIButton _btnSubmit;
        private UIScrollView _svContainer, imageContainerScroll;
        private UITapGestureRecognizer _tapImage;

        private FeedbackTextView _feedbackTextView;
        private OtherFeedbackComponent _otherFeedbackComponent;
        private BillRelatedFeedbackComponent _billRelatedFeedbackComponent;
        public StreetLampFeedbackComponent _streetLampRelatedFeedbackComponent;

        private SubmitFeedbackResponseModel _submitFeedback;

        public override void ViewDidLoad()
        {
            PageName = FeedbackConstants.Pagename_FeedbackForm;
            base.ViewDidLoad();
            NotifCenterUtility.AddObserver((NSString)"OnCountrySelected", OnCountrySelected);
            CheckMobileNumber();
            _otherFeedbackComponent = new OtherFeedbackComponent(this);
            _billRelatedFeedbackComponent = new BillRelatedFeedbackComponent(this);
            _streetLampRelatedFeedbackComponent = new StreetLampFeedbackComponent(this);
            SetHeader();
            AddScrollView();
            AddCTA();
            InitializeFeedbackComponents();
            CreateCommentSection();
            //Should be the last to add
            CreatePhotoUploadWidget();
            UpdateContentSize();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (string.Compare(FeedbackID, "1") == 0)
            {
                _billRelatedFeedbackComponent.SetSelectedAccountNumber();
            }
            else if (string.Compare(FeedbackID, "2") == 0)
            {
                _streetLampRelatedFeedbackComponent.SetState();
            }
            else if (string.Compare(FeedbackID, "3") == 0)
            {
                _otherFeedbackComponent.SetFeedbackType();
            }
        }

        private void OnCountrySelected(NSNotification obj)
        {
            NSDictionary userInfo = obj.UserInfo;
            CountryModel countryInfo = new CountryModel
            {
                CountryCode = userInfo.ValueForKey(new NSString("CountryCode")).ToString(),
                CountryName = userInfo.ValueForKey(new NSString("CountryName")).ToString(),
                CountryISDCode = userInfo.ValueForKey(new NSString("CountryISDCode")).ToString()
            };
            if (_mobileNumberComponent != null)
            {
                _mobileNumberComponent.CountryShortCode = countryInfo.CountryCode;
                _mobileNumberComponent.CountryCode = countryInfo.CountryISDCode;
                _mobileNumberComponent.ClearField();
                SetButtonEnable();
            }
        }

        private void CheckMobileNumber()
        {
            isMobileNumberAvailable = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                && !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.UserEntity[0]?.mobileNo)
                && !string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.UserEntity[0]?.mobileNo);
        }

        private void SetHeader()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            Title = DataManager.DataManager.SharedInstance.FeedbackCategory?.Find(x => x?.FeedbackCategoryId == FeedbackID)?.FeedbackCategoryName;
        }

        private void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
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
            _btnSubmit.SetTitle(GetCommonI18NValue(Constants.Common_Submit), UIControlState.Normal);
            _btnSubmit.Font = MyTNBFont.MuseoSans18_300;
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = MyTNBColor.SilverChalice;
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                ExecuteSubmitFeedback();
            };
            _btnSubmitContainer.AddSubview(_btnSubmit);
            View.AddSubview(_btnSubmitContainer);
        }

        private void CreatePhotoUploadWidget()
        {
            //Photo View
            _viewUploadPhoto = new UIView((new CGRect(18, (nfloat)(GetPhotoWidgetYCoordinate() + VIEW_PHOTO_MARGIN)
                , View.Frame.Width - 36, 180)))
            {
                BackgroundColor = UIColor.Clear
            };

            //Photo/s Title
            _lblPhotoTitle = new UILabel(new CGRect(0, 0, View.Frame.Width - 36, 14))
            {
                Text = GetI18NValue(FeedbackConstants.I18N_AttachPhotoTitle),
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans11_300
            };
            _viewUploadPhoto.AddSubview(_lblPhotoTitle);

            UILabel lblPhotoSubTitle = new UILabel(new CGRect(0, _lblPhotoTitle.Frame.GetMaxY() + 108
                , View.Frame.Width - 36, 14))
            {
                Text = GetI18NValue(FeedbackConstants.I18N_MaxFile),
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans11_300
            };
            _viewUploadPhoto.AddSubviews(new UIView[] { _lblPhotoTitle, lblPhotoSubTitle });
            _svContainer.AddSubview(_viewUploadPhoto);
            AddImageContainer();
        }

        private nfloat GetPhotoWidgetYCoordinate()
        {
            nfloat yCoord = 0.0f;
            yCoord = GetFeedbackCategoryViewHeight();
            yCoord += _viewFeedback?.Frame.Height ?? 0;
            return yCoord;
        }

        private void AddImageContainer()
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

                UIViewWithDashedLinerBorder dashedLineView = new UIViewWithDashedLinerBorder
                {
                    Frame = new CGRect(imageWidth, 0, 94, 94),
                    BackgroundColor = UIColor.White
                };
                dashedLineView.Layer.CornerRadius = 5.0f;
                dashedLineView.Tag = 10;

                UIImageView imgViewAdd = new UIImageView(new CGRect(35, 35, 24, 24))
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
                    ImagePickerDelegate imgPickerDelegate = new ImagePickerDelegate(this)
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
                imageContainerScroll.AddSubview(dashedLineView);
                imageWidth += 18 + 94;
                imageContainerScroll.ContentSize = new CGRect(18, 165, imageWidth, 94).Size;
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

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight()).Size;
        }

        private void CreateCommentSection()
        {
            _viewFeedback = new UIView((new CGRect(18, GetCommentSectionYCoordinate() + 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblFeedbackTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedback.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(GetI18NValue(FeedbackConstants.I18N_Feedback)
                    , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
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
                ScrollEnabled = true,
            };

            _iconFeedback = new UIImageView(new CGRect(0, _feedbackTextView.Frame.Height / 2, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Feedback")
            };

            _feedbackTextView.SetPlaceholder(GetI18NValue(FeedbackConstants.I18N_Feedback));
            _feedbackTextView.CreateDoneButton();

            _feedbackTextView.ShouldChangeText = (txtView, range, replacementString) =>
            {
                nint newLength = txtView.Text.Length + replacementString.Length - range.Length;
                return newLength <= 250;
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
                    _viewFeedback.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)GetCommentSectionYCoordinate(), 0f
                        , _viewFeedback, (float)(51f + _feedbackTextView.Frame.Height));
                    _viewLineFeedback.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)_feedbackTextView.Frame.GetMaxY()
                        , 3f, _viewLineFeedback, (float)_viewLineFeedback.Frame.Height);
                    _lblFeedbackSubTitle.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)_viewLineFeedback.Frame.GetMaxY()
                        , 3f, _lblFeedbackSubTitle, (float)_lblFeedbackSubTitle.Frame.Height);
                    _viewUploadPhoto.Frame = ViewHelper.UpdateFeedbackViewYCoord((float)(_viewFeedback.Frame.Y + _viewFeedback.Frame.Height)
                        , 0f, _viewUploadPhoto, (float)_viewUploadPhoto.Frame.Height);

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
            return _feedbackCategoryView?.Frame.Height ?? 0.0f;
        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_viewUploadPhoto.Frame.GetMaxY() + (_btnSubmitContainer.Frame.Height + 50f)));
        }

        private void InitializeFeedbackComponents()
        {
            if (string.Compare(FeedbackID, "1") == 0)
            {
                _feedbackCategoryView = _billRelatedFeedbackComponent.GetComponent();
            }
            else if (string.Compare(FeedbackID, "2") == 0)
            {
                _feedbackCategoryView = _streetLampRelatedFeedbackComponent.GetComponent();
            }
            else
            {
                _feedbackCategoryView = _otherFeedbackComponent.GetComponent();
            }
            _svContainer.AddSubview(_feedbackCategoryView);
        }

        private nfloat GetFeedbackCategoryViewHeight()
        {
            return _feedbackCategoryView?.Frame.Height ?? 0;
        }

        internal void SetButtonEnable()
        {
            bool isValidFeedback = _feedbackTextView.ValidateTextView(_feedbackTextView.Text, ANY_PATTERN) && _feedbackTextView.Text.Length != 0;
            bool isValidFields;
            if (string.Compare(FeedbackID, "1") == 0)
            {
                isValidFields = _billRelatedFeedbackComponent.IsValidEntry();
            }
            else if (string.Compare(FeedbackID, "2") == 0)
            {
                isValidFields = _streetLampRelatedFeedbackComponent.IsValidEntry();
            }
            else
            {
                isValidFields = _otherFeedbackComponent.IsValidEntry();
            }
            bool isValid = isValidFields && isValidFeedback;
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        private void ExecuteSubmitFeedback()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        _submitFeedback = new SubmitFeedbackResponseModel();
                        SubmitFeedback().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_submitFeedback != null && _submitFeedback?.d != null
                                   && _submitFeedback.d.IsSuccess && _submitFeedback?.d?.data != null)
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                                    GenericStatusPageViewController status = storyBoard.InstantiateViewController("GenericStatusPageViewController") as GenericStatusPageViewController;
                                    status.IsSuccess = true;
                                    status.ReferenceNumber = _submitFeedback?.d?.data?.ServiceReqNo;
                                    status.ReferenceDate = _submitFeedback?.d?.data?.DateCreated;
                                    status.StatusDisplayType = GenericStatusPageViewController.StatusType.Feedback;
                                    NavigationController.PushViewController(status, true);
                                }
                                else
                                {
                                    DisplayServiceError(_submitFeedback?.d?.DisplayMessage ?? string.Empty);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private object GetRequestParameters()
        {
            ServiceManager serviceManager = new ServiceManager();
            return new
            {
                serviceManager.usrInf,
                serviceManager.deviceInf,
                feedbackCategoryId = FeedbackID,
                feedbackTypeId = GetFeedbackTypeID(),
                accountNum = GetAccountNumber(),
                name = GetName(),
                phoneNum = GetMobileNumber(),
                feedbackMesage = _feedbackTextView.Text, //Common
                stateId = GetState(),
                location = GetLocation(),
                poleNum = GetPoleNumber(),
                images = GetImageList() //Common
            };
        }

        private Task SubmitFeedback()
        {
            object requestParameter = GetRequestParameters();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                _submitFeedback = serviceManager.OnExecuteAPIV6<SubmitFeedbackResponseModel>(FeedbackConstants.Service_SubmitFeedback, requestParameter);
            });
        }

        private List<ImageDataModel> GetImageList()
        {
            List<ImageDataModel> capturedImageList = new List<ImageDataModel>();
            UIImageHelper _imageHelper = new UIImageHelper();
            ImageDataModel imgData;
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
                            imgData = new ImageDataModel();
                            imgView = view.Subviews[i] as UIImageView;
                            resizedImage = _imageHelper.ResizeImage(imgView.Image);
                            imgData.imageHex = _imageHelper.ConvertImageToHex(resizedImage);
                            imgData.filesize = _imageHelper.GetImageFileSize(resizedImage).ToString();
                            imgData.fileName = FeedbackFileNameHelper.GenerateFileName();
                            capturedImageList.Add(imgData);
                        }
                    }
                }
            }
            return capturedImageList;
        }

        private string GetFeedbackTypeID()
        {
            if (string.Compare(FeedbackID, "3") == 0
                && DataManager.DataManager.SharedInstance.OtherFeedbackType != null)
            {
                return DataManager.DataManager.SharedInstance.OtherFeedbackType[DataManager.DataManager
                    .SharedInstance.CurrentSelectedFeedbackTypeIndex]?.FeedbackTypeId ?? string.Empty;
            }
            return string.Empty;
        }

        private string GetAccountNumber()
        {
            if (string.Compare(FeedbackID, "1") == 0)
            {
                return _billRelatedFeedbackComponent.GetAccountNumber() ?? string.Empty;
            }
            return string.Empty;
        }

        private string GetName()
        {
            if (IsLoggedIn)
            {
                SQLite.SQLiteDataManager.UserEntity user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                    ? DataManager.DataManager.SharedInstance.UserEntity[0]
                    : new SQLite.SQLiteDataManager.UserEntity();
                return user?.userName ?? string.Empty;
            }
            else
            {
                if (string.Compare(FeedbackID, "1") == 0)
                {
                    return _billRelatedFeedbackComponent.GetFullName();
                }
                else if (string.Compare(FeedbackID, "2") == 0)
                {
                    return _streetLampRelatedFeedbackComponent.GetFullName();
                }
                else
                {
                    return _otherFeedbackComponent.GetFullName();
                }
            }
        }

        private string GetMobileNumber()
        {
            if (IsLoggedIn && isMobileNumberAvailable)
            {
                SQLite.SQLiteDataManager.UserEntity user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                    ? DataManager.DataManager.SharedInstance.UserEntity[0]
                    : new SQLite.SQLiteDataManager.UserEntity();
                return user?.mobileNo ?? string.Empty;
            }
            else
            {
                if (string.Compare(FeedbackID, "1") == 0)
                {
                    return _billRelatedFeedbackComponent.GetMobileNumber();
                }
                else if (string.Compare(FeedbackID, "2") == 0)
                {
                    return _streetLampRelatedFeedbackComponent.GetMobileNumber();
                }
                else
                {
                    return _otherFeedbackComponent.GetMobileNumber();
                }
            }
        }

        private string GetEmailAddress()
        {
            if (IsLoggedIn)
            {
                SQLite.SQLiteDataManager.UserEntity user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                    ? DataManager.DataManager.SharedInstance.UserEntity[0]
                    : new SQLite.SQLiteDataManager.UserEntity();
                return user?.email ?? string.Empty;
            }
            else
            {
                if (string.Compare(FeedbackID, "1") == 0)
                {
                    return _billRelatedFeedbackComponent.GetEmail();
                }
                else if (string.Compare(FeedbackID, "2") == 0)
                {
                    return _streetLampRelatedFeedbackComponent.GetEmail();
                }
                else
                {
                    return _otherFeedbackComponent.GetEmail();
                }
            }
        }

        private string GetState()
        {
            if (string.Compare(FeedbackID, "2") == 0)
            {
                return _streetLampRelatedFeedbackComponent.GetState();
            }
            return string.Empty;
        }

        private string GetLocation()
        {
            if (string.Compare(FeedbackID, "2") == 0)
            {
                return _streetLampRelatedFeedbackComponent.GetLocation();
            }
            return string.Empty;
        }

        private string GetPoleNumber()
        {
            if (string.Compare(FeedbackID, "2") == 0)
            {
                return _streetLampRelatedFeedbackComponent.GetPoleNumber();
            }
            return string.Empty;
        }
    }
}