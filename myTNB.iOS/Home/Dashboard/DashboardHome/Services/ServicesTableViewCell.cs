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
            _view = new UIView(new CGRect(16, 0, cellWidth - 32, 60.0F));
            AddSubview(_view);
            _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards()
        {
            List<string> serviceList = new List<string>() { "Apply for Self Meter Reading", "Check Status", "Set Appointments", "Apply AutoPay" };
            nfloat height = 0F;
            for (int i = 0; i < serviceList.Count; i++)
            {
                UIView card = GetCard(serviceList[i], i);
                _view.AddSubview(card);
                if (i == serviceList.Count - 1)
                {
                    height = card.Frame.GetMaxY() + 8;
                }
            }
            CGRect newFrame = _view.Frame;
            newFrame.Height = height;
            _view.Frame = newFrame;
        }

        private UIView GetCard(string title, int index, Action action = null)
        {
            nfloat cardWidth = cellWidth * 0.4375F;
            nfloat cardHeight = cardWidth * 0.7857F;
            nfloat margin = _view.Frame.Width - (cardWidth * 2);
            nfloat xLoc = IsEvenCard(index) ? 0 : cardWidth + margin;
            nfloat yLoc = (cardHeight + margin);
            yLoc *= GetFactor(index);
            UIView view = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.Red };
            view.Layer.CornerRadius = 5.0F;
            nfloat xLblLoc = cardWidth * 0.09F;
            nfloat ylblLoc = cardHeight * 0.60F;
            UILabel lblTitle = new UILabel(new CGRect(xLblLoc, ylblLoc, cardWidth - (xLblLoc * 2), cardHeight * 0.3F))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans12_500,
                Lines = 0,
                Text = title
            };
            view.AddSubview(lblTitle);
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
    }
}
