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
    internal class ManageAccessDeleteAdapter : BaseCustomAdapter<UserManageAccessAccount>, ManageAccessContract.IUserActionsListener
    {
        private ManageAccessContract.IView mView;
        private AccountData accountData;
        customCheckboxListener customListner;

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

        public List<UserManageAccessAccount> GetAllNotifications()
        {
            List<UserManageAccessAccount> customerAccountList = UserManageAccessAccount.List(accountData?.AccountNum);
            return customerAccountList;
        }

        public interface customCheckboxListener
        {
            public void onCheckboxListener(int position);
        }

        public void setCustomCheckboxListner(customCheckboxListener listener)
        {
            this.customListner = listener;
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
                CheckBox btn_delete = (CheckBox)convertView.FindViewById(Resource.Id.CheckActionIcon);
                viewHolder = new ManageAccessFilterViewHolder(convertView);
                convertView.Tag = viewHolder;

                btn_delete.CheckedChange += (sender, e) =>
                {
                    if (e.IsChecked)
                    {
                        UserManageAccessAccount.SetSelected(data.AccNum, true, data.UserAccountId, data.email);
                        customListner.onCheckboxListener(position);
                        return;
                    }
                    else
                    {
                        UserManageAccessAccount.SetSelected(data.AccNum, false, data.UserAccountId, data.email);
                        customListner.onCheckboxListener(position);
                        return;
                    }
                };
            }
            else
            {
                viewHolder = convertView.Tag as ManageAccessFilterViewHolder;
            }
            try
            {
                if (data.IsPreRegister)
                {
                    viewHolder.txtUserAccessTitle.Text = data.email;
                    viewHolder.txtUserAccessBody.Visibility = ViewStates.Visible;
                    viewHolder.txtUserAccessBody.Text = Utility.GetLocalizedLabel("UserAccess", "pendingRegistration");
                }
                else
                {
                    viewHolder.txtUserAccessTitle.Text = data.name;
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
                TextViewUtils.SetMuseoSans300Typeface(txtUserAccessBody);
                TextViewUtils.SetMuseoSans500Typeface(txtUserAccessTitle);
            }
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            throw new NotImplementedException();
        }

        public void OnAddAccount()
        {
            throw new NotImplementedException();
        }

        public void OnAddLogUserAccess(AccountData accountData)
        {
            throw new NotImplementedException();
        }

        public void OnRemoveAccount(List<UserManageAccessAccount> DeletedSelectedUser,List<Models.DeleteAccessAccount> accounts, string AccountNum)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}