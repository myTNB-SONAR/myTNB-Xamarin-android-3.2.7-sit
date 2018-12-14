using Foundation;
using System;
using UIKit;
using myTNB.DataManager;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB
{
    public partial class HomeTabBarController : UITabBarController
    {
        public HomeTabBarController(IntPtr handle) : base(handle)
        {
        }

        string _imageSize = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Console.WriteLine("HOME DID LOAD");
            TabBar.Translucent = false;
            TabBar.BackgroundColor = UIColor.White;
            if (!DataManager.DataManager.SharedInstance.IsPromotionFirstLoad)
            {
                UpdatePromotions();
                DataManager.DataManager.SharedInstance.IsPromotionFirstLoad = true;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("HOME WILL APPEAR");
            UITabBarItem[] vcs = TabBar.Items;
            vcs[1].Enabled = ServiceCall.HasAccountList();
            UpdatePromotionTabBarIcon();
        }

        public override void ItemSelected(UITabBar tabbar, UITabBarItem item)
        {

        }

        bool HasUnreadPromotion(List<PromotionsModel> promotionList)
        {
            int index = promotionList.FindIndex(x => x.IsRead == false);
            Console.WriteLine("HasUnreadPromotion: " + (index > -1).ToString());
            return index > -1;
        }

        void UpdatePromotions()
        {
            _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);
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
                                UpdatePromotionTabBarIcon();
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                });
            });
        }

        void UpdatePromotionTabBarIcon()
        {
            PromotionsEntity wsManager = new PromotionsEntity();
            List<PromotionsModel> promotionList = new List<PromotionsModel>();
            promotionList = wsManager.GetAllItems();
            if (promotionList != null && promotionList.Count > 0 && HasUnreadPromotion(promotionList))
            {
                TabBar.Items[2].SelectedImage = UIImage.FromBundle("Tab-Promotions-Unread-Active");
                TabBar.Items[2].Image = UIImage.FromBundle("Tab-Promotions-Unread-Inactive");
            }
            else
            {
                TabBar.Items[2].Image = UIImage.FromBundle("Tab-Promotions");
                TabBar.Items[2].SelectedImage = UIImage.FromBundle("Tab-Promotions");
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
    }
}