using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace myTNB.Common
{
    internal static class CommonLinkAction
    {
        internal static List<string> RedirectTypeList = new List<string> {
            "inAppBrowser=",
            "externalBrowser=",
            "tel=",
            "whatsnew=",
            "faq=",
            "reward=",
            "http",
            "tel:",
            "whatsnewid=",
            "faqid=",
            "rewardid="
        };

        internal static Action<NSUrl> GetAction(CustomUIViewController controller)
        {
            Action<NSUrl> action = new Action<NSUrl>((url) =>
            {
                if (url != null)
                {
                    string absURL = url.AbsoluteString;
                    int whileCount = 0;
                    bool isContained = false;
                    for (int i = 0; i < RedirectTypeList.Count; i++)
                    {
                        if (absURL.Contains(RedirectTypeList[i]))
                        {
                            whileCount = i;
                            isContained = true;
                            break;
                        }
                    }

                    if (isContained)
                    {
                        if (RedirectTypeList[whileCount] == RedirectTypeList[0])
                        {
                            string urlString = absURL.Split(RedirectTypeList[0])[1];
                            BrowserViewController viewController = new BrowserViewController();
                            if (viewController != null)
                            {
                                viewController.URL = urlString;
                                viewController.IsDelegateNeeded = false;
                                UINavigationController navController = new UINavigationController(viewController)
                                {
                                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                                };
                                controller.PresentViewController(navController, true, null);
                            }
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[1])
                        {
                            string urlString = absURL.Split(RedirectTypeList[1])[1];
                            UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(urlString)));
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[2])
                        {
                            string urlString = absURL.Split(RedirectTypeList[2])[1];
                            if (!urlString.Contains("tel:"))
                            {
                                urlString = "tel:" + urlString;
                            }
                            UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[3])
                        {
                            string key = absURL.Split(RedirectTypeList[3])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            WhatsNewServices.OpenWhatsNewDetailsInDetails(key, controller);
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[4])
                        {
                            string key = absURL.Split(RedirectTypeList[4])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            if (!key.Contains("{"))
                            {
                                key = "{" + key;
                            }
                            if (!key.Contains("}"))
                            {
                                key = key + "}";
                            }
                            ViewHelper.GoToFAQScreenWithId(key);
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[5])
                        {
                            string key = absURL.Split(RedirectTypeList[5])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            RewardsServices.OpenRewardDetails(key, controller);
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[6])
                        {
                            string urlString = absURL;
                            BrowserViewController viewController = new BrowserViewController();
                            if (viewController != null)
                            {
                                viewController.NavigationTitle = "";
                                viewController.URL = urlString;
                                viewController.IsDelegateNeeded = false;
                                UINavigationController navController = new UINavigationController(viewController)
                                {
                                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                                };
                                controller.PresentViewController(navController, true, null);
                            }
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[7])
                        {
                            string urlString = absURL;
                            if (!urlString.Contains("tel:"))
                            {
                                urlString = "tel:" + urlString;
                            }
                            UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[8])
                        {
                            string key = absURL.Split(RedirectTypeList[8])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            WhatsNewServices.OpenWhatsNewDetailsInDetails(key, controller);
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[9])
                        {
                            string key = absURL.Split(RedirectTypeList[9])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            if (!key.Contains("{"))
                            {
                                key = "{" + key;
                            }
                            if (!key.Contains("}"))
                            {
                                key = key + "}";
                            }
                            ViewHelper.GoToFAQScreenWithId(key);
                        }
                        else if (RedirectTypeList[whileCount] == RedirectTypeList[10])
                        {
                            string key = absURL.Split(RedirectTypeList[10])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            RewardsServices.OpenRewardDetails(key, controller);
                        }
                    }
                }
            });
            return action;
        }
    }
}
