using System;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Helper;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class AccountsRecyclerViewAdapter : RecyclerView.Adapter, IFilterable
    {
        int accountsCardContainer = 0;
        int MAX_ACCOUNT_PER_CARD = 5;
        Filter accountsFilter;
        static HomeMenuContract.IHomeMenuView viewListener;

        List<List<AccountCardModel>> cardList = new List<List<AccountCardModel>>();
        List<AccountCardModel> accountModelList = new List<AccountCardModel>();
        public List<AccountCardModel> accountCardModelList;
        ViewGroup parentGroup;

        public AccountsRecyclerViewAdapter(HomeMenuContract.IHomeMenuView listener)
        {
            viewListener = listener;

            //BitmapFactory.Options dimensions = new BitmapFactory.Options();
            //dimensions.inJustDecodeBounds = true;
            //Bitmap mBitmap = BitmapFactory.decodeResource(getResources(), R.drawable.bitmap, dimensions);
            //int height = dimensions.outHeight;
            //int width = dimensions.outWidth;
        }

        public int GetAccountCardCount(List<AccountCardModel> list)
        {
            int cardCount;
            if (list.Count > 0)
            {
                cardCount = list.Count / MAX_ACCOUNT_PER_CARD;
                if ((list.Count % MAX_ACCOUNT_PER_CARD) > 0)
                {
                    cardCount++;
                }
            }
            else
            {
                cardCount = 1;
            }
            return cardCount;
        }

        public void UpdatedCardList()
        {
            cardList = new List<List<AccountCardModel>>();
            int cardsCounter = 0;
            int cardContainerCount = GetAccountCardCount(accountCardModelList);
            for (int i = 0; i < cardContainerCount; i++)
            {
                accountModelList = new List<AccountCardModel>();
                for (int j = cardsCounter; j < accountCardModelList.Count; j++)
                {
                    AccountCardModel model = accountCardModelList.ToArray()[j];
                    accountModelList.Add(model);
                    if (accountModelList.Count == MAX_ACCOUNT_PER_CARD)
                    {
                        cardsCounter = j;
                        cardsCounter++;
                        break;
                    }
                }
                cardList.Add(accountModelList);
            }
            viewListener.OnUpdateAccountListChanged(false);
        }

        public void SetAccountCards(List<SummaryDashBoardDetails> accountList)
        {

            accountCardModelList = GetAccountCardModelList(accountList);
            UpdatedCardList();
        }

        public void UpdateAccountCards(List<SummaryDashBoardDetails> accountList)
        {
            foreach (SummaryDashBoardDetails summaryDashBoardDetails in accountList)
            {
                foreach (AccountCardModel cardModel in accountCardModelList)
                {
                    if (cardModel.AccountNumber == summaryDashBoardDetails.AccNumber)
                    {
                        int accountType = Int32.Parse(summaryDashBoardDetails.AccType);
                        cardModel.AccountName = summaryDashBoardDetails.AccName;
                        cardModel.BillDueAmount = AccountModelFormatter.GetFormatAmount(summaryDashBoardDetails.AmountDue);
                        cardModel.BillDueNote = AccountModelFormatter.GetBillDueNote(accountType,
                            summaryDashBoardDetails.AmountDue, summaryDashBoardDetails.BillDueDate);
                        cardModel.AccountType = accountType;
                    }
                }
            }
            UpdatedCardList();
        }

        private List<AccountCardModel> GetAccountCardModelList(List<SummaryDashBoardDetails> accountList)
        {
            List<AccountCardModel> returnAccountCardModelList = new List<AccountCardModel>();
            AccountCardModel model;

            foreach (SummaryDashBoardDetails summaryDashBoardDetails in accountList)
            {
                model = new AccountCardModel();
                //model.AccountType = Int32.Parse(summaryDashBoardDetails.AccType);
                model.AccountName = summaryDashBoardDetails.AccName;
                model.AccountNumber = summaryDashBoardDetails.AccNumber;
                model.BillDueAmount = summaryDashBoardDetails.AmountDue;
                model.BillDueNote = summaryDashBoardDetails.BillDueDate;
                returnAccountCardModelList.Add(model);
            }
            return returnAccountCardModelList;
        }

        public override int ItemCount => GetAccountCardCount(accountCardModelList);

        public Filter Filter => GetFilter();

        private Filter GetFilter()
        {
            if (accountsFilter == null)
            {
                accountsFilter = new AccountCardsFilter(this, accountCardModelList);
            }
            return accountsFilter;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AccountsContainerViewHolder viewHolder = holder as AccountsContainerViewHolder;
            viewHolder.IsRecyclable = false;
            List<AccountCardModel> accountCardModel = cardList.ToArray()[position];
            foreach (AccountCardModel cardModel in accountCardModel)
            {
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                if (position == 0)
                {
                    layoutParams.LeftMargin = 32;
                    layoutParams.RightMargin = 16;
                    viewHolder.linearLayout.LayoutParameters = layoutParams;
                }
                else
                {
                    layoutParams.RightMargin = 16;
                    viewHolder.linearLayout.LayoutParameters = layoutParams;
                }
                CoordinatorLayout shimmerLayoutContainer = (CoordinatorLayout)LayoutInflater.From(parentGroup.Context).Inflate(Resource.Layout.account_card_shimmer_layout, parentGroup, false);

                ShimmerLoadingLayout.GetInstance().AddViewWithShimmer(parentGroup.Context,viewHolder.linearLayout,CreateAccountCard(cardModel),
                    shimmerLayoutContainer,
                    () =>
                    {
                        return cardModel.BillDueAmount != null;
                    });
            }
        }

        private int GetAccountIcon(int AccountType)
        {
            int iconResource;
            switch (AccountType)
            {
                case 1:
                    iconResource = Resource.Drawable.ic_display_normal_meter;
                    break;
                case 2:
                    iconResource = Resource.Drawable.ic_display_r_eleaf;
                    break;
                default:
                    iconResource = Resource.Drawable.ic_display_smart_meter;
                    break;
            }
            return iconResource;
        }

        private CoordinatorLayout CreateAccountCard(AccountCardModel cardModel)
        {
            CoordinatorLayout card = (CoordinatorLayout)LayoutInflater.From(parentGroup.Context).Inflate(Resource.Layout.card_layout, parentGroup, false);
            card.SetOnClickListener(new OnAccountCardClickListener(cardModel.AccountNumber));
            ImageView accountTypeIcon = card.FindViewById(Resource.Id.accountIcon) as ImageView;
            TextView accountName = card.FindViewById(Resource.Id.accountName) as TextView;
            TextView accountNumber = card.FindViewById(Resource.Id.accountNumber) as TextView;
            TextView billDueAmount = card.FindViewById(Resource.Id.billDueAmount) as TextView;
            TextView billDueNote = card.FindViewById(Resource.Id.billDueDate) as TextView;

            accountName.Id = cardModel.Id + 1;
            accountNumber.Id = cardModel.Id + 2;
            billDueAmount.Id = cardModel.Id + 3;
            billDueNote.Id = cardModel.Id + 4;
            card.Id = cardModel.Id + 5;

            accountName.Text = cardModel.AccountName;
            accountNumber.Text = cardModel.AccountNumber;
            billDueAmount.Text = cardModel.BillDueAmount;
            billDueNote.Text = cardModel.BillDueNote;

            accountTypeIcon.SetImageResource(GetAccountIcon(cardModel.AccountType));

            TextViewUtils.SetMuseoSans500Typeface(accountName, billDueAmount);
            TextViewUtils.SetMuseoSans300Typeface(accountNumber, billDueNote);
            return card;
        }

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

        public class OnAccountCardClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private string mAccountNumber = null;
            public OnAccountCardClickListener(string accountNumber)
            {
                mAccountNumber = accountNumber;
            }
            public void OnClick(View v)
            {
                viewListener.ShowAccountDetails(mAccountNumber);
            }
        }
    }
}
