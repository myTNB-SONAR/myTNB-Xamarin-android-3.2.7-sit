using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;

namespace myTNB.AndroidApp.Src.Utils.Custom.ProgressButton
{
    public class ResendButton : Android.Widget.Button
    {

        private float CornerRadius = 10f;
        private GradientDrawable ProgressDrawable;
        private GradientDrawable NormalDrawable;
        private int _MinProgress = 0;
        private int _MaxProgress = 100;
        private float _CurrentProgress = 0;

        private int _Counter = 0;

        public int MinProgress
        {
            get
            {
                return this._MinProgress;
            }
            set
            {
                this._MinProgress = value;
            }
        }

        public int MaxProgress
        {
            get
            {
                return this._MaxProgress;
            }
            set
            {
                this._MaxProgress = value;
            }
        }

        public float CurrentProgress
        {
            get
            {
                return this._CurrentProgress;
            }
            set
            {
                this._CurrentProgress = value;
            }
        }

        public int Counter
        {
            get
            {
                return this._Counter;
            }
            set
            {
                this._Counter = value;
            }
        }

        public ResendButton(Context context) : base(context)
        {
            Init(context, null);
        }

        public ResendButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context, attrs);
        }

        public ResendButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context, attrs);
        }

        public ResendButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(context, attrs);
        }



        private void Init(Context context, IAttributeSet attrs)
        {
            ProgressDrawable = (GradientDrawable)Resources.GetDrawable(Resource.Drawable.resend_inactive_progress).Mutate();
            ProgressDrawable.SetColor(Resources.GetColor(Resource.Color.resend_progress));
            ProgressDrawable.SetCornerRadius(CornerRadius);

            NormalDrawable = (GradientDrawable)Resources.GetDrawable(Resource.Drawable.resend_inactive_with_border_progress).Mutate();
            NormalDrawable.SetColor(Resources.GetColor(Resource.Color.white));
            NormalDrawable.SetCornerRadius(CornerRadius);

            //setBackgroundCompat(NormalDrawable);

        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {

            DrawProgress(canvas);
            setBackgroundCompat(ProgressDrawable);
            setBackgroundCompat(NormalDrawable);
            base.OnDraw(canvas);
        }


        private void DrawProgress(Canvas canvas)
        {
            float scale = (float)CurrentProgress / (float)MaxProgress;
            float indicatorWidth = (float)MeasuredWidth * scale;
            ProgressDrawable.SetBounds(2, 2, (int)indicatorWidth - 2, MeasuredHeight - 2);
            ProgressDrawable.Draw(canvas);
        }


        public void SetProgress(float newProgress)
        {
            this.CurrentProgress = newProgress;
            this.Invalidate();
        }

        /**
* Set the View's background. Masks the API changes made in Jelly Bean.
*
* @param drawable
*/
        public void setBackgroundCompat(Drawable drawable)
        {
            int pL = PaddingLeft;
            int pT = PaddingTop;
            int pR = PaddingRight;
            int pB = PaddingBottom;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {
                Background = drawable;
            }
            else
            {
                Background = drawable;
            }
            SetPadding(pL, pT, pR, pB);
        }

        public override IParcelable OnSaveInstanceState()
        {
            IParcelable superState = base.OnSaveInstanceState();
            SavedState savedState = new SavedState(superState);
            savedState.mProgress = CurrentProgress;

            return base.OnSaveInstanceState();
        }
        public override void OnRestoreInstanceState(IParcelable state)
        {
            if (state is SavedState)
            {
                SavedState savedState = (SavedState)state;
                CurrentProgress = savedState.mProgress;
                base.OnRestoreInstanceState(savedState.SuperState);
                SetProgress(CurrentProgress);
            }
            else
            {
                base.OnRestoreInstanceState(state);
            }
        }
        /**
     * A {@link android.os.Parcelable} representing the {@link com.dd.processbutton.ProcessButton}'s
     * state.
     */
        public class SavedState : BaseSavedState
        {

            public float mProgress;

            public SavedState(IParcelable parcel) : base(parcel)
            {
            }

            private SavedState(Parcel ins) : base(ins)
            {
                mProgress = ins.ReadInt();
            }
            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteFloat(mProgress);
            }
        }

    }
}
