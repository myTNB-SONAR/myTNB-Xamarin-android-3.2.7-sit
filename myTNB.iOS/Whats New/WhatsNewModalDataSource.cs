using System;
using System.Collections.Generic;
using System.Diagnostics;
using Carousels;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class WhatsNewModalDataSource : iCarouselDataSource
    {
        private List<WhatsNewModel> whatsNew;

        public Action OnTapDetails { get; set; }
        public EventHandler OnTapExit { get; set; }

        private nfloat margin = ScaleUtility.GetScaledWidth(16);
        private nfloat baseWidth = 0;

        public WhatsNewModalDataSource(List<WhatsNewModel> whatsNew)
        {
            this.whatsNew = whatsNew;
            baseWidth = UIScreen.MainScreen.Bounds.Width - (margin * 2);
        }

        public override nint GetNumberOfItems(iCarousel carousel)
        {
            return whatsNew?.Count ?? 0;
        }

        private void OnGetTapDetail()
        {
            OnTapDetails?.Invoke();
        }

        public nfloat GetXLocationFromFrame(CGRect frame, nfloat xValue)
        {
            ScaleUtility.GetXLocationFromFrame(frame, ref xValue);
            return xValue;
        }

        public nfloat GetYLocationFromFrame(CGRect frame, nfloat xValue)
        {
            ScaleUtility.GetYLocationFromFrame(frame, ref xValue);
            return xValue;
        }

        public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view)
        {
            WhatsNewModel whatsNewItem = whatsNew[(int)index];
            if (whatsNewItem.PortraitImage_PopUp.IsValid())
            {
                GetViewForPortrait(ref view, whatsNewItem);
            }
            else
            {
                GetViewForBanner(ref view, whatsNewItem);
            }

            if (!whatsNewItem.Donot_Show_In_WhatsNew)
            {
                view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnGetTapDetail();
                }));
            }

            return view;
        }

        private void GetViewForBanner(ref UIView view, WhatsNewModel whatsNewItem)
        {
            var ratio = 0.5;
            var imgWidth = baseWidth;
            var imgHeight = imgWidth * ratio;

            UIImageView imageView = new UIImageView(new CGRect(0, 0, imgWidth, imgHeight))
            {
                Image = UIImage.FromBundle("WhatsNew-Popup-Banner"),
                Tag = 1001
            };
            UpdateView(ref view, imageView, whatsNewItem);
            UIView refView = view;
            if (!whatsNewItem.PopUp_Text_Only)
            {
                CustomShimmerView shimmeringView = new CustomShimmerView();
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, imgWidth, imgHeight))
                { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, imgWidth, imgHeight))
                { BackgroundColor = UIColor.Clear };
                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;
                shimmeringView.SetValues();
                UIView imgLoadingView = new UIView(imageView.Bounds)
                {
                    BackgroundColor = UIColor.White
                };
                UIView viewImage = new UIView(new CGRect(0, 0, imgWidth, imgHeight))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree
                };
                viewShimmerContent.AddSubview(viewImage);
                imgLoadingView.AddSubview(viewShimmerParent);
                view.AddSubview(imgLoadingView);
                NSUrl url = new NSUrl(whatsNewItem.PopUp_HeaderImage);
                NSUrlSession session = NSUrlSession
                    .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                {
                    if (error == null && response != null && data != null)
                    {
                        InvokeOnMainThread(() =>
                        {
                            using (var image = UIImage.LoadFromData(data))
                            {
                                ratio = image != null ? image.Size.Height / image.Size.Width : 0;
                                imgHeight = imgWidth * ratio;
                                imageView.Frame = new CGRect(0, 0, imgWidth, imgHeight);
                                imageView.Image = image;
                                UpdateLocations(refView);
                                imgLoadingView.RemoveFromSuperview();
                            }
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            imgLoadingView.RemoveFromSuperview();
                        });
                    }
                });
                dataTask.Resume();
            }
        }

        private void UpdateLocations(UIView view)
        {
            UIImageView imgView = view.ViewWithTag(1001) as UIImageView;
            UITextView txtViewContent = view.ViewWithTag(1002) as UITextView;
            CustomUIView dontShowAgainView = view.ViewWithTag(1003) as CustomUIView;
            UIView viewLine = view.ViewWithTag(1004) as UIView;
            UIButton btnGotIt = view.ViewWithTag(1005) as UIButton;

            txtViewContent.Frame = new CGRect(new CGPoint(txtViewContent.Frame.X, GetYLocationFromFrame(imgView.Frame, 16)), txtViewContent.Frame.Size);
            dontShowAgainView.Frame = new CGRect(new CGPoint(dontShowAgainView.Frame.X, GetYLocationFromFrame(txtViewContent.Frame, 16)), dontShowAgainView.Frame.Size);
            nfloat lineYLoc = dontShowAgainView.Frame.Height > 0 ? ScaleUtility.GetScaledHeight(16) : 0;
            viewLine.Frame = new CGRect(new CGPoint(viewLine.Frame.X, dontShowAgainView.Frame.GetMaxY() + lineYLoc), viewLine.Frame.Size);
            btnGotIt.Frame = new CGRect(new CGPoint(btnGotIt.Frame.X, viewLine.Frame.GetMaxY()), btnGotIt.Frame.Size);
            view.Frame = new CGRect(0, 0, view.Frame.Width, btnGotIt.Frame.GetMaxY());
        }

        private void UpdateView(ref UIView view, UIImageView imageView, WhatsNewModel whatsNewItem)
        {
            UITextView txtView = GetBodyContent(whatsNewItem.PopUp_Text_Content, ScaleUtility.GetYLocationFromFrame(imageView.Frame, 16));
            CustomUIView viewDoNotShowAgain = new CustomUIView(new CGRect(ScaleUtility.GetScaledWidth(16), GetYLocationFromFrame(txtView.Frame, 16), 0, 0))
            {
                Tag = 1003
            };
            nfloat lineYLoc = 0;
            if (!whatsNewItem.Disable_DoNotShow_Checkbox)
            {
                viewDoNotShowAgain = GetDontShowAgainView(whatsNewItem, GetYLocationFromFrame(txtView.Frame, 16));
                lineYLoc = ScaleUtility.GetScaledHeight(16);
            }

            UIView lineView = new UIView(new CGRect(0, viewDoNotShowAgain.Frame.GetMaxY() + lineYLoc, baseWidth, ScaleUtility.GetScaledHeight(1)))
            {
                BackgroundColor = MyTNBColor.LinesGray,
                Tag = 1004
            };

            UIButton btnGotIt = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(0, lineView.Frame.GetMaxY(), baseWidth, ScaleUtility.GetScaledHeight(56)),
                BackgroundColor = UIColor.White,
                Tag = 1005,

            };
            btnGotIt.SetAttributedTitle(LabelHelper.CreateAttributedString(LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt)
                , MyTNBFont.MuseoSans16_500, MyTNBColor.PowerBlue), UIControlState.Normal);
            btnGotIt.TouchDown += OnTapExit;

            view = new UIView(new CGRect(0, 0, baseWidth, btnGotIt.Frame.GetMaxY()))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            view.Layer.CornerRadius = ScaleUtility.GetScaledWidth(6);
            view.AddSubviews(new UIView[] { imageView, txtView, viewDoNotShowAgain, lineView, btnGotIt });
        }

        private UITextView GetBodyContent(string message, nfloat yLoc)
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
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                ContentInset = new UIEdgeInsets(-5, 0, -5, 0),
                Tag = 1002
            };
            txtViewDetails.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            txtViewDetails.TextContainer.LineFragmentPadding = 0F;

            //Resize
            var maxDescriptionHeight = UIScreen.MainScreen.Bounds.Height;
            CGSize size = txtViewDetails.SizeThatFits(new CGSize(baseWidth - (ScaleUtility.GetScaledWidth(16) * 2), maxDescriptionHeight));
            nfloat txtViewHeight = size.Height > maxDescriptionHeight ? maxDescriptionHeight : size.Height;
            txtViewDetails.Frame = new CGRect(ScaleUtility.GetScaledWidth(16), yLoc, baseWidth - (ScaleUtility.GetScaledWidth(16) * 2), txtViewHeight);
            txtViewDetails.TextAlignment = UITextAlignment.Left;

            return txtViewDetails;
        }

        private CustomUIView GetDontShowAgainView(WhatsNewModel whatsNewItem, nfloat yLoc)
        {
            CustomUIView viewDoNotShowAgain = new CustomUIView(new CGRect(ScaleUtility.GetScaledWidth(16)
                , yLoc
                , baseWidth - (margin * 2)
                , ScaleUtility.GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true,
                Tag = 1003
            };
            viewDoNotShowAgain.Layer.CornerRadius = ScaleUtility.GetScaledHeight(4F);

            UIView viewCheckBox = new UIView(new CGRect(ScaleUtility.GetScaledWidth(5F), ScaleUtility.GetScaledHeight(5F), ScaleUtility.GetScaledWidth(13F), ScaleUtility.GetScaledHeight(13F)))
            {
                BackgroundColor = MyTNBColor.WhiteTwo
            };
            viewCheckBox.Layer.CornerRadius = ScaleUtility.GetScaledWidth(3F);
            viewCheckBox.Layer.BorderColor = UIColor.Clear.CGColor;
            viewCheckBox.Layer.BorderWidth = ScaleUtility.GetScaledWidth(1F);
            UIImageView imgViewCheckBox = new UIImageView(new CGRect(0, 0, ScaleUtility.GetScaledWidth(13F), ScaleUtility.GetScaledHeight(13F)))
            {
                Image = UIImage.FromBundle(LoginConstants.IMG_RememberIcon),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                BackgroundColor = UIColor.Clear
            };
            viewCheckBox.AddSubview(imgViewCheckBox);

            imgViewCheckBox.Hidden = !WhatsNewServices.GetIsSkipAppLaunch(whatsNewItem.ID);
            viewCheckBox.Layer.BorderColor = WhatsNewServices.GetIsSkipAppLaunch(whatsNewItem.ID) ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

            viewDoNotShowAgain.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                bool flag = WhatsNewServices.GetIsSkipAppLaunch(whatsNewItem.ID);
                flag = !flag;
                imgViewCheckBox.Hidden = !flag;
                viewCheckBox.Layer.BorderColor = flag ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
                WhatsNewServices.SetIsSkipAppLaunch(whatsNewItem.ID, flag);
            }));

            UILabel lblDontShowAgain = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBox.Frame, 8F), ScaleUtility.GetScaledHeight(4F), viewDoNotShowAgain.Frame.Width, ScaleUtility.GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_11_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_DontShowThisAgain)
            };

            viewDoNotShowAgain.AddSubviews(new UIView[] { viewCheckBox, lblDontShowAgain });
            return viewDoNotShowAgain;
        }

        #region Portrait
        private void GetViewForPortrait(ref UIView view, WhatsNewModel whatsNewItem)
        {
            double buttonHeight = ScaleUtility.GetScaledHeight(52F);
            double itemHeight = UIScreen.MainScreen.Bounds.Height - buttonHeight;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                itemHeight = itemHeight - (ScaleUtility.GetScaledHeight(68F) + (UIScreen.MainScreen.Bounds.Height * 0.202F));
            }
            else
            {
                itemHeight = itemHeight - (ScaleUtility.GetScaledHeight(18F) + (UIScreen.MainScreen.Bounds.Height * 0.102F));
            }

            double carHeight = itemHeight + buttonHeight;
            double buttonWidth = Math.Floor(baseWidth);

            view = new UIView(new CGRect(0, 0, baseWidth, carHeight))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };

            view.Layer.CornerRadius = ScaleUtility.GetScaledWidth(6);

            UIView mainView = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
            {
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = true
            };

            UIImageView imgView = new UIImageView(new CGRect(0, 0, baseWidth, itemHeight));
            CustomUIView viewDoNotShowAgain = GetDontShowAgainView(whatsNewItem, imgView.Bounds.Height - ScaleUtility.GetScaledHeight(34F));

            if (whatsNewItem.PortraitImage_PopUp.IsValid())
            {
                NSData imgData = WhatsNewPopupCache.GetImage(whatsNewItem.ID);
                if (imgData != null)
                {
                    using (var image = UIImage.LoadFromData(imgData))
                    {
                        imgView.Image = image;
                    }

                    mainView.AddSubview(imgView);
                }
                else
                {
                    try
                    {
                        UIView imgLoadingView = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                        {
                            BackgroundColor = UIColor.Clear
                        };

                        mainView.AddSubview(imgLoadingView);
                        mainView.AddSubview(imgView);

                        UIView viewImage = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                        {
                            BackgroundColor = MyTNBColor.PaleGreyThree
                        };

                        CustomShimmerView shimmeringView = new CustomShimmerView();
                        UIView viewShimmerParent = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                        { BackgroundColor = UIColor.Clear };
                        UIView viewShimmerContent = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                        { BackgroundColor = UIColor.Clear };
                        viewShimmerParent.AddSubview(shimmeringView);
                        shimmeringView.ContentView = viewShimmerContent;
                        shimmeringView.Shimmering = true;
                        shimmeringView.SetValues();

                        viewShimmerContent.AddSubview(viewImage);
                        imgLoadingView.AddSubview(viewShimmerParent);

                        NSUrl url = new NSUrl(whatsNewItem.PortraitImage_PopUp);
                        NSUrlSession session = NSUrlSession
                            .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                        NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                        {
                            if (error == null && response != null && data != null)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    using (var image = UIImage.LoadFromData(data))
                                    {
                                        imgView.Image = image;
                                    }
                                    WhatsNewPopupCache.SaveImage(whatsNewItem.ID, data);
                                    imgLoadingView.RemoveFromSuperview();
                                });
                            }
                        });
                        dataTask.Resume();
                    }
                    catch (MonoTouchException m)
                    {
                        Debug.WriteLine("Image load Error: " + m.Message);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Image load Error: " + e.Message);
                    }
                }
            }
            else
            {
                UIView imgLoadingView = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                {
                    BackgroundColor = UIColor.Clear
                };

                mainView.AddSubview(imgLoadingView);
                mainView.AddSubview(imgView);

                imgLoadingView.Layer.BorderWidth = 1;
                imgLoadingView.Layer.BorderColor = UIColor.Cyan.CGColor;

                imgView.Layer.BorderWidth = 1;
                imgView.Layer.BorderColor = UIColor.Green.CGColor;

                UIView viewImage = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree
                };

                CustomShimmerView shimmeringView = new CustomShimmerView();
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, baseWidth, itemHeight))
                { BackgroundColor = UIColor.Clear };
                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;
                shimmeringView.SetValues();

                viewShimmerContent.AddSubview(viewImage);
                imgLoadingView.AddSubview(viewShimmerParent);
            }
            view.AddSubview(mainView);

            if (!whatsNewItem.Disable_DoNotShow_Checkbox)
            {
                view.AddSubview(viewDoNotShowAgain);
            }

            UIButton btnGotIt = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(0, view.Frame.Height - buttonHeight, buttonWidth, buttonHeight),
                BackgroundColor = UIColor.White
            };
            btnGotIt.SetAttributedTitle(LabelHelper.CreateAttributedString(LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt), MyTNBFont.MuseoSans16_500, MyTNBColor.PowerBlue), UIControlState.Normal);
            btnGotIt.TouchDown += OnTapExit;
            view.AddSubview(btnGotIt);

            UIView lineView = new UIView(new CGRect(btnGotIt.Frame.Width + 1, view.Frame.Height - buttonHeight, 1, buttonHeight))
            {
                BackgroundColor = MyTNBColor.LinesGray
            };
            view.AddSubview(lineView);
        }
        #endregion
    }
}