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
using myTNB.Extensions;
using System.Diagnostics;

namespace myTNB
{
    public partial class FAQViewController : UIViewController
    {
        FAQModel _faq = new FAQModel();
        string _imageSize = string.Empty;
        public string faqId;
        public FAQViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
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
                        Debug.WriteLine("No Network");
                        ErrorHandler.DisplayNoDataAlert(this);
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

        void SetNavigationBar()
        {
            Title = _faq.title;
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        Task GetFAQs()
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
                    var sharedPreference = NSUserDefaults.StandardUserDefaults;
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
                //isValidTimeStamp = true;
                if (isValidTimeStamp)
                {
                    string faqItems = iService.GetFAQsItem();
                    //string faqItems = "{ \"Status\": \"Success\", \"Data\": [{ \"Image\": \"\", \"Question\": \"What is myTNB mobile app?\", \"Answer\": \"<p>myTNB mobile app is a digital service to manage your TNB electricity account. View and pay your electricity bills anytime, anywhere, and manage your TNB account on the go!<\\/p>\\n<p>&nbsp;<\\/p>\", \"ID\": \"{57F54274-71F9-4438-A654-D4F856A208D0}\" }, { \"Image\": \"\", \"Question\": \"What\u2019s new on this app?\", \"Answer\": \"<p>New features include a personalized, interactive Dashboard with a detailed view of your usage for the past 6 months, multiple account views, faster in-app payment with the option to save your credit cards, and a stress-free way to submit feedback, from bill-related matters to reporting faulty street lamps.<\\/p>\\n<p>&nbsp;<\\/p>\", \"ID\": \"{B0517580-60F4-45C5-8FE6-277F61216F68}\" }, { \"Image\": \"\", \"Question\": \"I have an existing myTNB account, can I use the same account?\", \"Answer\": \"<p><span>Yes, you certainly can! myTNB mobile app and myTNB Portal uses the same set of credentials (user ID and password).<\\/span><\\/p>\\n<p><span><\\/span><\\/p>\", \"ID\": \"{413D347F-45F5-467F-A564-6C79C551750A}\" }, { \"Image\": \"\", \"Question\": \"Where can I find my bill account number?\", \"Answer\": \"<p>Your 12 <span>&ndash;<\\/span> digit account number can be found on the top left corner of your monthly paper bill.<\\/p>\\n<p>&nbsp;<\\/p>\", \"ID\": \"{45DA5AC6-A038-428D-94EB-AB7186677358}\" }, { \"Image\": \"\", \"Question\": \"I am renting my home, how can I view more usage and account details?\", \"Answer\": \"<p><span>You would need to add your TNB electricity account as an owner. To do this, you need the IC number registered to the TNB account of the place you are renting.<\\/span><\\/p>\", \"ID\": \"{31D28F14-3D30-4441-8BA6-DFF91A6AEB84}\" }, { \"Image\": \"\", \"Question\": \"I am a landlord and would like my tenants to manage their electricity bills with ease.\", \"Answer\": \"<span>That<\\/span><span>&rsquo;<\\/span><span>s great! Your tenants are welcome to download and use myTNB mobile app, but for them to view usage and enjoy full functionalities of the app, they would need to add the TNB electricity account as owners. You can help them out by providing the IC number registered to the TNB electricity account.<\\/span>\", \"ID\": \"{1A6328D5-75BE-4AC4-8190-E157916D77CF}\" }, { \"Image\": \"\", \"Question\": \"I would like to pay for my family\u2019s electricity bills through the app. How do I do this?\", \"Answer\": \"<p>You can add your family members<span>&rsquo;<\\/span> TNB accounts and manage them through the app. On your Dashboard, tap on the dropdown button above the usage graph to access the add account function, or go to <span>&lsquo;<\\/span>My Account<span>&rsquo;<\\/span> under <span>&lsquo;<\\/span>More<span>&rsquo;<\\/span> on the navigation bar. Keep in mind that you will need the IC number registered to the TNB account in order to view full usage details.<\\/p>\", \"ID\": \"{911C8833-B650-4EF1-ACF8-874B321DAD89}\" }, { \"Image\": \"\", \"Question\": \"Can I add multiple TNB electricity accounts?\", \"Answer\": \"<p>Yes. You can add multiple accounts through the add account function. On your Dashboard, tap on the dropdown button above the usage graph to access the add account function, or go to <span>&lsquo;<\\/span>My Account<span>&rsquo;<\\/span> under <span>&lsquo;<\\/span>More<span>&rsquo;<\\/span> on the navigation bar.<\\/p>\", \"ID\": \"{BA0EBAD3-6AE7-40B3-8177-5488329BEAF1}\" }, { \"Image\": \"\", \"Question\": \"How do I check my electricity bill in myTNB mobile app?\", \"Answer\": \"<p>You can check your bill by tapping on <span>&lsquo;<\\/span>View Bill<span>&rsquo;<\\/span> on your Dashboard. Alternatively, go to the <span>&lsquo;<\\/span>Bills<span>&rsquo;<\\/span> tab on the navigation bar to view your bill and payment history.<\\/p>\", \"ID\": \"{573D76A8-0F8F-4006-A242-273D0665B6FD}\" }, { \"Image\": \"\", \"Question\": \"How do I pay my bills through myTNB mobile app?\", \"Answer\": \"<p>Paying bills through myTNB mobile app is quick and easy. Simply tap on the <span>&lsquo;<\\/span>Pay<span>&rsquo;<\\/span> button on your Dashboard and make a payment either with your credit card or through FPX (bank transfer).&nbsp;<\\/p>\", \"ID\": \"{829B9C19-B7B9-42BF-BC76-E10D9F2CD70B}\" }, { \"Image\": \"\", \"Question\": \"Can I store my credit card info in the app?\", \"Answer\": \"<p>Yes, you may choose to save your credit card details on the app for faster payment next time.&nbsp;<\\/p>\", \"ID\": \"{34D384C6-3962-4AF9-B991-307DE70416CC}\" }, { \"Image\": \"\", \"Question\": \"Can I pay more than my outstanding amount?\", \"Answer\": \"<p>Yes. The excess amount will be reflected in your next bill.&nbsp;<\\/p>\", \"ID\": \"{0E80824B-D1D0-48D0-B369-2CE8222F0927}\" }, { \"Image\": \"\", \"Question\": \"What is the maximum amount I can pay in one transaction on the app?\", \"Answer\": \"<p>You may pay up to RM5,000 in a single transaction on the app.<\\/p>\", \"ID\": \"{0DB06875-A6A5-4503-B2BB-759F6B58FC48}\" }, { \"Image\": \"\", \"Question\": \"Can I view my usage history beyond 6 months?\", \"Answer\": \"<p>Please visit myTNB Portal at <span><a href=\\\"http:\\/\\/www.mytnb.com.my\\/\\\"><span>www.mytnb.com.my<\\/span><\\/a><\\/span> for more in-depth information about your TNB electricity account, including usage and billing history of up to two years.&nbsp;<\\/p>\", \"ID\": \"{EA6708CE-2EA4-424C-BAF3-0451288C0A6F}\" }, { \"Image\": \"\", \"Question\": \"How current is the account information?\", \"Answer\": \"<p>Account information is up to date to the current day.&nbsp;<\\/p>\", \"ID\": \"{022ED270-EFAC-4F95-8B9F-9AF37AA1A5D3}\" }, { \"Image\": \"\", \"Question\": \"Can I view my feedback status in the app?\", \"Answer\": \"<p>You may view submitted feedback under the <span>&lsquo;<\\/span>Feedback<span>&rsquo;<\\/span> tab on the navigation bar. Once your feedback has been resolved, you will receive an SMS informing you of the update.&nbsp;<\\/p>\", \"ID\": \"{0602D664-C937-41CB-8850-F78C53A00FB0}\" }, { \"Image\": \"\", \"Question\": \"How can I contact TNB?\", \"Answer\": \"<p>Getting in touch has never been easier. Call us on 15454 for Outages &amp; Breakdowns and 1-300-88-5454 for Billing &amp; General enquiries about your account. You can also drop us an email at <span><a href=\\\"mailto:tnbcareline@tnb.com.my\\\"><span>tnbcareline@tnb.com.my<\\/span><\\/a><\\/span> or message us on Facebook at <span><a href=\\\"http:\\/\\/www.facebook.com\\/TNBCareline\\\"><span>http:\\/\\/www.facebook.com\\/TNBCareline<\\/span><\\/a><\\/span>. Alternatively, navigate to <span>&lsquo;<\\/span>Feedback<span>&rsquo;<\\/span> on the navigation bar to send an enquiry to our team.&nbsp;<\\/p>\", \"ID\": \"{EDFFAB69-B97C-4CE2-84ED-DADE1103A9D0}\" }, { \"Image\": \"\", \"Question\": \"Is my personal data safe?\", \"Answer\": \"<p>Absolutely. Your privacy is of utmost importance to us, therefore we employ the latest internet security technology to ensure the safety and confidentiality of your data.&nbsp;<\\/p>\", \"ID\": \"{ABDDFC27-ADAF-4D69-9EF6-D33E1F86E43C}\" }, { \"Image\": \"\", \"Question\": \"I love this app! Managing my TNB account has never been easier. How can I thank you?\", \"Answer\": \"<p>Thank you! You can share myTNB mobile app with your friends and family, and rate us on the Apple App Store or Google Play Store.&nbsp;<\\/p>\", \"ID\": \"{5459907F-7A0D-4ADC-A2CC-8134A520B63C}\" }] }";
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

        void GetFAQContent()
        {
            string faqContent = string.Empty;
            try
            {
                faqContent = System.IO.File.ReadAllText("FAQ.json");
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