using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
using AndroidViewAnimations;
using CheeseBind;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyAccount.Activity;
using myTNB.AndroidApp.Src.MyAccount.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MyAccount.Adapter
{
    internal class MyAccountAdapter : BaseCustomAdapter<CustomerBillingAccount>, MyAccountContract.IUserActionsListener
    {
        private MyAccountContract.IView mView;
        private AccountData accountData;
        customButtonListener customListner;

        public MyAccountAdapter(Context context) : base(context)
        {
        }

        public MyAccountAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public MyAccountAdapter(Context context, List<CustomerBillingAccount> itemList) : base(context, itemList)
        {
        }

        public MyAccountAdapter(Context context, List<CustomerBillingAccount> itemList, bool notify) : base(context, itemList, notify)
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

            CustomerBillingAccount account = GetItemObject(position);
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.MyAccountRowNew, parent, false);
                Button btn_delete = (Button)convertView.FindViewById(Resource.Id.delete);
                SwipeLayout swipeLayout = (SwipeLayout)convertView.FindViewById(Resource.Id.swipe);
                viewHolder = new MyAccountViewHolder(convertView);
                convertView.Tag = viewHolder;

                btn_delete.Click += (sender, e) =>
                {
                    if (customListner != null)
                    {
                        customListner.onButtonClickListner(position);
                        return;
                    }
                };

                btn_delete.SetBackgroundResource(Resource.Drawable.delete_icon_acc);
                viewHolder.txtAccountName.Text = account.AccDesc;
                viewHolder.txtAccountNum.Text = account.AccNum;

                if (account.AccountCategoryId.Equals("2"))
                {
                    viewHolder.imageLeaf.Visibility = ViewStates.Visible;
                }
                else
                {
                    viewHolder.imageLeaf.Visibility = ViewStates.Invisible;
                }
                // viewHolder.txtAccountName.Text = account.AccDesc;
                // viewHolder.txtAccountNum.Text = account.AccNum;
                // viewHolder.txtAccountManage.Text = Utility.GetLocalizedLabel("Common", "manage");

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

        public void OnRemoveAccount(string numacc, bool isOwner, bool IsInManageAccessList)
        {
            throw new NotImplementedException();
        }

        class MyAccountViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtAccountName)]
            internal TextView txtAccountName;

            [BindView(Resource.Id.txtAccountNum)]
            internal TextView txtAccountNum;

            [BindView(Resource.Id.imageLeaf)]
            public ImageView imageLeaf;

            public MyAccountViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtAccountName, txtAccountNum);
                //TextViewUtils.SetMuseoSans500Typeface(txtAccountManage);

               
                TextViewUtils.SetTextSize14(txtAccountName, txtAccountNum);
                //txtAccountManage.TextSize = TextViewUtils.GetFontSize(14f);

                // TextViewUtils.SetMuseoSans500Typeface(txtAccountManage);
                // TextViewUtils.SetTextSize14(txtAccountName, txtAccountNum, txtAccountManage);
            }
        }
    }
}