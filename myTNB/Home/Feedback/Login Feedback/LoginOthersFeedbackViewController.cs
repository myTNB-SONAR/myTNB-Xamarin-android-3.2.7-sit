using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Model;
using System.Threading.Tasks;
using myTNB.Home.Feedback;
using System.Collections.Generic;
using System.Drawing;

namespace myTNB
{
    public partial class LoginOthersFeedbackViewController : UIViewController
    {
        public LoginOthersFeedbackViewController(IntPtr handle) : base(handle)
        {
        }

        UILabel _lblFeedbackType;
        UILabel _lblFeedbackTypeTitle;
        UILabel _lblFeedbackTypeError;
        UIView _viewFeedbackType;
        UIView _viewLineFeedbackType;

        UITextField _txtFieldFeedback;
        UILabel _lblFeedbackTitle;
        UILabel _lblFeedbackSubTitle;
        UILabel _lblFeedbackError;
        UIView _viewFeedback;
        UIView _viewLineFeedback;

        UITextField _txtFieldMobileNo;
        UILabel _lblMobileNoTitle;
        UILabel _lblMobileNoError;
        UIView _viewMobileNo;
        UIView _viewLineMobileNo;

        UILabel lblMobileNoHint;

        UIButton _btnSubmit;

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        SubmitFeedbackResponseModel _submitFeedback = new SubmitFeedbackResponseModel();

        const string ANY_PATTERN = @".*";
        const string MOBILE_NO_PATTERN = @"^[0-9 \+]+$";

        string _feedbackMessage = string.Empty;
        string _mobileNo = string.Empty;

        List<ImageDataModel> _capturedImageList = new List<ImageDataModel>();
        UIImageHelper _imageHelper = new UIImageHelper();

        UIScrollView ScrollView;
        UIScrollView imageContainerScroll;
        int imageWidth = 0;
        int capturedImageCount = 0;
        int imageCount = 0;
        const int MAX_IMAGE = 2;

        float _viewMobileNoYCoord = 83f;
        float _viewFeedbackYCoord = 83f;
        float _lblPhotoTitleYCoord = 140f;
        float _imageContainerScrollYCoord = 165f;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InitializedSubviews();
            AddBackButton();
            SetEvents();
            SetVisibility();
            AddImageContainer();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (DataManager.DataManager.SharedInstance.OtherFeedbackType != null
                && DataManager.DataManager.SharedInstance.OtherFeedbackType.d != null
                && DataManager.DataManager.SharedInstance.OtherFeedbackType.d.data != null
                && DataManager.DataManager.SharedInstance.OtherFeedbackType.d.data.Count > 0)
            {
                _lblFeedbackType.Text = DataManager.DataManager.SharedInstance.OtherFeedbackType.d.data[DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex].FeedbackTypeName;
            }
        }

        internal void InitializedSubviews()
        {
            //Scrollview
            ScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            ScrollView.BackgroundColor = UIColor.Clear;
            View.AddSubview(ScrollView);

            //Feedback Type
            _viewFeedbackType = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)));
            _viewFeedbackType.BackgroundColor = UIColor.Clear;

            _lblFeedbackTypeTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedbackType.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "FEEDBACK TYPE",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedbackType.AddSubview(_lblFeedbackTypeTitle);

            _lblFeedbackTypeError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFeedbackType.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                            "Invalid feedback type",
                                               font: myTNBFont.MuseoSans9(),
                                            foregroundColor: myTNBColor.Tomato(),
                                            strokeWidth: 0
                                           ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedbackType.AddSubview(_lblFeedbackTypeError);


            UIImageView imgViewAccountNumber = new UIImageView(new CGRect(0, 12, 24, 24));
            imgViewAccountNumber.Image = UIImage.FromBundle("IC-FieldFeedback");
            _viewFeedbackType.AddSubview(imgViewAccountNumber);

            _lblFeedbackType = new UILabel(new CGRect(30, 12, _viewFeedbackType.Frame.Width, 24));
            _lblFeedbackType.AttributedText = new NSAttributedString(
                                            "Feedback type",
                                               font: myTNBFont.MuseoSans16(),
                                            foregroundColor: myTNBColor.SilverChalice(),
                                            strokeWidth: 0
            );//Noted: Temp Number, will create a list for this later.

            _viewFeedbackType.AddSubview(_lblFeedbackType);

            UIImageView imgDropDown = new UIImageView(new CGRect(_viewFeedbackType.Frame.Width - 30, 12, 24, 24));
            imgDropDown.Image = UIImage.FromBundle("IC-Action-Dropdown");
            _viewFeedbackType.AddSubview(imgDropDown);

            _viewLineFeedbackType = new UIView((new CGRect(0, 36, _viewFeedbackType.Frame.Width, 1)));
            _viewLineFeedbackType.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewFeedbackType.AddSubview(_viewLineFeedbackType);

            UITapGestureRecognizer tapFeedbackType = new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("FeedbackTableView", null);
                FeedbackTypeViewController feedbackTypeVC =
                    storyBoard.InstantiateViewController("FeedbackTypeViewController") as FeedbackTypeViewController;
                feedbackTypeVC._feedbackTypeList = DataManager.DataManager.SharedInstance.OtherFeedbackType.d.data;
                NavigationController.PushViewController(feedbackTypeVC, true);

            });
            _viewFeedbackType.AddGestureRecognizer(tapFeedbackType);

            //Elbert
            if (DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo == null || DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo == string.Empty)
            {
                _viewFeedbackYCoord += 67f;
                _lblPhotoTitleYCoord += 67f;
                _imageContainerScrollYCoord += 67f;

                //Mobile no.
                _viewMobileNo = new UIView((new CGRect(18, _viewMobileNoYCoord, View.Frame.Width - 36, 51)));
                _viewMobileNo.BackgroundColor = UIColor.Clear;

                _lblMobileNoTitle = new UILabel
                {
                    Frame = new CGRect(0, 0, _viewMobileNo.Frame.Width, 12),
                    AttributedText = new NSAttributedString(
                                                        "MOBILE NO.",
                                                           font: myTNBFont.MuseoSans9(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                        strokeWidth: 0
                                                       ),
                    TextAlignment = UITextAlignment.Left
                };
                _viewMobileNo.AddSubview(_lblMobileNoTitle);

                _lblMobileNoError = new UILabel
                {
                    Frame = new CGRect(0, 37, _viewMobileNo.Frame.Width, 14),
                    AttributedText = new NSAttributedString(
                                            "Invalid mobile no.",
                                               font: myTNBFont.MuseoSans9(),
                                            foregroundColor: myTNBColor.Tomato(),
                                            strokeWidth: 0
                                           ),
                    TextAlignment = UITextAlignment.Left
                };
                _viewMobileNo.AddSubview(_lblMobileNoError);

                lblMobileNoHint = new UILabel
                {
                    Frame = new CGRect(0, 37, _viewMobileNo.Frame.Width, 14),
                    AttributedText = new NSAttributedString(
                    "Please include the country code of your phone number.",
                    font: myTNBFont.MuseoSans9(),
                    foregroundColor: myTNBColor.TunaGrey(),
                    strokeWidth: 0
                ),
                    TextAlignment = UITextAlignment.Left
                };
                lblMobileNoHint.Hidden = true;
                _viewMobileNo.AddSubview(lblMobileNoHint);

                _txtFieldMobileNo = new UITextField
                {
                    Frame = new CGRect(0, 12, _viewMobileNo.Frame.Width, 24),
                    AttributedPlaceholder = new NSAttributedString(
                                                         "Mobile no.",
                                                           font: myTNBFont.MuseoSans16(),
                                                            foregroundColor: myTNBColor.SilverChalice(),
                                                           strokeWidth: 0
                                                        ),
                    TextColor = myTNBColor.TunaGrey()
                };
                _txtFieldMobileNo.KeyboardType = UIKeyboardType.NumberPad;
                CreateDoneButton(_txtFieldMobileNo);
                _viewMobileNo.AddSubview(_txtFieldMobileNo);

                _viewLineMobileNo = new UIView((new CGRect(0, 36, _viewMobileNo.Frame.Width, 1)));
                _viewLineMobileNo.BackgroundColor = myTNBColor.PlatinumGrey();
                _viewMobileNo.AddSubview(_viewLineMobileNo);

                _lblMobileNoError.Hidden = true;
                _lblMobileNoTitle.Hidden = true;

                SetTextFieldEvents(_txtFieldMobileNo, _lblMobileNoTitle
                                   , _lblMobileNoError, _viewLineMobileNo
                                   , lblMobileNoHint, MOBILE_NO_PATTERN);
                _textFieldHelper.CreateTextFieldLeftView(_txtFieldMobileNo, "Mobile");

                ScrollView.AddSubview(_viewMobileNo);
            }

            //Feedback

            _viewFeedback = new UIView((new CGRect(18, _viewFeedbackYCoord, View.Frame.Width - 36, 51)));
            _viewFeedback.BackgroundColor = UIColor.Clear;

            _lblFeedbackTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedback.Frame.Width, 12),
                AttributedText = new NSAttributedString(
                                                    "FEEDBACK",
                                                       font: myTNBFont.MuseoSans9(),
                                                    foregroundColor: myTNBColor.SilverChalice(),
                                                    strokeWidth: 0
                                                   ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedback.AddSubview(_lblFeedbackTitle);

            _lblFeedbackError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewFeedback.Frame.Width, 14),
                AttributedText = new NSAttributedString(
                                        "Invalid feedback",
                                           font: myTNBFont.MuseoSans9(),
                                        foregroundColor: myTNBColor.Tomato(),
                                        strokeWidth: 0
                                       ),
                TextAlignment = UITextAlignment.Left
            };
            _viewFeedback.AddSubview(_lblFeedbackError);

            _txtFieldFeedback = new UITextField
            {
                Frame = new CGRect(0, 12, _viewFeedback.Frame.Width, 24),
                AttributedPlaceholder = new NSAttributedString(
                                                     "Feedback",
                                                       font: myTNBFont.MuseoSans16(),
                                                        foregroundColor: myTNBColor.SilverChalice(),
                                                       strokeWidth: 0
                                                    ),
                TextColor = myTNBColor.TunaGrey()
            };
            _viewFeedback.AddSubview(_txtFieldFeedback);

            _viewLineFeedback = new UIView((new CGRect(0, 36, _viewFeedback.Frame.Width, 1)));
            _viewLineFeedback.BackgroundColor = myTNBColor.PlatinumGrey();
            _viewFeedback.AddSubview(_viewLineFeedback);

            _lblFeedbackSubTitle = new UILabel(new CGRect(18, _viewFeedback.Frame.Y + 40, View.Frame.Width - 36, 14));
            _lblFeedbackSubTitle.TextColor = myTNBColor.SilverChalice();
            _lblFeedbackSubTitle.Font = myTNBFont.MuseoSans9();
            HandleFeedbackTextChange();
            ScrollView.AddSubview(_lblFeedbackSubTitle);

            //Photo/s Title
            var lblPhotoTitle = new UILabel(new CGRect(18, _lblPhotoTitleYCoord, View.Frame.Width - 36, 14));
            lblPhotoTitle.Text = "ATTACH PHOTO / SCREENSHOT (OPTIONAL)";
            lblPhotoTitle.TextColor = myTNBColor.SilverChalice();
            lblPhotoTitle.Font = myTNBFont.MuseoSans9();

            var lblPhotoSubTitle = new UILabel(new CGRect(18, lblPhotoTitle.Frame.Y + 120, View.Frame.Width - 36, 14));
            lblPhotoSubTitle.Text = "Max 2 files";
            lblPhotoSubTitle.TextColor = myTNBColor.SilverChalice();
            lblPhotoSubTitle.Font = myTNBFont.MuseoSans9();

            //Submit Button
            _btnSubmit = new UIButton(UIButtonType.Custom);
            _btnSubmit.Frame = new CGRect(18, View.Frame.Height - (DeviceHelper.IsIphoneX() ? 152 : 128), View.Frame.Width - 36, 48);
            _btnSubmit.SetTitle("Submit", UIControlState.Normal);
            _btnSubmit.Font = myTNBFont.MuseoSans16();
            _btnSubmit.Layer.CornerRadius = 5.0f;
            _btnSubmit.BackgroundColor = myTNBColor.FreshGreen();
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                ExecuteSubmitFeedback();
            };

            //ScrollView main subviews
            ScrollView.AddSubview(_viewFeedbackType);
            ScrollView.AddSubview(_viewFeedback);
            ScrollView.AddSubview(lblPhotoTitle);
            ScrollView.AddSubview(lblPhotoSubTitle);
            ScrollView.AddSubview(_btnSubmit);

            //Scrollview content size
            ScrollView.ContentSize = new CGRect(0f, 0f, View.Frame.Width, 900f).Size;

            _textFieldHelper.CreateTextFieldLeftView(_txtFieldFeedback, "IC-Feedback");

            _btnSubmit.Enabled = false;
            _btnSubmit.BackgroundColor = myTNBColor.SilverChalice();
        }

        internal void SetTextFieldEvents(UITextField textField, UILabel lblTitle
                                         , UILabel lblError, UIView viewLine
                                         , UILabel lblHint, string pattern)
        {
            if (lblHint == null)
            {
                lblHint = new UILabel();
            }
            _textFieldHelper.SetKeyboard(textField);
            textField.EditingChanged += (sender, e) =>
            {
                UITextField txtField = sender as UITextField;
                if (txtField == _txtFieldFeedback)
                {
                    HandleFeedbackTextChange();
                }
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
                lblTitle.Hidden = textField.Text.Length == 0;
                SubmitButtonEnable();
            };
            textField.EditingDidBegin += (sender, e) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length == 0)
                    {
                        textField.Text += "+60 ";
                    }
                }
                lblHint.Hidden = lblError.Hidden ? textField.Text.Length == 0 : true;
                lblTitle.Hidden = textField.Text.Length == 0;
            };
            textField.ShouldEndEditing = (sender) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                }
                lblTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);

                lblError.Hidden = isValid || textField.Text.Length == 0;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isValid || textField.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textField.TextColor = isValid || textField.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();

                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                }
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters += (txtField, range, replacementString) =>
            {
                if (txtField == _txtFieldMobileNo)
                {
                    string content = ((UITextField)txtField).Text;
                    string preffix = string.Empty;
                    if (content.Length == 1)
                    {
                        preffix = content.Substring(0, 1);
                        if (preffix.Equals("+") && replacementString.Equals(string.Empty))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else if (txtField == _txtFieldFeedback)
                {
                    var newLength = textField.Text.Length + replacementString.Length - range.Length;
                    return newLength <= TNBGlobal.FeedbackMaxCharCount;
                }
                return true;
            };
        }

        internal void SubmitButtonEnable()
        {

            bool isValidFeedback = _textFieldHelper.ValidateTextField(_txtFieldFeedback.Text, ANY_PATTERN) && _txtFieldFeedback.Text.Length != 0;
            bool isValid = false;

            if (DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo == null || DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo == string.Empty)
            {
                bool isValidMobileNo = _textFieldHelper.ValidateTextField(_txtFieldMobileNo.Text, MOBILE_NO_PATTERN) && _txtFieldMobileNo.Text.Length != 0;
                isValid = isValidFeedback && isValidMobileNo;
            }
            else
            {
                isValid = isValidFeedback;
            }
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        internal void SetEvents()
        {
            SetTextFieldEvents(_txtFieldFeedback, _lblFeedbackTitle
                               , _lblFeedbackError, _viewLineFeedback
                               , null, ANY_PATTERN);
        }

        internal void SetVisibility()
        {
            _lblFeedbackTypeTitle.Hidden = true;
            _lblFeedbackTitle.Hidden = true;

            _lblFeedbackTypeError.Hidden = true;
            _lblFeedbackError.Hidden = true;
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.DismissViewController(true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        void ExecuteSubmitFeedback()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        _feedbackMessage = _txtFieldFeedback.Text;

                        if (DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo == null || DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo == string.Empty)
                        {
                            _mobileNo = _txtFieldMobileNo.Text;
                        }
                        else
                        {
                            _mobileNo = DataManager.DataManager.SharedInstance.UserEntity[0].mobileNo;
                        }

                        _capturedImageList = new List<ImageDataModel>();
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
                                        _capturedImageList.Add(imgData);
                                    }
                                }
                            }
                        }

                        SubmitFeedback().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_submitFeedback != null && _submitFeedback.d != null
                                   && _submitFeedback.d.isError.Equals("false")
                                   && _submitFeedback.d.data != null)
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                                    SubmitFeedbackSuccessViewController submitFeedbackSuccessVC =
                                        storyBoard.InstantiateViewController("SubmitFeedbackSuccessViewController") as SubmitFeedbackSuccessViewController;
                                    submitFeedbackSuccessVC.ServiceReqNo = _submitFeedback.d.data.ServiceReqNo;
                                    submitFeedbackSuccessVC.DateCreated = _submitFeedback.d.data.DateCreated;
                                    NavigationController.PushViewController(submitFeedbackSuccessVC, true);
                                }
                                else
                                {
                                    UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                                    SubmitFeedbackFailedViewController submitFeedbackFailedVC =
                                        storyBoard.InstantiateViewController("SubmitFeedbackFailedViewController") as SubmitFeedbackFailedViewController;
                                    NavigationController.PushViewController(submitFeedbackFailedVC, true);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                });
            });
        }

        internal void AddImageContainer()
        {
            if (capturedImageCount < MAX_IMAGE)
            {
                if (imageContainerScroll == null)
                {
                    imageContainerScroll = new UIScrollView(new CGRect(18, _imageContainerScrollYCoord, View.Frame.Width - 36, 94));
                    imageContainerScroll.ScrollEnabled = true;
                    imageContainerScroll.Bounces = false;
                    imageContainerScroll.DirectionalLockEnabled = true;
                    ScrollView.AddSubview(imageContainerScroll);
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

                dashedLineView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    UIImagePickerController imgPicker = new UIImagePickerController();
                    ImagePickerDelegate imgPickerDelegate = new ImagePickerDelegate(this);
                    imgPickerDelegate.Type = Enums.FeedbackCategory.LoginOthers;
                    imgPickerDelegate.DashedLineView = dashedLineView;
                    imgPicker.Delegate = imgPickerDelegate;

                    var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

                    alert.AddAction(UIAlertAction.Create("Camera", UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.Camera;
                        PresentViewController(imgPicker, true, null);
                    }));

                    alert.AddAction(UIAlertAction.Create("Camera Roll", UIAlertActionStyle.Default, (obj) =>
                    {
                        imgPicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                        PresentViewController(imgPicker, true, null);
                    }));

                    var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
                    alert.AddAction(cancelAction);
                    PresentViewController(alert, animated: true, completionHandler: null);
                }));

                imageContainerScroll.AddSubview(dashedLineView);
                imageWidth += 18 + 94;
                imageContainerScroll.ContentSize = new CGRect(18, 165, imageWidth, 94).Size;
                capturedImageCount++;
            }
        }

        internal void RepositionImageContent()
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
            UIImageView capturedImageView = new UIImageView(new CGRect(0, 0, view.Frame.Width, view.Frame.Height));
            capturedImageView.Image = image;
            capturedImageView.Tag = 1;

            UIImageView imgDelete = new UIImageView(new CGRect(65, 5, 24, 24));
            imgDelete.Image = UIImage.FromBundle("Delete");

            imageCount++;
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
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

            view.AddSubviews(new UIView[] { capturedImageView, imgDelete });
            AddImageContainer();

            ActivityIndicator.Hide();
        }

        Task SubmitFeedback()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    feedbackCategoryId = "3",
                    feedbackTypeId = DataManager.DataManager.SharedInstance.OtherFeedbackType.d.data[DataManager.DataManager.SharedInstance.CurrentSelectedFeedbackTypeIndex].FeedbackTypeId,
                    accountNum = string.Empty,
                    name = DataManager.DataManager.SharedInstance.UserEntity[0].displayName,
                    phoneNum = _mobileNo,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    deviceId = DataManager.DataManager.SharedInstance.UDID,
                    feedbackMesage = _feedbackMessage,
                    stateId = string.Empty,
                    location = string.Empty,
                    poleNum = string.Empty,
                    images = _capturedImageList
                };
                _submitFeedback = serviceManager.SubmitFeedback("SubmitFeedback", requestParameter);
            });
        }

        void CreateDoneButton(UITextField textField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                if (textField == _txtFieldMobileNo)
                {
                    if (textField.Text.Length < 4)
                    {
                        textField.Text = string.Empty;
                    }
                }
                textField.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            textField.InputAccessoryView = toolbar;
        }

        /// <summary>
        /// Handles the feedback text change.
        /// </summary>
        private void HandleFeedbackTextChange()
        {
            int charCount = TNBGlobal.FeedbackMaxCharCount - _txtFieldFeedback.Text.Length;
            string text = string.Format("{0} character{1} left", charCount, charCount != 1 ? "s" : string.Empty);
            _lblFeedbackSubTitle.Text = text;
        }
    }
}