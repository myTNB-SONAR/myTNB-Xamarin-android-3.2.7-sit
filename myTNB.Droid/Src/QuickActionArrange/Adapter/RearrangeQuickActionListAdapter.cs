
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Java.Util;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.Utils;
using static myTNB.Mobile.MobileEnums;

namespace myTNB_Android.Src.QuickActionArrange.Adapter
{
    public class RearrangeQuickActionListAdapter : RecyclerView.Adapter
    {
        List<MyServiceModel> serviceIconModel = new List<MyServiceModel>();

        private Android.App.Activity mActivity;
        //private OnItemInteractionListener listener;
        private OnItemDismissListener onItemDismissListener;
        private OnItemDragAndMoveListener onItemDragAndMoveListener;
        private int draggedItemPosition = -1;
        private RecyclerView recyclerView; // Added field to hold reference to RecyclerView

        public RearrangeQuickActionListAdapter(List<MyServiceModel> list, Android.App.Activity activity, RecyclerView recyclerView)
        {

            if (list == null)
            {
                this.serviceIconModel.Clear();
            }
            else
            {
                this.serviceIconModel = list;
            }
            this.mActivity = activity;
            this.recyclerView = recyclerView;
        }

        public override int ItemCount => serviceIconModel.Count;

        public List<MyServiceModel> GetCurrentList()
        {
            return serviceIconModel;
        }

        internal void OnItemDismiss(int adapterPosition)
        {
            //serviceIconModel.RemoveAt(adapterPosition);
            onItemDismissListener?.OnItemDismiss(adapterPosition);
            //NotifyDataSetChanged();
        }

        internal void removeItemFromList(int position)
        {
            serviceIconModel.RemoveAt(position);
            NotifyDataSetChanged();
        }

        public interface OnItemDismissListener
        {
            void OnItemDismiss(int adapterPosition);
        }

        public void SetOnItemDismissListener(OnItemDismissListener listener)
        {
            this.onItemDismissListener = listener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RearrangeQuickActionViewHolder vh = holder as RearrangeQuickActionViewHolder;

            try
            {
                MyServiceModel model = serviceIconModel[position];
                TextViewUtils.SetMuseoSans300Typeface(vh.txtAccountName);               
                vh.txtAccountName.Text = filterServiceName(serviceIconModel[position].ServiceName);
                TextViewUtils.SetTextSize13(vh.txtAccountName);

                try
                {
                    switch (model.ServiceType)
                    {
                        case ServiceEnum.SELFMETERREADING:
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.submit_meter);
                            break;
                        case ServiceEnum.SUBMITFEEDBACK:
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.feedback);
                            break;
                        case ServiceEnum.PAYBILL:
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.bills);
                            break;
                        case ServiceEnum.VIEWBILL:
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.pdf_bill);
                            break;
                        case ServiceEnum.APPLICATIONSTATUS:
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.check_status);
                            break;
                        case ServiceEnum.ENERGYBUDGET:
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.Check_Status_Icon);
                            break;
                        case ServiceEnum.MYHOME:
                            vh.QuickActionIcon.SetImageResource(Resource.Drawable.Icon_Quick_Access_MyHome);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public string filterServiceName(string name)
        {
            string inputString = name;
            string resultString = inputString.Replace("<br>", " ");
            return resultString;
        }

        public void onItemMove(int fromPosition, int toPosition)
        {
            MyServiceModel item = serviceIconModel[fromPosition];
            serviceIconModel.RemoveAt(fromPosition);
            serviceIconModel.Insert(toPosition, item);
            NotifyItemMoved(fromPosition, toPosition);
            OnItemDragAndMove();
        }

        internal void OnItemDragAndMove()
        {
            onItemDragAndMoveListener?.OnItemDragAndMove(true);
        }

        public interface OnItemDragAndMoveListener
        {
            void OnItemDragAndMove(bool flag);
        }

        public void SetOnItemDragAndMoveListener(OnItemDragAndMoveListener listener)
        {
            this.onItemDragAndMoveListener = listener;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.QuickActionRearrangeItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new RearrangeQuickActionViewHolder(itemView);
        }

        public class RearrangeQuickActionViewHolder : RecyclerView.ViewHolder
        {

            public TextView txtAccountName { get; private set; }

            public ImageView imageActionIcon { get; private set; }

            public ImageView QuickActionIcon { get; private set; }

            public RearrangeQuickActionViewHolder(View itemView) : base(itemView)
            {
                QuickActionIcon = itemView.FindViewById<ImageView>(Resource.Id.imageIcon);
                txtAccountName = itemView.FindViewById<TextView>(Resource.Id.txtAccountName);
                imageActionIcon = itemView.FindViewById<ImageView>(Resource.Id.imageActionIcon);
                TextViewUtils.SetMuseoSans300Typeface(txtAccountName);
                TextViewUtils.SetTextSize13(txtAccountName);
            }
        }
    }
}

