using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class ServicesTableViewCell : UITableViewCell
    {
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private UIView _view;
        private int rowFactor = -1;
        private nfloat xLoc;
        public UILabel _titleLabel;
        public ServicesTableViewCell(IntPtr handle) : base(handle)
        {
            _titleLabel = new UILabel(new CGRect(16f, 0, cellWidth - 32, 20f))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.PowerBlue
            };
            AddSubview(_titleLabel);
            _view = new UIView(new CGRect(16, _titleLabel.Frame.GetMaxY() + 8f, cellWidth - 32, 60.0F))
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_view);
            BackgroundColor = UIColor.Clear;
            _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards(bool hasData = false)
        {
            rowFactor = -1;
            xLoc = 0;
            for (int i = _view.Subviews.Length; i-- > 0;)
            {
                _view.Subviews[i].RemoveFromSuperview();
            }

            if (hasData)
            {
                AddContentData();
            }
            else
            {
                AddShimmer();
                InvokeInBackground(() =>
                {
                    System.Threading.Thread.Sleep(5000);
                    InvokeOnMainThread(() =>
                    {
                        AddCards(true);
                    });
                });
            }
        }

        private void AddShimmer()
        {
            nfloat height = 0F;
            for (int i = 0; i < 6; i++)
            {
                UIView card = GetShimmerCards(i);
                _view.AddSubview(card);
                if (i == 5)
                {
                    height = card.Frame.GetMaxY() + 12;
                }
            }
            CGRect newFrame = _view.Frame;
            newFrame.Height = height;
            _view.Frame = newFrame;
        }

        private void AddContentData()
        {
            List<ServicesTemp> tempData = new List<ServicesTemp>()
            {
                new ServicesTemp(){Title = "Apply for Self Meter Reading", Img = "Services-ApplySSMR"}
                , new ServicesTemp(){Title = "Check Status", Img = "Services-CheckStatus"}
                , new ServicesTemp(){Title = "Give Us Feedback", Img = "Services-Feedback"}
                , new ServicesTemp(){Title = "Set Appointments", Img = "Services-SetAppointments"}
                , new ServicesTemp(){Title = "Apply AutoPay", Img = "Services-ApplyAutoPay"}
            };

            nfloat height = 0F;
            for (int i = 0; i < tempData.Count; i++)
            {
                UIView card = GetCard(tempData[i], i);
                _view.AddSubview(card);
                if (i == tempData.Count - 1)
                {
                    height = card.Frame.GetMaxY() + 12;
                }
            }
            CGRect newFrame = _view.Frame;
            newFrame.Height = height;
            _view.Frame = newFrame;
        }

        private UIView GetCard(ServicesTemp serviceItem, int index, Action action = null)
        {
            nfloat cardWidth = (_view.Frame.Width - 24) / 3;
            nfloat cardHeight = cardWidth * 0.9545F;
            nfloat margin = 12;
            nfloat yLoc = (cardHeight + margin);
            yLoc *= GetFactor(index);
            if (rowFactor == GetFactor(index))
            {
                xLoc += cardWidth + margin;
            }
            else
            {
                rowFactor = (int)GetFactor(index);
                xLoc = 0;
            }

            UIView view = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
            AddCardShadow(ref view);

            nfloat imgSize = cardWidth * 0.34F;
            nfloat imgYLoc = cardHeight * 0.11F;
            UIImageView imgView = new UIImageView(new CGRect((view.Frame.Width - imgSize) / 2, imgYLoc, imgSize, imgSize))
            {
                Image = UIImage.FromBundle(serviceItem.Img)
            };

            nfloat xLblLoc = 16.0F;
            nfloat ylblLoc = cardHeight * 0.60F;
            UILabel lblTitle = new UILabel(new CGRect(xLblLoc, ylblLoc, cardWidth - (xLblLoc * 2), cardHeight * 0.3F))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans10_500,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = serviceItem.Title
            };
            view.AddSubviews(new UIView[] { imgView, lblTitle });
            return view;
        }

        private UIView GetShimmerCards(int index)
        {
            nfloat cardWidth = (_view.Frame.Width - 24) / 3;
            nfloat cardHeight = cardWidth * 0.9545F;
            nfloat margin = 12;
            nfloat yLoc = (cardHeight + margin);
            yLoc *= GetFactor(index);
            if (rowFactor == GetFactor(index))
            {
                xLoc += cardWidth + margin;
            }
            else
            {
                rowFactor = (int)GetFactor(index);
                xLoc = 0;
            }

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewParent = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
            AddCardShadow(ref viewParent);
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
            viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            nfloat viewImgWidth = viewShimmerContent.Frame.Width * 0.27F;
            UIView viewImg = new UIView(new CGRect((viewShimmerContent.Frame.Width - viewImgWidth) / 2, 16, viewImgWidth, viewImgWidth)) { BackgroundColor = MyTNBColor.PowderBlue };
            viewImg.Layer.CornerRadius = viewImgWidth / 2;
            UIView viewLbl = new UIView(new CGRect(12, viewImgWidth + 32, viewShimmerContent.Frame.Width - 24, 14)) { BackgroundColor = MyTNBColor.PowderBlue };

            viewShimmerContent.AddSubviews(new UIView[] { viewImg, viewLbl });

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;

            viewParent.AddSubviews(new UIView[] { });
            return viewParent;
        }

        private nfloat GetFactor(int index)
        {
            if (index < 1)
            {
                return 0;
            }
            return (nfloat)Math.Floor((decimal)index / 3);
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5.0F;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = 0.5F;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }

    //Todo: to be deleted
    public class ServicesTemp
    {
        public string Title { set; get; }
        public string Img { set; get; }
    }
}