using System;
using CoreGraphics;
using Foundation;
using myTNB.Enums;
using myTNB.Model;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class GreetingComponent
    {
        readonly UIView parentView;
        UIView baseView;
        UILabel greetingMessage;
        UIImageView greetingImage;
        UILabel _lblMsg;
        UIButton _btnRefresh;
        UIImageView _imgViewRefreshIcon;

        public Action OnRefresh { get; set; }

        public GreetingComponent(UIView parent)
        {
            parentView = parent;
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        private void CreateComponent(bool isTimeOut = false, BaseModel baseModelResponse = null)
        {
            if (isTimeOut)
            {
                float imageTopMargin = 24f;
                float iconWidth = DeviceHelper.GetScaledWidth(96f);
                float iconHeight = DeviceHelper.GetScaledHeight(96f);
                nfloat bottomMargin = DeviceHelper.GetScaledHeight(24f);
                nfloat lineTextHeight = 24f;
                nfloat labelWidth = parentView.Frame.Width - 50;

                _imgViewRefreshIcon = new UIImageView()
                {
                    Frame = new CGRect(DeviceHelper.GetCenterXWithObjWidth(iconWidth, parentView), DeviceHelper.GetScaledHeightWithY(imageTopMargin), iconWidth, iconHeight),
                    Image = UIImage.FromBundle("Refresh-Error-White")
                };

                var descMsg = !string.IsNullOrWhiteSpace(baseModelResponse?.RefreshMessage) ? baseModelResponse?.RefreshMessage : "Error_TimeOut".Translate();
                var btnText = !string.IsNullOrWhiteSpace(baseModelResponse?.RefreshBtnText) ? baseModelResponse?.RefreshMessage : "Error_RefreshBtnTitle".Translate();

                NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Center,
                    MinimumLineHeight = lineTextHeight,
                    MaximumLineHeight = lineTextHeight
                };

                UIStringAttributes msgAttributes = new UIStringAttributes
                {
                    Font = MyTNBFont.MuseoSans16_300,
                    ForegroundColor = UIColor.White,
                    BackgroundColor = UIColor.Clear,
                    ParagraphStyle = msgParagraphStyle
                };

                var attributedText = new NSMutableAttributedString(descMsg);
                attributedText.AddAttributes(msgAttributes, new NSRange(0, descMsg.Length));

                _lblMsg = new UILabel()
                {
                    AttributedText = attributedText,
                    Lines = 0
                };

                CGSize cGSize = _lblMsg.SizeThatFits(new CGSize(labelWidth, 1000f));
                _lblMsg.Frame = new CGRect(14f, _imgViewRefreshIcon.Frame.GetMaxY() + 24f, labelWidth, cGSize.Height);

                _btnRefresh = new UIButton(UIButtonType.Custom)
                {
                    Frame = new CGRect(25, _lblMsg.Frame.GetMaxY() + 16f, parentView.Frame.Width - 50, 48f)
                };
                _btnRefresh.Layer.CornerRadius = 4;
                _btnRefresh.BackgroundColor = UIColor.White;
                _btnRefresh.SetTitle(btnText, UIControlState.Normal);
                _btnRefresh.Font = MyTNBFont.MuseoSans16_500;
                _btnRefresh.SetTitleColor(MyTNBColor.PowerBlue, UIControlState.Normal);
                _btnRefresh.TouchUpInside += (sender, e) =>
                {
                    OnRefresh?.Invoke();
                };

                baseView = new UIView(new CGRect(0, 0, parentView.Frame.Width
                , _btnRefresh.Frame.GetMaxY() + bottomMargin));

                baseView.AddSubview(_imgViewRefreshIcon);
                baseView.AddSubview(_lblMsg);
                baseView.AddSubview(_btnRefresh);
            }
            else
            {
                nfloat topMargin = 20f;
                greetingMessage = new UILabel
                {
                    Frame = new CGRect(0, topMargin, parentView.Frame.Width, 50),
                    Font = MyTNBFont.MuseoSans16_500,
                    TextColor = MyTNBColor.SunGlow,
                    TextAlignment = UITextAlignment.Center,
                    Lines = 0,
                };

                nfloat origImageRatio = 117.0f / 320.0f; // height / width
                nfloat imageHeight = parentView.Frame.Width * origImageRatio;
                greetingImage = new UIImageView
                {
                    Frame = new CGRect(0, greetingMessage.Frame.GetMaxY() + 1, parentView.Frame.Width, imageHeight),
                    ContentMode = UIViewContentMode.ScaleAspectFit,
                };

                baseView = new UIView(new CGRect(0, 0, parentView.Frame.Width
                , topMargin + greetingMessage.Frame.Height + greetingImage.Frame.Height));

                baseView.AddSubview(greetingMessage);
                baseView.AddSubview(greetingImage);
            }
        }

        /// <summary>
        /// Gets the user interface.
        /// </summary>
        /// <returns>The user interface.</returns>
        public UIView GetUI(bool isTimeOut = false, BaseModel baseModelResponse = null)
        {
            CreateComponent(isTimeOut, baseModelResponse);
            return baseView;
        }

        /// <summary>
        /// Sets the mode.
        /// </summary>
        /// <param name="textMode">Text mode.</param>
        /// <param name="imageMode">Image mode.</param>
        /// <param name="customerName">Customer name.</param>
        public void SetMode(GreetingMode textMode, GreetingMode imageMode, string customerName)
        {
            string message = string.Empty;
            switch (textMode)
            {
                case GreetingMode.Morning:
                    message = "Component_GreetingMorning".Translate();
                    break;
                case GreetingMode.Afternoon:
                    message = "Component_GreetingAfternoon".Translate();
                    break;
                case GreetingMode.Evening:
                    message = "Component_GreetingEvening".Translate();
                    break;
            }

            greetingMessage.Text = string.Format("{0},{1}{2}", message, Environment.NewLine, customerName);

            switch (imageMode)
            {
                case GreetingMode.Morning:
                    greetingImage.Image = UIImage.FromBundle("Greeting-Morning");
                    break;
                case GreetingMode.Afternoon:
                    greetingImage.Image = UIImage.FromBundle("Greeting-Afternoon");
                    break;
                case GreetingMode.Evening:
                    greetingImage.Image = UIImage.FromBundle("Greeting-Evening");
                    break;
            }
        }
    }
}