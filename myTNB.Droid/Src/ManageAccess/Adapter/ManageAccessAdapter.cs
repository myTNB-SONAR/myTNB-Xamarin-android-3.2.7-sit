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
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.MyAccountRowNew, parent, false);
                Button btn_delete = (Button)convertView.FindViewById(Resource.Id.delete);
                SwipeLayout swipeLayout = (SwipeLayout)convertView.FindViewById(Resource.Id.swipe);
                viewHolder = new MyAccountViewHolder(convertView);
                convertView.Tag = viewHolder;

                swipeLayout.Opened += (sender, e) =>
                {
                    YoYo.With(Techniques.Tada)
                        .Duration(500)
                        .Delay(100)
                        .PlayOn(e.Layout.FindViewById(Resource.Id.trash));
                };

                btn_delete.Click += (sender, e) =>
                {
                    if (customListner != null)
                    {
                        customListner.onButtonClickListner(position);
                        return;
                    }

                    //CustomerBillingAccount account = GetItemObject(position);
                    //CustomerBillingAccount.Remove(account.AccNum);
                    //OnRemoveAccount(account.AccNum);
                    //NotifyDataSetChanged();
                    //((MyAccountActivity)mContext).ShowAddAccount();
                };



                btn_delete.SetBackgroundResource(Resource.Drawable.delete_icon_acc);
                viewHolder.txtAccountName.Text = account.AccDesc;
                viewHolder.txtAccountNum.Text = account.AccNum;

                //viewHolder.txtAccountManage.Text = Utility.GetLocalizedLabel("Common", "manage");

                /*if (account.AccountCategoryId.Equals("2"))
                {
                    viewHolder.imageLeaf.Visibility = ViewStates.Visible;
                }
                else
                {
                    viewHolder.imageLeaf.Visibility = ViewStates.Invisible;
                }*/

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

        public void OnRemoveAccount(string numacc)
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
                TextViewUtils.SetMuseoSans300Typeface( txtAccountNum);
                TextViewUtils.SetMuseoSans500Typeface(txtAccountName);
            }
        }
    }
}