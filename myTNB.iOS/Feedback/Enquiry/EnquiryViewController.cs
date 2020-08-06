using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.Feedback;
using myTNB.Home.Bill;
using myTNB.Model;
using myTNB.Registration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class EnquiryViewController : CustomUIViewController
    {
        public EnquiryViewController (IntPtr handle) : base (handle) { }

        public string FeedbackID = string.Empty;

        private UIView _viewAccountNo, _viewLineAccountNo, _accContainer, _viewTitleSection;
        private UILabel _lblAccountNoTitle, _lblAccountNoError;
        private UITextField _txtAccountNumber;

        private CustomUIView _ToolTipsView;
        private UIScrollView _svContainer;

        private GetSearchForAccountResponseModel getSearchForAccountResponseModel
            = new GetSearchForAccountResponseModel();
        List<string> getContractAccounts;

        public bool IsLoggedIn;

        private TextFieldHelper _textFieldHelper = new TextFieldHelper();

        //Cell General Enquiry
        public UIView Frame;
        public UILabel lblTitle;
        public UILabel lblSubtTitle;
        public UIView viewLine;
        public UILabel lblCount;
        public UIImageView imgViewIcon;

        //Cell Update Personal Detail
        public UIView Frame2;
        public UILabel lblTitle2;
        public UILabel lblSubtTitle2;
        public UIView viewLine2;
        public UILabel lblCount2;
        public UIImageView imgViewIcon2;

        internal nfloat _navBarHeight;
        private UIView _navbarView;
        private nfloat titleBarHeight = ScaleUtility.GetScaledHeight(24f);
        private CAGradientLayer _gradientLayer;

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;

            base.ViewDidLoad();
            SetHeader();
            //SetNavigation();
            AddScrollView();
            ConstructAccountNumberSelector();
            AddSectionTitle();
            CellGeneralEnquiry();
            CellUpdatePersonalDetail();
            SetEvents();
            SetView();
            SetVisibility();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            //NavigationController.NavigationBar.Hidden = true;
            string accountNo = DataManager.DataManager.SharedInstance.AccountNumber;
            if (!string.IsNullOrEmpty(accountNo))
            {
                _txtAccountNumber.Text = accountNo;

                SetAddAccountButtonEnable();

                DataManager.DataManager.SharedInstance.AccountNumber = string.Empty;
            }

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                if(NavigationController.NavigationBar.Hidden == true)
                NavigationController.NavigationBar.Hidden = false;
            }
        }

        private void SetHeader()
        {
            //if (NavigationController != null) { NavigationController.SetNavigationBarHidden(false, false); }
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = false;
            }

            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            Title = GetI18NValue(EnquiryConstants.submitEnquiryTitle);

        }

        private void SetNavigation()
        {

            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
                _navBarHeight = NavigationController.NavigationBar.Frame.Height;
            }

            _navbarView = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + _navBarHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(8f), _navbarView.Frame.Width, titleBarHeight));

            UIView viewBack = new UIView(new CGRect(BaseMarginWidth16, 0, GetScaledWidth(24F), titleBarHeight));
            UIImageView imgViewBack = new UIImageView(new CGRect(0, 0, GetScaledWidth(24F), titleBarHeight))
            {
                Image = UIImage.FromBundle(Constants.IMG_Back)
            };
            viewBack.AddSubview(imgViewBack);
            viewTitleBar.AddSubview(viewBack);

            UILabel lblTitle = new UILabel(new CGRect(GetScaledWidth(56F), 0, _navbarView.Frame.Width - (GetScaledWidth(56F) * 2), titleBarHeight))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(EnquiryConstants.generalEnquiryTitle)
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);


            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                //NavigationController.PopViewController(true);
                DismissViewController(true, null);

            }));

            _navbarView.AddSubviews(viewTitleBar);

            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            _gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            _gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            _gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            _gradientLayer.Frame = _navbarView.Bounds;
            _navbarView.Layer.InsertSublayer(_gradientLayer, 0);
            View.AddSubview(_navbarView);
        }


        private void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height)) //_navbarView.Frame.GetMaxY()
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            View.AddSubview(_svContainer);
        }

        private void AddSectionTitle()
        {
            _viewTitleSection = new UIView(new CGRect(0, _accContainer.Frame.GetMaxY(), View.Frame.Width, GetScaledHeight(48)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            UILabel lblSectionTitle = new UILabel(new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.enquiringTitle)
            };

            _viewTitleSection.AddSubview(lblSectionTitle);
            _svContainer.AddSubview(_viewTitleSection);
        }


        private void ConstructAccountNumberSelector()
        {
            _accContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, 123))
            {
                BackgroundColor = UIColor.White
            };

            _viewAccountNo = new UIView((new CGRect(18, 16, View.Frame.Width - 36, 51)))
            {
                BackgroundColor = UIColor.Clear
            };

            _lblAccountNoTitle = new UILabel
            {
                Frame = new CGRect(0, 0, _viewAccountNo.Frame.Width, 12),
                AttributedText = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.accNumberHint)//GetCommonI18NValue(Constants.Common_AccountNo)
                , AttributedStringUtility.AttributedStringType.Title),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _lblAccountNoError = new UILabel
            {
                Frame = new CGRect(0, 37, _viewAccountNo.Frame.Width, 14),
                AttributedText = AttributedStringUtility.GetAttributedString(GetI18NValue("validElectricityAccountNoError")
                    , AttributedStringUtility.AttributedStringType.Error),
                TextAlignment = UITextAlignment.Left,
                Hidden = true
            };

            _txtAccountNumber = new UITextField(new CGRect(0, 12, _viewAccountNo.Frame.Width - 84, 24)) //x:30
            {
                Font = MyTNBFont.MuseoSans16_300,
                TextColor = MyTNBColor.TunaGrey(),
                AttributedPlaceholder = AttributedStringUtility.GetAttributedString(GetI18NValue(EnquiryConstants.accNumberHint)
                    , AttributedStringUtility.AttributedStringType.Value)
            };
            _viewAccountNo.AddSubview(_txtAccountNumber);
            UIView viewScanner = DataManager.DataManager.SharedInstance.IsLoggedIn() ?
                new UIView(new CGRect(_viewAccountNo.Frame.Width - 64, 12, 24, 24)): new UIView(new CGRect(_viewAccountNo.Frame.Width - 30, 12, 24, 24));
            UIImageView scanner = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Scan")
            };

            viewScanner?.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                BarcodeScannerViewController viewController =
                    storyBoard.InstantiateViewController("BarcodeScannerViewController") as BarcodeScannerViewController;
                NavigationController.PushViewController(viewController, true);
            }));
            viewScanner.AddSubview(scanner);
            _viewAccountNo.AddSubview(viewScanner);

            if (DataManager.DataManager.SharedInstance.IsLoggedIn()) { 
            UIView imgDropDownFrame = new UIView(new CGRect(_viewAccountNo.Frame.Width - 30, 12, 24, 24));
            UIImageView imgDropDown = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("IC-Action-Dropdown")
            };
            imgDropDownFrame?.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = GetCommonI18NValue(Constants.Common_SelectElectricityAccount);
                viewController.Items = GetAccountList();
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);

            }));
            imgDropDownFrame.AddSubview(imgDropDown);
            _viewAccountNo.AddSubview(imgDropDownFrame);
            }

            _viewLineAccountNo = GenericLine.GetLine(new CGRect(0, 36, _viewAccountNo.Frame.Width, 1));

            _viewAccountNo.AddSubviews(new UIView[] { _lblAccountNoTitle, _lblAccountNoError
                , _txtAccountNumber, _viewLineAccountNo }); 

            _accContainer.AddSubview(_viewAccountNo);
            _accContainer.AddSubview(GetTooltipView(GetYLocationFromFrame(_viewAccountNo.Frame, 8)));

            _svContainer.AddSubview(_accContainer);

            SetKeyboard(_txtAccountNumber);
        }

        private void SetKeyboard(UITextField textField)
        {
            textField.KeyboardType = UIKeyboardType.NumberPad;
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.None;
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.ReturnKeyType = UIReturnKeyType.Done;
            _textFieldHelper.CreateDoneButton(textField);
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            bool visible = notification.Name == UIKeyboard.WillShowNotification;
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            UIView.CommitAnimations();
        }

        public void SetSelectedAccountNumber()
        {
            int index = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d == null
                || DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count == 0)
            {
                return;
            }

            _txtAccountNumber.Text = string.Format("{0}", DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum
                , DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accDesc);
        }

        public string GetAccountNumber()
        {
            if (IsLoggedIn)
            {
                int index = DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex;
                if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d.Count > 0)
                {
                    _lblAccountNoTitle.Text = DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum ?? string.Empty;

                    return DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum ?? string.Empty;
                }
                return string.Empty;
            }
            else
            {
                return _txtAccountNumber.Text ?? string.Empty;
            }
        }

        private List<string> GetAccountList()
        {
            if (DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d?.Count > 0)
            {
                List<string> accountList = new List<string>();
                foreach (CustomerAccountRecordModel item in DataManager.DataManager.SharedInstance.AccountRecordsList?.d)
                {
                    accountList.Add(string.Format("{0} - {1}", item?.accNum ?? string.Empty, item?.accDesc ?? string.Empty));
                }
                return accountList;
            }
            return new List<string>();
        }

        private void OnSelectAction(int index)
        {
            DataManager.DataManager.SharedInstance.CurrentSelectedFeedAccountNoIndex = index;
            _txtAccountNumber.Text = string.Format("{0}", DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accNum
            , DataManager.DataManager.SharedInstance.AccountRecordsList?.d[index]?.accDesc);

            SetAddAccountButtonEnable();

            _lblAccountNoTitle.Hidden = _lblAccountNoTitle.Text.Length == 0;
            _txtAccountNumber.LeftViewMode = UITextFieldViewMode.Never;

        }

        public CustomUIView GetTooltipView(nfloat yLoc)
        {

            _ToolTipsView = new CustomUIView(new CGRect(0, yLoc, View.Frame.Width, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White,
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(BaseMarginWidth16, 0, View.Frame.Width - (GetScaledWidth(16) * 2), GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_InfoBlue)
            };
            UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
                , GetScaledHeight(4), _ToolTipsView.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.accNumberInfo)

            };
            
            UITapGestureRecognizer tapInfo = new UITapGestureRecognizer(() =>
            {
                UIImage cimg;
                if (DataManager.DataManager.SharedInstance.imageWhereAcc == null)
                {
                    if(DataManager.DataManager.SharedInstance.imageWhereAccUrl != GetI18NValue(EnquiryConstants.imageWhereAcc))
                    { 
                        DataManager.DataManager.SharedInstance.imageWhereAccUrl = GetI18NValue(EnquiryConstants.imageWhereAcc);
                        DataManager.DataManager.SharedInstance.imageWhereAcc = GetFromUrl(TNBGlobal.SITECORE_URL + DataManager.DataManager.SharedInstance.imageWhereAccUrl);

                        cimg = DataManager.DataManager.SharedInstance.imageWhereAcc;
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.imageWhereAcc = GetFromUrl(TNBGlobal.SITECORE_URL + DataManager.DataManager.SharedInstance.imageWhereAccUrl);

                        cimg = DataManager.DataManager.SharedInstance.imageWhereAcc;
                    }
                }
                else
                {
                    cimg = DataManager.DataManager.SharedInstance.imageWhereAcc;
                }

                if(cimg != null)
                { 
                DisplayCustomAlert(GetI18NValue(EnquiryConstants.accNumberInfo)
                    , GetI18NValue(EnquiryConstants.accNumberDetails)
                    , new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_GotIt), null } }
                    , cimg);//UIImage.LoadFromData(cimg)
                }
            });

            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            _ToolTipsView.AddGestureRecognizer(tapInfo);
            viewInfo.AddSubviews(new UIView[] { imgView, lblDescription });
            _ToolTipsView.AddSubview(viewInfo);

            return _ToolTipsView;
        }

        public void CellGeneralEnquiry()
        {
            nfloat cellWidth = View.Frame.Width;
            nfloat cellHeight = 80;
            Frame = new UIView(new CGRect(0, _viewTitleSection.Frame.GetMaxY(), cellWidth, cellHeight))
            { BackgroundColor = UIColor.White };
            imgViewIcon = new UIImageView(new CGRect(16, 13, 48, 48))
            {
                Image = UIImage.FromBundle("IC-Tile-FeedbackBill")
            };            lblTitle = new UILabel(new CGRect(80, 16, cellWidth - 96, 16))
            {
                Text = GetI18NValue(EnquiryConstants.generalEnquiryTitle),
                TextColor = MyTNBColor.CharcoalGrey,
                Font = MyTNBFont.MuseoSans16_500
            };            lblSubtTitle = new UILabel(new CGRect(80, 38, cellWidth - 96, 16))
            {
                Text = GetI18NValue(EnquiryConstants.generalEnquiryDescription),
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans12_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };            lblCount = new UILabel(new CGRect(cellWidth - 38, 16, 20, 16))
            {
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Right,
                Font = MyTNBFont.MuseoSans12,
                Hidden = true
            };            viewLine = new UIView(new CGRect(0, cellHeight - 7, cellWidth, 7))
            {
                BackgroundColor = MyTNBColor.LightGrayBG,
                Hidden = false
            };

            Frame?.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ExecuteValidateContractAccount("Enquiry", "GeneralEnquiryViewController");
            }));
            Frame.AddSubviews(new UIView[] { imgViewIcon, lblTitle, lblSubtTitle, lblCount, viewLine });
            _svContainer.AddSubviews(Frame);        }

        public void CellUpdatePersonalDetail()
        {

            nfloat cellWidth = View.Frame.Width;
            nfloat cellHeight = 80;
            Frame2 = new UIView(new CGRect(0, Frame.Frame.GetMaxY(), cellWidth, cellHeight))
            {
                BackgroundColor = UIColor.White
            };
            imgViewIcon2 = new UIImageView(new CGRect(16, 13, 48, 48))
            {
                Image = UIImage.FromBundle("Update-PersonalDetails")
            };
            lblTitle2 = new UILabel(new CGRect(80, 16, cellWidth - 96, 16))
            {
                Text = GetI18NValue(EnquiryConstants.updatePersonalDetTitle),
                TextColor = MyTNBColor.CharcoalGrey,
                Font = MyTNBFont.MuseoSans16_500
            };            lblSubtTitle2 = new UILabel(new CGRect(80, 38, cellWidth - 96, 16))
            {
                Text = GetI18NValue(EnquiryConstants.personalDetailsDescription),
                TextColor = MyTNBColor.TunaGrey(),
                Font = MyTNBFont.MuseoSans12_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            lblCount2 = new UILabel(new CGRect(cellWidth - 38, 16, 20, 16))
            {
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Right,
                Font = MyTNBFont.MuseoSans12,
                Hidden = true
            };
            viewLine2 = new UIView(new CGRect(0, cellHeight - 7, cellWidth, 7))
            {
                BackgroundColor = MyTNBColor.LightGrayBG,
                Hidden = false
            };

            Frame2?.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {

                ExecuteValidateContractAccount("Enquiry", "UpdatePersonalDetailViewController");

            }));
            Frame2.AddSubviews(new UIView[] { imgViewIcon2, lblTitle2, lblSubtTitle2, lblCount2, viewLine2 });
            _svContainer.AddSubviews(Frame2);        }


        private void SetView()
        {
            _textFieldHelper.CreateTextFieldLeftView(_txtAccountNumber, "Account-Number");

        }

        private void SetTextFieldEvents(UITextField textField, UILabel textFieldTitle, UILabel textFieldError, UIView viewLine, string pattern)
        {
            textField.EditingChanged += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;

            };
            textField.EditingDidBegin += (sender, e) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                textField.LeftViewMode = UITextFieldViewMode.Never;
                viewLine.BackgroundColor = MyTNBColor.PowerBlue;
                textField.TextColor = MyTNBColor.TunaGrey();
            };
            textField.ShouldEndEditing = (sender) =>
            {
                textFieldTitle.Hidden = textField.Text.Length == 0;
                bool isValid = _textFieldHelper.ValidateTextField(textField.Text, pattern);
                if (textField == _txtAccountNumber)
                {
                    isValid = isValid && _textFieldHelper.ValidateTextFieldWithLength(textField.Text, TNBGlobal.AccountNumberLowCharLimit);
                }

                textFieldError.Hidden = isValid;

                viewLine.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                textField.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                SetAddAccountButtonEnable();
                return true;
            };
            textField.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };
            textField.ShouldChangeCharacters = (txtField, range, replacementString) =>
            {
                if (textField == _txtAccountNumber)
                {
                    string content = _textFieldHelper.TrimAllSpaces(((UITextField)txtField).Text);
                    nint count = content.Length + replacementString.Length - range.Length;
                    return count <= TNBGlobal.AccountNumberLowCharLimit;
                }

                return true;
            };

            textField.EditingDidEnd += (sender, e) =>
            {
                if (textField.Text.Length == 0)
                    textField.LeftViewMode = UITextFieldViewMode.UnlessEditing;
            };
        }

        private void SetEvents()
        {
            SetTextFieldEvents(_txtAccountNumber, _lblAccountNoTitle, _lblAccountNoError, _viewLineAccountNo, TNBGlobal.ACCOUNT_NO_PATTERN);
        }



        private void SetVisibility()
        {
            if (DataManager.DataManager.SharedInstance.IsFeedbackUpdateDetailDisabled)
            {
                Frame2.Hidden = true;
            }
            //else if(IsLoggedIn == false)
            //{
            //    Frame2.Hidden = true;
            //}
            else
            {
                Frame2.Hidden = false;
            }
        }

        private void SetAddAccountButtonEnable()
        {
            bool isAccountValid = _textFieldHelper.ValidateTextField(_txtAccountNumber.Text, TNBGlobal.ACCOUNT_NO_PATTERN)
                && _textFieldHelper.ValidateTextFieldWithLength(_txtAccountNumber.Text, TNBGlobal.AccountNumberLowCharLimit);

            bool isValid = isAccountValid;

            _viewLineAccountNo.BackgroundColor = isValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
            _txtAccountNumber.TextColor = isValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
            _lblAccountNoError.Hidden = isValid; 
            if (_txtAccountNumber.Text == string.Empty)
            {
                _lblAccountNoError.Text = GetI18NValue("plsEnterAcc");
            }
            else
            {
                _lblAccountNoError.Text = GetI18NValue("validElectricityAccountNoError");
            }
        }

        private void ExecuteValidateContractAccount(string uistoryboard, string viewcontroller)
        {
            ActivityIndicator.Show();

            View.EndEditing(true);

            getContractAccounts = new List<string>();
            getContractAccounts.Add(_txtAccountNumber.Text);

            string fullName = string.Empty;
            string Ic = string.Empty;

            ValidateAccountLinking().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (getSearchForAccountResponseModel != null
                        && getSearchForAccountResponseModel.d != null)
                    {

                        bool isAccountValid = _textFieldHelper.ValidateTextField(_txtAccountNumber.Text, TNBGlobal.ACCOUNT_NO_PATTERN)
                        && _textFieldHelper.ValidateTextFieldWithLength(_txtAccountNumber.Text, TNBGlobal.AccountNumberLowCharLimit);

                        if (isAccountValid)
                        {
                            for (int i = 0; i < getSearchForAccountResponseModel.d.Count; i++)
                            {
                                GetSearchForAccountModel item = getSearchForAccountResponseModel.d[i];

                                Ic = item.IC;
                                fullName = item.FullName;
                            }

                            if(Ic != string.Empty && fullName != string.Empty)
                            { 

                            DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryIC = Ic;
                            DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryFullName = fullName;
                            DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA = _txtAccountNumber.Text;

                            //UIStoryboard storyBoard = UIStoryboard.FromName(uistoryboard, null);
                            //UIViewController viewController = storyBoard.InstantiateViewController(viewcontroller) as UIViewController;
                            //    UINavigationController navController = new UINavigationController(viewController);
                            //    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            //    PresentViewController(navController, true, null);

                                UIStoryboard storyBoard = UIStoryboard.FromName(uistoryboard, null);
                                //GenericStatusPageViewController status = storyBoard.InstantiateViewController("GenericStatusPageViewController") as GenericStatusPageViewController;
                                UIViewController viewController = storyBoard.InstantiateViewController(viewcontroller) as UIViewController;
                                //status.IsSuccess = true;
                                //status.ReferenceNumber = _submitFeedback?.d?.data?.ServiceReqNo;
                                //status.ReferenceDate = _submitFeedback?.d?.data?.DateCreated;
                                //status.StatusDisplayType = GenericStatusPageViewController.StatusType.Feedback;
                                NavigationController.PushViewController(viewController, true);

                                ActivityIndicator.Hide();

                            }
                            else
                            {
                                isAccountValid = false;

                                _viewLineAccountNo.BackgroundColor = isAccountValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                                _txtAccountNumber.TextColor = isAccountValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                                _lblAccountNoError.Hidden = isAccountValid;

                                if (_txtAccountNumber.Text == string.Empty)
                                {
                                    _lblAccountNoError.Text = GetI18NValue("plsEnterAcc");
                                }
                                else
                                {
                                    _lblAccountNoError.Text = GetI18NValue("validElectricityAccountNoError");
                                }
                                //SetAddAccountButtonEnable();

                                ActivityIndicator.Hide();
                            }
                        }
                        else
                        {

                            isAccountValid = false;

                            _viewLineAccountNo.BackgroundColor = isAccountValid ? MyTNBColor.PlatinumGrey : MyTNBColor.Tomato;
                            _txtAccountNumber.TextColor = isAccountValid ? MyTNBColor.TunaGrey() : MyTNBColor.Tomato;
                            _lblAccountNoError.Hidden = isAccountValid;

                            if (_txtAccountNumber.Text == string.Empty)
                            {
                                _lblAccountNoError.Text = GetI18NValue("plsEnterAcc");
                            }
                            else
                            {
                                _lblAccountNoError.Text = GetI18NValue("validElectricityAccountNoError");
                            }

                            ActivityIndicator.Hide();
                        }

                    }
                    else
                    {
                        DisplayServiceError("");
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private Task ValidateAccountLinking()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    contractAccounts = getContractAccounts
                };
                getSearchForAccountResponseModel = serviceManager.OnExecuteAPIV6<GetSearchForAccountResponseModel>
                    ("GetSearchForAccount", requestParameter);
            });

        }

        private UIImage GetFromUrl(string uri)//temporary
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url, NSDataReadingOptions.Uncached, out NSError error))
                if (error != null) { return null; }
                else
                {
                    return UIImage.LoadFromData(data);
                    //return data;

                }
        }

    }
}