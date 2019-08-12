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
            nfloat padding = 16f;
            nfloat margin = 16f;

            _accountCardView = new UIView(new CGRect(0, _yLocation + margin, parentWidth, 60f))
            {
                BackgroundColor = UIColor.White
            };
            AddCardShadow(ref _accountCardView);

            _accountIcon = new UIImageView(new CGRect(12f, DeviceHelper.GetCenterYWithObjHeight(28f, _accountCardView), 28f, 28f))
            {
                Image = UIImage.FromBundle(_strAccountIcon ?? string.Empty)
            };

            _accountNickname = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + 12f, 12f, 150f, 20f))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.GreyishBrown,
                Text = _strNickname ?? string.Empty
            };

            _accountNo = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + 12f, _accountNickname.Frame.GetMaxY(), 110f, 20f))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = _strAccountNo ?? string.Empty
            };

            _amountDue = new UILabel(new CGRect(parentWidth - 100f - 12f, 12f, 100f, 20f))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Right
            };

            _dueDate = new UILabel(new CGRect(parentWidth - 100f - 12f, _amountDue.Frame.GetMaxY(), 100f, 20f))
            {
                Font = MyTNBFont.MuseoSans12_300,
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
            _accountNickname.Frame = IsUpdating ? new CGRect(_accountNickname.Frame.X, 16, _accountNickname.Frame.Width, 14)
                : new CGRect(_accountNickname.Frame.X, 12, _accountNickname.Frame.Width, 20);
            _accountNickname.Text = IsUpdating ? string.Empty : _strNickname ?? string.Empty;

            _accountNo.BackgroundColor = IsUpdating ? MyTNBColor.PowderBlue : UIColor.Clear;
            _accountNo.Frame = IsUpdating ? new CGRect(_accountNo.Frame.X, _accountNickname.Frame.GetMaxY() + 6, _accountNo.Frame.Width, 8)
                : new CGRect(_accountNo.Frame.X, _accountNickname.Frame.GetMaxY(), _accountNo.Frame.Width, 20);
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
                _amountDue.BackgroundColor = MyTNBColor.PowderBlue;
                _amountDue.Frame = new CGRect(_amountDue.Frame.X, 16, _amountDue.Frame.Width, 14);
                _dueDate.BackgroundColor = MyTNBColor.PowderBlue;
                _dueDate.Frame = new CGRect(_dueDate.Frame.X, _amountDue.Frame.GetMaxY() + 6, _dueDate.Frame.Width, 8);

            }
            else
            {
                _amountDue.BackgroundColor = UIColor.Clear;
                _amountDue.Frame = new CGRect(_amountDue.Frame.X, 12, _amountDue.Frame.Width, 20);
                _dueDate.BackgroundColor = UIColor.Clear;
                _dueDate.Frame = new CGRect(_dueDate.Frame.X, _amountDue.Frame.GetMaxY(), _dueDate.Frame.Width, 20);

                if (model != null)
                {
                    var amount = !model.IsReAccount ? model.amountDue : ChartHelper.UpdateValueForRE(model.amountDue);
                    _amountDue.AttributedText = TextHelper.CreateValuePairString(amount.ToString("N2", CultureInfo.InvariantCulture)
                        , TNBGlobal.UNIT_CURRENCY + " ", true, MyTNBFont.MuseoSans14_500
                        , MyTNBColor.TunaGrey(), MyTNBFont.MuseoSans14_500, MyTNBColor.TunaGrey());
                    var dateString = amount > 0 ? model.billDueDate : string.Empty;
                    if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A"))
                    {
                        _dueDate.Text = "Dashboard_AllCleared".Translate();
                        _dueDate.TextColor = MyTNBColor.CharcoalGrey;
                        _amountDue.TextColor = MyTNBColor.Grey;
                    }
                    else
                    {
                        _dueDate.TextColor = MyTNBColor.CharcoalGrey;
                        _amountDue.TextColor = MyTNBColor.GreyishBrown;

                        string datePrefix = model.IsReAccount ? "Dashboard_GetBy".Translate() : "Dashboard_PayBy".Translate();
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
                        , datePrefix + " ", true, MyTNBFont.MuseoSans12_300
                        , MyTNBColor.CharcoalGrey, MyTNBFont.MuseoSans12_300, MyTNBColor.CharcoalGrey);
                    }
                }
            }
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue.CGColor;
            view.Layer.ShadowOpacity = 1;
            view.Layer.ShadowOffset = new CGSize(0, 0);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }
    }
}
