using System;
using System.Diagnostics;
using System.Globalization;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class DashboardHomeAccountCard : BaseComponent
    {
        AccountsCardContentViewController _contentViewController;
        private readonly UIView _parentView;
        UIView _accountCardView, _viewNickname, _viewAcctNo, _viewAmountDue, _viewDueDate;
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
            nfloat padding = BaseMarginWidth16;
            nfloat margin = BaseMarginWidth12;
            nfloat iconWidth = GetScaledHeight(28f);
            nfloat nickNameWidth = GetScaledWidth(150f);
            nfloat labelWidth = GetScaledWidth(100f);

            _accountCardView = new UIView(new CGRect(0, _yLocation, parentWidth, cardHeight))
            {
                BackgroundColor = UIColor.White
            };
            AddCardShadow(ref _accountCardView);

            _accountIcon = new UIImageView(new CGRect(margin, GetYLocationToCenterObject(iconWidth, _accountCardView), iconWidth, iconWidth))
            {
                Image = UIImage.FromBundle(_strAccountIcon ?? string.Empty)
            };

            _accountNickname = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + margin, GetYLocationToCenterObject(labelHeight * 2, _accountCardView), nickNameWidth, labelHeight))
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

            _amountDue = new UILabel(new CGRect(parentWidth - labelWidth - margin, GetYLocationToCenterObject(labelHeight * 2, _accountCardView), labelWidth, labelHeight))
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

            _viewNickname = new UIView(new CGRect(_accountNickname.Frame.X, GetYLocationToCenterObject(GetScaledHeight(28f), _accountCardView), _accountNo.Frame.Width, GetScaledHeight(14f)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewNickname.Layer.CornerRadius = GetScaledHeight(3f);

            _viewAcctNo = new UIView(new CGRect(_accountNo.Frame.X, _accountNickname.Frame.GetMaxY() + GetScaledHeight(6f), _accountNo.Frame.Width - BaseMarginWidth16, GetScaledHeight(8f)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewAcctNo.Layer.CornerRadius = GetScaledHeight(2f);

            _viewAmountDue = new UIView(new CGRect(_amountDue.Frame.X, GetYLocationToCenterObject(GetScaledHeight(28f), _accountCardView), _dueDate.Frame.Width, GetScaledHeight(14f)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewAmountDue.Layer.CornerRadius = GetScaledHeight(3f);

            _viewDueDate = new UIView(new CGRect(_dueDate.Frame.X + BaseMarginWidth16, _amountDue.Frame.GetMaxY() + GetScaledHeight(6f), _dueDate.Frame.Width - BaseMarginWidth16, GetScaledHeight(8f)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            _viewDueDate.Layer.CornerRadius = GetScaledHeight(2f);

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

            viewShimmerContent.AddSubviews(new UIView { _accountIcon, _accountNickname, _accountNo, _amountDue, _dueDate, _viewNickname, _viewAcctNo, _viewAmountDue, _viewDueDate });
            _accountCardView.AddSubview(viewShimmerParent);

            _accountCardView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                _contentViewController.OnAccountCardSelected(_model);
            }));

            OnUpdateWidget(_model);
        }

        private void OnUpdateWidget(DueAmountDataModel model)
        {
            _accountIcon.BackgroundColor = IsUpdating ? MyTNBColor.PaleGrey : UIColor.Clear;
            _accountIcon.Layer.CornerRadius = IsUpdating ? _accountIcon.Frame.Width / 2 : 0;
            _accountIcon.Image = IsUpdating ? null : UIImage.FromBundle(_strAccountIcon ?? string.Empty);

            _viewNickname.Hidden = !IsUpdating;
            _accountNickname.Text = IsUpdating ? string.Empty : _strNickname ?? string.Empty;

            _viewAcctNo.Hidden = !IsUpdating;
            _accountNo.Text = IsUpdating ? string.Empty : _strAccountNo ?? string.Empty;

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
                _viewAmountDue.Hidden = false;
                _viewDueDate.Hidden = false;
            }
            else
            {
                _viewAmountDue.Hidden = true;
                _viewDueDate.Hidden = true;
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
