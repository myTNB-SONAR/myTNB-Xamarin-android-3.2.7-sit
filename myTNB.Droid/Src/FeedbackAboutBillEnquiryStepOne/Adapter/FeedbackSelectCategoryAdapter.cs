using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;

namespace myTNB_Android.Src.FeedbackAboutBillEnquiryStepOne.Adapter
{
    public class FeedbackSelectCategoryAdapter : BaseCustomAdapter<AccountData>
    {
        public FeedbackSelectCategoryAdapter(Context context) : base(context)
        {
        }

        public FeedbackSelectCategoryAdapter(Context context, bool notify) : base(context, notify)
        {
        }

        public FeedbackSelectCategoryAdapter(Context context, List<AccountData> itemList) : base(context, itemList)
        {
        }

        public FeedbackSelectCategoryAdapter(Context context, List<AccountData> itemList, bool notify) : base(context, itemList, notify)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            AccountListViewHolder vh = null;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.FeedbackSelectAccountRow, parent, false);
                vh = new AccountListViewHolder(convertView);
                convertView.Tag = vh;
            }
            else
            {
                vh = convertView.Tag as AccountListViewHolder;

            }

            AccountData item = GetItemObject(position);
            vh.txtSupplyAccountName.Text = item.AccountNickName;
            TextViewUtils.SetTextSize16(vh.txtSupplyAccountName);

            if (item.IsSelected)
            {
                vh.imageActionIcon.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.imageActionIcon.Visibility = ViewStates.Gone;

            }

            return convertView;
        }

    }

    public class AccountListViewHolder : BaseAdapterViewHolder
    {
        [BindView(Resource.Id.txtSupplyAccountName)]
        public TextView txtSupplyAccountName;

        [BindView(Resource.Id.imageActionIcon)]
        public ImageView imageActionIcon;

        public AccountListViewHolder(View itemView) : base(itemView)
        {
            TextViewUtils.SetMuseoSans300Typeface(txtSupplyAccountName);
        }
    }

}