using Android.Widget;
using Android.Animation;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Content;
using Android.Util;
using System;
using myTNB.AndroidApp.Src.Utils;
using Android.OS;

namespace myTNB.AndroidApp.Src.RearrangeAccount.MVP
{
    public class RearrangeAccountListView : ListView, ITypeEvaluator, GestureDetector.IOnGestureListener, AbsListView.IOnScrollListener
    {
        bool _reorderingEnabled = true;

        public bool ReorderingEnabled
        {
            get
            {
                return _reorderingEnabled;
            }
            set
            {
                if (!value)
                {
                    ItemLongClick -= HandleItemLongClick;
                }
                else
                {
                    ItemLongClick += HandleItemLongClick;
                }
                _reorderingEnabled = value;
            }
        }

        const int SMOOTH_SCROLL_AMOUNT_AT_EDGE = 60;

        int mLastEventY = -1;

        int mDownY = -1;
        int mDownX = -1;

        int mTotalOffset = 0;

        bool mCellIsMobile = false;
        private bool mIsMobileScrolling = false;
        private int mSmoothScrollAmountAtEdge = 0;

        const int INVALID_ID = -1;
        long mAboveItemId = INVALID_ID;
        long mMobileItemId = INVALID_ID;
        long mBelowItemId = INVALID_ID;

        View mobileView;
        Rect mHoverCellCurrentBounds;
        Rect mHoverCellOriginalBounds;

        const int INVALID_POINTER_ID = -1;
        int mActivePointerId = INVALID_POINTER_ID;

        private bool mIsWaitingForScrollFinish = false;
        private int mScrollState = (int) ScrollState.Idle;
        private const int mPointerIndexMask = (int)MotionEventActions.PointerIndexMask;
        private const int mPointerIndexShift = (int)MotionEventActions.PointerIndexShift;

        BitmapDrawable mHoverCell;
        GestureDetector dectector;
        Context mContext;

        private int mPreviousFirstVisibleItem = -1;
        private int mPreviousVisibleItemCount = -1;
        private int mCurrentFirstVisibleItem;
        private int mCurrentVisibleItemCount;
        private int mCurrentScrollState;
        private bool needToStop = false;

        ///
        /// Constructors
        ///
        public RearrangeAccountListView(Context context) : base(context)
        {
            init(context);
        }

        public RearrangeAccountListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            init(context);
        }

        public RearrangeAccountListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init(context);
        }

        public void init(Context context)
        {
            //	the detector handles all the gestures
            mContext = context;
            dectector = new GestureDetector(this);
            ItemLongClick += HandleItemLongClick;
            SetOnScrollListener(this);
            mSmoothScrollAmountAtEdge = (int)(SMOOTH_SCROLL_AMOUNT_AT_EDGE / DPUtils.GetDensity());
            needToStop = false;
        }


        #region Handlers


        void HandleItemLongClick(object sender, ItemLongClickEventArgs e)
        {
            Vibrator vibrator = (Vibrator)mContext.GetSystemService(Context.VibratorService);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                vibrator.Vibrate(VibrationEffect.CreateOneShot(150, 10));

            }
            else
            {
                vibrator.Vibrate(150);

            }
            mTotalOffset = 0;

            int position = PointToPosition(mDownX, mDownY);

            if (position < 0 || !LongClickable)
                return;

            int itemNum = position - FirstVisiblePosition;

            View selectedView = GetChildAt(itemNum);
            mMobileItemId = Adapter.GetItemId(position); // use this varable to keep track of which view is currently moving
            mHoverCell = GetAndAddHoverView(selectedView);
            selectedView.Visibility = ViewStates.Invisible; // set the visibility of the selected view to invisible

            mCellIsMobile = true;

            UpdateNeighborViewsForID(mMobileItemId);
        }


        void HandleHoverAnimatorUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e)
        {
            Invalidate();
        }

        void HandleHoverAnimationStart(object sender, EventArgs e)
        {
            Enabled = false;
            needToStop = true;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (needToStop)
            {
                return false;
            }
            return base.OnInterceptTouchEvent(ev);
        }

        /// <summary>
        ///  Resets the global variables and visbility of the mobile view
        /// </summary>
        void HandleHoverAnimationEnd(object sender, EventArgs e)
        {
            mAboveItemId = INVALID_ID;
            mMobileItemId = INVALID_ID;
            mBelowItemId = INVALID_ID;
            mHoverCell = null;
            Enabled = true;
            Invalidate();

            mobileView.Visibility = ViewStates.Visible;

            ((IRearrangeAccountListAdapter)Adapter).mMobileCellPosition = int.MinValue;

            ((IRearrangeAccountListAdapter)Adapter).NotifyChanged();

            needToStop = false;

            if (((IRearrangeAccountListAdapter)Adapter).GetIsChange())
            {
                ((RearrangeAccountActivity)mContext).EnableSaveButton();
            }
        }

        #endregion

        #region IOnGestureListener Implementation

        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return false;
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        

        public void OnLongPress(MotionEvent e)
        {

        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {

        }

        #endregion

        #region Bitmap Drawable Creation

        BitmapDrawable GetAndAddHoverView(View v)
        {

            int w = v.Width;
            int h = v.Height;
            int top = v.Top;
            int left = v.Left;

            Bitmap b = GetBitmapWithBorder(v);

            BitmapDrawable drawable = new BitmapDrawable(Resources, b);

            mHoverCellOriginalBounds = new Rect(left, top, left + w, top + h);
            mHoverCellCurrentBounds = new Rect(mHoverCellOriginalBounds);

            drawable.SetBounds(left, top, left + w, top + h);

            return drawable;
        }

        static Bitmap GetBitmapWithBorder(View v)
        {
            Bitmap bitmap = GetBitmapFromView(v);
            Canvas can = new Canvas(bitmap);

            Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);

            Paint paint = new Paint();
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = (int)DPUtils.ConvertDPToPx(2f);
            paint.Color = Color.ParseColor("#e4e4e4");
            can.DrawBitmap(bitmap, 0, 0, null);
            can.DrawRect(rect, paint);

            return bitmap;
        }

        static Bitmap GetBitmapFromView(View v)
        {
            try
            {
                Bitmap bitmap = Bitmap.CreateBitmap(v.Width, v.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(bitmap);
                v.Draw(canvas);
                return bitmap;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
            return default(Bitmap);

        }

        #endregion

        void UpdateNeighborViewsForID(long itemID)
        {
            int position = GetPositionForID(itemID);
            mAboveItemId = Adapter.GetItemId(position - 1);
            mBelowItemId = Adapter.GetItemId(position + 1);

        }

        public View GetViewForID(long itemID)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                View v = GetChildAt(i);
                int position = FirstVisiblePosition + i;
                long id = Adapter.GetItemId(position);
                if (id == itemID)
                {
                    return v;
                }
            }
            return null;
        }

        public int GetPositionForID(long itemID)
        {
            View v = GetViewForID(itemID);
            if (v == null)
            {
                return -1;
            }
            else
            {
                return GetPositionForView(v);
            }
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);
            if (mHoverCell != null)
            {
                mHoverCell.Draw(canvas);
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            try
            {
                dectector.OnTouchEvent(e);
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        mDownX = (int)e.GetX();
                        mDownY = (int)e.GetY();
                        mActivePointerId = e.GetPointerId(0);
                        break;
                    case MotionEventActions.Move:
                        if (mActivePointerId == INVALID_POINTER_ID)
                            break;

                        int pointerIndex = e.FindPointerIndex(mActivePointerId);
                        mLastEventY = (int)e.GetY(pointerIndex);
                        int deltaY = mLastEventY - mDownY;


                        if (mCellIsMobile)
                        { // Responsible for moving the bitmap drawable to the touch location
                            Enabled = false;

                            mHoverCellCurrentBounds.OffsetTo(mHoverCellOriginalBounds.Left,
                                mHoverCellOriginalBounds.Top + deltaY + mTotalOffset);
                            mHoverCell.SetBounds(mHoverCellCurrentBounds.Left, mHoverCellCurrentBounds.Top, mHoverCellCurrentBounds.Right, mHoverCellCurrentBounds.Bottom);
                            Invalidate();
                            HandleCellSwitch();

                            mIsMobileScrolling = false;

                            HandleMobileCellScroll();
                        }
                        break;
                    case MotionEventActions.Up:
                        TouchEventsEnded();
                        break;
                    case MotionEventActions.Cancel:
                        TouchEventsCancelled();
                        break;
                    case MotionEventActions.PointerUp:
                        pointerIndex = ((int)e.Action & mPointerIndexMask) >>
                            (mPointerIndexShift);
                        int pointerId = e.GetPointerId(pointerIndex);
                        if (pointerId == mActivePointerId)
                        {
                            TouchEventsEnded();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

            return base.OnTouchEvent(e);
        }


        void HandleCellSwitch()
        {
            try
            {
                int deltaY = mLastEventY - mDownY; // total distance moved since the last movement
                int deltaYTotal = mHoverCellOriginalBounds.Top + mTotalOffset + deltaY; // total distance moved from original long press position

                View belowView = GetViewForID(mBelowItemId); // the view currently below the mobile item
                View mobileView = GetViewForID(mMobileItemId); // the current mobile item view (this is NOT what you see moving around, thats just a dummy, this is the "invisible" cell on the list)
                View aboveView = GetViewForID(mAboveItemId); // the view currently above the mobile item

                // Detect if we have moved the drawable enough to justify a cell swap
                bool isBelow = (belowView != null) && (deltaYTotal > belowView.Top);
                bool isAbove = (aboveView != null) && (deltaYTotal < aboveView.Top);

                if (isBelow || isAbove)
                {

                    View switchView = isBelow ? belowView : aboveView; // get the view we need to animate

                    var diff = GetViewForID(mMobileItemId).Top - switchView.Top; // the difference between the top of the mobile view and top of the view we are about to switch with

                    // Lets animate the view sliding into its new position. Remember: the listview cell corresponding the mobile item is invisible so it looks like 
                    // the switch view is just sliding into position
                    ObjectAnimator anim = ObjectAnimator.OfFloat(switchView, "TranslationY", switchView.TranslationY, switchView.TranslationY + diff);
                    anim.SetDuration(30);
                    anim.Start();


                    // Swap out the mobile item id
                    mMobileItemId = GetPositionForView(switchView);

                    // Since the mobile item id has been updated, we also need to make sure and update the above and below item ids
                    UpdateNeighborViewsForID(mMobileItemId);

                    // One the animation ends, we want to adjust out visiblity 
                    anim.AnimationEnd += (sender, e) => {
                        // Swap the visbility of the views corresponding to the data items being swapped - since the "switchView" will become the "mobileView"
                        //						mobileView.Visibility = ViewStates.Visible;
                        //						switchView.Visibility = ViewStates.Invisible;

                        // Swap the items in the data source and then NotifyDataSetChanged()
                        ((IRearrangeAccountListAdapter)Adapter).SwapItems(GetPositionForView(mobileView), GetPositionForView(switchView));
                    };
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

        }

        /// <summary>
        /// Resets all the appropriate fields to a default state while also animating
        /// the hover cell back to its correct location.
        /// </summary>
        void TouchEventsEnded()
        {
            mobileView = GetViewForID(mMobileItemId);
            if (mCellIsMobile)
            {
                mCellIsMobile = false;
                mIsWaitingForScrollFinish = false;
                mIsMobileScrolling = false;
                mActivePointerId = INVALID_POINTER_ID;
                ((IRearrangeAccountListAdapter)Adapter).mMobileCellPosition = int.MinValue;

                if (mScrollState != (int)ScrollState.Idle)
                {
                    mIsWaitingForScrollFinish = true;
                    mCellIsMobile = true;
                    return;
                }

                mHoverCellCurrentBounds.OffsetTo(mHoverCellOriginalBounds.Left, mobileView.Top);

                ObjectAnimator hoverViewAnimator = ObjectAnimator.OfObject(mHoverCell, "Bounds", this, mHoverCellCurrentBounds);
                hoverViewAnimator.Update += HandleHoverAnimatorUpdate;
                hoverViewAnimator.AnimationStart += HandleHoverAnimationStart;
                hoverViewAnimator.AnimationEnd += HandleHoverAnimationEnd;
                hoverViewAnimator.Start();
            }
            else
            {
                TouchEventsCancelled();
            }
        }

        /// <summary>
        /// By Implementing the ITypeEvaluator Inferface, we are able to set this as the the ITypeEvaluator for the hoverViewAnimator
        /// This method is responsible for animating the drawable to its final location after touch events end.
        /// </summary>
        public Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
        {
            var startValueRect = startValue as Rect;
            var endValueRect = endValue as Rect;

            return new Rect(Interpolate(startValueRect.Left, endValueRect.Left, fraction),
                Interpolate(startValueRect.Top, endValueRect.Top, fraction),
                Interpolate(startValueRect.Right, endValueRect.Right, fraction),
                Interpolate(startValueRect.Bottom, endValueRect.Bottom, fraction));
        }

        /// <summary>
        /// Interpolate the specified start, end and fraction for use in the Evaluate method above for a smooth animation
        /// </summary>
        public int Interpolate(int start, int end, float fraction)
        {
            return (int)(start + fraction * (end - start));
        }

        /// <summary>
        /// Resets all the appropriate fields to a default state.
        /// Resets the visibility of the currently mobile view
        /// </summary>
        void TouchEventsCancelled()
        {
            mobileView = GetViewForID(mMobileItemId);
            if (mCellIsMobile)
            {
                mAboveItemId = INVALID_ID;
                mMobileItemId = INVALID_ID;
                mBelowItemId = INVALID_ID;
                mHoverCell = null;
                Invalidate();
                RequestLayout();
            }

            if (mobileView != null)
                mobileView.Visibility = ViewStates.Visible;

            Enabled = true;
            mCellIsMobile = false;
            mIsMobileScrolling = false;
            mActivePointerId = INVALID_POINTER_ID;

            ((IRearrangeAccountListAdapter)Adapter).mMobileCellPosition = int.MinValue;

            ((IRearrangeAccountListAdapter)Adapter).NotifyChanged();
        }

        private void HandleMobileCellScroll()
        {
            mIsMobileScrolling = HandleMobileCellScroll(mHoverCellCurrentBounds);
        }

        public bool HandleMobileCellScroll(Rect r)
        {
            int offset = ComputeVerticalScrollOffset();
            int height = Height;
            int extent = ComputeVerticalScrollExtent();
            int range = ComputeVerticalScrollRange();
            int hoverViewTop = r.Top;
            int hoverHeight = r.Height();

            if (hoverViewTop <= 0 && offset > 0)
            {
                SmoothScrollBy(-mSmoothScrollAmountAtEdge, 0);
                return true;
            }

            if (hoverViewTop + hoverHeight >= height && (offset + extent) < range)
            {
                SmoothScrollBy(mSmoothScrollAmountAtEdge, 0);
                return true;
            }

            return false;
        }

        void IOnScrollListener.OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            try
            {
                mCurrentFirstVisibleItem = firstVisibleItem;
                mCurrentVisibleItemCount = visibleItemCount;

                mPreviousFirstVisibleItem = (mPreviousFirstVisibleItem == -1) ? mCurrentFirstVisibleItem
                        : mPreviousFirstVisibleItem;
                mPreviousVisibleItemCount = (mPreviousVisibleItemCount == -1) ? mCurrentVisibleItemCount
                        : mPreviousVisibleItemCount;

                CheckAndHandleFirstVisibleCellChange();
                CheckAndHandleLastVisibleCellChange();

                mPreviousFirstVisibleItem = mCurrentFirstVisibleItem;
                mPreviousVisibleItemCount = mCurrentVisibleItemCount;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        void IOnScrollListener.OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            try
            {
                mCurrentScrollState = (int)scrollState;
                mScrollState = (int)scrollState;
                IsScrollCompleted();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void CheckAndHandleFirstVisibleCellChange()
        {
            if (mCurrentFirstVisibleItem != mPreviousFirstVisibleItem)
            {
                if (mCellIsMobile && mMobileItemId != INVALID_ID)
                {
                    UpdateNeighborViewsForID(mMobileItemId);
                    HandleCellSwitch();
                }
            }
        }

        public void CheckAndHandleLastVisibleCellChange()
        {
            int currentLastVisibleItem = mCurrentFirstVisibleItem + mCurrentVisibleItemCount;
            int previousLastVisibleItem = mPreviousFirstVisibleItem + mPreviousVisibleItemCount;
            if (currentLastVisibleItem != previousLastVisibleItem)
            {
                if (mCellIsMobile && mMobileItemId != INVALID_ID)
                {
                    UpdateNeighborViewsForID(mMobileItemId);
                    HandleCellSwitch();
                }
            }
        }

        private void IsScrollCompleted()
        {
            if (mCurrentVisibleItemCount > 0 && mCurrentScrollState == (int) ScrollState.Idle)
            {
                if (mCellIsMobile && mIsMobileScrolling)
                {
                    HandleMobileCellScroll();
                }
                else if (mIsWaitingForScrollFinish)
                {
                    TouchEventsEnded();
                }
            }
        }
    }
}