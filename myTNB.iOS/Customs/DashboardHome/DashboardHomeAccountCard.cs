using System;
using System.Diagnostics;
using System.Globalization;
using CoreGraphics;
using Facebook.Shimmer;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class DashboardHomeAccountCard
    {
        FBShimmeringView _shimmeringView = new FBShimmeringView();

        AccountsContentViewController _contentViewController;
        private readonly UIView _parentView;
        UIView _accountCardView;
        UIImageView _accountIcon;
        UILabel _accountNickname, _accountNo, _amountDue, _dueDate;
        string _strAccountIcon, _strNickname, _strAccountNo;
        nfloat _yLocation = 0f;
        DueAmountDataModel _model = new DueAmountDataModel();

        public DashboardHomeAccountCard(AccountsContentViewController controller, UIView parentView, nfloat yLocation)
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

            _accountCardView = new UIView(new CGRect(16f, _yLocation + margin, parentWidth - (padding * 2), 60f))
            {
                BackgroundColor = UIColor.White
            };
            _accountCardView.Layer.CornerRadius = 5f;
            AddCardShadow(ref _accountCardView);

            _accountIcon = new UIImageView(new CGRect(12f, DeviceHelper.GetCenterYWithObjHeight(28f, _accountCardView), 28f, 28f))
            {
                Image = UIImage.FromBundle(_strAccountIcon)
            };

            _accountNickname = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + 12f, 12f, 150f, 20f))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.GreyishBrown,
                Text = _strNickname
            };

            _accountNo = new UILabel(new CGRect(_accountIcon.Frame.GetMaxX() + 12f, _accountNickname.Frame.GetMaxY(), 110f, 20f))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.CharcoalGrey,
                Text = _strAccountNo
            };

            _amountDue = new UILabel(new CGRect(parentWidth - 100f - 12f - (16f * 2), 12f, 100f, 20f))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.GreyishBrown,
                TextAlignment = UITextAlignment.Right
            };

            _dueDate = new UILabel(new CGRect(parentWidth - 100f - 12f - (16f * 2), _amountDue.Frame.GetMaxY(), 100f, 20f))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Right
            };

            _accountCardView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                _contentViewController.OnAccountCardSelected(_model);
            }));

            _accountCardView.AddSubviews(new UIView { _accountIcon, _accountNickname, _accountNo, _amountDue, _dueDate });
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
            _model = model ?? new DueAmountDataModel();
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

        private void SetUIForLoading(bool isLoading)
        {
            _shimmeringView.Shimmering = isLoading;

            UIColor shimmerColor = isLoading ? UIColor.LightGray : UIColor.Clear;
            _accountIcon.BackgroundColor = shimmerColor;
            _accountNickname.BackgroundColor = shimmerColor;
            _accountNo.BackgroundColor = shimmerColor;
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
