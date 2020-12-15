using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
using AndroidViewAnimations;
using CheeseBind;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyAccount.Activity;
using myTNB_Android.Src.ManageAccess.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.ManageAccess.Adapter
{
    internal class ManageAccessDeleteAdapter : BaseCustomAdapter<UserManageAccessAccount>
    {
        private ManageAccessContract.IView mView;
        private AccountData accountData;

        public ManageAccessDeleteAdapter(Context context) : base(context)
        {
        }

        public ManageAccessDeleteAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public ManageAccessDeleteAdapter(Context context, List<UserManageAccessAccount> itemList) : base(context, itemList)
        {
        }

        public ManageAccessDeleteAdapter(Context context, List<UserManageAccessAccount> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public void DisableAll()
        {
            foreach (UserManageAccessAccount data in itemList)
            {
                data.isSelected = false;
            }
            NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            UserManageAccessAccount data = GetItemObject(position);
            ManageAccessFilterViewHolder viewHolder = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.ManageAccessDeleteRow, parent, false);
                viewHolder = new ManageAccessFilterViewHolder(convertView);
                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as ManageAccessFilterViewHolder;
            }
            try
            {
                viewHolder.txtUserAccessTitle.Text = data.name;
                if (data.IsPreRegister)
                {
                    viewHolder.txtUserAccessBody.Visibility = ViewStates.Visible;
                    viewHolder.txtUserAccessBody.Text = data.email;
                }
                else
                {
                    viewHolder.txtUserAccessBody.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return convertView;
        }

        class ManageAccessFilterViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtUserAccessTitle)]
            public TextView txtUserAccessTitle;

            [BindView(Resource.Id.txtUserAccessBody)]
            public TextView txtUserAccessBody;

            [BindView(Resource.Id.CheckActionIcon)]
            public CheckBox CheckActionIcon;

            public ManageAccessFilterViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtUserAccessTitle, txtUserAccessBody);
            }
        }
    }
}