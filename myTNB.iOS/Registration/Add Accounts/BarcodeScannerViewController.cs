using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;
using ZXing.Mobile;

namespace myTNB.Registration
{
    public partial class BarcodeScannerViewController : CustomUIViewController
    {
        public BarcodeScannerViewController(IntPtr handle) : base(handle) { }

        private ZXingScannerView scannerView;
        private UIView scanLayerView;
        private UILabel lblScanStatus;

        private MobileBarcodeScanningOptions ScanningOptions { get; set; }
        private bool ContinuousScanning { get; set; }

        public UIViewController AsViewController()
        {
            return this;
        }

        public void Cancel()
        {
            InvokeOnMainThread(scannerView.StopScanning);
        }

        public override void ViewDidLoad()
        {
            PageName = AddAccountConstants.PageName;
            base.ViewDidLoad();
            NavigationItem.Title = GetI18NValue(AddAccountConstants.I18N_NavTitle);
            ActivityIndicator.Show();
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
            UIApplication.SharedApplication.StatusBarOrientation = UIInterfaceOrientation.Portrait;
            NavigationItem.HidesBackButton = true;
            AddBackButton();
            SetScanningOptions();
            InitializeScanView();
        }

        private void SetScanningOptions()
        {
            ScanningOptions = new MobileBarcodeScanningOptions
            {
                AutoRotate = false,
                TryHarder = true,
                TryInverted = true,
                DelayBetweenContinuousScans = 2000
            };//options;
        }

        private void InitializeScanView()
        {
            UILabel lblDescription = new UILabel
            {
                Frame = new CGRect(16, 16, View.Frame.Width - 32, 60),
                AttributedText = new NSAttributedString(
                    GetI18NValue(AddAccountConstants.I18N_ScanMessage),
                    font: MyTNBFont.MuseoSans16_300,
                    foregroundColor: MyTNBColor.TunaGrey(),
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            View.AddSubview(lblDescription);


            UIView scanOverlayView = new UIView(new CGRect(0, 70, View.Frame.Width, View.Frame.Height - 70));

            nfloat overlayWidth = scanOverlayView.Frame.Width;
            nfloat overlayHeight = scanOverlayView.Frame.Height;
            UIColor scanBGColor = new UIColor(red: 0.73f, green: 0.73f, blue: 0.74f, alpha: 0.3f);

            UIView topLayer = new UIView(new CGRect(0, 0, overlayWidth, (overlayHeight / 2) - 120))
            {
                BackgroundColor = scanBGColor
            };

            UIView leftLayer = new UIView(new CGRect(0, (overlayHeight / 2) - 120, 18, 120))
            {
                BackgroundColor = scanBGColor
            };

            scanLayerView = new UIView(new CGRect(18, (overlayHeight / 2) - 120, overlayWidth - 36, 120));
            scanLayerView.Layer.BorderWidth = 2.0f;
            scanLayerView.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;

            UIView rightLayer = new UIView(new CGRect(scanOverlayView.Frame.Width - 18, (overlayHeight / 2) - 120, 18, 120))
            {
                BackgroundColor = scanBGColor
            };

            UIView bottomLayer = new UIView(new CGRect(0, (overlayHeight / 2), overlayWidth, (overlayHeight / 2)))
            {
                BackgroundColor = scanBGColor
            };

            lblScanStatus = new UILabel
            {
                Frame = new CGRect(0, (overlayHeight / 2) + 20, overlayWidth, 16),
                AttributedText = new NSAttributedString(
                    GetI18NValue(AddAccountConstants.I18N_InvalidBarcode),
                    font: MyTNBFont.MuseoSans12,
                    foregroundColor: MyTNBColor.Tomato,
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Center
            };
            lblScanStatus.Hidden = true;
            scanOverlayView.AddSubview(lblScanStatus);

            scanOverlayView.AddSubview(topLayer);
            scanOverlayView.AddSubview(leftLayer);
            scanOverlayView.AddSubview(scanLayerView);
            scanOverlayView.AddSubview(rightLayer);
            scanOverlayView.AddSubview(bottomLayer);

            scannerView = new ZXingScannerView(new CGRect(0, lblDescription.Frame.GetMaxY() + 10, View.Frame.Width, View.Frame.Height - 70))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                UseCustomOverlayView = true,
                CustomOverlayView = scanOverlayView
            };

            scannerView.OnCancelButtonPressed += delegate
            {
                NavigationController.PopViewController(true);
            };

            View.AddSubview(scannerView);
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        }

        public void Torch(bool on)
        {
            if (scannerView != null)
                scannerView.Torch(on);
        }

        public void ToggleTorch()
        {
            if (scannerView != null)
                scannerView.ToggleTorch();
        }

        public void PauseAnalysis()
        {
            scannerView.PauseAnalysis();
        }

        public void ResumeAnalysis()
        {
            scannerView.ResumeAnalysis();
        }

        public bool IsTorchOn
        {
            get { return scannerView.IsTorchOn; }
        }

        public override void ViewDidAppear(bool animated)
        {
            try
            {
                scannerView.OnScannerSetupComplete += HandleOnScannerSetupComplete;
                Task.Factory.StartNew(() =>
                {
                    BeginInvokeOnMainThread(() => scannerView.StartScanning(result =>
                    {
                        if (!ContinuousScanning)
                        {
                            if (result != null)
                            {
                                string accountNumber = result.ToString();
                                if (!string.IsNullOrEmpty(accountNumber) && accountNumber?.Length > 12)
                                {
                                    accountNumber = accountNumber.Substring(0, 12);
                                }
                                Regex regex = new Regex(TNBGlobal.ACCOUNT_NO_PATTERN);
                                Match match = regex.Match(accountNumber);
                                if (match.Success)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        scanLayerView.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
                                        lblScanStatus.Hidden = true;

                                        DataManager.DataManager.SharedInstance.AccountNumber = accountNumber;
                                        NavigationController?.PopViewController(true);
                                    });
                                }
                                else
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        scanLayerView.Layer.BorderColor = MyTNBColor.Tomato.CGColor;
                                        lblScanStatus.Hidden = false;
                                    });
                                }
                            }
                        }
                    }, ScanningOptions));
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            if (scannerView != null)
            {
                scannerView.StopScanning();
            }
            scannerView.OnScannerSetupComplete -= HandleOnScannerSetupComplete;
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            if (scannerView != null)
            {
                scannerView.DidRotate(InterfaceOrientation);
            }
        }
        public override bool ShouldAutorotate()
        {
            if (ScanningOptions.AutoRotate != null)
            {
                return (bool)ScanningOptions.AutoRotate;
            }
            return false;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.Portrait;
        }

        private void HandleOnScannerSetupComplete()
        {
            BeginInvokeOnMainThread(() =>
            {
                ActivityIndicator.Hide();
                UIView.BeginAnimations("zoomout");
                UIView.SetAnimationDuration(2.0f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.EaseOut);
                UIView.CommitAnimations();
            });
        }

        private void AddBackButton()
        {
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = false;
            }

            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }
    }
}