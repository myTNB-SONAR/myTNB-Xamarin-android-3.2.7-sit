﻿using System;
using CoreGraphics;
using myTNB.Enums;
using UIKit;

namespace myTNB
{
    public class TariffSelectionComponent : BaseComponent
    {
        CustomUIView _containerView;
        UIView _parentView, _rmKwhSelectionView, _tariffSelectionView, _monthDayView;
        UIImageView _tariffIcon, _rmKwhIcon, _monthDayIcon;
        UILabel _rmKwhLabel, _tariffLabel, _monthDayLabel;
        nfloat containerHeight = ScaleUtility.GetScaledHeight(24f);
        SmartMeterViewEnum _smViewEnum;
        public bool isTariffDisabled;

        public TariffSelectionComponent(UIView parentView, SmartMeterViewEnum smViewEnum = SmartMeterViewEnum.Month)
        {
            _parentView = parentView;
            _smViewEnum = smViewEnum;
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
            CreateMonthDayView();
            AdjustViews();
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
            nfloat tariffXPos;

            if (_smViewEnum == SmartMeterViewEnum.Month)
            {
                tariffXPos = width - tariffWidth - BaseMarginWidth16;
            }
            else
            {
                tariffXPos = _rmKwhSelectionView.Frame.GetMaxX() + GetScaledWidth(17f);
            }

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

        private void CreateMonthDayView()
        {
            if (_smViewEnum == SmartMeterViewEnum.Month)
                return;

            nfloat width = _parentView.Frame.Width;
            nfloat monthDayWidth = _smViewEnum == SmartMeterViewEnum.Day ? GetScaledWidth(87f) : GetScaledWidth(71f);
            nfloat monthDayHeight = GetScaledHeight(24f);
            nfloat monthDayXPos = width - monthDayWidth - BaseMarginWidth16;
            _monthDayView = new UIView(new CGRect(monthDayXPos, 0, monthDayWidth, monthDayHeight))
            {
                BackgroundColor = UIColor.White,
            };
            _monthDayView.Layer.CornerRadius = GetScaledHeight(13f);
            _containerView.AddSubview(_monthDayView);
            nfloat iconXPos = GetScaledWidth(12f);
            nfloat iconYPos = GetScaledHeight(4f);
            nfloat iconWidth = GetScaledWidth(16f);
            nfloat iconHeight = GetScaledHeight(16f);
            _monthDayIcon = new UIImageView(new CGRect(iconXPos, iconYPos, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(Constants.IMG_ArrowLeftBlueIcon)
            };
            _monthDayView.AddSubview(_monthDayIcon);
            nfloat labelXPos = GetScaledWidth(32f);
            nfloat labelYPos = GetScaledHeight(4f);
            nfloat labelWidth = GetScaledWidth(64f);
            nfloat labelHeight = GetScaledHeight(16f);
            _monthDayLabel = new UILabel(new CGRect(labelXPos, labelYPos, labelWidth, labelHeight))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Left,
                Text = GetMonthDayString()
            };
            _monthDayView.AddSubview(_monthDayLabel);
        }

        private string GetMonthDayString()
        {
            string str = string.Empty;
            switch (_smViewEnum)
            {
                case SmartMeterViewEnum.Day:
                    str = LanguageUtility.GetCommonI18NValue(Constants.I18N_Months);
                    break;
                case SmartMeterViewEnum.Hour:
                    str = LanguageUtility.GetCommonI18NValue(Constants.I18N_Days);
                    break;
            }
            return str;
        }

        private void AdjustViews()
        {
            if (_smViewEnum != SmartMeterViewEnum.Month)
            {
                nfloat newXPos = _monthDayView.Frame.GetMinX() - _tariffSelectionView.Frame.Width - BaseMarginWidth16;
                ViewHelper.AdjustFrameSetX(_tariffSelectionView, newXPos);
            }
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
