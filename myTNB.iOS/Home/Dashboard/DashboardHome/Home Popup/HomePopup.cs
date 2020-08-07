using System;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB.Home.Dashboard.DashboardHome
{
    public static class HomePopup
    {
        private static nfloat BaseMargin = ScaleUtility.BaseMarginWidth16;
        private static nfloat BaseWidth = UIScreen.MainScreen.Bounds.Width - (BaseMargin * 2);
        private static nfloat TopMargin = 0;
        private static nfloat BottomMargin = 0;
        private static nfloat MaxHeight = 0;
        private static nfloat ButtonViewHeight = 0;
        private static nfloat MaxContainerHeight = 0;

        private static UIView View;
        private static UIView PopUpView;
        private static UIView LineView;
        private static CustomUIView ContentContainer;
        private static CustomUIView ButtonView;
        private static CustomUIView DontShowAgainView;
        private static UIScrollView ScrollView;

        private static nint Tag_Container = 1001;
        private static nint Tag_Line = 1002;
        private static nint Tag_CTA = 1003;
        private static nint Tag_DontShowAgain = 1004;
        private static nint Tag_NormalImage = 1005;
        private static nint Tag_TextContent = 1006;

        private static Action<WhatsNewModel> OnTapPopUp;
        private static WhatsNewModel WhatsNewItem;

        private static void CreateContainer()
        {
            ConfigureValues();
            View = new UIView(UIScreen.MainScreen.Bounds)
            {
                BackgroundColor = new UIColor(73 / 255, 73 / 255, 74 / 255, 0.5F),
                ClipsToBounds = true
            };
        }

        private static void ConfigureValues()
        {
            TopMargin = ScaleUtility.GetScaledHeight(50) + DeviceHelper.TopSafeAreaInset;
            BottomMargin = ScaleUtility.GetScaledHeight(50) + DeviceHelper.BottomSafeAreaInset;
            MaxHeight = UIScreen.MainScreen.Bounds.Height - TopMargin - BottomMargin;
            ButtonViewHeight = ScaleUtility.GetScaledHeight(53);
            MaxContainerHeight = MaxHeight - ButtonViewHeight;
        }

        private static void CreatePopUp(CustomUIViewController c)
        {
            //PopUp Main Container
            PopUpView = new UIView(new CGRect(BaseMargin
                , TopMargin
                , BaseWidth
                , MaxHeight))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            PopUpView.Layer.CornerRadius = ScaleUtility.GetScaledWidth(6);
            View.AddSubview(PopUpView);

            ContentContainer = new CustomUIView(new CGRect(0
                , 0
                , BaseWidth
                , MaxHeight - ScaleUtility.GetScaledHeight(54)))
            {
                Tag = Tag_Container,
                ClipsToBounds = true
            };
            ContentContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (!WhatsNewItem.Donot_Show_In_WhatsNew && OnTapPopUp != null)
                {
                    OnTapPopUp?.Invoke(WhatsNewItem);
                    Dismiss();
                }
            }));

            DontShowAgainView = new CustomUIView() { Tag = Tag_DontShowAgain };
            if (!WhatsNewItem.Disable_DoNotShow_Checkbox)
            {
                CreateDontShowAgain(WhatsNewItem.PortraitImage_PopUp.IsValid());
            }

            nfloat lineView_Y = WhatsNewItem.PortraitImage_PopUp.IsValid()
                ? ContentContainer.Frame.GetMaxY()
                : WhatsNewItem.Disable_DoNotShow_Checkbox
                    ? ContentContainer.Frame.GetMaxY()
                    : DontShowAgainView.Frame.GetMaxY();

            LineView = new UIView(new CGRect(0
                , lineView_Y
                , BaseWidth
                , ScaleUtility.GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.LinesGray,
                Tag = Tag_Line
            };
            CreateCTAView();

            PopUpView.AddSubviews(new UIView[] { ContentContainer, DontShowAgainView, LineView, ButtonView });

            if (WhatsNewItem.PortraitImage_PopUp.IsValid())
            {
                ConstructPortraitPopUp(c);
            }
            else
            {
                ConstructNormalPopUp(c);
            }
        }
        #region CTA
        private static void CreateCTAView()
        {
            ButtonView = new CustomUIView(new CGRect(0
                , LineView.Frame.GetMaxY()
                , BaseWidth
                , ButtonViewHeight))
            { Tag = Tag_CTA };
            UILabel lblTitle = new UILabel(new CGRect(0
                , 0
                , BaseWidth
                , ButtonView.Frame.Height))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Center,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt)
            };
            ButtonView.AddSubview(lblTitle);
            ButtonView.AddGestureRecognizer(new UITapGestureRecognizer(() => { Dismiss(); }));
        }
        #endregion
        #region Don't Show Again View
        private static void CreateDontShowAgain(bool isPortrait)
        {
            DontShowAgainView = new CustomUIView(new CGRect(ScaleUtility.BaseMarginWidth12
                , isPortrait ? ContentContainer.Frame.Height - ScaleUtility.GetScaledHeight(32)
                    : ScaleUtility.GetYLocationFromFrame(ContentContainer.Frame, 16)
                , BaseWidth - ScaleUtility.GetScaledWidth(24)
                , ScaleUtility.GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White,
                Tag = Tag_DontShowAgain
            };
            DontShowAgainView.Layer.CornerRadius = ScaleUtility.GetScaledWidth(4);

            UIImageView checkBoxImage = new UIImageView(new CGRect(ScaleUtility.GetScaledWidth(5)
                , ScaleUtility.GetScaledWidth(5)
                , ScaleUtility.GetScaledWidth(14)
                , ScaleUtility.GetScaledWidth(14)))
            {
                Image = null
            };
            checkBoxImage.Layer.CornerRadius = ScaleUtility.GetScaledWidth(3);
            checkBoxImage.Layer.BorderWidth = ScaleUtility.GetScaledWidth(1);
            checkBoxImage.Layer.BorderColor = MyTNBColor.VeryLightPinkSeven.CGColor;

            UILabel lblDescription = new UILabel(new CGRect(checkBoxImage.Frame.GetMaxX() + ScaleUtility.GetScaledWidth(8)
                , ScaleUtility.GetScaledHeight(4)
                , DontShowAgainView.Frame.Width - ScaleUtility.GetScaledWidth(35)
                , ScaleUtility.GetScaledHeight(16)))
            {
                Font = TNBFont.MuseoSans_11_500,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_DontShowThisAgain)
            };

            DontShowAgainView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                checkBoxImage.Image = checkBoxImage.Image == null ? UIImage.FromBundle(LoginConstants.IMG_RememberIcon) : null;
                bool iSkiped = WhatsNewServices.GetIsSkipAppLaunch(WhatsNewItem.ID);
                iSkiped = !iSkiped;
                WhatsNewServices.SetIsSkipAppLaunch(WhatsNewItem.ID, iSkiped);
            }));

            DontShowAgainView.AddSubviews(new UIView[] { checkBoxImage, lblDescription });
        }
        #endregion

        private static void ConstructPortraitPopUp(CustomUIViewController c)
        {
            UIImageView portraitImage = new UIImageView(new CGRect(0, 0, ContentContainer.Frame.Width, ContentContainer.Frame.Height));
            ContentContainer.AddSubview(portraitImage);
            ImageDownload(c, portraitImage, WhatsNewItem.PortraitImage_PopUp);
        }

        private static void ConstructNormalPopUp(CustomUIViewController c)
        {
            ScrollView = new UIScrollView(new CGRect(0
                , 0
                , BaseWidth
                , 0))
            {
                ShowsVerticalScrollIndicator = false,
                Bounces = false
            };
            ContentContainer.AddSubview(ScrollView);
            UIImageView imageView = new UIImageView(new CGRect(0
                , 0
                , BaseWidth
                , BaseWidth * 0.5))
            {
                Image = UIImage.FromBundle("WhatsNew-Popup-Banner"),
                Tag = Tag_NormalImage
            };
            ScrollView.AddSubview(imageView);
            UITextView textContent = GetBodyContent(c, WhatsNewItem.PopUp_Text_Content, ScaleUtility.GetYLocationFromFrame(imageView.Frame, 16));
            ScrollView.AddSubview(textContent);
            if (!WhatsNewItem.PopUp_Text_Only && WhatsNewItem.PopUp_HeaderImage.IsValid())
            {
                ImageDownload(c, imageView, WhatsNewItem.PopUp_HeaderImage, true);
            }
            else
            {
                Resize();
            }
        }

        #region Text Content
        private static UITextView GetBodyContent(CustomUIViewController c, string message, nfloat yLoc)
        {
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.PowerBlue,
                Font = TNBFont.MuseoSans_14_300,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(message
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            // Body
            UITextView txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = false,
                Selectable = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                ContentInset = new UIEdgeInsets(-5, 0, -5, 0),
                Tag = Tag_TextContent
            };
            txtViewDetails.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            txtViewDetails.TextContainer.LineFragmentPadding = 0F;
            SetContentActions(c, ref txtViewDetails);

            //Resize
            var maxDescriptionHeight = UIScreen.MainScreen.Bounds.Height;
            CGSize size = txtViewDetails.SizeThatFits(new CGSize(BaseWidth - (ScaleUtility.GetScaledWidth(16) * 2), maxDescriptionHeight));
            nfloat txtViewHeight = size.Height > maxDescriptionHeight ? maxDescriptionHeight : size.Height;
            txtViewDetails.Frame = new CGRect(ScaleUtility.GetScaledWidth(16)
                , yLoc
                , BaseWidth - (ScaleUtility.GetScaledWidth(16) * 2)
                , txtViewHeight);
            txtViewDetails.TextAlignment = UITextAlignment.Left;
            return txtViewDetails;
        }

        #region Add Textview Actions
        private static void SetContentActions(CustomUIViewController c, ref UITextView textView)
        {
            textView.Delegate = new TextViewDelegate(c.LinkAction)
            {
                InteractWithURL = false
            };
        }
        #endregion

        #endregion
        #region Image Download
        private static void ImageDownload(CustomUIViewController c, UIImageView imageView, string urlString, bool isNormal = false)
        {
            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, imageView.Frame.Width, imageView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, imageView.Frame.Width, imageView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();
            UIView imgLoadingView = new UIView(imageView.Bounds)
            {
                BackgroundColor = UIColor.White
            };
            UIView viewImage = new UIView(new CGRect(0, 0, imageView.Frame.Width, imageView.Frame.Height))
            {
                BackgroundColor = MyTNBColor.PaleGreyThree
            };
            viewShimmerContent.AddSubview(viewImage);
            imgLoadingView.AddSubview(viewShimmerParent);
            ContentContainer.AddSubview(imgLoadingView);

            NSUrl url = new NSUrl(urlString);
            NSUrlSession session = NSUrlSession
                .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
            NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
            {
                if (error == null && response != null && data != null)
                {
                    c.InvokeOnMainThread(() =>
                    {
                        using (var image = UIImage.LoadFromData(data))
                        {
                            imageView.Image = image;
                            imgLoadingView.RemoveFromSuperview();
                            if (isNormal)
                            {
                                var width = image.Size.Width;
                                var height = image.Size.Height;
                                var ratio = height / width;
                                imageView.Frame = new CGRect(0
                                    , 0
                                    , BaseWidth
                                    , BaseWidth * ratio);
                                Resize();
                            }
                        }
                    });
                }
                else
                {
                    c.InvokeOnMainThread(() =>
                    {
                        if (isNormal)
                        {
                            imgLoadingView.RemoveFromSuperview();
                        }
                    });
                }
            });
            dataTask.Resume();
        }
        #endregion
        #region Resize
        private static void Resize()
        {
            UIImageView imageView = ScrollView.ViewWithTag(Tag_NormalImage) as UIImageView;
            UITextView textContent = ScrollView.ViewWithTag(Tag_TextContent) as UITextView;

            textContent.Frame = new CGRect(textContent.Frame.X
                , ScaleUtility.GetYLocationFromFrame(imageView.Frame, 16)
                , textContent.Frame.Width
                , textContent.Frame.Height);

            nfloat contentHeight = textContent.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16);
            nfloat containerHeight = contentHeight > MaxContainerHeight
                ? MaxContainerHeight
                : contentHeight;

            ContentContainer.Frame = new CGRect(ContentContainer.Frame.Location
                , new CGSize(ContentContainer.Frame.Width
                    , containerHeight));

            ScrollView.Frame = new CGRect(0, 0, BaseWidth, containerHeight);
            ScrollView.ContentSize = new CGSize(BaseWidth
                , contentHeight);

            if (!WhatsNewItem.Disable_DoNotShow_Checkbox)
            {
                DontShowAgainView.Frame = new CGRect(DontShowAgainView.Frame.X
                    , ContentContainer.Frame.GetMaxY()
                    , DontShowAgainView.Frame.Width
                    , DontShowAgainView.Frame.Height);
            }

            LineView.Frame = new CGRect(LineView.Frame.X
                , WhatsNewItem.Disable_DoNotShow_Checkbox
                    ? ContentContainer.Frame.GetMaxY()
                    : DontShowAgainView.Frame.GetMaxY()
                , LineView.Frame.Width
                , LineView.Frame.Height);

            ButtonView.Frame = new CGRect(ButtonView.Frame.X
                , LineView.Frame.GetMaxY()
                , ButtonView.Frame.Width
                , ButtonView.Frame.Height);

            nfloat popUpView_Y = ((MaxHeight - ButtonView.Frame.GetMaxY()) / 2) + TopMargin;

            PopUpView.Frame = new CGRect(PopUpView.Frame.X
                , popUpView_Y
                , PopUpView.Frame.Width
                , ButtonView.Frame.GetMaxY());
        }
        #endregion
        private static void Dismiss()
        {
            if (View != null)
            {
                View.RemoveFromSuperview();
                View = null;
            }
        }

        public static void DisplayMarketingPopup(this CustomUIViewController c, WhatsNewModel whatsNew, Action<WhatsNewModel> OnTap)
        {
            WhatsNewItem = whatsNew;
            OnTapPopUp = OnTap;
            CreateContainer();
            CreatePopUp(c);
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            window.AddSubview(View);
        }
    }
}