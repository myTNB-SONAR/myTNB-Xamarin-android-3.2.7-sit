using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    /// <summary>
    /// Line chart component.
    /// </summary>
    public class LineChartComponent : UIView
    {
        readonly List<CGPoint> _points;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:myTNB.Dashboard.DashboardComponents.LineChartComponent"/> class.
        /// </summary>
        /// <param name="points">Points.</param>
        public LineChartComponent(List<CGPoint> points)
        {
            _points = points;
            Opaque = false;
        }

        /// <summary>
        /// Draw the specified rect.
        /// </summary>
        /// <param name="rect">Rect.</param>
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (_points?.Count < 0)
            {
                return;
            }
            float lineWidth = 2.0f;
            float circleDiameter = 5.0f;

            //get graphics context
            using (CGContext g = UIGraphics.GetCurrentContext())
            {

                //set up drawing attributes

                UIColor.White.SetStroke();
                UIColor.Yellow.SetFill();
#if true
                var path = new UIBezierPath();
                path.MoveTo(_points[0]);
                for (int i = 1; i < _points.Count; i++)
                {
                    path.AddLineTo(_points[i]);
                }
                path.LineWidth = lineWidth;
                path.Stroke();

                for (int i = 0; i < _points.Count; i++)
                {
                    var point = _points[i];
                    point.X -= circleDiameter / 2;
                    point.Y -= circleDiameter / 2;

                    var circle = UIBezierPath.FromOval(new CGRect(point, new CGSize(circleDiameter, circleDiameter)));
                    circle.Fill();
                }

                //g.SaveState();
#else // CG
                g.SetLineWidth(3);
                //create geometry
                var path = new CGPath();

                path.AddLines(_points.ToArray());

                //path.CloseSubpath();

                //add geometry to graphics context and draw it
                g.AddPath(path);
                g.DrawPath(CGPathDrawingMode.FillStroke);
#endif
            }
        }
    }
}
