using System;
using CoreGraphics;
using myTNB.Enums;

using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class GreetingComponent
    {
        UIView parentView;
        UIView baseView;
        UILabel greetingMessage;
        UIImageView greetingImage;
        UILabel _lblMsg;
        UIButton _btnRefresh;

        public Action OnRefresh { get; set; }

        public GreetingComponent(UIView parent)
        {
            parentView = parent;
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        private void CreateComponent(bool isTimeOut = false)
        {
            var topMargin = 20f;
            greetingMessage = new UILabel
            {
                Frame = new CGRect(0, topMargin, parentView.Frame.Width, 50),
                Font = myTNBFont.MuseoSans16_500(),
                TextColor = myTNBColor.SunGlow(),
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

            var addtlHeight = 0f;
            if (isTimeOut)
            {
                _lblMsg = new UILabel
                {
                    Frame = new CGRect(25, greetingImage.Frame.GetMaxY() + 10f, parentView.Frame.Width - 50, 50),
                    Font = myTNBFont.MuseoSans12_300(),
                    TextColor = UIColor.White,
                    TextAlignment = UITextAlignment.Center,
                    Lines = 0,
                    Text = "Error_TimeOut".Translate(),
                    BackgroundColor = UIColor.Clear
                };

                _btnRefresh = new UIButton(UIButtonType.Custom)
                {
                    Frame = new CGRect(25, _lblMsg.Frame.GetMaxY(), parentView.Frame.Width - 50, 44f)
                };
                _btnRefresh.Layer.CornerRadius = 4;
                _btnRefresh.Layer.BorderColor = UIColor.White.CGColor;
                _btnRefresh.BackgroundColor = UIColor.Clear;
                _btnRefresh.Layer.BorderWidth = 1;
                _btnRefresh.SetTitle("Common_Refresh".Translate(), UIControlState.Normal);
                _btnRefresh.Font = myTNBFont.MuseoSans18_300();
                _btnRefresh.SetTitleColor(UIColor.White, UIControlState.Normal);
                _btnRefresh.TouchUpInside += (sender, e) =>
                {
                    OnRefresh?.Invoke();
                };

                addtlHeight = (float)(_lblMsg.Frame.Height + _btnRefresh.Frame.Height + topMargin);
            }

            baseView = new UIView(new CGRect(0, 0, parentView.Frame.Width
                , topMargin + greetingMessage.Frame.Height + greetingImage.Frame.Height + addtlHeight));

            baseView.AddSubview(greetingMessage);
            baseView.AddSubview(greetingImage);

            if (isTimeOut)
            {
                baseView.AddSubview(_lblMsg);
                baseView.AddSubview(_btnRefresh);
            }
        }

        /// <summary>
        /// Gets the user interface.
        /// </summary>
        /// <returns>The user interface.</returns>
        public UIView GetUI(bool isTimeOut = false)
        {
            CreateComponent(isTimeOut);
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