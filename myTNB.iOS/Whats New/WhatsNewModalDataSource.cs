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

        public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view)
        {
            double buttonHeight = ScaleUtility.GetScaledHeight(44);
            double itemHeight = UIScreen.MainScreen.Bounds.Height - (ScaleUtility.GetScaledHeight(18F) + (UIScreen.MainScreen.Bounds.Height * 0.202F)) - buttonHeight;
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

            UITapGestureRecognizer tapDetail = new UITapGestureRecognizer(OnGetTapDetail)
            {
                NumberOfTapsRequired = 1
            };
            mainView.AddGestureRecognizer(tapDetail);
            view.AddSubview(mainView);

            UIButton btnGotIt = new UIButton(UIButtonType.Custom);
            btnGotIt.Frame = new CGRect(0, view.Frame.Height - buttonHeight, buttonWidth, buttonHeight);
            btnGotIt.SetAttributedTitle(LabelHelper.CreateAttributedString(LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt), MyTNBFont.MuseoSans14_500, MyTNBColor.PowerBlue), UIControlState.Normal);
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
