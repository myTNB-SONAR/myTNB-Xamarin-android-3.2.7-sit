using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class RefreshComponent
    {
        private string _image, _message, _btnTitle;
        private Action _action;
        private bool _isSolidCTABG;
        private nfloat _viewWidth;
        private string Event_Refresh = "Refresh";
        private UIView _refreshView, _parentView;

        public string PageName { set; private get; } = "Refresh";
        public bool IsPlannedDownTime { set; private get; } = false;
        public bool IsAutoAddView { set; private get; } = false;

        public RefreshComponent(string message = "", string btnTitle = ""
            , Action action = null, bool isSolidCTABG = true, string image = "")
        {
            _image = image;
            _message = message;
            _btnTitle = btnTitle;
            _action = action;
            _isSolidCTABG = isSolidCTABG;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            _refreshView = new UIView { BackgroundColor = UIColor.White };

            nfloat imgWidth = ScaleUtility.GetScaledWidth(IsPlannedDownTime ? 80 : 69);
            nfloat imgHeight = ScaleUtility.GetScaledWidth(64);

            UIImageView imgView = new UIImageView(new CGRect((width - imgWidth) / 2
                , ScaleUtility.GetScaledHeight(24), imgWidth, imgHeight))
            {
                Image = UIImage.FromBundle(_image.IsValid()
                    ? _image
                    : (IsPlannedDownTime ? Constants.IMG_IconPlannedDowntime : Constants.IMG_IconRefresh))
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

        public UIView GetUI(UIView parentView, bool isBanner = false)
        {
            _parentView = parentView;
            _viewWidth = _parentView.Frame.Width;
            if (isBanner)
            {
                CreateBannerComponent();
            }
            else
            {
                CreateComponent();
            }

            if (IsAutoAddView && _parentView != null && _refreshView != null)
            {
                _parentView.AddSubview(_refreshView);
                _parentView.SendSubviewToBack(_refreshView);
            }

            return _refreshView;
        }

        private void CreateBannerComponent()
        {
            _refreshView = new UIView { BackgroundColor = UIColor.White };
            UIImageView bannerImage = new UIImageView(new CGRect(0, 0, _viewWidth, _viewWidth * 0.76875F))
            {
                Image = UIImage.FromBundle(IsPlannedDownTime ? Constants.IMG_BannerPlannedDownTime : Constants.IMG_BannerRefresh)
            };
            UILabel lblMessage = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(32)
                , ScaleUtility.GetYLocationFromFrame(bannerImage.Frame, 16), _viewWidth - ScaleUtility.GetScaledWidth(64), 1000))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = MyTNBColor.Grey,
                Text = _message.IsValid() ? _message
                    : (IsPlannedDownTime ? LanguageUtility.GetErrorI18NValue(Constants.Error_PlannedDownTimeMessage)
                        : LanguageUtility.GetCommonI18NValue(Constants.Common_RefreshMessage))
            };
            nfloat lblHeight = lblMessage.GetLabelHeight(1000);
            lblMessage.Frame = new CGRect(lblMessage.Frame.Location, new CGSize(lblMessage.Frame.Width, lblHeight));

            _refreshView.AddSubviews(new UIView[] { bannerImage, lblMessage });

            if (!IsPlannedDownTime)
            {
                CustomUIButtonV2 btnRefresh = new CustomUIButtonV2
                {
                    EventName = Event_Refresh,
                    PageName = PageName,
                    Frame = new CGRect(ScaleUtility.GetScaledWidth(16)
                        , ScaleUtility.GetYLocationFromFrame(lblMessage.Frame, ScaleUtility.GetScaledHeight(16))
                        , _viewWidth - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(48)),
                    BackgroundColor = MyTNBColor.FreshGreen
                };

                btnRefresh.SetTitle(_btnTitle.IsValid() ? _btnTitle : LanguageUtility.GetCommonI18NValue(Constants.Common_RefreshNow), UIControlState.Normal);
                btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (_action != null)
                    {
                        _action.Invoke();
                    }
                }));
                _refreshView.AddSubview(btnRefresh);
            }
            _refreshView.Frame = new CGRect(0, 0, _viewWidth, _parentView.Frame.Height);
        }
    }
}