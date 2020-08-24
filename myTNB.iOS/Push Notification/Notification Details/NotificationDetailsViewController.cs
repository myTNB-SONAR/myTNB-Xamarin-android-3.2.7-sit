using System;
using UIKit;
using myTNB.Model;
using CoreGraphics;
using System.Threading.Tasks;
using myTNB.PushNotification;
using CoreAnimation;
using Foundation;
using System.Collections.Generic;
using myTNB.SQLite.SQLiteDataManager;
using System.Text.RegularExpressions;
using myTNB.DataManager;

namespace myTNB
{
    public partial class NotificationDetailsViewController : CustomUIViewController
    {
        public NotificationDetailsViewController(IntPtr handle) : base(handle) { }
        private UILabel _lblTitle;
        private UIView _navbarView, _viewCTA;
        private UITextView _txtViewDetails;
        private CAGradientLayer _gradientLayer;
        private UIScrollView _scrollView;
        private UIImageView _bgImageView;
        private CustomUIButtonV2 _btnPrimary, _btnSecondary;

        private nfloat _navBarHeight, _previousScrollOffset;
        private nfloat titleBarHeight = ScaleUtility.GetScaledHeight(24f);
        private bool _isViewDidload;

        private DeleteNotificationResponseModel _deleteNotificationResponse;
        private SMRAccountActivityInfoResponseModel _smrActivityInfoResponse;
        private bool _isCTAEnabled = false;
        public UserNotificationDataModel NotificationInfo { set; private get; } = new UserNotificationDataModel();
        public bool IsPayEnabled { set; private get; }

        public override void ViewDidLoad()
        {
            PageName = PushNotificationConstants.Pagename_PushNotificationDetails;
            NavigationController.NavigationBarHidden = false;
            if (TabBarController != null && TabBarController.TabBar != null)
            {
                TabBarController.TabBar.Hidden = true;
                ExtendedLayoutIncludesOpaqueBars = true;
                EdgesForExtendedLayout = UIRectEdge.Bottom;
            }
            base.ViewDidLoad();
            _isViewDidload = true;
            AddSubview();
            SetNavigation();
        }

        public override void ViewWillAppear(bool animated)
        {
            if (TabBarController != null && TabBarController.TabBar != null)
            {
                TabBarController.TabBar.Hidden = true;
            }
            base.ViewWillAppear(animated);
            NavigationController.SetNavigationBarHidden(true, true);
            int unreadCount = PushNotificationHelper.GetNotificationCount();
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = unreadCount == 0 ? 0 : unreadCount - 1;
            if (NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.TerminationCompleted)
            {
                InvokeOnMainThread(async () =>
                {
                    ActivityIndicator.Show();
                    SMRAccountStatusResponseModel response = await ServiceCall.GetAccountsSMRStatus(new List<string> { NotificationInfo.AccountNum ?? string.Empty });
                    if (response != null && response.d != null && response.d.IsSuccess && response.d.data != null && response.d.data.Count == 1)
                    {
                        _isCTAEnabled = !response.d.data[0].isTaggedSMR;
                        if (_btnPrimary != null)
                        {
                            _btnPrimary.Enabled = _isCTAEnabled;
                            UpdateCTA(ref _btnPrimary, false, _isCTAEnabled);
                        }
                    }
                    ActivityIndicator.Hide();
                });
            }
        }

        #region Navigation
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

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(8f)
                , _navbarView.Frame.Width, titleBarHeight));

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
                Text = NotificationInfo.NotificationTitle ?? string.Empty
            };

            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.TextColor = UIColor.White;
            viewTitleBar.AddSubview(lblTitle);

            CustomUIView viewDelete = new CustomUIView(new CGRect(ViewWidth - GetScaledWidth(40), 0, GetScaledWidth(24F), titleBarHeight));
            viewDelete.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnDeleteNotification();
            }));
            viewDelete.AddSubview(new UIImageView(new CGRect(0, 0, GetScaledWidth(24F), titleBarHeight))
            {
                Image = UIImage.FromBundle(PushNotificationConstants.IMG_Delete)
            });
            viewTitleBar.AddSubview(viewDelete);

            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationController.PopViewController(true);
            }));

            _navbarView.AddSubview(viewTitleBar);

            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            _gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            _gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            _gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            _gradientLayer.Frame = _navbarView.Bounds;
            _gradientLayer.Opacity = 0f;
            _navbarView.Layer.InsertSublayer(_gradientLayer, 0);
            View.AddSubview(_navbarView);
        }

        private void AddViewWithOpacity(float opacity)
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            gradientLayer.Frame = _navbarView.Bounds;
            gradientLayer.Opacity = opacity;
            _navbarView.Layer.ReplaceSublayer(_gradientLayer, gradientLayer);
            _gradientLayer = gradientLayer;
        }

        private void OnTableViewScrolled(object sender, EventArgs e)
        {
            nfloat safeInset = DeviceHelper.TopSafeAreaInset;
            _previousScrollOffset = _scrollView.ContentOffset.Y;
            if (!_isViewDidload)
            {
                CGRect frame = _bgImageView.Frame;
                if ((nfloat)Math.Abs(frame.Y) == frame.Height) { return; }

                nfloat newYLoc = 0 - _previousScrollOffset - safeInset;
                frame.Y = newYLoc;
                _bgImageView.Frame = frame;
            }
            var opac = _previousScrollOffset / _navbarView.Frame.Height;
            float absOpacity = Math.Abs((float)opac);
            if (_isViewDidload || Math.Abs(_previousScrollOffset) == safeInset)
            {
                absOpacity = 0;
                _isViewDidload = false;
            }
            AddViewWithOpacity(absOpacity);
        }
        #endregion
        private string BannerImage
        {
            get
            {
                if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.SSMR
                    && PushNotificationConstants.SSMRBannerImageDictionary.ContainsKey(NotificationInfo.SSMRNotificationType))
                {
                    return PushNotificationConstants.SSMRBannerImageDictionary[NotificationInfo.SSMRNotificationType];
                }
                if (PushNotificationConstants.BannerImageDictionary.ContainsKey(NotificationInfo.BCRMNotificationType))
                {
                    return PushNotificationConstants.BannerImageDictionary[NotificationInfo.BCRMNotificationType];
                }
                return string.Empty;
            }
        }

        private void AddSubview()
        {
            SetCTA();
            _scrollView = new UIScrollView(new CGRect(0, 0, ViewWidth
                , View.Frame.Height - _viewCTA.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            _scrollView.Bounces = false;
            _scrollView.Scrolled += OnTableViewScrolled;
            _scrollView.ShowsVerticalScrollIndicator = false;

            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(BannerImage),
                BackgroundColor = UIColor.White
            };

            View.Add(_bgImageView);
            AddTitle();
            AddDetails();
            View.AddSubview(_scrollView);
            _scrollView.ContentSize = new CGSize(ViewWidth, _txtViewDetails.Frame.GetMaxY());
        }

        private void AddTitle()
        {
            _lblTitle = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(_bgImageView.Frame, 24)
               , ViewWidth - GetScaledWidth(10), GetScaledHeight(48)))
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_16_500,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = NotificationInfo.Title ?? string.Empty
            };
            nfloat lblHeight = _lblTitle.GetLabelHeight(ViewHeight / 2);
            _lblTitle.Frame = new CGRect(_lblTitle.Frame.X, _lblTitle.Frame.Y, _lblTitle.Frame.Width, lblHeight);
            _scrollView.Add(_lblTitle);
        }

        private void AddDetails()
        {
            // HTML / Plain text for UITextView
            NSError htmlBodyError = null;
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_14_500,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };
            string message = NotificationInfo.Message + "<br>";
            if (NotificationInfo != null && !string.IsNullOrEmpty(NotificationInfo.Message) && !string.IsNullOrWhiteSpace(NotificationInfo.Message)
                && DataManager.DataManager.SharedInstance != null && DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0)
            {
                int accountIndex = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == NotificationInfo.AccountNum);
                if (accountIndex > -1)
                {
                    string accountNickname = DataManager.DataManager.SharedInstance.AccountRecordsList.d[accountIndex].accountNickName ?? string.Empty;
                    if (string.IsNullOrEmpty(accountNickname) || string.IsNullOrWhiteSpace(accountNickname))
                    {
                        accountNickname = string.Format(GetCommonI18NValue(PushNotificationConstants.I18N_CustomerAccountNumber), NotificationInfo.AccountNum);
                    }
                    message = Regex.Replace(message, PushNotificationConstants.REGEX_AccountNickname, accountNickname);
                }
                else
                {
                    message = Regex.Replace(message, PushNotificationConstants.REGEX_AccountNickname
                        , string.Format(GetCommonI18NValue(PushNotificationConstants.I18N_CustomerAccountNumber), NotificationInfo.AccountNum));
                }
            }
            else
            {
                message = Regex.Replace(message, PushNotificationConstants.REGEX_AccountNickname
                    , string.Format(GetCommonI18NValue(PushNotificationConstants.I18N_CustomerAccountNumber), NotificationInfo.AccountNum));
            }

            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(message ?? string.Empty
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, (float)GetScaledHeight(14));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey
            }, new NSRange(0, htmlBody.Length));

            _txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                ContentInset = new UIEdgeInsets(0, -5, 0, -5)
            };
            _txtViewDetails.ScrollIndicatorInsets = UIEdgeInsets.Zero;

            //Resize
            CGSize size = _txtViewDetails.SizeThatFits(new CGSize(BaseMarginedWidth, ViewHeight));
            _txtViewDetails.Frame = new CGRect(BaseMargin, GetYLocationFromFrame(_lblTitle.Frame, 16), BaseMarginedWidth, size.Height);
            _txtViewDetails.TextAlignment = UITextAlignment.Left;
            Action<NSUrl> action = new Action<NSUrl>(RedirectAlert);
            _txtViewDetails.Delegate = new TextViewDelegate(action);
            _scrollView.Add(_txtViewDetails);
        }

        private void RedirectAlert(NSUrl url)
        {
            string absURL = url?.AbsoluteString;
            if (!string.IsNullOrEmpty(absURL))
            {
                int whileCount = 0;
                bool isContained = false;
                for (int i = 0; i < AlertHandler.RedirectTypeList.Count; i++)
                {
                    if (absURL.Contains(AlertHandler.RedirectTypeList[i]))
                    {
                        whileCount = i;
                        isContained = true;
                        break;
                    }
                }

                if (isContained)
                {
                    if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[0])
                    {
                        string urlString = absURL.Split(AlertHandler.RedirectTypeList[0])[1];
                        BrowserViewController viewController = new BrowserViewController();
                        if (viewController != null)
                        {
                            viewController.URL = urlString;
                            viewController.IsDelegateNeeded = false;
                            UINavigationController navController = new UINavigationController(viewController)
                            {
                                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                            };
                            PresentViewController(navController, true, null);
                        }
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[1])
                    {
                        string urlString = absURL.Split(AlertHandler.RedirectTypeList[1])[1];
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(urlString)));
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[2])
                    {
                        string urlString = absURL.Split(AlertHandler.RedirectTypeList[2])[1];
                        if (!urlString.Contains("tel:"))
                        {
                            urlString = "tel:" + urlString;
                        }
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[3])
                    {
                        string key = absURL.Split(AlertHandler.RedirectTypeList[3])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        WhatsNewServices.OpenWhatsNewDetails(key, this);
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[4])
                    {
                        string key = absURL.Split(AlertHandler.RedirectTypeList[4])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        if (!key.Contains("{"))
                        {
                            key = "{" + key;
                        }
                        if (!key.Contains("}"))
                        {
                            key = key + "}";
                        }
                        ViewHelper.GoToFAQScreenWithId(key);
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[5])
                    {
                        string key = absURL.Split(AlertHandler.RedirectTypeList[5])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        RewardsServices.OpenRewardDetails(key, this);
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[6])
                    {
                        string urlString = absURL;
                        BrowserViewController viewController = new BrowserViewController();
                        if (viewController != null)
                        {
                            viewController.NavigationTitle = "";
                            viewController.URL = urlString;
                            viewController.IsDelegateNeeded = false;
                            UINavigationController navController = new UINavigationController(viewController)
                            {
                                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                            };
                            PresentViewController(navController, true, null);
                        }
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[7])
                    {
                        string urlString = absURL;
                        if (!urlString.Contains("tel:"))
                        {
                            urlString = "tel:" + urlString;
                        }
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[8])
                    {
                        string key = absURL.Split(AlertHandler.RedirectTypeList[8])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        WhatsNewServices.OpenWhatsNewDetails(key, this);
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[9])
                    {
                        string key = absURL.Split(AlertHandler.RedirectTypeList[9])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        if (!key.Contains("{"))
                        {
                            key = "{" + key;
                        }
                        if (!key.Contains("}"))
                        {
                            key = key + "}";
                        }
                        ViewHelper.GoToFAQScreenWithId(key);
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[10])
                    {
                        string key = absURL.Split(AlertHandler.RedirectTypeList[10])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        RewardsServices.OpenRewardDetails(key, this);
                    }
                }
            }
        }

        private void SetCTA()
        {
            if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.Maintenance
                || NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.None)
            {
                _viewCTA = new UIView();
            }
            else
            {
                nfloat height = GetScaledHeight(80) + DeviceHelper.BottomSafeAreaInset;
                _viewCTA = new UIView(new CGRect(0, View.Frame.Height - height, ViewWidth, height))
                {
                    BackgroundColor = UIColor.White
                };
                EvaluateCTA();
            }
            View.AddSubview(_viewCTA);
        }

        private void EvaluateCTA()
        {
            nfloat btnWidth = (ViewWidth - GetScaledWidth(36)) / 2;
            if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.BillDue
                || NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.Dunning
                || NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.NewBill
                || NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.Disconnection)
            {
                _btnPrimary = new CustomUIButtonV2
                {
                    Frame = new CGRect(BaseMargin, GetScaledHeight(16), btnWidth, GetScaledHeight(48))
                };
                UpdateCTA(ref _btnPrimary, false);
                _btnPrimary.SetTitle(GetI18NValue(NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.Disconnection
                    ? PushNotificationConstants.I18N_ContactTNB : PushNotificationConstants.I18N_ViewBill), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.Disconnection)
                    {
                        OnContact();
                    }
                    else
                    {
                        OnViewDetails();
                    }
                }));
                _btnSecondary = new CustomUIButtonV2
                {
                    Frame = new CGRect(_btnPrimary.Frame.GetMaxX() + GetScaledWidth(4), GetScaledHeight(16), btnWidth, GetScaledHeight(48))
                };
                UpdateCTA(ref _btnSecondary, true, IsPayEnabled);
                _btnSecondary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_Paynow), UIControlState.Normal);
                _btnSecondary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnPay();
                }));

                _viewCTA.AddSubviews(new UIView[] { _btnPrimary, _btnSecondary });
            }
            else if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.Reconnection)
            {
                _btnPrimary = new CustomUIButtonV2
                {
                    Frame = new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48))
                };
                UpdateCTA(ref _btnPrimary, false);
                _btnPrimary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_ViewMyUsage), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnViewUsage();
                }));
                _viewCTA.AddSubview(_btnPrimary);
            }
            else if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.SSMR)
            {
                EvaluateSSMRCTA();
            }
            else if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.PaymentSuccess)
            {
                nfloat width = NotificationInfo != null && NotificationInfo.MerchantTransId.IsValid() ? btnWidth : BaseMarginedWidth;
                _btnPrimary = new CustomUIButtonV2
                {
                    Frame = new CGRect(BaseMargin, GetScaledHeight(16), width, GetScaledHeight(48))
                };
                UpdateCTA(ref _btnPrimary, false);
                _btnPrimary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_PaymentHistory), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnViewBill();
                }));
                _viewCTA.AddSubview(_btnPrimary);

                if (NotificationInfo != null && NotificationInfo.MerchantTransId.IsValid())
                {
                    _btnSecondary = new CustomUIButtonV2
                    {
                        Frame = new CGRect(_btnPrimary.Frame.GetMaxX() + GetScaledWidth(4), GetScaledHeight(16), btnWidth, GetScaledHeight(48))
                    };
                    UpdateCTA(ref _btnSecondary, true);
                    _btnSecondary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_ViewReceipt), UIControlState.Normal);
                    _btnSecondary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        OnViewReceipt();
                    }));
                    _viewCTA.AddSubview(_btnSecondary);
                }
            }
            else if (NotificationInfo.BCRMNotificationType == Enums.BCRMNotificationEnum.PaymentFail)
            {
                _btnPrimary = new CustomUIButtonV2
                {
                    Frame = new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48))
                };
                UpdateCTA(ref _btnPrimary, true);
                _btnPrimary.SetTitle(GetCommonI18NValue(Constants.Common_TryAgain), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnPay();
                }));
                _viewCTA.AddSubview(_btnPrimary);
            }
        }

        private void EvaluateSSMRCTA()
        {
            _btnPrimary = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, GetScaledHeight(48))
            };
            if (NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.OpenMeterReadingPeriod
                || NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.NoSubmissionReminder
                || NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.MissedSubmission)
            {
                _btnPrimary.Enabled = NotificationInfo.IsSMRPeriodOpen;
                UpdateCTA(ref _btnPrimary, true, NotificationInfo.IsSMRPeriodOpen);
                _btnPrimary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_SubmitMeterReading), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSubmitMeterReading();
                }));
            }
            else if (NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.TerminationCompleted)
            {
                _btnPrimary.Enabled = _isCTAEnabled;
                UpdateCTA(ref _btnPrimary, false, _isCTAEnabled);
                _btnPrimary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_ReenableSSMR), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnViewMeterReadingHistory();
                }));
            }
            else if (NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.RegistrationCompleted)
            {
                UpdateCTA(ref _btnPrimary);
                _btnPrimary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_ViewMyUsage), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnViewUsage();
                }));
            }
            else if (NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.RegistrationCancelled
                || NotificationInfo.SSMRNotificationType == Enums.SSMRNotificationEnum.TerminationCancelled)
            {
                UpdateCTA(ref _btnPrimary, false);
                _btnPrimary.SetTitle(GetI18NValue(PushNotificationConstants.I18N_ContactTNB), UIControlState.Normal);
                _btnPrimary.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnContact();
                }));
            }
            _viewCTA.AddSubview(_btnPrimary);
        }

        private void UpdateCTA(ref CustomUIButtonV2 btn, bool isSolidBg = true, bool isEnabled = true)
        {
            if (isSolidBg)
            {
                btn.BackgroundColor = isEnabled ? MyTNBColor.FreshGreen : MyTNBColor.BrownGrey;
                btn.SetTitleColor(UIColor.White, UIControlState.Normal);
                btn.Layer.BorderColor = (isEnabled ? MyTNBColor.FreshGreen : MyTNBColor.BrownGrey).CGColor;
            }
            else
            {
                btn.BackgroundColor = UIColor.White;
                btn.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
                btn.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
                btn.Layer.BorderWidth = 1;
            }
        }

        private bool IsValidWeblinks
        {
            get { return DataManager.DataManager.SharedInstance.WebLinks != null; }
        }

        #region CTA Events
        private void OnPay()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                        SelectBillsViewController viewController = storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                        if (viewController != null)
                        {
                            DataManager.DataManager.SharedInstance.SelectAccount(NotificationInfo.AccountNum);
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void OnViewDetails()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.SelectAccount(NotificationInfo.AccountNum);
                        UIStoryboard storyBoard = UIStoryboard.FromName("BillDetails", null);
                        BillDetailsViewController viewController =
                            storyBoard.InstantiateViewController("BillDetailsView") as BillDetailsViewController;
                        if (viewController != null)
                        {
                            viewController.AccountNumber = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
                            viewController.IsFreshCall = true;
                            viewController.IsRoot = true;
                            NavigationController.PushViewController(viewController, true);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void OnViewUsage()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.SelectAccount(NotificationInfo.AccountNum);
                        UIStoryboard storyboard = UIStoryboard.FromName("Usage", null);
                        UsageViewController viewController = storyboard.InstantiateViewController("UsageViewController") as UsageViewController;
                        if (viewController != null)
                        {
                            NavigationController.PushViewController(viewController, true);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void OnViewBill()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.SelectAccount(NotificationInfo.AccountNum);
                        UIStoryboard storyboard = UIStoryboard.FromName("Dashboard", null);
                        BillViewController viewController = storyboard.InstantiateViewController("BillViewController") as BillViewController;
                        if (viewController != null)
                        {
                            NavigationController.PushViewController(viewController, true);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void OnViewReceipt()
        {
            if (NotificationInfo == null || !NotificationInfo.MerchantTransId.IsValid())
            {
                DisplayServiceError(string.Empty);
                return;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Receipt", null);
                        ReceiptViewController viewController =
                            storyBoard.InstantiateViewController("ReceiptViewController") as ReceiptViewController;
                        if (viewController != null)
                        {
                            viewController.DetailedInfoNumber = NotificationInfo.MerchantTransId;
                            viewController.showAllReceipts = true;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void OnContact()
        {
            if (IsValidWeblinks)
            {
                int index = DataManager.DataManager.SharedInstance.WebLinks.FindIndex(x => x.Code.ToLower().Equals("tnbcle"));
                if (index > -1)
                {
                    string number = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                    if (!string.IsNullOrEmpty(number) && !string.IsNullOrWhiteSpace(number))
                    {
                        NSUrl url = new NSUrl(new Uri("tel:" + number).AbsoluteUri);
                        UIApplication.SharedApplication.OpenUrl(url);
                        return;
                    }
                }
            }
            DisplayServiceError(string.Empty);
        }

        private void OnViewMeterReadingHistory()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        DataManager.DataManager.SharedInstance.SelectAccount(NotificationInfo.AccountNum);
                        UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
                        SSMRReadingHistoryViewController viewController =
                            storyBoard.InstantiateViewController("SSMRReadingHistoryViewController") as SSMRReadingHistoryViewController;
                        if (viewController != null)
                        {
                            viewController.IsFromNotification = true;
                            viewController.IsRoot = true;
                            viewController.AccountNumber = NotificationInfo.AccountNum;
                            NavigationController.PushViewController(viewController, true);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void OnSubmitMeterReading()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        DataManager.DataManager.SharedInstance.SelectAccount(NotificationInfo.AccountNum);
                        await GetSMRAccountActivityInfo(DataManager.DataManager.SharedInstance.SelectedAccount).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_smrActivityInfoResponse != null &&
                                    _smrActivityInfoResponse.d != null &&
                                    _smrActivityInfoResponse.d.data != null &&
                                    _smrActivityInfoResponse.d.IsSuccess)
                                {
                                    SSMRActivityInfoCache.SetReadingHistoryCache(_smrActivityInfoResponse
                                        , DataManager.DataManager.SharedInstance.SelectedAccount);
                                    if (SSMRActivityInfoCache.ViewPreviousReading != null &&
                                        SSMRActivityInfoCache.ViewPreviousReading.Count > 0)
                                    {
                                        UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
                                        SSMRReadMeterViewController viewController =
                                            storyBoard.InstantiateViewController("SSMRReadMeterViewController") as SSMRReadMeterViewController;
                                        if (viewController != null)
                                        {
                                            viewController.IsRoot = true;
                                            NavigationController.PushViewController(viewController, true);
                                        }
                                    }
                                    else
                                    {
                                        var title = SSMRActivityInfoCache.ViewDataModel.DisplayTitle;
                                        var msg = SSMRActivityInfoCache.ViewDataModel.DisplayMessage;
                                        string displayTitle = (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title)) ? title : "Sorry, we are unable to perform this action right now.";
                                        string displayMsg = (!string.IsNullOrEmpty(msg) && !string.IsNullOrWhiteSpace(msg)) ? msg : "Please try again later. If this problem persists, contact the <b><a href=\"tel: 1300885454\">TNB Careline</a></b> and we will help you.";
                                        DisplayCustomAlert(displayTitle, displayMsg, GetCommonI18NValue(Constants.Common_GotIt), null);
                                    }
                                }
                                else
                                {
                                    DisplayServiceError(_smrActivityInfoResponse?.d?.DisplayMessage ?? string.Empty);
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

        private void OnDeleteNotification()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await DeleteUserNotification(NotificationInfo?.Id);
                        if (_deleteNotificationResponse != null && _deleteNotificationResponse?.d != null
                            && _deleteNotificationResponse?.d?.didSucceed == true)
                        {
                            DataManager.DataManager.SharedInstance.IsNotificationDeleted = true;
                            var index = DataManager.DataManager.SharedInstance.UserNotifications.FindIndex(x => x.Id == NotificationInfo?.Id);
                            if (index > -1)
                            {
                                DataManager.DataManager.SharedInstance.UserNotifications.RemoveAt(index);
                            }
                            NavigationController?.PopViewController(true);
                        }
                        else
                        {
                            DisplayServiceError(_deleteNotificationResponse?.d?.DisplayMessage ?? string.Empty);
                        }
                        ActivityIndicator.Hide();
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }
        #endregion

        #region Service Call
        private Task GetSMRAccountActivityInfo(CustomerAccountRecordModel account)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object request = new
                {
                    contractAccount = account.accNum,
                    isOwnedAccount = account.isOwned,
                    serviceManager.usrInf
                };
                _smrActivityInfoResponse = serviceManager.OnExecuteAPIV6<SMRAccountActivityInfoResponseModel>(PushNotificationConstants.Service_GetSMRAccountActivityInfo, request);
            });
        }

        private Task DeleteUserNotification(string id)
        {
            var user = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    updatedNotifications = new List<UpdateNotificationModel>(){
                        new UpdateNotificationModel()
                        {
                            NotificationType = NotificationInfo.NotificationType,
                            NotificationId = id
                        }
                    }
                };
                _deleteNotificationResponse = serviceManager.OnExecuteAPIV6<DeleteNotificationResponseModel>(PushNotificationConstants.Service_DeleteNotification, requestParameter);
            });
        }
        #endregion
    }
}