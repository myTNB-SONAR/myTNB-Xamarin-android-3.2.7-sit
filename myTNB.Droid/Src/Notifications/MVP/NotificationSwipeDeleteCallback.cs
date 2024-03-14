using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;



using Android.Text;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using myTNB.Android.Src.Notifications.Adapter;

namespace myTNB.Android.Src.Notifications.MVP
{
    enum ButtonState
    {
        GONE,
        LEFT_VISIBLE,
        RIGHT_VISIBLE
    }
    public class NotificationSwipeDeleteCallback : ItemTouchHelper.Callback
    {
        private static bool swipeBack = false;
        private static ButtonState buttonShowedState = ButtonState.GONE;
        private static float buttonWidth = 300;
        private static RecyclerView.ViewHolder currentItemViewHolder = null;
        private static RectF buttonInstance = null;
        private Drawable mDeleteIcon;
        private Drawable mReadIcon;
        private int intrinsicDeleteWidth;
        private int intrinsicDeleteHeight;
        private int intrinsicReadWidth;
        private int intrinsicReadHeight;
        private Context mContext;
        private static NotificationContract.IView notificationViewListener;

        public NotificationSwipeDeleteCallback(NotificationContract.IView listener, Drawable deleteIcon, Drawable readIcon)
        {
            notificationViewListener = listener;
            mDeleteIcon = deleteIcon;
            mReadIcon = readIcon;
            SetIconsLayout();
        }

        public void SetInitialState(){
          swipeBack = false;
          buttonShowedState = ButtonState.GONE;
        }

        private void SetIconsLayout()
        {
            intrinsicDeleteWidth = mDeleteIcon.IntrinsicWidth;
            intrinsicDeleteHeight = mDeleteIcon.IntrinsicHeight;
            intrinsicReadWidth = mReadIcon.IntrinsicWidth;
            intrinsicReadHeight = mReadIcon.IntrinsicHeight;
            buttonWidth = intrinsicDeleteWidth * 3;
        }

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            //UserNotificationData userNotificationData = recyclerView.GetAdapter();
            NotificationRecyclerAdapter notificationAdapter = (NotificationRecyclerAdapter)recyclerView.GetAdapter();
            if (viewHolder != null && viewHolder.AdapterPosition > -1 && notificationAdapter.GetItemObject(viewHolder.AdapterPosition).IsRead)
            {
                return MakeMovementFlags(0, ItemTouchHelper.Left);
            }
            return MakeMovementFlags(0,ItemTouchHelper.Left | ItemTouchHelper.Right);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            return false;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            int notificationPos = viewHolder.AdapterPosition;

            if (direction == ItemTouchHelper.Left)
            {
                notificationViewListener.DeleteNotificationByPosition(notificationPos);
            }
            else
            {
                notificationViewListener.ReadNotificationByPosition(notificationPos);
            }
        }

        public override int ConvertToAbsoluteDirection(int flags, int layoutDirection)
        {
            if (swipeBack)
            {
                swipeBack = buttonShowedState != ButtonState.GONE;
                if (buttonShowedState == ButtonState.LEFT_VISIBLE || buttonShowedState == ButtonState.RIGHT_VISIBLE)
                {
                    return 0;
                }
            }
            return base.ConvertToAbsoluteDirection(flags, layoutDirection);
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            View itemView = viewHolder.ItemView;
            int itemHeight = itemView.Height;

            ColorDrawable bg = new ColorDrawable();
            Paint p = new Paint();
            RectF rightButton = null, leftButton = null;
            if (dX < 0 || buttonShowedState == ButtonState.RIGHT_VISIBLE)
            {
                bg.Color = Color.Red;
                bg.SetBounds(
                        itemView.Right + (int)dX, itemView.Top, itemView.Right, itemView.Bottom);
                bg.Draw(c);

                rightButton = new RectF(itemView.Right - (buttonWidth - 5), itemView.Top, itemView.Right, itemView.Bottom);
                p.Color = Color.Red;
                c.DrawRoundRect(rightButton, 0, 0, p);

                int deleteIconTop = itemView.Top + (itemHeight - intrinsicDeleteHeight) / 2;
                int deleteIconMargin = (itemHeight - intrinsicDeleteHeight) / 2;
                int deleteIconLeft = itemView.Right - deleteIconMargin - intrinsicDeleteWidth;
                int deleteIconRight = itemView.Right - deleteIconMargin;
                int deleteIconBottom = deleteIconTop + intrinsicDeleteHeight;

                mDeleteIcon.SetBounds(deleteIconLeft, deleteIconTop, deleteIconRight, deleteIconBottom);
                mDeleteIcon.Draw(c);

            }
            else
            {
                bg.Color = Color.Blue;
                bg.SetBounds(
                        itemView.Left, itemView.Top, itemView.Left + (int)dX, itemView.Bottom);
                bg.Draw(c);

                leftButton = new RectF(itemView.Left, itemView.Top, itemView.Left + (buttonWidth - 20), itemView.Bottom);
                p.Color = Color.Blue;
                c.DrawRoundRect(leftButton, 0, 0, p);

                int markReadIconTop = itemView.Top + (itemHeight - intrinsicReadHeight) / 2;
                int markReadIconMargin = (itemHeight - intrinsicReadHeight) / 2;
                int markReadIconLeft = itemView.Left + markReadIconMargin;
                int markReadIconRight = itemView.Left + markReadIconMargin + intrinsicReadWidth;
                int markReadIconBottom = markReadIconTop + intrinsicReadHeight;

                mReadIcon.SetBounds(markReadIconLeft, markReadIconTop, markReadIconRight, markReadIconBottom);
                mReadIcon.Draw(c);
            }

            buttonInstance = null;
            if (buttonShowedState == ButtonState.LEFT_VISIBLE)
            {
                buttonInstance = leftButton;
            }
            else if (buttonShowedState == ButtonState.RIGHT_VISIBLE)
            {
                buttonInstance = rightButton;
            }

            if (actionState == ItemTouchHelper.ActionStateSwipe)
            {
                if (buttonShowedState != ButtonState.GONE)
                {
                    if (buttonShowedState == ButtonState.LEFT_VISIBLE) dX = Math.Max(dX, buttonWidth);
                    if (buttonShowedState == ButtonState.RIGHT_VISIBLE) dX = Math.Min(dX, -buttonWidth);
                    base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
                }
                else
                {
                    SetTouchListener(this, c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
                }
            }

            if (buttonShowedState == ButtonState.GONE)
            {
                base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
            }

            currentItemViewHolder = viewHolder;
        }

        public void OnChildDrawBase(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
        }

        private void SetTouchListener(NotificationSwipeDeleteCallback swipeDeleteCallback, Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            recyclerView.SetOnTouchListener(new ItemTouchListener(swipeDeleteCallback, c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive));
        }


        class ItemTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            Canvas c;
            float dX, dY;
            RecyclerView recyclerView;
            RecyclerView.ViewHolder viewHolder;
            int actionState;
            bool isCurrentlyActive;
            NotificationSwipeDeleteCallback swipeDeleteCallback;
            public ItemTouchListener(NotificationSwipeDeleteCallback swipeDeleteCallback, Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
            {
                this.dX = dX;
                this.dY = dY;
                this.recyclerView = recyclerView;
                this.c = c;
                this.viewHolder = viewHolder;
                this.actionState = actionState;
                this.isCurrentlyActive = isCurrentlyActive;
                this.swipeDeleteCallback = swipeDeleteCallback;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                swipeBack = e.Action == MotionEventActions.Cancel || e.Action == MotionEventActions.Up;
                if (swipeBack)
                {
                    if(this.dX < -buttonWidth)
                    {
                        buttonShowedState = ButtonState.RIGHT_VISIBLE;
                    }else if (this.dX > buttonWidth){
                        buttonShowedState = ButtonState.LEFT_VISIBLE;
                    }
                }

                if (buttonShowedState != ButtonState.GONE)
                {
                    this.recyclerView.SetOnTouchListener(new ItemTouchDownListener(swipeDeleteCallback,this.c, this.recyclerView, this.viewHolder, this.dX, this.dY, this.actionState, this.isCurrentlyActive));
                    for (int i = 0; i < recyclerView.ChildCount; ++i)
                    {
                        recyclerView.GetChildAt(i).Clickable = false;
                        recyclerView.GetChildAt(i).LongClickable = false;
                    }
                }
                return false;
            }
        }

        class ItemTouchDownListener : Java.Lang.Object, View.IOnTouchListener
        {
            Canvas c;
            float dX, dY;
            RecyclerView recyclerView;
            RecyclerView.ViewHolder viewHolder;
            int actionState;
            bool isCurrentlyActive;
            NotificationSwipeDeleteCallback swipeDeleteCallback;
            public ItemTouchDownListener(NotificationSwipeDeleteCallback swipeDeleteCallback,Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
            {
                this.dX = dX;
                this.dY = dY;
                this.recyclerView = recyclerView;
                this.c = c;
                this.viewHolder = viewHolder;
                this.actionState = actionState;
                this.isCurrentlyActive = isCurrentlyActive;
                this.swipeDeleteCallback = swipeDeleteCallback;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                if (e.Action == MotionEventActions.Down)
                {
                    this.recyclerView.SetOnTouchListener(new ItemTouchUpListener(swipeDeleteCallback, this.c, this.recyclerView, this.viewHolder, this.dX, this.dY, this.actionState, this.isCurrentlyActive));
                }
                return false;
            }
        }

        class ItemTouchUpListener : Java.Lang.Object, View.IOnTouchListener
        {
            Canvas c;
            float dX, dY;
            RecyclerView recyclerView;
            RecyclerView.ViewHolder viewHolder;
            int actionState;
            bool isCurrentlyActive;
            NotificationSwipeDeleteCallback swipeDeleteCallback;
            public ItemTouchUpListener(NotificationSwipeDeleteCallback swipeDeleteCallback, Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
            {
                this.dX = dX;
                this.dY = dY;
                this.recyclerView = recyclerView;
                this.c = c;
                this.viewHolder = viewHolder;
                this.actionState = actionState;
                this.isCurrentlyActive = isCurrentlyActive;
                this.swipeDeleteCallback = swipeDeleteCallback;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                if (e.Action == MotionEventActions.Up)
                {
                    this.swipeDeleteCallback.OnChildDrawBase(c, recyclerView, viewHolder, 0f, dY, actionState, isCurrentlyActive);
                    this.recyclerView.SetOnTouchListener(new OnTouchFalseListener());
                    for (int i = 0; i < recyclerView.ChildCount; ++i)
                    {
                        recyclerView.GetChildAt(i).Clickable = true;
                        recyclerView.GetChildAt(i).LongClickable = true;
                    }
                    swipeBack = false;
                    if (buttonInstance != null && buttonInstance.Contains(e.GetX(), e.GetY()))
                    {
                        if (buttonShowedState == ButtonState.LEFT_VISIBLE)
                        {
                            notificationViewListener.ReadNotificationByPosition(viewHolder.AdapterPosition);
                        }
                        else if (buttonShowedState == ButtonState.RIGHT_VISIBLE)
                        {
                            notificationViewListener.DeleteNotificationByPosition(viewHolder.AdapterPosition);

                        }
                    }
                    buttonShowedState = ButtonState.GONE;
                    currentItemViewHolder = null;
                }
                return false;
            }
        }

        class OnTouchFalseListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                return false;
            }
        }

        public void DrawOptionButtons(Canvas c)
        {
            if (currentItemViewHolder != null)
            {
                View itemView = currentItemViewHolder.ItemView;
                ColorDrawable bg = new ColorDrawable();
                bg.Color = Color.Red;
                bg.SetBounds(
                        itemView.Right, itemView.Top, itemView.Right, itemView.Bottom);
                bg.Draw(c);

                TextPaint textPaint = new TextPaint();
                textPaint.Color = Color.White;
                textPaint.TextSize = 25;
                c.DrawText("Delete", itemView.Right - (textPaint.TextSize * 4), itemView.Top + (itemView.Height / 2), textPaint);
            }
        }
    }
}
