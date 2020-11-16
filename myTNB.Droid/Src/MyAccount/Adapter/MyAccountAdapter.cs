using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidSwipeLayout;
using AndroidViewAnimations;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyAccount.Adapter
{
    internal class MyAccountAdapter : BaseCustomAdapter<CustomerBillingAccount>
    {
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

                swipeLayout.Opened += (sender, e) =>
                {
                    YoYo.With(Techniques.Tada)
                        .Duration(500)
                        .Delay(100)
                        .PlayOn(e.Layout.FindViewById(Resource.Id.trash));
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