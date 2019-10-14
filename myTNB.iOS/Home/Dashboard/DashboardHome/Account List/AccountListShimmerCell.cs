using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountListShimmerCell : CustomUITableViewCell
    {
        private UIView _viewNickname, _viewAcctNo, _viewAmountDue, _viewDueDate;

        public AccountListShimmerCell(IntPtr handle) : base(handle)
        {
            _viewNickname = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(24F), GetScaledWidth(98F), GetScaledHeight(16F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewNickname.Layer.CornerRadius = GetScaledHeight(3f);

            _viewAcctNo = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_viewNickname.Frame, 4F), GetScaledWidth(81F), GetScaledHeight(16F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewAcctNo.Layer.CornerRadius = GetScaledHeight(3f);

            _viewAmountDue = new UIView(new CGRect(_cellWidth - GetScaledWidth(69F) - BaseMarginWidth16, GetScaledHeight(24F), GetScaledWidth(69F), GetScaledHeight(16F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewAmountDue.Layer.CornerRadius = GetScaledHeight(3f);

            _viewDueDate = new UIView(new CGRect(_cellWidth - GetScaledWidth(75F) - BaseMarginWidth16, GetYLocationFromFrame(_viewAmountDue.Frame, 4F), GetScaledWidth(75F), GetScaledHeight(16F)))
            {
                BackgroundColor = MyTNBColor.PaleGrey25
            };
            _viewDueDate.Layer.CornerRadius = GetScaledHeight(3f);

            UIView lineView = new UIView(new CGRect(BaseMarginWidth16, GetScaledHeight(76F), _cellWidth - GetScaledWidth(32), GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            AddSubview(lineView);

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
            AddSubview(viewShimmerParent);
        }
    }
}
