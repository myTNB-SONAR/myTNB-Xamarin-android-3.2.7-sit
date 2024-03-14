using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.Base.Adapter;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB.Android.Src.MyProfileDetail.Adapter
{
    internal class MyProfileAdapter : BaseCustomAdapter<CustomerBillingAccount>
    {
        public MyProfileAdapter(Context context) : base(context)
        {
        }

        public MyProfileAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public MyProfileAdapter(Context context, List<CustomerBillingAccount> itemList) : base(context, itemList)
        {
        }

        public MyProfileAdapter(Context context, List<CustomerBillingAccount> itemList, bool notify) : base(context, itemList, notify)
        {
        }



        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MyAccountViewHolder viewHolder = null;
            CustomerBillingAccount account = GetItemObject(position);
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.MyAccountRow, parent, false);
                viewHolder = new MyAccountViewHolder(convertView);
                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as MyAccountViewHolder;
            }

            viewHolder.txtAccountName.Text = account.AccDesc;
            viewHolder.txtAccountNum.Text = account.AccNum;
            viewHolder.txtAccountManage.Text = Utility.GetLocalizedLabel("Common","manage");

            if (account.AccountCategoryId.Equals("2"))
            {
                viewHolder.imageLeaf.Visibility = ViewStates.Visible;
            }
            else
            {
                viewHolder.imageLeaf.Visibility = ViewStates.Invisible;
            }

            return convertView;
        }

        class MyAccountViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtAccountName)]
            internal TextView txtAccountName;

            [BindView(Resource.Id.txtAccountNum)]
            internal TextView txtAccountNum;

            [BindView(Resource.Id.txtAccountManage)]
            internal TextView txtAccountManage;

            [BindView(Resource.Id.imageLeaf)]
            public ImageView imageLeaf;

            public MyAccountViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtAccountName, txtAccountNum);
                TextViewUtils.SetMuseoSans500Typeface(txtAccountManage);
            }
        }
    }
}