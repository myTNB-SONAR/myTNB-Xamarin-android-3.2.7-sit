using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class RefreshComponent
    {
        private string _image;
        private string _message;
        private string _btnTitle;
        private Action _action;
        private bool _isSolidCTABG;

        public string PageName { set; private get; } = "Refresh";
        public bool IsPlannedDownTime { set; private get; } = false;
        private string Event_Refresh = "Refresh";

        public RefreshComponent(string message, string btnTitle = ""
            , Action action = null, bool isSolidCTABG = true, string image = "")
        {
            _image = image;
            _message = message;
            _btnTitle = btnTitle;
            _action = action;
            _isSolidCTABG = isSolidCTABG;
        }

        private UIView _refreshView;
        private UIView _parentView;

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            _refreshView = new UIView { BackgroundColor = UIColor.White };

            nfloat imgWidth = ScaleUtility.GetScaledWidth(IsPlannedDownTime ? 80 : 69);
            nfloat imgHeight = ScaleUtility.GetScaledWidth(64);

            UIImageView imgView = new UIImageView(new CGRect((width - imgWidth) / 2
                , ScaleUtility.GetScaledHeight(24), imgWidth, imgHeight))
            {
                Image = UIImage.FromBundle(string.IsNullOrEmpty(_image) || string.IsNullOrWhiteSpace(_image)
                    ? IsPlannedDownTime ? RefreshConstants.IMG_PlannedDowntime : RefreshConstants.IMG_RefreshIcon
                    : _image)
            };

            UILabel lblMessage = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(16)
                , imgView.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16), width - ScaleUtility.GetScaledWidth(32), 100))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = MyTNBColor.Grey,
                Text = _message.IsValid() ? _message
                    : IsPlannedDownTime ? LanguageUtility.GetErrorI18NValue(Constants.Error_PlannedDownTimeMessage)
                        : string.Empty
            };

            nfloat newHeight = lblMessage.GetLabelHeight(1000f);
            if (newHeight < ScaleUtility.GetScaledHeight(32))
            {
                newHeight = ScaleUtility.GetScaledHeight(32);
            }
            lblMessage.Frame = new CGRect(lblMessage.Frame.Location, new CGSize(lblMessage.Frame.Width, newHeight));
            _refreshView.AddSubviews(new UIView[] { imgView, lblMessage });

            CustomUIButtonV2 btnRefresh = new CustomUIButtonV2();

            if (IsPlannedDownTime)
            {
                btnRefresh.Frame = new CGRect(ScaleUtility.GetScaledWidth(16), lblMessage.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16)
               , width - ScaleUtility.GetScaledWidth(32), 0);
            }
            else
            {

                btnRefresh.Frame = new CGRect(ScaleUtility.GetScaledWidth(16), lblMessage.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16)
                , width - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(48));
                btnRefresh.EventName = Event_Refresh;
                btnRefresh.PageName = PageName;
                btnRefresh.SetTitle(_btnTitle, UIControlState.Normal);
                btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (_action != null)
                    {
                        _action.Invoke();
                    }
                }));
                if (_isSolidCTABG)
                {
                    btnRefresh.BackgroundColor = MyTNBColor.FreshGreen;
                }
                else
                {
                    btnRefresh.BackgroundColor = UIColor.White;
                    btnRefresh.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
                    btnRefresh.Layer.BorderWidth = 1;
                    btnRefresh.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
                }
            }
            _refreshView.AddSubview(btnRefresh);
            _refreshView.Frame = new CGRect(0, 0, width, btnRefresh.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(16));
        }

        public UIView GetUI(UIView parentView)
        {
            _parentView = parentView;
            CreateComponent();
            return _refreshView;
        }
    }
}