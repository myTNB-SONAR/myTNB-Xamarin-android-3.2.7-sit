using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.Promotions;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using Newtonsoft.Json;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System.Collections.Generic;

namespace myTNB
{
    public partial class PromotionsViewController : UIViewController
    {
        public PromotionsViewController(IntPtr handle) : base(handle)
        {
        }

        public bool IsDelegateNeeded = false;
        UIImageView imgViewNoPromotions;
        UILabel lblDetails;

        string _imageSize = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Console.WriteLine("PROMOTION DID LOAD");
            SetNavigationBar();
            promotionsTableView.Frame = new CGRect(0
                                                   , DeviceHelper.IsIphoneX() ? 88 : 64
                                                   , View.Frame.Width
                                                   , View.Frame.Height - 49 - (DeviceHelper.IsIphoneX() ? 88 : 64));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("PROMOTION WILL APPEAR");
            _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);
            promotionsTableView.Hidden = true;
            promotionsTableView.Source = new PromotionsDataSource(this, new List<PromotionsModel>());
            promotionsTableView.ReloadData();
            //SetSubViews();
            if (DataManager.DataManager.SharedInstance.IsPromotionFirstLoad)
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ActivityIndicator.Show();
                            GetPromotions().ContinueWith(task =>
                            {
                                InvokeOnMainThread(() =>
                                {
                                    SetSubViews();
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
            else
            {
                SetSubViews();
            }
        }

        Task GetPromotions()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, _imageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                bool isValidTimeStamp = false;
                string promotionTS = iService.GetPromotionsTimestampItem();
                PromotionsTimestampResponseModel promotionTimeStamp = JsonConvert.DeserializeObject<PromotionsTimestampResponseModel>(promotionTS);
                if (promotionTimeStamp != null && promotionTimeStamp.Status.Equals("Success")
                    && promotionTimeStamp.Data != null && promotionTimeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(promotionTimeStamp.Data[0].Timestamp)
                    && !string.IsNullOrWhiteSpace(promotionTimeStamp.Data[0].Timestamp))
                {
                    var sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCorePromotionTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, "SiteCorePromotionTimeStamp");
                        sharedPreference.Synchronize();
                        isValidTimeStamp = true;
                    }
                    else
                    {
                        if (currentTS.Equals(promotionTimeStamp.Data[0].Timestamp))
                        {
                            isValidTimeStamp = false;
                        }
                        else
                        {
                            sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, "SiteCorePromotionTimeStamp");
                            sharedPreference.Synchronize();
                            isValidTimeStamp = true;
                        }
                    }
                }
                //isValidTimeStamp = true;
                if (isValidTimeStamp)
                {
                    string promotionsItems = iService.GetPromotionsItem();
                    PromotionsResponseModel promotionResponse = JsonConvert.DeserializeObject<PromotionsResponseModel>(promotionsItems);
                    if (promotionResponse != null && promotionResponse.Status.Equals("Success")
                        && promotionResponse.Data != null && promotionResponse.Data.Count > 0)
                    {
                        PromotionsEntity wsManager = new PromotionsEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(promotionResponse.Data);
                    }
                }
            });
        }

        internal void SetNavigationBar()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Promotions");
            titleBarComponent.SetNotificationVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        void SetSubViews()
        {
            if (imgViewNoPromotions != null)
            {
                imgViewNoPromotions.RemoveFromSuperview();
            }
            if (lblDetails != null)
            {
                lblDetails.RemoveFromSuperview();
            }
            PromotionsEntity wsManager = new PromotionsEntity();
            List<PromotionsModel> promotionList = new List<PromotionsModel>();
            promotionList = wsManager.GetAllItems();
            if (promotionList != null && promotionList.Count > 0)
            {
                promotionsTableView.ClearsContextBeforeDrawing = true;
                promotionsTableView.Source = new PromotionsDataSource(this, promotionList);
                promotionsTableView.ReloadData();
                promotionsTableView.Hidden = false;
                promotionsTableView.ShowsVerticalScrollIndicator = false;
                promotionsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            }
            else
            {
                promotionsTableView.Hidden = true;
                SetNoPromotionScreen();
            }
        }

        void SetNoPromotionScreen()
        {
            imgViewNoPromotions = new UIImageView(new CGRect(DeviceHelper.GetScaledSizeByWidth(26.6f)
                                                                         , DeviceHelper.GetScaledSizeByHeight(32.6f)
                                                                         , DeviceHelper.GetScaledSizeByWidth(46.9f)
                                                                         , DeviceHelper.GetScaledSizeByHeight(26.4f)));
            imgViewNoPromotions.Image = UIImage.FromBundle(("IC-Empty-Promotion"));

            lblDetails = new UILabel(new CGRect(44
                                                        , DeviceHelper.GetScaledSizeByHeight(61.8f)
                                                        , View.Frame.Width - 88
                                                       , 32));
            lblDetails.Text = "No promotions currently.\r\nCheck back later!";
            lblDetails.TextColor = myTNBColor.SilverChalice();
            lblDetails.Font = myTNBFont.MuseoSans12();
            lblDetails.Lines = 2;
            lblDetails.TextAlignment = UITextAlignment.Center;

            View.AddSubviews(new UIView[] { imgViewNoPromotions, lblDetails });
        }

        internal void OnPromotionItemSelect(PromotionsModel promotion)
        {
            ActivityIndicator.Show();
            UIStoryboard storyBoard = UIStoryboard.FromName("PromotionDetails", null);
            PromotionDetailsViewController viewController =
                storyBoard.InstantiateViewController("PromotionDetailsViewController") as PromotionDetailsViewController;
            viewController.Promotion = promotion;
            var navController = new UINavigationController(viewController);
            PresentViewController(navController, true, null);
            ActivityIndicator.Hide();
        }
    }
}