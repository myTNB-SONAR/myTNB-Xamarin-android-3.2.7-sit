using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class CameraCropOverlay : ImageView
    {
        public CameraCropOverlay(Context context) : base(context)
        {
        }

        public CameraCropOverlay(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            Paint paint = new Paint(PaintFlags.AntiAlias);
            paint.Color = Resources.GetColor(Resource.Color.black);
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawPaint(paint);

            //Draw transparent shape
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
            //int canvasW = Width;
            //int canvasH = Height;
            //Point centerOfCanvas = new Point(canvasW / 2, canvasH / 2);
            //int rectW = 100;
            //int rectH = 100;
            //int left = centerOfCanvas.X- (rectW / 2);
            //int top = centerOfCanvas.Y - (rectH / 2);
            //int right = centerOfCanvas.X + (rectW / 2);
            //int bottom = centerOfCanvas.Y + (rectH / 2);
            Rect rect = new Rect(100, 100, 200, 200);
            canvas.DrawRect(rect,paint);
        }
    }
}
