using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
//using AndroidViewAnimations;
using CheeseBind;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyAccount.Activity;
using myTNB.AndroidApp.Src.ManageAccess.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.ManageAccess.Adapter
{
    internal class ManageAccessAdapter : BaseCustomAdapter<UserManageAccessAccount>, ManageAccessContract.IUserActionsListener
    {
        private ManageAccessContract.IView mView;
        private AccountData accountData;
        customButtonListener customListner;

        public ManageAccessAdapter(Context context) : base(context)
        {
        }

        public ManageAccessAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public ManageAccessAdapter(Context context, List<UserManageAccessAccount> itemList) : base(context, itemList)
        {
        }

        public ManageAccessAdapter(Context context, List<UserManageAccessAccount> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public interface customButtonListener
        {
            public void onButtonClickListner(int position);
        }

        public void setCustomButtonListner(customButtonListener listener)
        {
            this.customListner = listener;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MyAccountViewHolder viewHolder = null;

            UserManageAccessAccount account = GetItemObject(position);
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.MyAccountRowUserAccessNew, parent, false);
                Button btn_delete = (Button)convertView.FindViewById(Resource.Id.delete);
                SwipeLayout swipeLayout = (SwipeLayout)convertView.FindViewById(Resource.Id.swipe);
                viewHolder = new MyAccountViewHolder(convertView);
                convertView.Tag = viewHolder;

                /*swipeLayout.Opened += (sender, e) =>
                {
                    YoYo.With(Techniques.Tada)
                        .Duration(500)
                        .Delay(100)
                        .PlayOn(e.Layout.FindViewById(Resource.Id.trash));
                };*/

                btn_delete.Click += (sender, e) =>
                {
                    if (customListner != null)
                    {
                        customListner.onButtonClickListner(position);
                        return;
                    }
                };



                btn_delete.SetBackgroundResource(Resource.Drawable.delete_icon_acc);

                if (account.IsPreRegister)
                {
                    viewHolder.txtAccountName.Text = account.email;
                    viewHolder.txtAccountNum.Visibility = ViewStates.Visible;
                    viewHolder.txtAccountNum.Text = Utility.GetLocalizedLabel("UserAccess", "pendingRegistration");
                    //viewHolder.txtAccountNum.TextSize = TextViewUtils.GetFontSize(14);
                }
                else
                {
                    viewHolder.txtAccountName.Text = account.name;
                    viewHolder.txtAccountNum.Visibility = ViewStates.Gone;
                }

            }
            else
            {
                viewHolder = convertView.Tag as MyAccountViewHolder;
            }

            return convertView;
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            throw new System.NotImplementedException();
        }

        public void OnAddAccount()
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void OnRemoveAccount(List<UserManageAccessAccount> DeletedSelectedUser, List<Models.DeleteAccessAccount> accounts, string AccountNum)
        {
            throw new NotImplementedException();
        }

        public void OnAddLogUserAccess(AccountData accountData)
        {
            throw new NotImplementedException();
        }

        class MyAccountViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtAccountName)]
            internal TextView txtAccountName;

            [BindView(Resource.Id.txtAccountNum)]
            internal TextView txtAccountNum;


            public MyAccountViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtAccountNum);
                TextViewUtils.SetMuseoSans500Typeface(txtAccountName);

                TextViewUtils.SetTextSize14(txtAccountNum);
                TextViewUtils.SetTextSize12(txtAccountName);
            }
        }
    }
}