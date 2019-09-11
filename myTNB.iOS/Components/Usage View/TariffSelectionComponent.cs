using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class TariffSelectionComponent : BaseComponent
    {
        CustomUIView _containerView;
        UIView _parentView, _rmKwhSelectionView, _tariffSelectionView;
        UIImageView _tariffIcon, _rmKwhIcon;
        UILabel _rmKwhLabel, _tariffLabel;
        nfloat containerHeight = ScaleUtility.GetScaledHeight(24f);
        public bool isTariffDisabled;

        public TariffSelectionComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            _containerView = new CustomUIView(new CGRect(0, 0, width, containerHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            CreateRMKwhView();
            CreateTariffView();
        }

        private void CreateRMKwhView()
        {
            nfloat rmSelectWidth = GetScaledWidth(60f);
            nfloat rmSelectHeight = GetScaledHeight(24f);
            _rmKwhSelectionView = new UIView(new CGRect(BaseMarginWidth16, 0, rmSelectWidth, rmSelectHeight))
            {
                BackgroundColor = UIColor.White,
            };
            _rmKwhSelectionView.Layer.CornerRadius = GetScaledHeight(13f);
            _containerView.AddSubview(_rmKwhSelectionView);

            nfloat rmKwhLabelXPos = GetScaledWidth(13f);
            nfloat rmKwhLabelYPos = GetScaledHeight(4f);
            nfloat rmKwhLabelWidth = GetScaledWidth(19f);
            nfloat rmKwhLabelHeight = GetScaledHeight(16f);
            _rmKwhLabel = new UILabel(new CGRect(rmKwhLabelXPos, rmKwhLabelYPos, rmKwhLabelWidth, rmKwhLabelHeight))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left
            };
            _rmKwhSelectionView.AddSubview(_rmKwhLabel);

            nfloat rmKwhIconXPos = GetScaledWidth(40f);
            nfloat rmKwhIconYPos = GetScaledHeight(9f);
            nfloat rmKwhIconWidth = GetScaledWidth(8f);
            nfloat rmKwhIconHeight = GetScaledHeight(6f);
            _rmKwhIcon = new UIImageView(new CGRect(rmKwhIconXPos, rmKwhIconYPos, rmKwhIconWidth, rmKwhIconHeight))
            {
                Image = UIImage.FromBundle(Constants.IMG_RMKwHDropdownIcon)
            };
            _rmKwhSelectionView.AddSubview(_rmKwhIcon);
        }

        private void CreateTariffView()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat tariffWidth = GetScaledWidth(108f);
            nfloat tariffHeight = GetScaledHeight(24f);
            nfloat tariffXPos = width - tariffWidth - BaseMarginWidth16;
            _tariffSelectionView = new UIView(new CGRect(tariffXPos, 0, tariffWidth, tariffHeight))
            {
                BackgroundColor = UIColor.White,
            };
            _tariffSelectionView.Layer.CornerRadius = GetScaledHeight(13f);
            _containerView.AddSubview(_tariffSelectionView);

            nfloat tarrifIconXPos = GetScaledWidth(12f);
            nfloat tarrifIconYPos = GetScaledHeight(4f);
            nfloat tariffIconWidth = GetScaledWidth(16f);
            nfloat tariffIconHeight = GetScaledHeight(16f);
            _tariffIcon = new UIImageView(new CGRect(tarrifIconXPos, tarrifIconYPos, tariffIconWidth, tariffIconHeight))
            {
                Image = UIImage.FromBundle(Constants.IMG_TariffEyeOpenIcon)
            };
            _tariffSelectionView.AddSubview(_tariffIcon);

            nfloat tariffLabelXPos = GetScaledWidth(32f);
            nfloat tariffLabelYPos = GetScaledHeight(4f);
            nfloat tariffLabelWidth = GetScaledWidth(64f);
            nfloat tariffLabelHeight = GetScaledHeight(16f);
            _tariffLabel = new UILabel(new CGRect(tariffLabelXPos, tariffLabelYPos, tariffLabelWidth, tariffLabelHeight))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Text = LanguageUtility.GetCommonI18NValue(Constants.I18N_ShowTariff)
            };
            _tariffSelectionView.AddSubview(_tariffLabel);
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }
        #region RMKWH Methods
        public void SetGestureRecognizerFoRMKwH(UITapGestureRecognizer recognizer)
        {
            _rmKwhSelectionView.AddGestureRecognizer(recognizer);
        }

        public void SetRMkWhLabel(RMkWhEnum rMkWhEnum)
        {
            _rmKwhLabel.Text = UsageHelper.GetRMkWhValueStringForEnum(rMkWhEnum);
            UpdateFrameForSelection(rMkWhEnum);
        }

        private void UpdateFrameForSelection(RMkWhEnum rMkWhEnum)
        {
            switch (rMkWhEnum)
            {
                case RMkWhEnum.RM:
                    ViewHelper.AdjustFrameSetX(_rmKwhLabel, GetScaledWidth(13f));
                    ViewHelper.AdjustFrameSetWidth(_rmKwhLabel, GetScaledWidth(19f));
                    break;
                case RMkWhEnum.kWh:
                    ViewHelper.AdjustFrameSetX(_rmKwhLabel, GetScaledWidth(10f));
                    ViewHelper.AdjustFrameSetWidth(_rmKwhLabel, GetScaledWidth(25f));
                    break;
            }
        }
        #endregion
        #region TARIFF Methods
        public void SetGestureRecognizerForTariff(UITapGestureRecognizer recognizer)
        {
            _tariffSelectionView.AddGestureRecognizer(recognizer);
        }

        public void UpdateTariffButton(bool showTariff)
        {
            _tariffIcon.Image = showTariff ? UIImage.FromBundle(Constants.IMG_TariffEyeCloseIcon) : UIImage.FromBundle(Constants.IMG_TariffEyeOpenIcon);
            _tariffLabel.Text = showTariff ? LanguageUtility.GetCommonI18NValue(Constants.I18N_HideTariff) : LanguageUtility.GetCommonI18NValue(Constants.I18N_ShowTariff);
        }

        public void SetTariffButtonDisable(bool isDisable)
        {
            isTariffDisabled = isDisable;
            _tariffIcon.Image = isDisable ? UIImage.FromBundle(Constants.IMG_TariffEyeDisableIcon) : UIImage.FromBundle(Constants.IMG_TariffEyeCloseIcon);
            _tariffLabel.TextColor = isDisable ? MyTNBColor.SilverChalice : MyTNBColor.WaterBlue;
        }
        #endregion
    }
}
