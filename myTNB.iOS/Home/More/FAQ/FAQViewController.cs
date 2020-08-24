using System;
using UIKit;
using Newtonsoft.Json;
using CoreGraphics;
using myTNB.Home.More.FAQ;
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
        private List<FAQsModel> _faq = new List<FAQsModel>();
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
            tableviewFAQ.Source = new FAQDataSource(new List<FAQsModel>());
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
                                _faq = wsManager.GetAllItems();

                                if (_faq == null || (_faq != null && _faq.Count == 0))
                                {
                                    GetFAQContent();
                                }
                                tableviewFAQ.Source = new FAQDataSource(_faq);
                                tableviewFAQ.ReloadData();
                                if (!string.IsNullOrEmpty(faqId))
                                {
                                    ScrollToRow(_faq, faqId);
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

        private void ScrollToRow(List<FAQsModel> faqList, string fId)
        {
            try
            {
                int sectionIndex = faqList.FindIndex(x => x.ID == fId);
                if (sectionIndex > -1)
                {
                    NSIndexPath path = NSIndexPath.FromRowSection(0, sectionIndex);
                    tableviewFAQ.ScrollToRow(path, UITableViewScrollPosition.Top, false);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("DEBUG - ScrollToRow Error: " + e.Message);
#endif
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
                _faq = JsonConvert.DeserializeObject<List<FAQsModel>>(faqContent);
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
                _faq = new List<FAQsModel>();
            }
        }
    }
}