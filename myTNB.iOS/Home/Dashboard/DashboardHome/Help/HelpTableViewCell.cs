using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using UIKit;

namespace myTNB
{
    public class HelpTableViewCell : CustomUITableViewCell
    {
        private DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private UIScrollView _scrollView;
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat cardWidth;
        private nfloat cardHeight;
        private int _imgIndex;
        public UILabel _titleLabel;
        private UIPageControl _pageControl;
        int _currentPageIndex = 0;
        int _totalItems;
        public HelpTableViewCell(IntPtr handle) : base(handle)
        {
            cardWidth = GetScaledWidth(92F);
            cardHeight = GetScaledHeight(56F);
            _titleLabel = new UILabel(new CGRect(BaseMarginWidth16, 0, cellWidth - GetScaledWidth(32F), GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.WaterBlue
            };
            AddSubview(_titleLabel);
            UIView view = new UIView(new CGRect(0, GetYLocationFromFrame(_titleLabel.Frame, 8F), cellWidth, cardHeight + GetScaledHeight(12F)))
            {
                BackgroundColor = UIColor.Clear
            };
            _scrollView = new UIScrollView(new CGRect(0, 0, view.Frame.Width, cardHeight))
            {
                ClipsToBounds = false,
                ScrollEnabled = true,
                ShowsHorizontalScrollIndicator = false
            };
            _scrollView.Scrolled += OnScrolled;

            view.AddSubview(_scrollView);
            AddSubview(view);
            BackgroundColor = UIColor.Clear;
            AddPageControl(view.Frame.GetMaxY());
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards(List<HelpModel> helpList, bool isLoading)
        {
            for (int i = _scrollView.Subviews.Length; i-- > 0;)
            {
                _scrollView.Subviews[i].RemoveFromSuperview();
            }

            if (isLoading || helpList == null)
            {
                AddShimmer();
            }
            else
            {
                AddContentData(helpList);
                _totalItems = (int)Math.Ceiling((decimal)helpList.Count / 3);
                if (_totalItems > 1)
                {
                    UpdatePageControl(_pageControl, _currentPageIndex, _totalItems);
                }
            }
        }

        private void AddShimmer()
        {
            nfloat xLoc = ScaleUtility.BaseMarginWidth16;
            for (int i = 0; i < 4; i++)
            {
                CustomShimmerView shimmeringView = new CustomShimmerView();
                UIView viewParent = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
                viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

                UIView viewShimmerUI = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = MyTNBColor.PaleGreyThree };
                viewShimmerContent.AddSubview(viewShimmerUI);

                viewShimmerParent.AddSubview(shimmeringView);
                shimmeringView.ContentView = viewShimmerContent;
                shimmeringView.Shimmering = true;
                shimmeringView.SetValues();
                AddShimmerShadow(ref viewParent);
                _scrollView.Add(viewParent);
                xLoc += cardWidth + ScaleUtility.BaseMarginWidth8;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private void AddContentData(List<HelpModel> helpList)
        {
            nfloat xLoc = ScaleUtility.BaseMarginWidth16;
            _imgIndex = -1;
            for (int i = 0; i < helpList.Count; i++)
            {
                if (!ShowItemForSmartMeter(helpList[i].TagType))
                    continue;

                string helpKey = helpList[i].TargetItem;
                UIView helpCardView = new UIView(new CGRect(xLoc, 0, cardWidth, cardHeight)) { ClipsToBounds = true };
                helpCardView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    ViewHelper.GoToFAQScreenWithId(helpKey);
                }));

                UIView gradientBG = new UIView(new CGRect(0, 0, cardWidth, cardHeight));
                CGColor startColor = MyTNBColor.LightIndigo.CGColor;
                CGColor endColor = MyTNBColor.ClearBlue.CGColor;

                List<int> sList = GetRGBList(helpList[i].BGStartColor);
                if (sList?.Count == 3)
                {
                    UIColor sColor = GradientColour(sList);
                    startColor = sColor?.CGColor;
                }

                List<int> eList = GetRGBList(helpList[i].BGEndColor);
                if (eList?.Count == 3)
                {
                    UIColor eColor = GradientColour(eList);
                    endColor = eColor?.CGColor;
                }

                CAGradientLayer gradientLayer = new CAGradientLayer
                {
                    Colors = new[] { startColor, endColor }
                };
                gradientLayer.Locations = new NSNumber[] { 0, 1 };
                gradientLayer.Frame = gradientBG.Bounds;
                gradientBG.Layer.InsertSublayer(gradientLayer, 0);

                AddCardShadow(ref helpCardView);

                NSError htmlBodyError = null;
                NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(helpList[i]?.Title ?? string.Empty + "<br>"
               , ref htmlBodyError, TNBFont.FONTNAME_500, (float)TNBFont.GetFontSize(12F));
                NSMutableAttributedString attributedString = new NSMutableAttributedString(htmlBody);
                attributedString.AddAttributes(new UIStringAttributes
                {
                    ForegroundColor = UIColor.White,
                    ParagraphStyle = new NSMutableParagraphStyle { HyphenationFactor = 1 }
                }, new NSRange(0, htmlBody.Length));

                nfloat xRef = TNBGlobal.APP_LANGUAGE == "EN" ? 8 : 6;
                UITextView lblHelp = new UITextView(new CGRect(GetScaledWidth(xRef), GetScaledHeight(12F)
                    , helpCardView.Frame.Width - GetScaledWidth(xRef * 2), helpCardView.Frame.Height - GetScaledHeight(24F)))
                {
                    Editable = false,
                    ScrollEnabled = true,
                    AttributedText = attributedString,
                    ContentInset = new UIEdgeInsets(-5, 0, -5, 0),
                    BackgroundColor = UIColor.Clear
                };
                lblHelp.ScrollIndicatorInsets = UIEdgeInsets.Zero;
                lblHelp.TextContainer.LineFragmentPadding = 0F;

                helpCardView.AddSubviews(new UIView[] { gradientBG, lblHelp });
                _scrollView.Add(helpCardView);
                xLoc += cardWidth + ScaleUtility.BaseMarginWidth8;
            }
            _scrollView.ContentSize = new CGSize(xLoc, cardHeight);
        }

        private UIColor GradientColour(List<int> RGB)
        {
            UIColor colour = UIColor.White;
            if (!string.IsNullOrEmpty(RGB[0].ToString()) &&
                !string.IsNullOrEmpty(RGB[1].ToString()) &&
                !string.IsNullOrEmpty(RGB[2].ToString()))
            {
                colour = UIColor.FromRGB(RGB[0], RGB[1], RGB[2]);
            }
            return colour;
        }

        private List<int> GetRGBList(string rgbString)
        {
            List<int> rgbList = new List<int>();
            var rgbSplit = rgbString.Split('|');

            if (rgbSplit.Length > 0)
            {
                foreach (string str in rgbSplit)
                {
                    if (Int32.TryParse(str, out int numValue))
                        rgbList.Add(numValue);
                }
            }
            return rgbList;
        }

        private int GetBackgroundImage(int index)
        {
            _imgIndex = (index / 7) % 1 == 0 ? _imgIndex + 1 : 0;
            _imgIndex = _imgIndex > 6 || _imgIndex < 0 ? 0 : _imgIndex;
            return _imgIndex;
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 4.0F;
            view.ClipsToBounds = true;
        }

        private void AddShimmerShadow(ref UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.VeryLightPinkFive.CGColor;
            view.Layer.ShadowOpacity = 0.8F;
            view.Layer.ShadowOffset = new CGSize(0, 4);
            view.Layer.ShadowRadius = 4;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }

        private bool ShowItemForSmartMeter(TagEnum tagType)
        {
            if (tagType == TagEnum.SM)
            {
                return _dashboardHomeHelper.HasSmartMeterAccounts;
            }
            return true;
        }

        private void AddPageControl(nfloat yPos)
        {
            _pageControl = new UIPageControl(new CGRect(0, yPos - (DashboardHomeConstants.PageControlHeight / 2.5), Frame.Width, DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = UIColor.Clear,
                PageIndicatorTintColor = MyTNBColor.BrownGreyThree30,
                CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue,
                UserInteractionEnabled = false,
                Transform = CGAffineTransform.MakeScale(1.25F, 1.25F)
            };
            AddSubview(_pageControl);
        }

        private void UpdatePageControl(UIPageControl pageControl, int current, int pages)
        {
            if (pageControl != null)
            {
                pageControl.CurrentPage = current;
                pageControl.Pages = pages;
                pageControl.UpdateCurrentPageDisplay();
            }
        }

        private void OnScrolled(object sender, EventArgs e)
        {
            if (_totalItems <= 1) { return; }

            if (sender is UIScrollView scrollView)
            {
                for (int i = 0; i < scrollView.Subviews.Length; i++)
                {
                    UIView helpView = scrollView.Subviews[i];
                    int totalCount = scrollView.Subviews.Length;
                    if (helpView != null && helpView.GetType() != typeof(UIImageView))
                    {
                        nfloat maxXPos = helpView.Frame.X - scrollView.ContentOffset.X + helpView.Frame.Width;
                        if (maxXPos <= _cellWidth)
                        {
                            nfloat objPos = i + 1;
                            if (objPos < totalCount)
                            {
                                if (objPos % 3 == 0)
                                {
                                    _currentPageIndex = (int)((objPos / 3) - 1);
                                    UpdatePageControl(_pageControl, _currentPageIndex, _totalItems);
                                }
                                else if (objPos > 3 && (totalCount - objPos == 2 || totalCount - objPos == 1))
                                {
                                    _currentPageIndex = _totalItems - 1;
                                    UpdatePageControl(_pageControl, _currentPageIndex, _totalItems);
                                }
                            }
                            else if (objPos == totalCount && objPos % 3 == 0)
                            {
                                _currentPageIndex = (int)((objPos / 3) - 1);
                                UpdatePageControl(_pageControl, _currentPageIndex, _totalItems);
                            }
                            else if (objPos == totalCount && objPos % 3 != 0)
                            {
                                _currentPageIndex = _totalItems - 1;
                                UpdatePageControl(_pageControl, _currentPageIndex, _totalItems);
                            }
                        }
                    }
                }
            }
        }
    }
}