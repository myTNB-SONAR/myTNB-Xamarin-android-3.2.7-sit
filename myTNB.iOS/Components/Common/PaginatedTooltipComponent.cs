using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class PaginatedTooltipComponent : BaseComponent
    {
        UIView _parentView, _containerView, _toolTipFooterView;
        UIScrollView _scrollView;
        UILabel _proceedLabel;
        int _currentPageIndex;
        List<SSMRMeterReadWalkthroughModel> _ssmrData;
        List<BillsTooltipModel> _billsTooltipData;
        List<SMRMROValidateRegisterDetailsInfoModel> _previousMeterList;
        UIPageControl _pageControl;
        bool isSSMRData;

        public PaginatedTooltipComponent(UIView parent)
        {
            _parentView = parent;
        }

        private void CreateSSMRTooltip()
        {
            nfloat widthMargin = GetScaledWidth(18f);
            nfloat width = _parentView.Frame.Width;
            nfloat height = GetScaledHeight(500f);
            _containerView = new UIView(new CGRect(widthMargin, GetYLocationToCenterObject(height, _parentView), width - (widthMargin * 2), height))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            _containerView.Layer.CornerRadius = 5f;
            SetToolTipScrollView();
            SetSubViewsForSSMRTooltip();
        }

        private void CreateBillDetailsTooltip()
        {
            nfloat widthMargin = GetScaledWidth(18f);
            nfloat width = _parentView.Frame.Width;
            nfloat height = GetScaledHeight(500f);
            _containerView = new UIView(new CGRect(widthMargin, GetYLocationToCenterObject(height, _parentView), width - (widthMargin * 2), height))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            _containerView.Layer.CornerRadius = 5f;
            SetToolTipScrollView();
            SetSubViewsForBillDetailsTooltip();
        }

        #region Bill Details
        public UIView GetBillDetailsTooltip()
        {
            CreateBillDetailsTooltip();
            return _containerView;
        }

        public void SetBillsToolTipData(List<BillsTooltipModel> data)
        {
            if (data != null)
            {
                _billsTooltipData = data;
            }
        }

        private void SetSubViewsForBillDetailsTooltip()
        {
            nfloat widthMargin = GetScaledWidth(16f);
            nfloat width = _scrollView.Frame.Width;
            nfloat newHeight = 0f;
            for (int i = 0; i < _billsTooltipData.Count; i++)
            {
                UIView viewContainer = new UIView(_scrollView.Bounds);
                viewContainer.BackgroundColor = UIColor.White;

                UIImage displayImage;
                if (_billsTooltipData[i].IsSitecoreData)
                {
                    if (string.IsNullOrEmpty(_billsTooltipData[i].Image) || string.IsNullOrWhiteSpace(_billsTooltipData[i].Image))
                    {
                        displayImage = UIImage.FromBundle(string.Empty);
                    }
                    else
                    {
                        try
                        {
                            displayImage = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(_billsTooltipData[i].Image)));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Image load Error: " + e.Message);
                            displayImage = UIImage.FromBundle(string.Empty);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_billsTooltipData[i].Image) || string.IsNullOrWhiteSpace(_billsTooltipData[i].Image))
                    {
                        displayImage = UIImage.FromBundle(string.Empty);
                    }
                    else
                    {
                        displayImage = UIImage.FromBundle(_billsTooltipData[i].Image);
                    }
                }

                nfloat origImageRatio = 180.0f / 284.0f;
                nfloat imageHeight = viewContainer.Frame.Width * origImageRatio;
                UIImageView imageView = new UIImageView(new CGRect(0, 0, viewContainer.Frame.Width, imageHeight))
                {
                    Image = displayImage,
                };
                viewContainer.AddSubview(imageView);

                UILabel title = new UILabel(new CGRect(widthMargin, imageView.Frame.GetMaxY() + GetScaledHeight(16f), viewContainer.Frame.Width - (widthMargin * 2), 0))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = _billsTooltipData[i].Title
                };

                CGSize titleNewSize = title.SizeThatFits(new CGSize(viewContainer.Frame.Width - (widthMargin * 2), 1000f));
                ViewHelper.AdjustFrameSetHeight(title, titleNewSize.Height);

                viewContainer.AddSubview(title);

                nfloat yPos = title.Frame.GetMaxY() + BaseMarginHeight16;
                if (_billsTooltipData[i].Description.Count > 0)
                {
                    List<string> descriptionList = _billsTooltipData[i].Description;
                    for (int o = 0; o < descriptionList.Count; o++)
                    {
                        UIView itemView = ItemView(viewContainer, yPos, o, descriptionList[o]);
                        viewContainer.AddSubview(itemView);
                        yPos = itemView.Frame.GetMaxY() + GetScaledHeight(8F);
                    }
                }
                ViewHelper.AdjustFrameSetX(viewContainer, i * width);
                ViewHelper.AdjustFrameSetWidth(viewContainer, width);
                ViewHelper.AdjustFrameSetHeight(viewContainer, yPos + GetScaledHeight(8f));

                _scrollView.AddSubview(viewContainer);
                if (newHeight < viewContainer.Frame.GetMaxY())
                {
                    newHeight = viewContainer.Frame.GetMaxY();
                }
            }
            _scrollView.ContentSize = new CGSize(_scrollView.Frame.Width * _billsTooltipData.Count, newHeight);
            ViewHelper.AdjustFrameSetHeight(_scrollView, newHeight);

            SetToolTipBillDetailsFooterView();
        }

        private UIView ItemView(UIView parent, nfloat yPos, int index, string text)
        {
            nfloat width = parent.Frame.Width - (BaseMarginWidth16 * 2);
            nfloat height = GetScaledHeight(20F);
            UIView itemView = new UIView(new CGRect(BaseMarginWidth16, yPos, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            UIView numberView = new UIView(new CGRect(0, 0, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            numberView.Layer.CornerRadius = GetScaledHeight(10F);
            UILabel numberLabel = new UILabel(new CGRect(GetScaledWidth(7F), GetScaledHeight(2F), GetScaledWidth(6F), GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Center,
                Text = (index + 1).ToString()
            };
            numberView.AddSubview(numberLabel);
            nfloat labelWidth = width - numberView.Frame.GetMaxX() - GetScaledWidth(8F);
            UILabel descLabel = new UILabel(new CGRect(numberView.Frame.GetMaxX() + GetScaledWidth(8F), 0, labelWidth, GetScaledHeight(20F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                Text = text
            };
            itemView.AddSubviews(new UIView { numberView, descLabel });
            CGSize size = descLabel.SizeThatFits(new CGSize(labelWidth, 1000f));
            ViewHelper.AdjustFrameSetHeight(descLabel, size.Height);
            if (descLabel.Frame.Height > height)
            {
                ViewHelper.AdjustFrameSetHeight(itemView, descLabel.Frame.GetMaxY());
            }
            ViewHelper.AdjustFrameSetY(descLabel, GetYLocationToCenterObject(descLabel.Frame.Height, itemView));
            return itemView;
        }

        private void SetToolTipBillDetailsFooterView()
        {
            _toolTipFooterView = new UIView(new CGRect(0, _scrollView.Frame.GetMaxY(), _containerView.Frame.Width, 130f))
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true,
                UserInteractionEnabled = true
            };
            if (_billsTooltipData.Count > 1)
            {
                AddPageControl();
                UpdatePageControl(_pageControl, _currentPageIndex, _billsTooltipData.Count);
            }
            else
            {
                if (_pageControl != null)
                {
                    _pageControl.Hidden = true;
                }
            }

            UIView line = new UIView(new CGRect(0, (_pageControl != null) ? _pageControl.Frame.GetMaxY() + GetScaledHeight(16f) : _scrollView.Frame.GetMaxY() + GetScaledHeight(16f)
                , _toolTipFooterView.Frame.Width, GetScaledHeight(1f)))
            {
                BackgroundColor = MyTNBColor.VeryLightPink
            };
            _toolTipFooterView.AddSubview(line);

            _proceedLabel = new UILabel(new CGRect(0, line.Frame.GetMaxY() + GetScaledHeight(16f)
                , _toolTipFooterView.Frame.Width, GetScaledHeight(24f)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "Got It!",//GetI18NValue(SSMRConstants.I18N_ImReady),
                TextAlignment = UITextAlignment.Center,
                UserInteractionEnabled = true
            };
            _toolTipFooterView.AddSubview(_proceedLabel);

            ViewHelper.AdjustFrameSetHeight(_toolTipFooterView, _proceedLabel.Frame.GetMaxY() + GetScaledHeight(16f));

            _containerView.AddSubview(_toolTipFooterView);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            ViewHelper.AdjustFrameSetHeight(_containerView, _toolTipFooterView.Frame.GetMaxY());
            ViewHelper.AdjustFrameSetY(_containerView, (currentWindow.Frame.Height / 2) - (_containerView.Frame.Height / 2));
        }
        #endregion

        #region SSMR
        public UIView GetSSMRTooltip()
        {
            CreateSSMRTooltip();
            return _containerView;
        }

        public void SetSSMRData(List<SSMRMeterReadWalkthroughModel> data)
        {
            if (data != null)
            {
                _ssmrData = data;
                isSSMRData = true;
            }
        }

        public void SetPreviousMeterData(List<SMRMROValidateRegisterDetailsInfoModel> data)
        {
            if (data != null)
            {
                _previousMeterList = data;
            }
        }

        private void SetSubViewsForSSMRTooltip()
        {
            nfloat widthMargin = GetScaledWidth(16f);
            nfloat width = _scrollView.Frame.Width;
            nfloat newHeight = 0f;
            for (int i = 0; i < _ssmrData.Count; i++)
            {
                UIView viewContainer = new UIView(_scrollView.Bounds);
                viewContainer.BackgroundColor = UIColor.White;

                UIImage displayImage;
                if (_ssmrData[i].IsSitecoreData)
                {
                    if (string.IsNullOrEmpty(_ssmrData[i].Image) || string.IsNullOrWhiteSpace(_ssmrData[i].Image))
                    {
                        displayImage = UIImage.FromBundle(string.Empty);
                    }
                    else
                    {
                        try
                        {
                            displayImage = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(_ssmrData[i].Image)));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Image load Error: " + e.Message);
                            displayImage = UIImage.FromBundle(string.Empty);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_ssmrData[i].Image) || string.IsNullOrWhiteSpace(_ssmrData[i].Image))
                    {
                        displayImage = UIImage.FromBundle(string.Empty);
                    }
                    else
                    {
                        displayImage = UIImage.FromBundle(_ssmrData[i].Image);
                    }
                }

                nfloat origImageRatio = 155.0f / 284.0f;
                nfloat imageHeight = viewContainer.Frame.Width * origImageRatio;
                UIImageView imageView = new UIImageView(new CGRect(0, 0, viewContainer.Frame.Width, imageHeight))
                {
                    Image = displayImage,
                };
                viewContainer.AddSubview(imageView);

                UILabel title = new UILabel(new CGRect(widthMargin, imageView.Frame.GetMaxY() + GetScaledHeight(24f), viewContainer.Frame.Width - (widthMargin * 2), 0))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = _ssmrData[i]?.Title ?? string.Empty
                };

                CGSize titleNewSize = title.SizeThatFits(new CGSize(viewContainer.Frame.Width - (widthMargin * 2), 1000f));
                ViewHelper.AdjustFrameSetHeight(title, titleNewSize.Height);

                viewContainer.AddSubview(title);
                string desc = _ssmrData[i]?.Description ?? string.Empty;
                if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc) && desc.Contains("{0}"))
                {
                    int count = _previousMeterList.Count;
                    string missingReading = string.Empty;
                    for (int j = 0; j < count; j++)
                    {
                        missingReading += string.IsNullOrEmpty(_previousMeterList[j].ReadingUnitDisplayTitle)
                            ? _previousMeterList[j].ReadingUnit : _previousMeterList[j].ReadingUnitDisplayTitle;
                        if (j != count - 1) { missingReading += ", "; }
                    }
                    desc = string.Format(desc, count, missingReading);
                }

                NSError htmlBodyError = null;
                NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(desc
                    , ref htmlBodyError, MyTNBFont.FONTNAME_300, (float)GetScaledHeight(14F));
                NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
                mutableHTMLBody.AddAttributes(new UIStringAttributes
                {
                    ForegroundColor = MyTNBColor.CharcoalGrey
                }, new NSRange(0, htmlBody.Length));
                UITextView description = new UITextView(new CGRect(widthMargin
                    , title.Frame.GetMaxY() + GetScaledHeight(12f), viewContainer.Frame.Width - (widthMargin * 2), 0))
                {
                    Editable = false,
                    ScrollEnabled = true,
                    AttributedText = mutableHTMLBody,
                    UserInteractionEnabled = false,
                    ContentInset = new UIEdgeInsets(0, -5, 0, -5)
                };
                description.ScrollIndicatorInsets = UIEdgeInsets.Zero;
                CGSize size = description.SizeThatFits(new CGSize(description.Frame.Width, 1000F));
                description.Frame = new CGRect(description.Frame.Location, new CGSize(description.Frame.Width, size.Height));

                viewContainer.AddSubview(description);

                ViewHelper.AdjustFrameSetX(viewContainer, i * width);
                ViewHelper.AdjustFrameSetWidth(viewContainer, width);
                ViewHelper.AdjustFrameSetHeight(viewContainer, description.Frame.GetMaxY() + GetScaledHeight(32f));

                _scrollView.AddSubview(viewContainer);
                if (newHeight < viewContainer.Frame.GetMaxY())
                {
                    newHeight = viewContainer.Frame.GetMaxY();
                }
            }
            _scrollView.ContentSize = new CGSize(_scrollView.Frame.Width * _ssmrData.Count, newHeight);
            ViewHelper.AdjustFrameSetHeight(_scrollView, newHeight);

            SetToolTipSSMRFooterView();
        }

        private void SetToolTipSSMRFooterView()
        {
            _toolTipFooterView = new UIView(new CGRect(0, _scrollView.Frame.GetMaxY(), _containerView.Frame.Width, 130f))
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true,
                UserInteractionEnabled = true
            };
            if (_ssmrData.Count > 1)
            {
                AddPageControl();
                UpdatePageControl(_pageControl, _currentPageIndex, _ssmrData.Count);
            }
            else
            {
                if (_pageControl != null)
                {
                    _pageControl.Hidden = true;
                }
            }

            UIView line = new UIView(new CGRect(0, _pageControl.Frame.GetMaxY() + GetScaledHeight(16f)
                , _toolTipFooterView.Frame.Width, GetScaledHeight(1f)))
            {
                BackgroundColor = MyTNBColor.VeryLightPink
            };
            _toolTipFooterView.AddSubview(line);

            _proceedLabel = new UILabel(new CGRect(0, line.Frame.GetMaxY() + GetScaledHeight(16f)
                , _toolTipFooterView.Frame.Width, GetScaledHeight(24f)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "I'm Ready!",//GetI18NValue(SSMRConstants.I18N_ImReady),
                TextAlignment = UITextAlignment.Center,
                UserInteractionEnabled = true
            };
            _toolTipFooterView.AddSubview(_proceedLabel);

            ViewHelper.AdjustFrameSetHeight(_toolTipFooterView, _proceedLabel.Frame.GetMaxY() + GetScaledHeight(16f));

            _containerView.AddSubview(_toolTipFooterView);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            ViewHelper.AdjustFrameSetHeight(_containerView, _toolTipFooterView.Frame.GetMaxY());
            ViewHelper.AdjustFrameSetY(_containerView, (currentWindow.Frame.Height / 2) - (_containerView.Frame.Height / 2));
        }
        #endregion

        private void SetToolTipScrollView()
        {
            _scrollView = new UIScrollView(new CGRect(0, 0, _containerView.Frame.Width, 0f))
            {
                Delegate = new PaginatedTooltipDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = true,
                BackgroundColor = UIColor.Clear,
                Hidden = false,
                Bounces = false
            };

            _containerView.AddSubview(_scrollView);
        }

        public void SetGestureRecognizer(UITapGestureRecognizer recognizer)
        {
            if (_proceedLabel != null)
            {
                _proceedLabel.AddGestureRecognizer(recognizer);
            }
        }

        private void AddPageControl()
        {
            if (_pageControl != null)
            {
                _pageControl.RemoveFromSuperview();
            }
            _pageControl = new UIPageControl(new CGRect(0, 0, _toolTipFooterView.Frame.Width, DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = MyTNBColor.WaterBlue,
                PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo,
                CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue,
                UserInteractionEnabled = false
            };
            _toolTipFooterView.AddSubview(_pageControl);
        }

        private void UpdatePageControl(UIPageControl pageControl, int current, int pages)
        {
            pageControl.CurrentPage = current;
            pageControl.Pages = pages;
            pageControl.UpdateCurrentPageDisplay();
        }

        private void ScrollViewHasPaginated()
        {
            UpdatePageControl(_pageControl, _currentPageIndex, isSSMRData ? _ssmrData.Count : _billsTooltipData.Count);
        }

        private class PaginatedTooltipDelegate : UIScrollViewDelegate
        {
            PaginatedTooltipComponent _controller;
            public PaginatedTooltipDelegate(PaginatedTooltipComponent controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._scrollView.ContentOffset.X / _controller._scrollView.Frame.Width);
                if (newPageIndex == _controller._currentPageIndex)
                    return;

                _controller._currentPageIndex = newPageIndex;
                _controller.ScrollViewHasPaginated();
            }
        }
    }
}
