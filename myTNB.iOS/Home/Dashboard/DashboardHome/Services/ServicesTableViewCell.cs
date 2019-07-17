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
        public ServicesTableViewCell(IntPtr handle) : base(handle)
        {
            _view = new UIView(new CGRect(16, 0, cellWidth - 32, 60.0F))
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

        public void AddCards()
        {
            List<ServicesTemp> tempData = new List<ServicesTemp>()
            {
                new ServicesTemp(){Title = "Apply for Self Meter Reading", Img = "Services-ApplySSMR"}
                , new ServicesTemp(){Title = "Check Status", Img = "Services-CheckStatus"}
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
                    height = card.Frame.GetMaxY() + 8;
                }
            }
            CGRect newFrame = _view.Frame;
            newFrame.Height = height;
            _view.Frame = newFrame;
        }

        private UIView GetCard(ServicesTemp serviceItem, int index, Action action = null)
        {
            nfloat cardWidth = (_view.Frame.Width - 8) / 2;
            nfloat cardHeight = cardWidth * 0.7857F;
            nfloat margin = 8;
            nfloat xLoc = IsEvenCard(index) ? 0 : cardWidth + margin;
            nfloat yLoc = (cardHeight + margin);
            yLoc *= GetFactor(index);

            UIView view = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
            view.Layer.CornerRadius = 5.0F;
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
                Font = MyTNBFont.MuseoSans12_500,
                Lines = 0,
                Text = serviceItem.Title
            };
            view.AddSubviews(new UIView[] { imgView, lblTitle });
            return view;
        }

        private bool IsEvenCard(int index)
        {
            return index % 2 == 0;
        }

        private nfloat GetFactor(int index)
        {
            if (index < 1)
            {
                return 0;
            }
            return (nfloat)Math.Floor((decimal)index / 2);
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = 1;
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