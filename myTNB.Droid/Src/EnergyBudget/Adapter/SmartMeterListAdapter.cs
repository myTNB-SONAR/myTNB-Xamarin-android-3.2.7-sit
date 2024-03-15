using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP;
using myTNB.AndroidApp.Src.Utils;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.EnergyBudget.Adapter
{
    internal class SmartMeterListAdapter : BaseCustomAdapter<SMRAccount>
    {
        public SmartMeterListAdapter(Context context) : base(context)
        {
        }

        public SmartMeterListAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public SmartMeterListAdapter(Context context, List<SMRAccount> itemList) : base(context, itemList)
        {
        }

        public SmartMeterListAdapter(Context context, List<SMRAccount> itemList, bool notify) : base(context, itemList, notify)
        {
        }



        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MyAccountViewHolder viewHolder = null;
            SMRAccount account = GetItemObject(position);
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.MySmartMeterRow, parent, false);
                viewHolder = new MyAccountViewHolder(convertView);
                convertView.Tag = viewHolder;
            }
            else
            {
                viewHolder = convertView.Tag as MyAccountViewHolder;
            }

            viewHolder.txtAccountName.Text = account.accountName;
            TextViewUtils.SetTextSize16(viewHolder.txtAccountName);

            if (account.accountSelected)
            {
                viewHolder.imageActionIconTick.Visibility = ViewStates.Visible;
            }
            else
            {
                viewHolder.imageActionIconTick.Visibility = ViewStates.Invisible;
            }

            return convertView;
        }

        class MyAccountViewHolder : BaseAdapterViewHolder
        {
            [BindView(Resource.Id.txtAccountName)]
            internal TextView txtAccountName;

            [BindView(Resource.Id.imageActionIconTick)]
            internal ImageView imageActionIconTick;

            public MyAccountViewHolder(View itemView) : base(itemView)
            {
                TextViewUtils.SetMuseoSans300Typeface(txtAccountName);
                TextViewUtils.SetTextSize16(txtAccountName);
            }
        }
    }
}