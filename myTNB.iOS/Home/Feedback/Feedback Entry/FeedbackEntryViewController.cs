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

        const string ANY_PATTERN = @".*";
        const int MAX_IMAGE = 2;
        const float TXTVIEW_DEFAULT_MARGIN = 24f;
        const float UNIVERSAL_MARGIN = 7f;
        const float VIEW_PHOTO_MARGIN = 20f;

        int imageWidth = 0;
        int capturedImageCount = 0;
        int imageCount = 0;

        //Widgets
        UIView _btnSubmitContainer, _viewUploadPhoto, _feedbackCategoryView, _viewFeedback
            , _viewLineFeedback;
        UILabel _lblPhotoTitle, _lblFeedbackTitle, _lblFeedbackSubTitle, _lblFeedbackError;
        UIImageView _iconFeedback;
        UIButton _btnSubmit;
        UIScrollView _svContainer, imageContainerScroll;
        UITapGestureRecognizer _tapImage;

        FeedbackTextView _feedbackTextView;
        OtherFeedbackComponent _otherFeedbackComponent;
        BillRelatedFeedbackComponent _billRelatedFeedbackComponent;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _otherFeedbackComponent = new OtherFeedbackComponent(this);
            _billRelatedFeedbackComponent = new BillRelatedFeedbackComponent(this);
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
            if (string.Compare(FeedbackID, "3") == 0)
            {
                _otherFeedbackComponent.SetFeedbackType();
            }
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
                ExecuteSubmitFeedback();
            };
            _btnSubmitContainer.AddSubview(_btnSubmit);
            View.AddSubview(_btnSubmitContainer);
        }

        void CreatePhotoUploadWidget()
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

        nfloat GetPhotoWidgetYCoordinate()
        {
            nfloat yCoord = 0.0f;
            yCoord = GetFeedbackCategoryViewHeight();
            yCoord += _viewFeedback?.Frame.Height ?? 0;
            return yCoord;
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
                    ImagePickerDelegate imgPickerDelegate = new ImagePickerDelegate(this)
                    {
                        //imgPickerDelegate.Type = Enums.FeedbackCategory.LoginOthers;
                        DashedLineView = dashedLineView
                    };
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

        void UpdateContentSize()
        {
            //float scrollViewHeight = (float)(_viewFeedback?.Frame.Height + _viewUploadPhoto?.Frame.Height + GetNonLoginWidgetHeight());
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight()).Size;
        }

        void CreateCommentSection()
        {
            _viewFeedback = new UIView((new CGRect(18, GetCommentSectionYCoordinate(), View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblFeedbackTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewFeedback.Frame.Width, 12),
                AttributedText = new NSAttributedString("Feedback_Title".Translate().ToUpper()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.SilverChalice()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _feedbackTextView = new FeedbackTextView
            {
                Frame = new CGRect(TXTVIEW_DEFAULT_MARGIN, 12, View.Frame.Width - 60, 36),
                Editable = true,
                Font = myTNBFont.MuseoSans18_300(),
                TextAlignment = UITextAlignment.Left,
                TextColor = myTNBColor.TunaGrey(),
                BackgroundColor = UIColor.Clear,
                EnablesReturnKeyAutomatically = true,
                TranslatesAutoresizingMaskIntoConstraints = true,
                ScrollEnabled = true,
            };

            _iconFeedback = new UIImageView(new CGRect(0, _feedbackTextView.Frame.Height / 2, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Feedback")
            };

            _feedbackTextView.SetPlaceholder("Feedback_Title".Translate());
            _feedbackTextView.CreateDoneButton();

            _feedbackTextView.ShouldChangeText = (txtView, range, replacementString) =>
            {
                var newLength = txtView.Text.Length + replacementString.Length - range.Length;
                return newLength <= 250;
            };

            _lblFeedbackError = new UILabel
            {
                Frame = new CGRect(0, _feedbackTextView.Frame.Height, _feedbackTextView.Frame.Width - 36, 14),
                AttributedText = new NSAttributedString("Invalid_Feedback".Translate()
                    , font: myTNBFont.MuseoSans11_300()
                    , foregroundColor: myTNBColor.Tomato()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _viewLineFeedback = new UIView((new CGRect(0, _feedbackTextView.Frame.GetMaxY() + 3f, _viewFeedback.Frame.Width, 1)))
            {
                BackgroundColor = myTNBColor.PlatinumGrey()
            };

            _lblFeedbackSubTitle = new UILabel(new CGRect(0, _viewLineFeedback.Frame.GetMaxY(), _feedbackTextView.Frame.Width - 36, 14))
            {
                TextColor = myTNBColor.SilverChalice(),
                Font = myTNBFont.MuseoSans11_300()
            };

            _viewFeedback.AddSubviews(new UIView[] { _lblFeedbackTitle, _feedbackTextView
                , _iconFeedback, _lblFeedbackError, _viewLineFeedback, _lblFeedbackSubTitle});
            SetTextViewEvents(_feedbackTextView, _lblFeedbackTitle, _lblFeedbackError,
                 _viewLineFeedback, null, ANY_PATTERN);
            _svContainer.AddSubview(_viewFeedback);
        }

        internal void SetTextViewEvents(FeedbackTextView textView, UILabel lblTitle
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
                    var frame = new CGRect();
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
                //SubmitButtonEnable();
                SetButtonEnable();
            };
            textView.ShouldBeginEditing = (sender) =>
            {
                var frame = new CGRect();
                frame = _feedbackTextView.Frame;
                _iconFeedback.Hidden = true;
                frame.X = 0.0f;
                _feedbackTextView.Frame = frame;
                viewLine.BackgroundColor = myTNBColor.PowerBlue();
                lblError.Hidden = true;
                _lblFeedbackSubTitle.Hidden = false;
                textView.TextColor = myTNBColor.TunaGrey();
                return true;
            };
            textView.ShouldEndEditing = (sender) =>
            {
                lblTitle.Hidden = textView.Text.Length == 0;
                bool isValid = _feedbackTextView.ValidateTextView(textView.Text, pattern);

                lblError.Hidden = isValid || textView.Text.Length == 0;
                _lblFeedbackSubTitle.Hidden = !lblError.Hidden;
                lblHint.Hidden = true;
                viewLine.BackgroundColor = isValid || textView.Text.Length == 0 ? myTNBColor.PlatinumGrey() : myTNBColor.Tomato();
                textView.TextColor = isValid || textView.Text.Length == 0 ? myTNBColor.TunaGrey() : myTNBColor.Tomato();

                if (textView.Text.Length == 0)
                {
                    var frame = new CGRect();
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
                    var newLength = textView.Text.Length + replacementString.Length - range.Length;
                    return newLength <= TNBGlobal.FeedbackMaxCharCount;
                }
                return true;
            };
        }

        void HandleFeedbackTextViewChange()
        {
            int charCount = TNBGlobal.FeedbackMaxCharCount - _feedbackTextView.Text.Length;
            _lblFeedbackSubTitle.Text = string.Format(charCount > 1 ? "Feedback_CharactersLeft".Translate()
                : "Feedback_CharacterLeft".Translate(), charCount);
        }

        nfloat GetCommentSectionYCoordinate()
        {
            nfloat yCoord = 0.0f;
            yCoord = _feedbackCategoryView?.Frame.Height ?? 0;
            return yCoord;
        }

        nfloat GetScrollHeight()
        {
            return (nfloat)((_viewUploadPhoto.Frame.GetMaxY() + (_btnSubmitContainer.Frame.Height + 50f)));
        }

        void InitializeFeedbackComponents()
        {
            if (string.Compare(FeedbackID, "1") == 0)
            {
                _feedbackCategoryView = _billRelatedFeedbackComponent.GetComponent();
            }
            else if (string.Compare(FeedbackID, "2") == 0)
            {

            }
            else
            {
                _feedbackCategoryView = _otherFeedbackComponent.GetComponent();
            }
            _svContainer.AddSubview(_feedbackCategoryView);
        }

        nfloat GetFeedbackCategoryViewHeight()
        {
            return _feedbackCategoryView?.Frame.Height ?? 0;
        }

        internal UITapGestureRecognizer GetFeedbackTypeGestureRecognizer()
        {
            return new UITapGestureRecognizer(() =>
           {
               UIStoryboard storyBoard = UIStoryboard.FromName("FeedbackTableView", null);
               FeedbackTypeViewController feedbackTypeVC =
                   storyBoard.InstantiateViewController("FeedbackTypeViewController") as FeedbackTypeViewController;
               feedbackTypeVC._feedbackTypeList = DataManager.DataManager.SharedInstance.OtherFeedbackType;
               NavigationController.PushViewController(feedbackTypeVC, true);
           });
        }

        internal UITapGestureRecognizer GetAccountNumberGestureRecognizer()
        {
            return new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("FeedbackTableView", null);
                SelectAccountNoViewController selectAccountNoVC =
                    storyBoard.InstantiateViewController("SelectAccountNoViewController") as SelectAccountNoViewController;
                var navController = new UINavigationController(selectAccountNoVC);
                NavigationController?.PushViewController(selectAccountNoVC, true);
            });
        }

        internal void SetButtonEnable()
        {
            bool isValidFeedback = _feedbackTextView.ValidateTextView(_feedbackTextView.Text, ANY_PATTERN) && _feedbackTextView.Text.Length != 0;
            bool isValidFields = false;
            if (string.Compare(FeedbackID, "1") == 0)
            {
                isValidFields = _billRelatedFeedbackComponent.IsValidEntry();
            }
            else if (string.Compare(FeedbackID, "2") == 0)
            {

            }
            else
            {
                isValidFields = _otherFeedbackComponent.IsValidEntry();
            }
            bool isValid = isValidFields && isValidFeedback;
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        void ExecuteSubmitFeedback()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }
    }
}