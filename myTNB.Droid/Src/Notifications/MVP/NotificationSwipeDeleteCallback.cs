using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Text;
using Android.Views;

namespace myTNB_Android.Src.Notifications.MVP
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

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            return MakeMovementFlags(0,ItemTouchHelper.Left | ItemTouchHelper.Right);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            return false;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            //throw new NotImplementedException();
        }

        //public override int ConvertToAbsoluteDirection(int flags, int layoutDirection)
        //{
        //    if (swipeBack)
        //    {
        //        swipeBack = buttonShowedState != ButtonState.GONE;
        //        return 0;
        //    }
        //    return base.ConvertToAbsoluteDirection(flags, layoutDirection);
        //}

        private void drawText(String text, Canvas c, RectF button, Paint p)
        {
            float textSize = 60;
            p.Color = Color.White;
            p.AntiAlias = true;
            p.TextSize = textSize;

            float textWidth = p.MeasureText(text);
            c.DrawText(text, button.CenterX() - (textWidth / 2), button.CenterY() + (textSize / 2), p);
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            //View itemView = viewHolder.ItemView;
            //ColorDrawable bg = new ColorDrawable();
            //bg.Color = Color.Red;
            //bg.SetBounds(
            //        itemView.Right, itemView.Top, itemView.Right, itemView.Bottom);
            //bg.Draw(c);

            //TextPaint textPaint = new TextPaint();
            //textPaint.Color = Color.White;
            //textPaint.TextSize = 25;
            //c.DrawText("Delete", itemView.Right - (textPaint.TextSize * 4), itemView.Top + (itemView.Height / 2), textPaint);

            View itemView = viewHolder.ItemView;
            Paint p = new Paint();

            RectF leftButton = new RectF(itemView.Left, itemView.Top, itemView.Left + (buttonWidth - 20), itemView.Bottom);
            p.Color = Color.Blue;
            c.DrawRoundRect(leftButton, 16, 16, p);
            drawText("EDIT", c, leftButton, p);

            RectF rightButton = new RectF(itemView.Right - (buttonWidth - 20), itemView.Top, itemView.Right, itemView.Bottom);
            p.Color = Color.Red;
            c.DrawRoundRect(rightButton, 16, 16, p);
            drawText("DELETE", c, rightButton, p);

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
                    }
                    swipeBack = false;
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
