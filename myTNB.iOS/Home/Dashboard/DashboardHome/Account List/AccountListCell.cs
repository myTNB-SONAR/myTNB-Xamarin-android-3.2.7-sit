using System;
using System.Diagnostics;
using System.Globalization;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class AccountListCell : CustomUITableViewCell
    {
        private UIView _viewDataCell, _viewShimmerCell, _viewNickname, _viewAcctNo, _viewAmountDue, _viewDueDate;
        private UILabel _nickname, _accountNumber, _amountDue, _dueDate;
        private UIImageView _imgRe;

        public AccountListCell(IntPtr handle) : base(handle)
        {
            _viewDataCell = new UIView(Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_viewDataCell);

            _nickname = new UILabel(new CGRect(BaseMarginWidth16, GetScaledHeight(12F), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _viewDataCell.AddSubview(_nickname);

            _imgRe = new UIImageView(new CGRect(0, GetScaledHeight(14F), GetScaledWidth(12F), GetScaledHeight(12F)))
            {
                Image = UIImage.FromBundle(DashboardHomeConstants.Img_RELeaf),
                Hidden = true
            };
            _viewDataCell.AddSubview(_imgRe);

            _accountNumber = new UILabel(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_nickname.Frame, 4F), (_cellWidth - GetScaledWidth(32F)) * 0.50F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = UIColor.FromWhiteAlpha(1, 0.60F)
            };
            _viewDataCell.AddSubview(_accountNumber);

            _amountDue = new UILabel(new CGRect(_cellWidth - BaseMarginWidth16 - (_cellWidth * 0.40F), GetScaledHeight(12F), 0, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right
            };
            _viewDataCell.AddSubview(_amountDue);

            _dueDate = new UILabel(new CGRect(_cellWidth - BaseMarginWidth16 - ((_cellWidth - GetScaledWidth(32F)) * 0.50F), GetYLocationFromFrame(_amountDue.Frame, 4F), (_cellWidth - GetScaledWidth(32F)) * 0.50F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = UIColor.FromWhiteAlpha(1, 0.60F),
                TextAlignment = UITextAlignment.Right
            };
            _viewDataCell.AddSubview(_dueDate);

            UIView lineView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_accountNumber.Frame, 12F), _cellWidth - GetScaledWidth(32), GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            _viewDataCell.AddSubview(lineView);
        }

        public void SetAccountCell(DueAmountDataModel model)
        {
            if (_viewDataCell != null)
            {
                _viewDataCell.Hidden = false;
            }

            if (_viewShimmerCell != null)
            {
                _viewShimmerCell.RemoveFromSuperview();
            }

            if (model != null)
            {
                _nickname.Text = model.accNickName;
                _accountNumber.Text = model.accNum;

                var amount = !model.IsReAccount ? model.amountDue : model.amountDue * -1;
                var absAmount = Math.Abs(amount);
                _amountDue.AttributedText = TextHelper.CreateValuePairString(absAmount.ToString("N2", CultureInfo.InvariantCulture)
                    , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_12_500
                    , UIColor.White, TNBFont.MuseoSans_12_500, UIColor.White);
                var dateString = amount > 0 ? model.billDueDate : string.Empty;
                if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A"))
                {
                    _dueDate.Text = amount < 0 ? model.IsReAccount ? GetI18NValue(DashboardHomeConstants.I18N_ReceivedExtra) : GetI18NValue(DashboardHomeConstants.I18N_PaidExtra) : GetI18NValue(DashboardHomeConstants.I18N_AllCleared);
                    _dueDate.TextColor = UIColor.FromWhiteAlpha(1, 0.60F);
                    _amountDue.TextColor = amount < 0 ? !model.IsReAccount ? MyTNBColor.LightGreenBlue : UIColor.White : UIColor.White;
                }
                else
                {
                    _dueDate.TextColor = UIColor.FromWhiteAlpha(1, 0.60F);
                    _amountDue.TextColor = UIColor.White;

                    string datePrefix = model.IsReAccount ? GetI18NValue(DashboardHomeConstants.I18N_GetBy) : GetI18NValue(DashboardHomeConstants.I18N_PayBy);
                    if (model.IsReAccount && model.IncrementREDueDateByDays > 0)
                    {
                        try
                        {
                            var format = @"dd/MM/yyyy";
                            DateTime due = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
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
                    , UIColor.FromWhiteAlpha(1, 0.60F), TNBFont.MuseoSans_12_300, UIColor.FromWhiteAlpha(1, 0.60F));
                }

                nfloat cellWidth = _cellWidth - GetScaledWidth(32F);
                CGSize amountDueSize = _amountDue.SizeThatFits(new CGSize(1000F, GetScaledHeight(16F)));
                ViewHelper.AdjustFrameSetWidth(_amountDue, amountDueSize.Width);
                ViewHelper.AdjustFrameSetX(_amountDue, _cellWidth - _amountDue.Frame.Width - BaseMarginWidth16);

                nfloat widthForNickName = cellWidth - _amountDue.Frame.Width;
                _imgRe.Hidden = !model.IsReAccount;

                if (model.IsReAccount)
                {
                    widthForNickName -= GetScaledWidth(18F);
                    CGSize nameSize = _nickname.SizeThatFits(new CGSize(widthForNickName, GetScaledHeight(16F)));
                    ViewHelper.AdjustFrameSetWidth(_nickname, nameSize.Width <= widthForNickName ? nameSize.Width : widthForNickName);
                    nfloat addtl = nameSize.Width <= widthForNickName ? GetScaledWidth(4F) : 0;
                    ViewHelper.AdjustFrameSetX(_imgRe, _nickname.Frame.GetMaxX() + addtl);
                }
                else
                {
                    CGSize nameSize = _nickname.SizeThatFits(new CGSize(widthForNickName, GetScaledHeight(16F)));
                    ViewHelper.AdjustFrameSetWidth(_nickname, nameSize.Width <= widthForNickName ? nameSize.Width : widthForNickName);
                }
            }
        }

        public void SetShimmerCell()
        {
            if (_viewDataCell != null)
            {
                _viewDataCell.Hidden = true;
            }

            if (_viewShimmerCell != null)
            {
                _viewShimmerCell.RemoveFromSuperview();
            }

            _viewShimmerCell = new UIView(Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_viewShimmerCell);

            _viewNickname = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(12F), GetScaledWidth(98F), GetScaledHeight(14F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewNickname.Layer.CornerRadius = GetScaledHeight(2F);

            _viewAcctNo = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_viewNickname.Frame, 8F), GetScaledWidth(81F), GetScaledHeight(14F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewAcctNo.Layer.CornerRadius = GetScaledHeight(2F);

            _viewAmountDue = new UIView(new CGRect(_cellWidth - GetScaledWidth(69F) - BaseMarginWidth16, GetScaledHeight(12F), GetScaledWidth(69F), GetScaledHeight(14F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewAmountDue.Layer.CornerRadius = GetScaledHeight(2F);

            _viewDueDate = new UIView(new CGRect(_cellWidth - GetScaledWidth(75F) - BaseMarginWidth16, GetYLocationFromFrame(_viewAmountDue.Frame, 8F), GetScaledWidth(75F), GetScaledHeight(14F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewDueDate.Layer.CornerRadius = GetScaledHeight(2F);

            UIView lineView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_viewAcctNo.Frame, 12F), _cellWidth - GetScaledWidth(32), GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            _viewShimmerCell.AddSubview(lineView);

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, _cellWidth, _cellHeight))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, _cellWidth, _cellHeight))
            { BackgroundColor = UIColor.Clear };
            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            viewShimmerContent.AddSubviews(new UIView { _viewNickname, _viewAcctNo, _viewAmountDue, _viewDueDate });
            _viewShimmerCell.AddSubview(viewShimmerParent);
        }
    }
}
