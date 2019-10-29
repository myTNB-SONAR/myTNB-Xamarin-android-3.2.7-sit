using System;
using UIKit;
using myTNB.Model;
using Newtonsoft.Json;
using CoreGraphics;
using myTNB.Home.More.FAQ;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using myTNB.SitecoreCMS.Model;
using Foundation;
using myTNB.SQLite.SQLiteDataManager;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.Profile;

namespace myTNB
{
    public partial class FAQViewController : CustomUIViewController
    {
        private FAQModel _faq = new FAQModel();
        private string _imageSize = string.Empty;
        public string faqId;
        public FAQViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            PageName = ProfileConstants.Pagename_FAQ;
            base.ViewDidLoad();
            SetNavigationBar();
            tableviewFAQ.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - 64);
            tableviewFAQ.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);
            tableviewFAQ.Source = new FAQDataSource(new List<FAQDataModel>());
            tableviewFAQ.ReloadData();
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        GetFAQs().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                FAQEntity wsManager = new FAQEntity();
                                List<FAQsModel> faqList = wsManager.GetAllItems();
                                if (faqList != null && faqList.Count > 0)
                                {
                                    tableviewFAQ.Source = new FAQDataSource(faqList, true);
                                    tableviewFAQ.ReloadData();
                                    if (!string.IsNullOrEmpty(faqId))
                                    {
                                        ScrollToRow(faqList, faqId);
                                    }
                                }
                                else
                                {
                                    GetFAQContent();
                                    tableviewFAQ.Source = new FAQDataSource(_faq.data);
                                    tableviewFAQ.ReloadData();
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
        /// <summary>
        /// Scrolls to specific table row based on faqID parameter.
        /// </summary>
        /// <param name="faqList">FAQ list.</param>
        /// <param name="fId">F identifier.</param>
        private void ScrollToRow(List<FAQsModel> faqList, string fId)
        {
            int sectionIndex = faqList.FindIndex(x => x.ID == fId);

            if (sectionIndex > -1)
            {
                NSIndexPath path = NSIndexPath.FromRowSection(0, sectionIndex);
                tableviewFAQ.ScrollToRow(path, UITableViewScrollPosition.Top, false);
            }
        }

        private void SetNavigationBar()
        {
            Title = GetI18NValue(ProfileConstants.I18N_NavTitle);
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private Task GetFAQs()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, _imageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                bool isValidTimeStamp = false;
                string faqTS = iService.GetFAQsTimestampItem();
                FAQTimestampResponseModel faqTimeStamp = JsonConvert.DeserializeObject<FAQTimestampResponseModel>(faqTS);
                if (faqTimeStamp != null && faqTimeStamp.Status.Equals("Success")
                    && faqTimeStamp.Data != null && faqTimeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(faqTimeStamp.Data[0].Timestamp)
                    && !string.IsNullOrWhiteSpace(faqTimeStamp.Data[0].Timestamp))
                {
                    NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCoreFAQTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(faqTimeStamp.Data[0].Timestamp, "SiteCoreFAQTimeStamp");
                        sharedPreference.Synchronize();
                        isValidTimeStamp = true;
                    }
                    else
                    {
                        if (currentTS.Equals(faqTimeStamp.Data[0].Timestamp))
                        {
                            isValidTimeStamp = false;
                        }
                        else
                        {
                            sharedPreference.SetString(faqTimeStamp.Data[0].Timestamp, "SiteCoreFAQTimeStamp");
                            sharedPreference.Synchronize();
                            isValidTimeStamp = true;
                        }
                    }
                }
                if (isValidTimeStamp)
                {
                    string faqItems = iService.GetFAQsItem();
                    FAQsResponseModel faqResponse = JsonConvert.DeserializeObject<FAQsResponseModel>(faqItems);
                    if (faqResponse != null && faqResponse.Status.Equals("Success")
                        && faqResponse.Data != null && faqResponse.Data.Count > 0)
                    {
                        FAQEntity wsManager = new FAQEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(faqResponse.Data);
                    }
                }
            });
        }

        private void GetFAQContent()
        {
            try
            {
                string faqContent = System.IO.File.ReadAllText("FAQ.json");
                _faq = JsonConvert.DeserializeObject<FAQModel>(faqContent);
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
                _faq = new FAQModel();
            }
        }
    }
}