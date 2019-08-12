using System;
using UIKit;

namespace myTNB
{
    public class CustomUIButtonV2 : UIButton
    {
        public CustomUIButtonV2(bool isWhiteBG = false)
        {
            SetDefaultUIButton(isWhiteBG);
        }

        private void SetDefaultUIButton(bool isWhiteBG = false)
        {
            Layer.CornerRadius = 5.0F;
            Layer.BorderColor = UIColor.White.CGColor;
            Layer.BorderWidth = 1.0F;
#pragma warning disable XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            Font = MyTNBFont.MuseoSans16_500;
#pragma warning restore XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            if (isWhiteBG)
            {
                BackgroundColor = UIColor.White;
                SetTitleColor(MyTNBColor.WaterBlue, UIControlState.Normal);
            }
        }
    }
}