using System;
using System.Diagnostics;
using System.Globalization;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class DashboardHomeAccountCard
    {
        AccountsCardContentViewController _contentViewController;
        private readonly UIView _parentView;
        UIView _accountCardView;
        UIImageView _accountIcon;
        UILabel _accountNickname, _accountNo, _amountDue, _dueDate;
        string _strAccountIcon, _strNickname, _strAccountNo;
        nfloat _yLocation = 0f;
        DueAmountDataModel _model = new DueAmountDataModel();

        public bool IsUpdating { set; get; }

        nfloat labelHeight = ScaleUtility.GetScaledHeight(20f);
        nfloat cardHeight = ScaleUtility.GetScaledHeight(60f);

        public DashboardHomeAccountCard(AccountsCardContentViewController controller, UIView parentView, nfloat yLocation)
        {
            _contentViewController = controller;
            _parentView = parentView;
            _yLocation = yLocation;
        }

        private void CreateComponent()
        {
            nfloat parentHeight = _parentView.Frame.Height;
            nfloat parentWidth = _parentView.Frame.Width;
            nfloat padding = ScaleUtility.BaseMarginWidth16;
            nfloat margin = ScaleUtility.BaseMarginWidth12;
            nfloat iconWidth = ScaleUtility.GetScaledHeight(28f);
            nfloat nickNameWidth = ScaleUtility.GetScaledWidth(150f);
            nfloat labelWidth = ScaleUtility.GetScaledWidth(100f);

            _accountCardView = new UIView(new CGRect(0, _yLocation, parentWidth, cardHeight))
            {
                BackgroundColor = UIColor.White
            };
            AddCardShadow(ref _accountCardView);

            _accountIcon = new UIImageView(new CGRect(margin, ScaleUtility.GetYLocationToCenterObject(iconWidth, _accountCardView), iconWidth, iconWidth))
            {
                Image = UIImage.FromBundle(_strAccountIcon ?? string.Empty)
            };

            _accountNickname = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + margin, ScaleUtility.GetYLocationToCenterObject(labelHeight * 2, _accountCardView), nickNameWidth, labelHeight))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.GreyishBrown,
                Text = _strNickname ?? string.Empty
            };

            _accountNo = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + margin, _accountNickname.Frame.GetMaxY(), labelWidth, labelHeight))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = _strAccountNo ?? string.Empty
            };

            _amountDue = new UILabel(new CGRect(parentWidth - labelWidth - margin, ScaleUtility.GetYLocationToCenterObject(labelHeight * 2, _accountCardView), labelWidth, labelHeight))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Right
            };

            _dueDate = new UILabel(new CGRect(parentWidth - labelWidth - margin, _amountDue.Frame.GetMaxY(), labelWidth, labelHeight))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Right
            };

            OnUpdateWidget(_model);
            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, _accountCardView.Frame.Width
                , _accountCardView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, _accountCardView.Frame.Width
                , _accountCardView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = IsUpdating;
            shimmeringView.SetValues();

            viewShimmerContent.AddSubviews(new UIView { _accountIcon, _accountNickname, _accountNo, _amountDue, _dueDate });
            _accountCardView.AddSubview(viewShimmerParent);

            _accountCardView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                _contentViewController.OnAccountCardSelected(_model);
            }));
        }

        private void OnUpdateWidget(DueAmountDataModel model)
        {
            _accountIcon.BackgroundColor = IsUpdating ? MyTNBColor.PowderBlue : UIColor.Clear;
            _accountIcon.Layer.CornerRadius = IsUpdating ? _accountIcon.Frame.Width / 2 : 0;
            _accountIcon.Image = IsUpdating ? null : UIImage.FromBundle(_strAccountIcon ?? string.Empty);

            _accountNickname.BackgroundColor = IsUpdating ? MyTNBColor.PowderBlue : UIColor.Clear;
            _accountNickname.Frame = IsUpdating ? new CGRect(_accountNickname.Frame.X, ScaleUtility.GetYLocationToCenterObject(ScaleUtility.GetScaledHeight(28f), _accountCardView), _accountNo.Frame.Width, ScaleUtility.GetScaledHeight(14f))
                : new CGRect(_accountNickname.Frame.X, ScaleUtility.GetYLocationToCenterObject(labelHeight * 2, _accountCardView), _accountNickname.Frame.Width, labelHeight);
            _accountNickname.Text = IsUpdating ? string.Empty : _strNickname ?? string.Empty;
            _accountNickname.Layer.CornerRadius = IsUpdating ? ScaleUtility.GetScaledHeight(10f) : 0f;

            _accountNo.BackgroundColor = IsUpdating ? MyTNBColor.PowderBlue : UIColor.Clear;
            _accountNo.Frame = IsUpdating ? new CGRect(_accountNo.Frame.X, _accountNickname.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(6f), _accountNo.Frame.Width - ScaleUtility.BaseMarginWidth16, ScaleUtility.GetScaledHeight(8f))
                : new CGRect(_accountNo.Frame.X, _accountNickname.Frame.GetMaxY(), _accountNo.Frame.Width, labelHeight);
            _accountNo.Text = IsUpdating ? string.Empty : _strAccountNo ?? string.Empty;
            _accountNo.Layer.CornerRadius = IsUpdating ? ScaleUtility.GetScaledHeight(10f) : 0f;

            AdjustLabels(model);
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

        public void SetModel(DueAmountDataModel model)
        {
            _model = model;
        }

        public void SetAccountIcon(string text)
        {
            _strAccountIcon = text ?? string.Empty;
        }

        public void SetNickname(string text)
        {
            _strNickname = text ?? string.Empty;
        }

        public void SetAccountNo(string text)
        {
            _strAccountNo = text ?? string.Empty;
        }

        public void SetTapAccountCardEvent(UITapGestureRecognizer tapGesture)
        {
            _accountCardView.AddGestureRecognizer(tapGesture);
        }

        public void AdjustLabels(DueAmountDataModel model)
        {
            if (IsUpdating)
            {
                _amountDue.Text = string.Empty;
                _dueDate.Text = string.Empty;
                _amountDue.BackgroundColor = MyTNBColor.PowderBlue;
                _amountDue.Frame = new CGRect(_amountDue.Frame.X, ScaleUtility.GetYLocationToCenterObject(ScaleUtility.GetScaledHeight(28f), _accountCardView), _dueDate.Frame.Width, ScaleUtility.GetScaledHeight(14f));
                _amountDue.Layer.CornerRadius = ScaleUtility.GetScaledHeight(10f);
                _dueDate.BackgroundColor = MyTNBColor.PowderBlue;
                _dueDate.Frame = new CGRect(_dueDate.Frame.X + ScaleUtility.BaseMarginWidth16, _amountDue.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(6f), _dueDate.Frame.Width - ScaleUtility.BaseMarginWidth16, ScaleUtility.GetScaledHeight(8f));
                _dueDate.Layer.CornerRadius = ScaleUtility.GetScaledHeight(10f);
            }
            else
            {
                _amountDue.BackgroundColor = UIColor.Clear;
                _amountDue.Frame = new CGRect(_amountDue.Frame.X, ScaleUtility.GetYLocationToCenterObject(labelHeight * 2, _accountCardView), _amountDue.Frame.Width, labelHeight);
                _amountDue.Layer.CornerRadius = 0f;
                _dueDate.BackgroundColor = UIColor.Clear;
                _dueDate.Frame = new CGRect(_dueDate.Frame.X, _amountDue.Frame.GetMaxY(), _dueDate.Frame.Width, labelHeight);
                _dueDate.Layer.CornerRadius = 0f;
                if (model != null)
                {
                    var amount = !model.IsReAccount ? model.amountDue : ChartHelper.UpdateValueForRE(model.amountDue);
                    var absAmount = Math.Abs(amount);
                    _amountDue.AttributedText = TextHelper.CreateValuePairString(absAmount.ToString("N2", CultureInfo.InvariantCulture)
                        , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_14_500
                        , MyTNBColor.TunaGrey(), TNBFont.MuseoSans_14_500, MyTNBColor.TunaGrey());
                    var dateString = amount > 0 ? model.billDueDate : string.Empty;
                    if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A"))
                    {
                        _dueDate.Text = amount < 0 ? _contentViewController.GetI18NValue(DashboardHomeConstants.I18N_PaidExtra) : _contentViewController.GetI18NValue(DashboardHomeConstants.I18N_AllCleared);
                        _dueDate.TextColor = MyTNBColor.CharcoalGrey;
                        _amountDue.TextColor = amount < 0 ? MyTNBColor.AlgaeGreen : MyTNBColor.Grey;
                    }
                    else
                    {
                        _dueDate.TextColor = MyTNBColor.CharcoalGrey;
                        _amountDue.TextColor = MyTNBColor.GreyishBrown;

                        string datePrefix = model.IsReAccount ? _contentViewController.GetI18NValue(DashboardHomeConstants.I18N_GetBy) : _contentViewController.GetI18NValue(DashboardHomeConstants.I18N_PayBy);
                        if (model.IsReAccount && model.IncrementREDueDateByDays > 0)
                        {
                            try
                            {
                                var format = @"dd/MM/yyyy";
                                DateTime due = DateTime.ParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture);
                                due = due.AddDays(model.IncrementREDueDateByDays);
                                dateString = due.ToString(format);
                            }
                            catch (FormatException)
                            {
                                Debug.WriteLine("Unable to parse '{0}'", dateString);
                            }
                        }
                        string formattedDate = DateHelper.GetFormattedDate(dateString, "dd MMM");
                        _dueDate.AttributedText = TextHelper.CreateValuePairString(formattedDate
                        , datePrefix + " ", true, TNBFont.MuseoSans_12_300
                        , MyTNBColor.CharcoalGrey, TNBFont.MuseoSans_12_300, MyTNBColor.CharcoalGrey);
                    }
                }
            }
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = .5f;
            view.Layer.ShadowOffset = new CGSize(0, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
