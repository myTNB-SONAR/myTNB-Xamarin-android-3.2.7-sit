using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class WhatsNewDetailsViewController : CustomUIViewController
    {
        public WhatsNewModel WhatsNewModel;
        private UIBarButtonItem _btnShare;
        private UIScrollView _scrollView;

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

            UIView imageContainer = new UIView(new CGRect(0, 0, ViewWidth, imgHeight))
            {
                BackgroundColor = UIColor.Clear,
            };

            UIImageView imageView = new UIImageView(imageContainer.Bounds)
            {
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            if (WhatsNewModel.Image.IsValid())
            {
                NSData imgData = WhatsNewCache.GetImage(WhatsNewModel.ID);
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
                        NSUrl url = new NSUrl(WhatsNewModel.Image);
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
                InvokeOnMainThread(() =>
                {
                    imageView.Image = UIImage.FromBundle(WhatsNewConstants.Img_WhatsNewDefaultBanner);
                });
            }

            nfloat viewWidth = ViewWidth - (BaseMarginWidth16 * 2);

            UITextView titleTextView = CreateHTMLContent(WhatsNewModel.Title, true);
            CGSize titleTextViewSize = titleTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(titleTextView, titleTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(titleTextView, GetScaledWidth(16F));
            ViewHelper.AdjustFrameSetY(titleTextView, GetYLocationFromFrame(imageView.Frame, 20F));
            ViewHelper.AdjustFrameSetWidth(titleTextView, viewWidth);

            UITextView descTextView = CreateHTMLContent(WhatsNewModel.Description, false);
            CGSize descTextViewSize = descTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(descTextView, descTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(descTextView, GetScaledWidth(16F));
            ViewHelper.AdjustFrameSetY(descTextView, GetYLocationFromFrame(titleTextView.Frame, 20F));
            ViewHelper.AdjustFrameSetWidth(descTextView, viewWidth);

            _scrollView.AddSubview(imageContainer);
            _scrollView.AddSubview(imageView);
            _scrollView.AddSubview(titleTextView);
            _scrollView.AddSubview(descTextView);
            UpdateScrollViewContentSize(descTextView);
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
                    BrowserViewController viewController = new BrowserViewController();
                    if (viewController != null)
                    {
                        viewController.NavigationTitle = GetI18NValue(WhatsNewConstants.I18N_Title);
                        viewController.URL = url.AbsoluteString;
                        viewController.IsDelegateNeeded = false;
                        UINavigationController navController = new UINavigationController(viewController)
                        {
                            ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                        };
                        PresentViewController(navController, true, null);
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

