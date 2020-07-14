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
        List<WhatsNewModel> whatsnews;
        UIView parentView;

        public Action OnTapDetails { get; set; }
        public EventHandler OnTapExit { get; set; }

        public WhatsNewModalDataSource(UIView parent, List<WhatsNewModel> whatnew)
        {
            parentView = parent;
            whatsnews = whatnew;
        }

        public override nint GetNumberOfItems(iCarousel carousel)
        {
            return whatsnews?.Count ?? 0;
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

        public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view)
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
            float marginPercentage = 0.056F;
            double margin = UIScreen.MainScreen.Bounds.Width * marginPercentage;
            double carWidth = UIScreen.MainScreen.Bounds.Width - (margin * 2);
            double carHeight = itemHeight + buttonHeight;
            double buttonWidth = Math.Floor(carWidth);

            var whatsnew = whatsnews[(int)index];

            view = new UIView(new CGRect(0, 0, carWidth, carHeight))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };

            view.Layer.CornerRadius = 6.0f;

            UIView mainView = new UIView(new CGRect(0, 0, carWidth, itemHeight))
            {
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = true
            };

            UIImageView imgView = new UIImageView(new CGRect(0, 0, carWidth, itemHeight));
            UIView viewDoNotShowAgain = new UIView(new CGRect(ScaleUtility.GetScaledHeight(12F), imgView.Bounds.Height - ScaleUtility.GetScaledHeight(34F), imgView.Bounds.Width - ScaleUtility.GetScaledHeight(24F), ScaleUtility.GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
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

            imgViewCheckBox.Hidden = !WhatsNewServices.GetIsSkipAppLaunch(whatsnew.ID);
            viewCheckBox.Layer.BorderColor = WhatsNewServices.GetIsSkipAppLaunch(whatsnew.ID) ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;

            viewCheckBox.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                bool flag = WhatsNewServices.GetIsSkipAppLaunch(whatsnew.ID);
                flag = !flag;
                imgViewCheckBox.Hidden = !flag;
                viewCheckBox.Layer.BorderColor = flag ? UIColor.Clear.CGColor : MyTNBColor.VeryLightPinkSeven.CGColor;
                WhatsNewServices.SetIsSkipAppLaunch(whatsnew.ID, flag);
            }));

            UILabel lblDontShowAgain = new UILabel(new CGRect(GetXLocationFromFrame(viewCheckBox.Frame, 8F), ScaleUtility.GetScaledHeight(4F), viewDoNotShowAgain.Frame.Width, ScaleUtility.GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_11_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = LanguageUtility.GetCommonI18NValue(Constants.Common_DontShowThisAgain)
            };

            viewDoNotShowAgain.AddSubviews(new UIView[] { viewCheckBox, lblDontShowAgain });


            if (whatsnew.PortraitImage_PopUp.IsValid())
            {
                NSData imgData = WhatsNewPopupCache.GetImage(whatsnew.ID);
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
                        UIView imgLoadingView = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                        {
                            BackgroundColor = UIColor.Clear
                        };

                        mainView.AddSubview(imgLoadingView);
                        mainView.AddSubview(imgView);

                        UIView viewImage = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                        {
                            BackgroundColor = MyTNBColor.PaleGreyThree
                        };

                        CustomShimmerView shimmeringView = new CustomShimmerView();
                        UIView viewShimmerParent = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                        { BackgroundColor = UIColor.Clear };
                        UIView viewShimmerContent = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                        { BackgroundColor = UIColor.Clear };
                        viewShimmerParent.AddSubview(shimmeringView);
                        shimmeringView.ContentView = viewShimmerContent;
                        shimmeringView.Shimmering = true;
                        shimmeringView.SetValues();

                        viewShimmerContent.AddSubview(viewImage);
                        imgLoadingView.AddSubview(viewShimmerParent);

                        NSUrl url = new NSUrl(whatsnew.PortraitImage_PopUp);
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
                                    WhatsNewPopupCache.SaveImage(whatsnew.ID, data);
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
                UIView imgLoadingView = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                {
                    BackgroundColor = UIColor.Clear
                };

                mainView.AddSubview(imgLoadingView);
                mainView.AddSubview(imgView);

                UIView viewImage = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree
                };

                CustomShimmerView shimmeringView = new CustomShimmerView();
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, carWidth, itemHeight))
                { BackgroundColor = UIColor.Clear };
                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;
                shimmeringView.SetValues();

                viewShimmerContent.AddSubview(viewImage);
                imgLoadingView.AddSubview(viewShimmerParent);
            }

            UITapGestureRecognizer tapDetail = new UITapGestureRecognizer(OnGetTapDetail)
            {
                NumberOfTapsRequired = 1
            };
            mainView.AddGestureRecognizer(tapDetail);
            view.AddSubview(mainView);

            if (!whatsnew.Disable_DoNotShow_Checkbox)
            {
                view.AddSubview(viewDoNotShowAgain);
            }

            UIButton btnGotIt = new UIButton(UIButtonType.Custom);
            btnGotIt.Frame = new CGRect(0, view.Frame.Height - buttonHeight, buttonWidth, buttonHeight);
            btnGotIt.SetAttributedTitle(LabelHelper.CreateAttributedString(LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt), MyTNBFont.MuseoSans16_500, MyTNBColor.PowerBlue), UIControlState.Normal);
            btnGotIt.BackgroundColor = UIColor.White;
            btnGotIt.TouchDown += OnTapExit;
            view.AddSubview(btnGotIt);

            UIView lineView = new UIView(new CGRect(btnGotIt.Frame.Width + 1, view.Frame.Height - buttonHeight, 1, buttonHeight));
            lineView.BackgroundColor = MyTNBColor.LinesGray;
            view.AddSubview(lineView);

            return view;
        }
    }
}
