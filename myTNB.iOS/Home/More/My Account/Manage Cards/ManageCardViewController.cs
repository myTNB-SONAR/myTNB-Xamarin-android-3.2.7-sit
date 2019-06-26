using System;
using UIKit;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Home.More.MyAccount.ManageCards;
using myTNB.Model;
using System.Threading.Tasks;
using myTNB.DataManager;

namespace myTNB
{
    public partial class ManageCardViewController : UIViewController
    {
        public ManageCardViewController(IntPtr handle) : base(handle)
        {
        }

        BaseResponseModel _removeCardResponse = new BaseResponseModel();

        UIView _viewNotificationMsg;
        UILabel _lblNotificationDetails;
        UIImageView _imgNoCards;
        UILabel _lblNoCards;
        UILabel _lblTitle;

        public override void ViewDidLoad()
        {
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

        internal void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Manage_CardsDebitTitle".Translate());
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void SetSubviews()
        {
            _lblTitle = new UILabel(new CGRect(18, DeviceHelper.IsIphoneXUpResolution() ? 104 : 80, View.Frame.Width - 36, 36));
            _lblTitle.Font = MyTNBFont.MuseoSans14_300;
            _lblTitle.TextColor = MyTNBColor.TunaGrey();
            _lblTitle.Lines = 0;
            _lblTitle.LineBreakMode = UILineBreakMode.WordWrap;
            _lblTitle.Text = "Manage_AddCardMessage".Translate();
            View.AddSubview(_lblTitle);
        }

        internal void InitializeNotificationMessage()
        {
            if (_viewNotificationMsg == null)
            {
                _viewNotificationMsg = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 64));
                _viewNotificationMsg.BackgroundColor = MyTNBColor.SunGlow;
                _viewNotificationMsg.Layer.CornerRadius = 2.0f;
                _viewNotificationMsg.Hidden = true;

                _lblNotificationDetails = new UILabel(new CGRect(16, 16, _viewNotificationMsg.Frame.Width - 32, 32));
                _lblNotificationDetails.TextAlignment = UITextAlignment.Left;
                _lblNotificationDetails.Font = MyTNBFont.MuseoSans12;
                _lblNotificationDetails.TextColor = MyTNBColor.TunaGrey();
                _lblNotificationDetails.Text = TNBGlobal.EMPTY_ADDRESS;
                _lblNotificationDetails.Lines = 0;
                _lblNotificationDetails.LineBreakMode = UILineBreakMode.WordWrap;

                _viewNotificationMsg.AddSubview(_lblNotificationDetails);

                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                currentWindow.AddSubview(_viewNotificationMsg);
            }
        }

        internal void ShowNotificationMessage()
        {
            _viewNotificationMsg.Hidden = false;
            _viewNotificationMsg.Alpha = 1.0f;
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewNotificationMsg.Alpha = 0.0f;
            }, () =>
            {
                _viewNotificationMsg.Hidden = true;
            });
        }

        internal void ExecuteRemoveAccount(int index, string lastDigits)
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
                                        });
                                    });
                                }
                                else
                                {
                                    _lblNotificationDetails.Text = string.Format("Manage_CardNotRemovedMessage".Translate(), lastDigits);
                                    ActivityIndicator.Hide();
                                    ShowNotificationMessage();
                                }
                            });
                        });
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        void OnDeleteCardDone(string lastDigits)
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
                    _imgNoCards = new UIImageView(new CGRect((View.Frame.Width / 2) - 75, 178, 150, 150));
                    _imgNoCards.Image = UIImage.FromBundle("Card-Empty");
                    _lblNoCards = new UILabel(new CGRect(44, 344, View.Frame.Width - 88, 60));
                    _lblNoCards.TextAlignment = UITextAlignment.Center;
                    _lblNoCards.Text = "Manage_NoSavedCards".Translate();
                    _lblNoCards.Font = MyTNBFont.MuseoSans12_300;
                    _lblNoCards.TextColor = MyTNBColor.SilverChalice;
                    _lblNoCards.Lines = 3;
                    View.AddSubviews(new UIView[] { _imgNoCards, _lblNoCards });
                }
                _imgNoCards.Hidden = false;
                _lblNoCards.Hidden = false;
                _lblTitle.Hidden = true; ;
            }
            _lblNotificationDetails.Text = string.Format("Manage_CardRemovedSuccessMessage".Translate(), lastDigits);
            ActivityIndicator.Hide();
            ShowNotificationMessage();
        }

        internal void HandleDeleteCardEvent(int index)
        {
            string lastDigits = DataManager.DataManager.SharedInstance.RegisteredCards.d.data[index].LastDigits;
            lastDigits = lastDigits.Substring(lastDigits.Length - 4);
            string message = string.Format("Manage_RemoveCardMessage".Translate(), lastDigits);
            var alert = UIAlertController.Create("Manage_RemoveCardTitle".Translate(), message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Common_Ok".Translate(), UIAlertActionStyle.Default, (obj) =>
            {
                ExecuteRemoveAccount(index, lastDigits);
            }));
            alert.AddAction(UIAlertAction.Create("Common_Cancel".Translate(), UIAlertActionStyle.Cancel, (obj) => { }));
            PresentViewController(alert, animated: true, completionHandler: null);
        }

        internal Task RemoveRegisteredCard(int index)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    registeredCardId = DataManager.DataManager.SharedInstance.RegisteredCards.d.data[index].Id
                };
                _removeCardResponse = serviceManager.BaseServiceCall("RemoveRegisteredCard", requestParameter);
            });
        }
    }
}