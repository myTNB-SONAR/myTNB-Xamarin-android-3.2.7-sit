using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.Home.Components;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class RewardDetailsViewController : CustomUIViewController
    {
        public RewardsModel RewardModel;
        private UIView _footerView;
        private UIScrollView _scrollView;

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName_RewardDetails;
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            View.Frame = new CGRect(0, 0, width, height);
            View.BackgroundColor = UIColor.White;
            base.ViewDidLoad();
            SetNavigationBar();
            AddFooterView();
            SetScrollView();
            PrepareDetailView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            InvokeInBackground(async () =>
            {
                await RewardsServices.UpdateRewards(RewardModel, RewardsServices.RewardProperties.Read, true);
                RewardModel.IsRead = true;
                RewardsServices.UpdateRewardItem(RewardModel);
            });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            Title = GetI18NValue(RewardsConstants.I18N_Title);

            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            UIBarButtonItem btnShareReward = new UIBarButtonItem(UIImage.FromBundle(RewardsConstants.Img_ShareIcon), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Debug.WriteLine("btnShareReward");
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnShareReward;
        }

        private void SetScrollView()
        {
            if (_scrollView != null)
            {
                _scrollView.RemoveFromSuperview();
                _scrollView = null;
            }
            _scrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight - GetScaledHeight(80F)))
            {
                BackgroundColor = UIColor.Clear,
                Bounces = true
            };
            View.AddSubview(_scrollView);
        }

        private void PrepareDetailView()
        {
            UIView imageContainer = new UIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(180F)))
            {
                BackgroundColor = UIColor.Clear,
            };
            UIImageView imageView = new UIImageView(imageContainer.Bounds)
            {
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            if (RewardModel.Image.IsValid())
            {
                try
                {
                    ActivityIndicatorComponent _activityIndicator = new ActivityIndicatorComponent(imageContainer);
                    _activityIndicator.Show();
                    NSUrl url = new NSUrl(RewardModel.Image);
                    NSUrlSession session = NSUrlSession
                        .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
                    NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
                    {
                        if (error == null && response != null && data != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                imageView.Image = UIImage.LoadFromData(data);
                                _activityIndicator.Hide();
                            });
                        }
                        else
                        {
                            // Default image goes here...
                            //InvokeOnMainThread(() =>
                            //{
                            //    imageView.Image = UIImage.LoadFromData(data);
                            //    _activityIndicator.Hide();
                            //});
                        }
                    });
                    dataTask.Resume();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Image load Error: " + e.Message);
                    // Default image goes here...
                }
            }
            else
            {
                // Default image goes here...
            }

            nfloat viewWidth = ViewWidth - (BaseMarginWidth16 * 2);

            UITextView titleTextView = CreateHTMLContent(RewardModel.Title, true);
            CGSize titleTextViewSize = titleTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(titleTextView, titleTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(titleTextView, GetScaledWidth(16F));
            ViewHelper.AdjustFrameSetY(titleTextView, GetYLocationFromFrame(imageView.Frame, 20F));
            ViewHelper.AdjustFrameSetWidth(titleTextView, viewWidth);

            nfloat iconWidth = GetScaledWidth(24F);
            nfloat iconHeight = GetScaledHeight(24F);

            UIView rewardPeriodView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(titleTextView.Frame, 18F), viewWidth, 0))
            {
                BackgroundColor = UIColor.Clear
            };

            UIImageView rpIcon = new UIImageView(new CGRect(0, 0, iconWidth, iconHeight))
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromBundle(RewardsConstants.Img_RewardPeriodIcon)
            };

            UILabel rpTitle = new UILabel(new CGRect(GetXLocationFromFrame(rpIcon.Frame, 4F), 0, viewWidth - (rpIcon.Frame.GetMaxX() + GetScaledWidth(4F)), GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(RewardsConstants.I18N_RewardPeriod)
            };

            UITextView rpTextView = CreateHTMLContent(RewardModel.PeriodLabel);
            CGSize rpTextViewSize = rpTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(rpTextView, rpTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(rpTextView, 0);
            ViewHelper.AdjustFrameSetY(rpTextView, GetYLocationFromFrame(rpTitle.Frame, 10F));
            ViewHelper.AdjustFrameSetWidth(rpTextView, viewWidth);

            UIView rpLine = new UIView(new CGRect(0, GetYLocationFromFrame(rpTextView.Frame, 16F), viewWidth, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkSeven
            };

            rewardPeriodView.AddSubviews(new UIView { rpIcon, rpTitle, rpTextView, rpLine });

            ViewHelper.AdjustFrameSetHeight(rewardPeriodView, rpLine.Frame.GetMaxY());

            UIView locationView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(rewardPeriodView.Frame, 18F), viewWidth, 0))
            {
                BackgroundColor = UIColor.Clear
            };

            UIImageView locationIcon = new UIImageView(new CGRect(0, 0, iconWidth, iconHeight))
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromBundle(RewardsConstants.Img_RewardLocationIconn)
            };

            UILabel locationTitle = new UILabel(new CGRect(GetXLocationFromFrame(locationIcon.Frame, 4F), 0, viewWidth - (locationIcon.Frame.GetMaxX() + GetScaledWidth(4F)), GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(RewardsConstants.I18N_Location)
            };

            UITextView locationTextView = CreateHTMLContent(RewardModel.LocationLabel);
            CGSize locationTextViewSize = locationTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(locationTextView, locationTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(locationTextView, 0);
            ViewHelper.AdjustFrameSetY(locationTextView, GetYLocationFromFrame(locationTitle.Frame, 10F));
            ViewHelper.AdjustFrameSetWidth(locationTextView, viewWidth);

            UIView locationLine = new UIView(new CGRect(0, GetYLocationFromFrame(locationTextView.Frame, 16F), viewWidth, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkSeven
            };

            locationView.AddSubviews(new UIView { locationIcon, locationTitle, locationTextView, locationLine });

            ViewHelper.AdjustFrameSetHeight(locationView, locationLine.Frame.GetMaxY());

            UIView tandCView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(locationView.Frame, 18F), viewWidth, 0))
            {
                BackgroundColor = UIColor.Clear
            };

            UIImageView tandCIcon = new UIImageView(new CGRect(0, 0, iconWidth, iconHeight))
            {
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromBundle(RewardsConstants.Img_RewardTCIcon)
            };

            UILabel tandCTitle = new UILabel(new CGRect(GetXLocationFromFrame(tandCIcon.Frame, 4F), 0, viewWidth - (tandCIcon.Frame.GetMaxX() + GetScaledWidth(4F)), GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = GetI18NValue(RewardsConstants.I18N_TNC)
            };

            UITextView tandCTextView = CreateHTMLContent(RewardModel.TandCLabel);
            CGSize tandCTextViewSize = tandCTextView.SizeThatFits(new CGSize(viewWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(tandCTextView, tandCTextViewSize.Height);
            ViewHelper.AdjustFrameSetX(tandCTextView, 0);
            ViewHelper.AdjustFrameSetY(tandCTextView, GetYLocationFromFrame(tandCTitle.Frame, 10F));
            ViewHelper.AdjustFrameSetWidth(tandCTextView, viewWidth);

            tandCView.AddSubviews(new UIView { tandCIcon, tandCTitle, tandCTextView });

            ViewHelper.AdjustFrameSetHeight(tandCView, tandCTextView.Frame.GetMaxY());

            _scrollView.AddSubviews(new UIView { imageContainer, imageView, titleTextView, rewardPeriodView, locationView, tandCView });
            UpdateScrollViewContentSize(tandCView);
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
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };

            UITextView textView = new UITextView()
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                UserInteractionEnabled = true,
                TextContainerInset = UIEdgeInsets.Zero
            };

            return textView;
        }

        private void UpdateScrollViewContentSize(UIView lastView)
        {
            _scrollView.ContentSize = new CGSize(ViewWidth, lastView.Frame.GetMaxY());
        }

        private void AddFooterView()
        {
            nfloat height = GetScaledHeight(80F);
            nfloat width = ViewWidth;
            _footerView = new UIView(new CGRect(0, ViewHeight - height, width, height + GetBottomPadding))
            {
                BackgroundColor = UIColor.White
            };
            _footerView.Layer.MasksToBounds = false;
            _footerView.Layer.ShadowColor = MyTNBColor.SilverChalice10.CGColor;
            _footerView.Layer.ShadowOpacity = 1F;
            _footerView.Layer.ShadowOffset = new CGSize(0, -1);
            _footerView.Layer.ShadowRadius = 8;
            _footerView.Layer.ShadowPath = UIBezierPath.FromRect(_footerView.Bounds).CGPath;
            View.AddSubview(_footerView);

            CustomUIView saveBtnContainer = new CustomUIView(new CGRect(BaseMarginWidth16, GetScaledHeight(16F), (width / 2) - GetScaledWidth(18F), GetScaledHeight(48F)))
            {
                BackgroundColor = UIColor.White
            };
            saveBtnContainer.Layer.CornerRadius = 4f;
            saveBtnContainer.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            saveBtnContainer.Layer.BorderWidth = 1f;

            saveBtnContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("saveBtnContainer on tap");
                RewardModel.IsSaved = !RewardModel.IsSaved;
                InvokeInBackground(async () =>
                {
                    await RewardsServices.UpdateRewards(RewardModel, RewardsServices.RewardProperties.Favourite, RewardModel.IsSaved);
                });
                InvokeOnMainThread(() =>
                {
                    UpdateSaveButton(saveBtnContainer);
                });
            }));

            _footerView.AddSubview(saveBtnContainer);

            UIView saveBtnView = new UIView(new CGRect(0, 0, 0, GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Tag = 3000
            };
            saveBtnContainer.AddSubview(saveBtnView);

            nfloat imgWidth = GetScaledWidth(18F);
            nfloat imgHeight = GetScaledHeight(15F);
            UIImageView imgView = new UIImageView(new CGRect(0, 0, imgWidth, imgHeight))
            {
                Image = UIImage.FromBundle(RewardsConstants.Img_HeartUnsavedGreenIcon),
                Tag = 3001
            };
            saveBtnView.AddSubview(imgView);

            nfloat saveLblWidth = saveBtnContainer.Frame.Width - imgView.Frame.Width - GetScaledWidth(2F);
            UILabel saveLbl = new UILabel(new CGRect(imgView.Frame.GetMaxX() + GetScaledWidth(8F), GetScaledHeight(12F), saveLblWidth, GetScaledHeight(24F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.FreshGreen,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Tag = 3002
            };
            saveBtnView.AddSubview(saveLbl);
            UpdateSaveButton(saveBtnContainer);
            UIButton btnUseNow = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(saveBtnContainer.Frame.GetMaxX() + GetScaledWidth(4F), GetScaledHeight(16F), (width / 2) - GetScaledWidth(18F), GetScaledHeight(48F))
            };
            btnUseNow.Layer.CornerRadius = GetScaledHeight(4F);
            btnUseNow.Layer.BackgroundColor = MyTNBColor.FreshGreen.CGColor;
            btnUseNow.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnUseNow.Layer.BorderWidth = GetScaledHeight(1F);
            btnUseNow.SetTitle(GetI18NValue(RewardsConstants.I18N_UseNow), UIControlState.Normal);
            btnUseNow.Font = TNBFont.MuseoSans_16_500;
            btnUseNow.TouchUpInside += (sender, e) =>
            {
                Debug.WriteLine("btnUseNow on tap");
            };
            _footerView.AddSubview(btnUseNow);
        }

        private void UpdateSaveButton(CustomUIView viewRef)
        {
            UIView saveBtnView = viewRef.ViewWithTag(3000) as UIView;
            UIImageView imgView = viewRef.ViewWithTag(3001) as UIImageView;
            UILabel saveLbl = viewRef.ViewWithTag(3002) as UILabel;

            imgView.Image = UIImage.FromBundle(RewardModel.IsSaved ? RewardsConstants.Img_HeartSavedGreenIcon : RewardsConstants.Img_HeartUnsavedGreenIcon);
            saveLbl.Text = GetI18NValue(RewardModel.IsSaved ? RewardsConstants.I18N_Unsave : RewardsConstants.I18N_Save);

            nfloat saveLblWidth = viewRef.Frame.Width - imgView.Frame.Width - GetScaledWidth(2F);
            CGSize cGSizeLbl = saveLbl.SizeThatFits(new CGSize(saveLblWidth, GetScaledHeight(24F)));

            ViewHelper.AdjustFrameSetHeight(saveLbl, cGSizeLbl.Height);
            ViewHelper.AdjustFrameSetWidth(saveLbl, cGSizeLbl.Width);
            ViewHelper.AdjustFrameSetY(saveLbl, GetYLocationToCenterObject(saveLbl.Frame.Height, saveBtnView));
            ViewHelper.AdjustFrameSetY(imgView, GetYLocationToCenterObject(imgView.Frame.Height, saveBtnView));
            ViewHelper.AdjustFrameSetWidth(saveBtnView, saveLbl.Frame.GetMaxX());
            ViewHelper.AdjustFrameSetY(saveBtnView, GetYLocationToCenterObject(saveBtnView.Frame.Height, viewRef));
            ViewHelper.AdjustFrameSetX(saveBtnView, GetXLocationToCenterObject(saveBtnView.Frame.Width, viewRef));
        }
    }
}