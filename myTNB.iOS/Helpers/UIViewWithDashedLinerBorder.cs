using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class UIViewWithDashedLinerBorder : UIView
    {
        public UIViewWithDashedLinerBorder()
        {
        }
        /// <summary>
        /// Draw the specified CGRect.
        /// </summary>
        /// <returns>The draw.</returns>
        /// <param name="rect">Rect.</param>
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            var path = UIBezierPath.FromRoundedRect(rect, 0);

            //MyTNBColor.SectionGrey.SetFill();
            UIColor.White.SetFill();
            path.Fill();

            UIColor.LightGray.SetStroke();
            path.LineWidth = 3;

            nfloat[] dashPattern = new nfloat[] { 2, 2 };
            path.SetLineDash(dashPattern, 0);
            path.Stroke();
        }
    }
}