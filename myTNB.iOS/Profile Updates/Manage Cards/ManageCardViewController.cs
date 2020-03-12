using System;
using UIKit;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Home.More.MyAccount.ManageCards;
using myTNB.Model;
using System.Threading.Tasks;
using myTNB.DataManager;
using myTNB.MyAccount;
using System.Collections.Generic;

namespace myTNB
{
    public partial class ManageCardViewController : CustomUIViewController
    {
        public ManageCardViewController(IntPtr handle) : base(handle) { }

        private BaseResponseModelV2 _removeCardResponse = new BaseResponseModelV2();

        private UIView _viewNotificationMsg;
        private UILabel _lblNotificationDetails, _lblNoCards, _lblTitle;
        private UIImageView _imgNoCards;

        public override void ViewDidLoad()
        {
            PageName = MyAccountConstants.Pagename_ManageCards;
            base.ViewDidLoad();
            SetNavigationBar();
            SetSubviews();
            manageCardsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 156 : 132, View.Frame.Width, View.Frame.Height - 132);
            manageCardsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            manageCardsTableView.Source = new ManageCardsDataSource(this);
            manageCardsTableView.ReloadData();
            InitializeNotificationMessage();
        }

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(MyAccountConstants.I18N_Title));
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        private void SetSubviews()
        {
            _lblTitle = new UILabel(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 36))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey(),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = GetI18NValue(MyAccountConstants.I18N_Details)
            };
            View.AddSubview(_lblTitle);
        }

        private void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64))
                {
                    BackgroundColor = MyTNBColor.SunGlow,
                    Hidden = true
                };
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = TNBGlobal.EMPTY_ADDRESS,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };

                _viewNotificationMsg.AddSubview(_lblNotificationDetails);

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(_viewNotificationMsg);
            }
        }

        internal void ShowNotificationMessage()
        {
            _viewNotificationMsg.Hidden = false;
            _viewNotificationMsg.Alpha = 1.0f;
            UIView.Animate(1, 3, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewNotificationMsg.Alpha = 0.0f;
            }, () =>
            {
                _viewNotificationMsg.Hidden = true;
            });
        }

        private void ExecuteRemoveAccount(int index, string lastDigits)
        {
            ActivityIndicator.Show();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        RemoveRegisteredCard(index).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (ServiceCall.ValidateBaseResponse(_removeCardResponse))
                                {
                                    ServiceCall.GetRegisteredCards().ContinueWith(cardTask =>
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            OnDeleteCardDone(lastDigits);

                                            if (DataManager.DataManager.SharedInstance.RegisteredCards != null && DataManager.DataManager.SharedInstance.RegisteredCards.d != null)
                                            {
                                                if (!DataManager.DataManager.SharedInstance.RegisteredCards.d.IsSuccess)
                                                {
                                                    DisplayCustomAlert(GetErrorI18NValue(Constants.Error_DefaultErrorTitle),
                                                    GetErrorI18NValue(Constants.Error_ProfileCCErrorMsg),
                                                    new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_Ok), null } });
                                                }
                                            }
                                            else
                                            {
                                                DisplayCustomAlert(GetErrorI18NValue(Constants.Error_DefaultErrorTitle),
                                                    GetErrorI18NValue(Constants.Error_ProfileCCErrorMsg),
                                                    new Dictionary<string, Action> { { GetCommonI18NValue(Constants.Common_Ok), null } });
                                            }
                                        });
                                    });
                                }
                                else
                                {
                                    _lblNotificationDetails.Text = _removeCardResponse?.d?.DisplayMessage
                                        ?? string.Format(GetI18NValue(MyAccountConstants.I18N_CardNotRemovedMessage), lastDigits);
                                    ActivityIndicator.Hide();
                                    ShowNotificationMessage();
                                }
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        private void OnDeleteCardDone(string lastDigits)
        {
            if (DataManager.DataManager.SharedInstance.RegisteredCards != null
                && DataManager.DataManager.SharedInstance.RegisteredCards.d != null
                && DataManager.DataManager.SharedInstance.RegisteredCards.d.data != null
                && DataManager.DataManager.SharedInstance.RegisteredCards.d.data.Count > 0)
            {
                if (_imgNoCards != null && _lblNoCards != null)
                {
                    _imgNoCards.Hidden = true;
                    _lblNoCards.Hidden = true;
                }
                _lblTitle.Hidden = false;
                manageCardsTableView.Hidden = false;
                manageCardsTableView.Source = new ManageCardsDataSource(this);
                manageCardsTableView.ReloadData();
            }
            else
            {
                manageCardsTableView.Hidden = true;
                if (_imgNoCards == null || _lblNoCards == null)
                {
                    _imgNoCards = new UIImageView(new CGRect((View.Frame.Width / 2) - 75, 178, 150, 150))
                    {
                        Image = UIImage.FromBundle("Card-Empty")
                    };
                    _lblNoCards = new UILabel(new CGRect(44, 344, View.Frame.Width - 88, 60))
                    {
                        TextAlignment = UITextAlignment.Center,
                        Text = GetI18NValue(MyAccountConstants.I18N_NoCards),
                        Font = MyTNBFont.MuseoSans12_300,
                        TextColor = MyTNBColor.SilverChalice,
                        Lines = 0,
                        LineBreakMode = UILineBreakMode.WordWrap
                    };
                    nfloat newHeight = _lblNoCards.GetLabelHeight(1000);
                    _lblNoCards.Frame = new CGRect(_lblNoCards.Frame.Location, new CGSize(_lblNoCards.Frame.Width, newHeight));
                    View.AddSubviews(new UIView[] { _imgNoCards, _lblNoCards });
                }
                _imgNoCards.Hidden = false;
                _lblNoCards.Hidden = false;
                _lblTitle.Hidden = true;
            }
            _lblNotificationDetails.Text = string.Format(GetI18NValue(MyAccountConstants.I18N_CardRemoveSuccess), lastDigits);
            ActivityIndicator.Hide();
            ShowNotificationMessage();
        }

        internal void HandleDeleteCardEvent(int index)
        {
            string lastDigits = DataManager.DataManager.SharedInstance.RegisteredCards.d.data[index].LastDigits;
            lastDigits = lastDigits.Substring(lastDigits.Length - 4);
            string message = string.Format(GetI18NValue(MyAccountConstants.I18N_RemoveCardMessage), lastDigits);
            UIAlertController alert = UIAlertController.Create(GetI18NValue(MyAccountConstants.I18N_RemoveCardTitle)
                , message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Ok), UIAlertActionStyle.Default, (obj) =>
            {
                ExecuteRemoveAccount(index, lastDigits);
            }));
            alert.AddAction(UIAlertAction.Create(GetCommonI18NValue(Constants.Common_Cancel), UIAlertActionStyle.Cancel, (obj) => { }));
            alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        private Task RemoveRegisteredCard(int index)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    registeredCardId = DataManager.DataManager.SharedInstance.RegisteredCards.d.data[index].Id
                };
                _removeCardResponse = serviceManager.BaseServiceCallV6(MyAccountConstants.Service_RemoveRegisteredCard, requestParameter);
            });
        }
    }
}