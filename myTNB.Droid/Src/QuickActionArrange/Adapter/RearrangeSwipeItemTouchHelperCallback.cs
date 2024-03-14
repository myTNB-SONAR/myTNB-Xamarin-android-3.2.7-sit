
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.QuickActionArrange.Adapter
{
    enum ButtonState
    {
        GONE,
        LEFT_VISIBLE,
        RIGHT_VISIBLE
    }
    public class RearrangeSwipeItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        private static bool swipeBack = false;
        private readonly RearrangeQuickActionListAdapter adapter;
        private int intrinsicDeleteWidth;
        private int intrinsicDeleteHeight;
        private Drawable mDeleteIcon;
        private static float buttonWidth = 300;
        private static RecyclerView.ViewHolder currentItemViewHolder = null;
        private static ButtonState buttonShowedState = ButtonState.GONE;
        private ColorDrawable background;
        private float density;

        public RearrangeSwipeItemTouchHelperCallback(RearrangeQuickActionListAdapter adapter, Context context)
        {
            this.adapter = adapter;
            density = context.Resources.DisplayMetrics.Density;
            background = new ColorDrawable(Color.Red);
        }

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            int swipeFlags = ItemTouchHelper.Start;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            adapter.onItemMove(viewHolder.AdapterPosition, target.AdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            if (direction == ItemTouchHelper.Start) // Check if it's a left swipe
            {
                currentItemViewHolder = viewHolder;
                adapter.OnItemDismiss(viewHolder.AdapterPosition);
            }
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);

            View itemView = viewHolder.ItemView;

            if (viewHolder.AdapterPosition == -1)
            {
                return;
            }

            if (dX < 0)
            {
                string copywriting = Utility.GetLocalizedLabel("RearrangeQuickAction", "hide");
                if (string.IsNullOrEmpty(copywriting))
                {
                    copywriting = "HIDE";
                }

                // Swiping to the left (delete)
                int itemHeight = itemView.Height;

                ColorDrawable bg = new ColorDrawable();
                Paint p = new Paint();
                RectF rightButton = null;

                rightButton = new RectF(itemView.Right - (buttonWidth - 5), itemView.Top, itemView.Right, itemView.Bottom - (int)(6 * density));
                p.Color = Color.Red;
                c.DrawRoundRect(rightButton, 0, 0, p);

                // Add a margin to the bottom of the delete button
                int marginBottom = 6; // Adjust this value as needed
                rightButton.Bottom += marginBottom;
                // Assuming context is the Context instance
                background.SetBounds(itemView.Right + (int)dX, itemView.Top, itemView.Right, itemView.Bottom - (int)(6 * density));
                background.Draw(c);

                TextPaint textPaint = new TextPaint();
                textPaint.Color = Color.White;
                textPaint.TextSize = 35;

                // Calculate the width and height of the text
                float textWidth = textPaint.MeasureText(copywriting);
                float textHeight = textPaint.Descent() - textPaint.Ascent();

                // Set a margin (you can adjust this based on your preference)
                float margin = 30;

                // Calculate the X-coordinate to place the text on the right side with a margin
                float textX = itemView.Right - textWidth - margin;

                float heightreal = itemView.Height - textHeight - (int)(6 * density);

                // Calculate the Y-coordinate to center the text vertically
                float textY = itemView.Top + heightreal / 2 - textPaint.Ascent();

                c.DrawText(copywriting, textX, textY, textPaint);
            }
            else
            {
                // Swiping to the right (not implemented in this example)
                background.SetBounds(0, 0, 0, 0);
                background.Draw(c);
            }

        }

        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            if (actionState != ItemTouchHelper.ActionStateDrag)
            {
                if (viewHolder != null)
                {
                    View itemView = viewHolder.ItemView;
                    itemView.Alpha = 1.0f; // Example: Change alpha during selection
                    itemView.ScaleX = 1.0f; // Example: Increase scale during selection
                    itemView.ScaleY = 1.0f;
                }
            }

            base.OnSelectedChanged(viewHolder, actionState);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);

            if (viewHolder != null)
            {
                View itemView = viewHolder.ItemView;
                itemView.Alpha = 1.0f; // Example: Reset alpha after clearing selection
                itemView.ScaleX = 1.0f; // Example: Reset scale after clearing selection
                itemView.ScaleY = 1.0f;
            }
        }


        public override bool IsLongPressDragEnabled => true;
        public override bool IsItemViewSwipeEnabled => true;
    }

}

