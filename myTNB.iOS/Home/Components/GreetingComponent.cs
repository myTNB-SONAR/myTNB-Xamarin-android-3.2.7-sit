using System;
using CoreGraphics;
using myTNB.Enums;
using myTNB.Extensions;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class GreetingComponent
    {
        UIView parentView;
        UIView baseView;
        UILabel greetingMessage;
        UIImageView greetingImage;

        public GreetingComponent(UIView parent)
        {
            parentView = parent;
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        private void CreateComponent()
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

            baseView = new UIView(new CGRect(0, 0, parentView.Frame.Width, topMargin + greetingMessage.Frame.Height + greetingImage.Frame.Height));

            baseView.AddSubview(greetingMessage);
            baseView.AddSubview(greetingImage);
        }

        /// <summary>
        /// Gets the user interface.
        /// </summary>
        /// <returns>The user interface.</returns>
        public UIView GetUI()
        {
            CreateComponent();
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
            switch(textMode)
            {
                case GreetingMode.Morning:
                    message = "GoodMorningGreeting".Translate();
                    break;
                case GreetingMode.Afternoon:
                    message = "GoodAfternoonGreeting".Translate();
                    break;
                case GreetingMode.Evening:
                    message = "GoodEveningGreeting".Translate();
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
