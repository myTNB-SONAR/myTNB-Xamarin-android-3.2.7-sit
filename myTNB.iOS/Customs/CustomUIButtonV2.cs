using System;
using UIKit;

namespace myTNB
{
    public class CustomUIButtonV2 : UIButton
    {
        public CustomUIButtonV2()
        {
            SetDefaultUIButton();
        }

        private void SetDefaultUIButton()
        {
            Layer.CornerRadius = 5.0F;
            Layer.BorderColor = UIColor.White.CGColor;
            Layer.BorderWidth = 1.0F;
#pragma warning disable XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            Font = MyTNBFont.MuseoSans16_500;
#pragma warning restore XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
        }
    }
}