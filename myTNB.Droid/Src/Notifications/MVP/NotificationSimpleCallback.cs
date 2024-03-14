using System;
using Android.Graphics;
using Android.Graphics.Drawables;


using Android.Text;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using myTNB.Android.Src.Notifications.Adapter;

namespace myTNB.Android.Src.Notifications.MVP
{
    public class NotificationSimpleCallback : ItemTouchHelper.SimpleCallback
    {
        private NotificationRecyclerAdapter mAdapter;
        public NotificationSimpleCallback(RecyclerView.Adapter adapter, int dragDirs, int swipeDirs) : base(dragDirs, swipeDirs)
        {
            mAdapter = (NotificationRecyclerAdapter)adapter;
        }
        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder viewHolder1)
        {
            return false;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int i)
        {
            int position = viewHolder.AdapterPosition;
            mAdapter.RemoveItem(position);
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, Boolean isCurrentlyActive)
        {
            View itemView = viewHolder.ItemView;
            ColorDrawable bg = new ColorDrawable();
            bg.Color = Color.Red;
            bg.SetBounds(
                    itemView.Right + (int)dX, itemView.Top, itemView.Right, itemView.Bottom);
            bg.Draw(c);

            TextPaint textPaint = new TextPaint();
            textPaint.Color = Color.White;
            textPaint.TextSize = 25;
            c.DrawText("Delete", itemView.Right - (textPaint.TextSize * 4), itemView.Top + (itemView.Height / 2), textPaint);

            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
        }
    }
}
