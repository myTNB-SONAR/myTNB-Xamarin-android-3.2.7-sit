using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB
{
    public class WhatsNewDetailsViewController : CustomUIViewController
    {
        public WhatsNewModel WhatsNewModel;
        private UIBarButtonItem _btnShare;
        private UIScrollView _scrollView;
        private UITextView _titleTextView;
        private UITextView _descTextView;

        public override void ViewDidLoad()
        {
            PageName = WhatsNewConstants.Pagename_WhatsNew;
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            View.Frame = new CGRect(0, 0, width, height);
            View.BackgroundColor = UIColor.White;
            base.ViewDidLoad();
            SetNavigationBar();
            SetScrollView();
            PrepareDetailView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            Title = GetI18NValue(WhatsNewConstants.I18N_Title);
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            _btnShare = new UIBarButtonItem(UIImage.FromBundle(WhatsNewConstants.Img_ShareIcon), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (NetworkUtility.isReachable)
                {
                    ActivityIndicator.Show();
                    BaseService baseService = new BaseService();
                    APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
                    string linkUrl = baseService.GetDomain(env) + "/whatsnew/redirect.aspx/wnid=" + WhatsNewModel.ID;

                    var deeplinkUrl = string.Empty;
                    var components = CommonServices.GenerateLongURL(linkUrl);
                    components.GetShortenUrl((shortUrl, warnings, error) =>
                    {
                        if (error == null)
                        {
                            deeplinkUrl = shortUrl.AbsoluteString;
                        }
                        else
                        {
                            deeplinkUrl = linkUrl;
                        }
                        ShareAction(deeplinkUrl);
                        ActivityIndicator.Hide();
                    });
                }
                else
                {
                    AlertHandler.DisplayNoDataAlert(this);
                }
            });
            NavigationItem.RightBarButtonItem = _btnShare;
        }

        private void ShareAction(string deeplinkUrl)
        {
            if (deeplinkUrl.IsValid())
            {
                NSObject item = NSObject.FromObject(deeplinkUrl);
                NSObject[] activityItems = { item };
                UIActivity[] applicationActivities = null;
                UIActivityViewController activityController = new UIActivityViewController(activityItems, applicationActivities);
                UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
                activityController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(activityController, true, null);
            }
        }

        private void SetScrollView()
        {
            if (_scrollView != null)
            {
                _scrollView.RemoveFromSuperview();
                _scrollView = null;
            }
            _scrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.Clear,
                Bounces = true
            };
            View.AddSubview(_scrollView);
        }

        private void PrepareDetailView()
        {
            nfloat imgHeight = GetScaledHeight(180F);

            bool isImageDetailAvailable = true;

            UIView imageContainer = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
            {
                BackgroundColor = UIColor.Clear,
            };

            UIImageView imageView = new UIImageView(imageContainer.Bounds)
            {
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            Dictionary<string, string> listDescription = WhatsNewDetailDescriptionCache.GetImages(WhatsNewModel.ID);
            Dictionary<string, string> ImageDescriptionDictionary = new Dictionary<string, string>();
            List<string> imageUrls = new List<string>();

            if (WhatsNewModel.Description.Contains("<img"))
            {
                string urlHeightWidthRegex = "(<img\\b|(?!^)\\G)[^>]*?\\b(src|width|height)=([\"']?)([^\"]*)\\3";
                System.Text.RegularExpressions.MatchCollection matcheImgSrc = System.Text.RegularExpressions.Regex.Matches(WhatsNewModel.Description, urlHeightWidthRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                bool foundWidth = false;
                bool foundHeight = false;
                nfloat textImgWidth = 0;
                nfloat textImgHeight = 0;
                for (int index = 0; index < matcheImgSrc.Count; index++)
                {
                    if (matcheImgSrc[index].Groups[2].Value == "width")
                    {
                        if (foundHeight)
                        {
                            textImgWidth = nfloat.Parse(matcheImgSrc[index].Groups[4].Value);
                            nfloat deviceWidth = ViewWidth - (BaseMarginWidth16 * 2);
                            WhatsNewModel.Description = WhatsNewModel.Description.Replace("width=\"" + textImgWidth + "\"", "width=\"" + deviceWidth.ToString() + "\"");

                            nfloat calImgRatio = deviceWidth / textImgWidth;
                            nfloat deviceHeight = textImgHeight * calImgRatio;

                            WhatsNewModel.Description = WhatsNewModel.Description.Replace("height=\"" + textImgHeight + "\"", "height=\"" + deviceHeight.ToString() + "\"");

                            foundHeight = false;
                        }
                        else
                        {
                            foundWidth = true;
                            textImgWidth = nfloat.Parse(matcheImgSrc[index].Groups[4].Value);
                        }
                    }
                    else if (matcheImgSrc[index].Groups[2].Value == "height")
                    {
                        if (foundWidth)
                        {
                            textImgHeight = nfloat.Parse(matcheImgSrc[index].Groups[4].Value);
                            nfloat deviceWidth = ViewWidth - (BaseMarginWidth16 * 2);
                            WhatsNewModel.Description = WhatsNewModel.Description.Replace("width=\"" + textImgWidth + "\"", "width=\"" + deviceWidth.ToString() + "\"");

                            nfloat calImgRatio = deviceWidth / textImgWidth;
                            nfloat deviceHeight = textImgHeight * calImgRatio;

                            WhatsNewModel.Description = WhatsNewModel.Description.Replace("height=\"" + textImgHeight + "\"", "height=\"" + deviceHeight.ToString() + "\"");

                            foundWidth = false;
                        }
                        else
                        {
                            foundHeight = true;
                            textImgHeight = nfloat.Parse(matcheImgSrc[index].Groups[4].Value);
                        }
                    }
                }

                string urlRegex = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
                System.Text.RegularExpressions.MatchCollection matchesImgSrc = System.Text.RegularExpressions.Regex.Matches(WhatsNewModel.Description, urlRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                for(int count = 0; count < matchesImgSrc.Count; count++)
                {
                    System.Text.RegularExpressions.Match m = matchesImgSrc[count];

                    string href = m.Groups[1].Value;
                    if (href.Contains("http"))
                    {
                        ImageDescriptionDictionary.Add(WhatsNewModel.ID + "_" + count.ToString(), href);
                        WhatsNewModel.Description = WhatsNewModel.Description.Replace(href, WhatsNewConstants.defaultDescriptionImage);
                        imageUrls.Add(href);
                    }
                    else if (href.Contains(WhatsNewConstants.defaultDescriptionImage))
                    {
                        if (listDescription != null && listDescription.Count > 0)
                        {
                            imageUrls.Add(listDescription[WhatsNewModel.ID + "_" + count.ToString()]);
                        }
                    }
                }

                if (listDescription == null)
                {
                    WhatsNewDetailDescriptionCache.SaveImages(WhatsNewModel.ID, ImageDescriptionDictionary);
                    listDescription = ImageDescriptionDictionary;
                }
                else
                {
                    ImageDescriptionDictionary = listDescription;
                }
            }

            if (WhatsNewModel.Image_DetailsView.IsValid())
            {
                NSData imgData = WhatsNewDetailCache.GetImage(WhatsNewModel.ID);
                if (imgData != null)
                {
                    imageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                    using (var image = UIImage.LoadFromData(imgData))
                    {
                        imageView.Image = image;
                    }
                }
                else
                {
                    try
                    {
                        UIView imgLoadingView = new UIView(imageContainer.Bounds)
                        {
                            BackgroundColor = UIColor.Clear
                        };
                        _scrollView.AddSubview(imgLoadingView);

                        UIView viewImage = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
                        {
                            BackgroundColor = MyTNBColor.PaleGreyThree
                        };

                        CustomShimmerView shimmeringView = new CustomShimmerView();
                        UIView viewShimmerParent = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
                        { BackgroundColor = UIColor.Clear };
                        UIView viewShimmerContent = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
                        { BackgroundColor = UIColor.Clear };
                        viewShimmerParent.AddSubview(shimmeringView);
                        shimmeringView.ContentView = viewShimmerContent;
                        shimmeringView.Shimmering = true;
                        shimmeringView.SetValues();

                        viewShimmerContent.AddSubview(viewImage);
                        imgLoadingView.AddSubview(viewShimmerParent);
                        NSUrl url = new NSUrl(WhatsNewModel.Image_DetailsView);
                        NSUrlSession session = NSUrlSession
                            .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                        NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                        {
                            if (error == null && response != null && data != null)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    imageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                                    using (var image = UIImage.LoadFromData(data))
                                    {
                                        imageView.Image = image;
                                    }
                                    WhatsNewDetailCache.SaveImage(WhatsNewModel.ID, data);
                                    imgLoadingView.RemoveFromSuperview();
                                });
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    imageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                                    imgLoadingView.RemoveFromSuperview();
                                });
                            }
                        });
                        dataTask.Resume();
                    }
                    catch (MonoTouchException m)
                    {
                        Debug.WriteLine("Image load Error: " + m.Message);
                        InvokeOnMainThread(() =>
                        {
                            imageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                        });
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Image load Error: " + e.Message);
                        InvokeOnMainThread(() =>
                        {
                            imageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                        });
                    }
                }
            }
            else
            {
                isImageDetailAvailable = false;
            }

            nfloat viewWidth = ViewWidth - (BaseMarginWidth16 * 2);

            _titleTextView = CreateHTMLContent(WhatsNewModel.Title, true);
            CGSize titleTextViewSize = _titleTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(_titleTextView, titleTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(_titleTextView, GetScaledWidth(16F));
            if (isImageDetailAvailable)
            {
                ViewHelper.AdjustFrameSetY(_titleTextView, GetYLocationFromFrame(imageView.Frame, 20F));
            }
            else
            {
                ViewHelper.AdjustFrameSetY(_titleTextView, GetScaledHeight(16F));
            }
            ViewHelper.AdjustFrameSetWidth(_titleTextView, viewWidth);
            _descTextView = CreateHTMLContent(WhatsNewModel.Description, false);
            CGSize descTextViewSize = _descTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(_descTextView, descTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(_descTextView, GetScaledWidth(16F));
            ViewHelper.AdjustFrameSetY(_descTextView, GetYLocationFromFrame(_titleTextView.Frame, 20F));
            ViewHelper.AdjustFrameSetWidth(_descTextView, viewWidth);

            if (isImageDetailAvailable)
            {
                _scrollView.AddSubview(imageContainer);
                _scrollView.AddSubview(imageView);
            }
            _scrollView.AddSubview(_titleTextView);
            _scrollView.AddSubview(_descTextView);
            UpdateScrollViewContentSize(_descTextView);

            if (imageUrls != null && imageUrls.Count > 0)
            {
                Task.Run(() =>
                {
                    try
                    {
                        bool downloadFailed = false;

                        List<string> downloadImageUrls = new List<string>();

                        for (int imgCount = 0; imgCount < imageUrls.Count; imgCount++)
                        {
                            try
                            {
                                WebClient webClient = new WebClient();
                                var outByteArray = webClient.DownloadData(new Uri(imageUrls[imgCount]));
                                var contentType = webClient.ResponseHeaders["Content-Type"];
                                if (contentType != null &&
                                    contentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                                {
                                    downloadImageUrls.Add("data:" + contentType + ";base64," + System.Convert.ToBase64String(outByteArray));
                                }
                            }
                            catch (Exception e)
                            {
                                downloadFailed = true;
                                Debug.WriteLine("Exception in Image Download: " + e.Message);
                            }
                        }

                        if (!downloadFailed)
                        {
                            InvokeOnMainThread(() =>
                            {
                                try
                                {
                                    for (int imgCount = 0; imgCount < downloadImageUrls.Count; imgCount++)
                                    {
                                        string urlRegex = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
                                        System.Text.RegularExpressions.MatchCollection matchesImgSrc = System.Text.RegularExpressions.Regex.Matches(WhatsNewModel.Description, urlRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                                        if (matchesImgSrc != null && matchesImgSrc.Count == downloadImageUrls.Count)
                                        {
                                            System.Text.RegularExpressions.Match m = matchesImgSrc[imgCount];

                                            string tag = m.Groups[0].Value;
                                            string href = m.Groups[1].Value;
                                            tag = tag.Replace(href, downloadImageUrls[imgCount]);
                                            WhatsNewModel.Description = WhatsNewModel.Description.Replace(m.Groups[0].Value, tag);
                                        }
                                    }

                                    _descTextView.RemoveFromSuperview();
                                    _descTextView = CreateHTMLContent(WhatsNewModel.Description, false);
                                    CGSize _descTextViewSize = _descTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
                                    ViewHelper.AdjustFrameSetHeight(_descTextView, _descTextViewSize.Height);
                                    ViewHelper.AdjustFrameSetX(_descTextView, GetScaledWidth(16F));
                                    ViewHelper.AdjustFrameSetY(_descTextView, GetYLocationFromFrame(_titleTextView.Frame, 20F));
                                    ViewHelper.AdjustFrameSetWidth(_descTextView, viewWidth);

                                    _scrollView.AddSubview(_descTextView);
                                    UpdateScrollViewContentSize(_descTextView);

                                    WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
                                    whatsNewEntity.UpdateDescription(WhatsNewModel.ID, WhatsNewModel.Description);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine("Exception in Image Download: " + e.Message);
                                }
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception in Image Download: " + e.Message);
                    }
                });
            }
        }

        private void UpdateScrollViewContentSize(UIView lastView)
        {
            if (_scrollView != null && lastView != null)
            {
                _scrollView.ContentSize = new CGSize(ViewWidth, lastView.Frame.GetMaxY());
            }
        }

        private UITextView CreateHTMLContent(string textValue, bool isTitle = false)
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(textValue
                , ref htmlBodyError, isTitle ? TNBFont.FONTNAME_500 : TNBFont.FONTNAME_300
                , isTitle ? (float)GetScaledHeight(16F) : (float)GetScaledHeight(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = isTitle ? MyTNBColor.WaterBlue : MyTNBColor.CharcoalGrey,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Left,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_14_500,
                UnderlineColor = UIColor.Clear,
                UnderlineStyle = NSUnderlineStyle.None
            };

            UITextView textView = new UITextView()
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                TextContainerInset = UIEdgeInsets.Zero
            };

            Action<NSUrl> action = new Action<NSUrl>((url) =>
            {
                if (url != null)
                {
                    string absURL = url.AbsoluteString;
                    int whileCount = 0;
                    bool isContained = false;
                    for (int i = 0; i < AlertHandler.RedirectTypeList.Count; i++)
                    {
                        if (absURL.Contains(AlertHandler.RedirectTypeList[i]))
                        {
                            whileCount = i;
                            isContained = true;
                            break;
                        }
                    }

                    if (isContained)
                    {
                        if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[0])
                        {
                            string urlString = absURL.Split(AlertHandler.RedirectTypeList[0])[1];
                            BrowserViewController viewController = new BrowserViewController();
                            if (viewController != null)
                            {
                                viewController.URL = urlString;
                                viewController.IsDelegateNeeded = false;
                                UINavigationController navController = new UINavigationController(viewController)
                                {
                                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                                };
                                PresentViewController(navController, true, null);
                            }
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[1])
                        {
                            string urlString = absURL.Split(AlertHandler.RedirectTypeList[1])[1];
                            UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(urlString)));
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[2])
                        {
                            string urlString = absURL.Split(AlertHandler.RedirectTypeList[2])[1];
                            if (!urlString.Contains("tel:"))
                            {
                                urlString = "tel:" + urlString;
                            }
                            UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[3])
                        {
                            string key = absURL.Split(AlertHandler.RedirectTypeList[3])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            WhatsNewServices.OpenWhatsNewDetailsInDetails(key, this);
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[4])
                        {
                            string key = absURL.Split(AlertHandler.RedirectTypeList[4])[1];
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
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[5])
                        {
                            string key = absURL.Split(AlertHandler.RedirectTypeList[5])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            RewardsServices.OpenRewardDetails(key, this);
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[6])
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
                                PresentViewController(navController, true, null);
                            }
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[7])
                        {
                            string urlString = absURL;
                            if (!urlString.Contains("tel:"))
                            {
                                urlString = "tel:" + urlString;
                            }
                            UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[8])
                        {
                            string key = absURL.Split(AlertHandler.RedirectTypeList[8])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            WhatsNewServices.OpenWhatsNewDetailsInDetails(key, this);
                        }
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[9])
                        {
                            string key = absURL.Split(AlertHandler.RedirectTypeList[9])[1];
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
                        else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[10])
                        {
                            string key = absURL.Split(AlertHandler.RedirectTypeList[10])[1];
                            key = key.Replace("%7B", "{").Replace("%7D", "}");
                            int index = key.IndexOf("}");
                            if (index > -1 && index < key.Length - 1)
                            {
                                key = key.Remove(index + 1);
                            }
                            key = key.Replace("{", "").Replace("}", "");
                            RewardsServices.OpenRewardDetails(key, this);
                        }
                    }
                }
            });
            textView.Delegate = new TextViewDelegate(action)
            {
                InteractWithURL = false
            };

            return textView;
        }
    }
}

