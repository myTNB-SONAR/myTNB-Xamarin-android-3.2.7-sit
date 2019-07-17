using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class DashboardHomeAccountCard
    {
        private readonly UIView _parentView;
        UIView _accountCardView;
        UIImageView _accountIcon;
        UILabel _accountNickname, _accountNo, _amountDue, _dueDate;

        nfloat _yLocation = 0f;

        public DashboardHomeAccountCard(UIView view, nfloat yLocation)
        {
            _parentView = view;
            _yLocation = yLocation;
        }

        private void CreateComponent()
        {
            nfloat parentHeight = _parentView.Frame.Height;
            nfloat parentWidth = _parentView.Frame.Width;
            nfloat padding = 16f;
            nfloat margin = 15f;

            _accountCardView = new UIView(new CGRect(16f, _yLocation + margin, parentWidth - (padding * 2), 60f))
            {
                BackgroundColor = UIColor.White
            };
            _accountCardView.Layer.CornerRadius = 5f;
            AddCardShadow(ref _accountCardView);

            _accountIcon = new UIImageView(new CGRect(12f, DeviceHelper.GetCenterYWithObjHeight(28f, _accountCardView), 28f, 28f))
            {
                Image = UIImage.FromBundle("Accounts-Smart-Meter-Icon")
            };

            _accountNickname = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + 12f, 12f, 69f, 20f))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                Text = @"Bukit Kiara"
            };

            _accountNo = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + 12f, _accountNickname.Frame.GetMaxY(), 81f, 20f))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = UIColor.LightGray,
                Text = @"289378273183"
            };

            _accountCardView.AddSubviews(new UIView { _accountIcon, _accountNickname, _accountNo });
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _accountCardView;
        }

        public UIView GetView()
        {
            return _accountCardView;
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
}
