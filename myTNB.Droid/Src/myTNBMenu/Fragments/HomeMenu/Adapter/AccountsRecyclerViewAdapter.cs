using System;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class AccountsRecyclerViewAdapter : RecyclerView.Adapter
    {
        int accountsContainer = 0;
        ViewGroup parentGroup;
        public AccountsRecyclerViewAdapter(int count)
        {
            accountsContainer = count;
        }

        public override int ItemCount => accountsContainer;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AccountsContainerViewHolder viewHolder = holder as AccountsContainerViewHolder;

            viewHolder.linearLayout.AddView(CreateAccountCard());
            viewHolder.linearLayout.AddView(CreateAccountCard());
            viewHolder.linearLayout.AddView(CreateAccountCard());
            viewHolder.linearLayout.AddView(CreateAccountCard());
            viewHolder.linearLayout.AddView(CreateAccountCard());

        }

        private CoordinatorLayout CreateAccountCard()
        {
            CoordinatorLayout card = (CoordinatorLayout)LayoutInflater.From(parentGroup.Context).Inflate(Resource.Layout.card_layout, parentGroup, false);
            TextView accountName = card.FindViewById(Resource.Id.accountName) as TextView;
            TextView accountNumber = card.FindViewById(Resource.Id.accountNumber) as TextView;
            TextView billDueAmount = card.FindViewById(Resource.Id.billDueAmount) as TextView;
            TextView billDueDate = card.FindViewById(Resource.Id.billDueDate) as TextView;
            TextViewUtils.SetMuseoSans500Typeface(accountName, billDueAmount);
            TextViewUtils.SetMuseoSans300Typeface(accountNumber, billDueDate);
            return card;
        }

        //private CardView CreateCard()
        //{
        //    CardView card = new CardView(parentGroup.Context);
        //    LayoutParams layoutParams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        //    layoutParams.SetMargins(15, 12, 15, 12);
        //    card.LayoutParameters = layoutParams;
        //    card.UseCompatPadding = true;
        //    card.Elevation = 4;
        //    card.Radius = 3;
        //    return card;
        //}

        //private ViewGroup CreateCardContainer()
        //{
        //    ConstraintLayout constraintLayout = new ConstraintLayout(parentGroup.Context);
        //    LayoutParams layoutParams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        //    layoutParams.SetMargins(15, 12, 15, 12);
        //    constraintLayout.LayoutParameters = layoutParams;


        //    ImageView accountImage = new ImageView(parentGroup.Context);
        //    LayoutParams imageLayoutParams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
        //    accountImage.LayoutParameters = imageLayoutParams;

        //    return constraintLayout;
        //}



        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.account_container_adapter;
            parentGroup = parent;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new AccountsContainerViewHolder(itemView);
        }

        public class AccountsContainerViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout linearLayout;
            public AccountsContainerViewHolder(View itemView) : base(itemView)
            {
                linearLayout = itemView as LinearLayout;
            }
        }
    }
}
