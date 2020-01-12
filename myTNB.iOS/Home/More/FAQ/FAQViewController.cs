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
using myTNB.SitecoreCMS;

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
                        SitecoreServices.Instance.LoadFAQs().ContinueWith(task =>
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
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void GetFAQContent()
        {
            try
            {
                string faqContent = FAQManager.Instance.GetFAQ(TNBGlobal.APP_LANGUAGE == "EN" ? FAQManager.Language.EN : FAQManager.Language.MS);
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