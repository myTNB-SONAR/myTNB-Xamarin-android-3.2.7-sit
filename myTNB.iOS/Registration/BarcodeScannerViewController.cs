using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;
using ZXing.Mobile;

namespace myTNB.Registration
{
    public partial class BarcodeScannerViewController : UIViewController
    {
        public BarcodeScannerViewController(IntPtr handle) : base(handle)
        {
        }
        UIStatusBarStyle originalStatusBarStyle = UIStatusBarStyle.Default;
        const string ACCOUNT_NO_PATTERN = @"^[0-9]{12,14}$";
        ZXingScannerView scannerView;
        UIView scanLayerView;
        UILabel lblScanStatus;

        MobileBarcodeScanningOptions ScanningOptions { get; set; }
        MobileBarcodeScanner Scanner { get; set; }
        bool ContinuousScanning { get; set; }

        public UIViewController AsViewController()
        {
            return this;
        }

        public void Cancel()
        {
            this.InvokeOnMainThread(scannerView.StopScanning);
        }


        public override void ViewDidLoad()
        {
            NavigationItem.Title = "Add Electricity Account";
            ActivityIndicator.Show();
            UIApplication.SharedApplication.StatusBarOrientation = UIInterfaceOrientation.Portrait;
            this.NavigationItem.HidesBackButton = true;
            AddBackButton();
            SetScanningOptions();
            InitializeScanView();
        }

        internal void SetScanningOptions()
        {
            this.ScanningOptions = new MobileBarcodeScanningOptions();//options;
            this.ScanningOptions.AutoRotate = false;
            this.ScanningOptions.TryHarder = true;
            this.ScanningOptions.TryInverted = true;
            this.ScanningOptions.DelayBetweenContinuousScans = 2000;
        }

        internal void InitializeScanView()
        {
            UILabel lblDescription = new UILabel
            {
                Frame = new CGRect(16, 16, View.Frame.Width - 32, 60),
                AttributedText = new NSAttributedString(
                    "Scan the barcode on your bill to retrieve account details.",
                    font: myTNBFont.MuseoSans16_300(),
                    foregroundColor: myTNBColor.TunaGrey(),
                    strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left
            };
            lblDescription.Lines = 0;
            lblDescription.LineBreakMode = UILineBreakMode.WordWrap;
            View.AddSubview(lblDescription);


            UIView scanOverlayView = new UIView(new CGRect(0, 70, View.Frame.Width, View.Frame.Height - 70));

            nfloat overlayWidth = scanOverlayView.Frame.Width;
            nfloat overlayHeight = scanOverlayView.Frame.Height;
            UIColor scanBGColor = new UIColor(red: 0.73f, green: 0.73f, blue: 0.74f, alpha: 0.3f);

            UIView topLayer = new UIView(new CGRect(0, 0, overlayWidth, (overlayHeight / 2) - 120));
            topLayer.BackgroundColor = scanBGColor;

            UIView leftLayer = new UIView(new CGRect(0, (overlayHeight / 2) - 120, 18, 120));
            leftLayer.BackgroundColor = scanBGColor;

            scanLayerView = new UIView(new CGRect(18, (overlayHeight / 2) - 120, overlayWidth - 36, 120));
            scanLayerView.Layer.BorderWidth = 2.0f;
            scanLayerView.Layer.BorderColor = myTNBColor.SilverChalice().CGColor;
            //scanLayerView.Layer.CornerRadius = 10.0f;

            UIView rightLayer = new UIView(new CGRect(scanOverlayView.Frame.Width - 18, (overlayHeight / 2) - 120, 18, 120));
            rightLayer.BackgroundColor = scanBGColor;

            UIView bottomLayer = new UIView(new CGRect(0, (overlayHeight / 2), overlayWidth, (overlayHeight / 2)));
            bottomLayer.BackgroundColor = scanBGColor;

            lblScanStatus = new UILabel
            {
                Frame = new CGRect(0, (overlayHeight / 2) + 20, overlayWidth, 16),
                AttributedText = new NSAttributedString(
                    "Invalid barcode.",
                    font: myTNBFont.MuseoSans12(),
                    foregroundColor: myTNBColor.Tomato(),
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

            scannerView = new ZXingScannerView(new CGRect(0, lblDescription.Frame.GetMaxY() + 10, View.Frame.Width, View.Frame.Height - 70));
            scannerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            scannerView.UseCustomOverlayView = true;
            scannerView.CustomOverlayView = scanOverlayView;

            scannerView.OnCancelButtonPressed += delegate
            {
                this.NavigationController.PopViewController(true);
            };

            this.View.AddSubview(scannerView);
            this.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
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

                originalStatusBarStyle = UIApplication.SharedApplication.StatusBarStyle;

                if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                {
                    UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
                    SetNeedsStatusBarAppearanceUpdate();
                }
                else
                {
                    UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.BlackTranslucent, false);
                }

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
                                Regex regex = new Regex(ACCOUNT_NO_PATTERN);
                                Match match = regex.Match(accountNumber);
                                if (match.Success)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        scanLayerView.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
                                        lblScanStatus.Hidden = true;

                                        DataManager.DataManager.SharedInstance.AccountNumber = accountNumber;
                                        this.NavigationController?.PopViewController(true);
                                    });
                                }
                                else
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        scanLayerView.Layer.BorderColor = myTNBColor.Tomato().CGColor;
                                        lblScanStatus.Hidden = false;
                                    });
                                }
                            }
                        }
                    }, this.ScanningOptions));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
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

        public override void ViewWillDisappear(bool animated)
        {
            UIApplication.SharedApplication.SetStatusBarStyle(originalStatusBarStyle, false);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            if (scannerView != null)
            {
                scannerView.DidRotate(this.InterfaceOrientation);
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

        void HandleOnScannerSetupComplete()
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

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.NavigationController.PopViewController(true);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }
    }
}