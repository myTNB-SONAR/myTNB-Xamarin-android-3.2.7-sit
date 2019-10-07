using System;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Helper;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter
{
    public class AccountsRecyclerViewAdapter : RecyclerView.Adapter, IFilterable
    {
        int accountsCardContainer = 0;
        int MAX_ACCOUNT_PER_CARD = 5;
        Filter accountsFilter;
        static HomeMenuFragment viewListener;

        List<List<AccountCardModel>> cardList = new List<List<AccountCardModel>>();
        List<AccountCardModel> accountModelList = new List<AccountCardModel>();
        public List<AccountCardModel> accountCardModelList = new List<AccountCardModel>();
        ViewGroup parentGroup;

        public AccountsRecyclerViewAdapter(HomeMenuContract.IHomeMenuView listener)
        {
            viewListener = listener as HomeMenuFragment;

            //BitmapFactory.Options dimensions = new BitmapFactory.Options();
            //dimensions.inJustDecodeBounds = true;
            //Bitmap mBitmap = BitmapFactory.decodeResource(getResources(), R.drawable.bitmap, dimensions);
            //int height = dimensions.outHeight;
            //int width = dimensions.outWidth;
        }

        public int GetAccountCardCount(List<AccountCardModel> list)
        {
            int cardCount;
            if (list != null && list.Count > 0)
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

        public void SetAccountCardsFromLocal(List<SummaryDashBoardDetails> accountList)
        {

            accountCardModelList = GetAccountCardModelListFromLocal(accountList);
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
                        cardModel.AccountNumber = summaryDashBoardDetails.AccNumber;
                        cardModel.AccountName = summaryDashBoardDetails.AccName;
                        cardModel.IsNegativeAmount = (accountType == 2) ? false : AccountModelFormatter.IsNegativeAmount(summaryDashBoardDetails.AmountDue);
                        cardModel.IsZeroAmount = AccountModelFormatter.IsAmountCleared(summaryDashBoardDetails.AmountDue);
                        cardModel.BillDueAmount = AccountModelFormatter.GetFormatAmount(summaryDashBoardDetails.AmountDue);
                        cardModel.BillDueNote = AccountModelFormatter.GetBillDueNote(accountType,
                            summaryDashBoardDetails.AmountDue, summaryDashBoardDetails.BillDueDate, summaryDashBoardDetails.IsTaggedSMR);
                        cardModel.AccountType = accountType;
                        cardModel.IsTaggedSMR = summaryDashBoardDetails.IsTaggedSMR;
                        if (summaryDashBoardDetails.SmartMeterCode == "0")
                        {
                            cardModel.SmartMeterCode = Int32.Parse(summaryDashBoardDetails.SmartMeterCode);
                        }
                        else
                        {
                            cardModel.SmartMeterCode = 3;
                        }
                    }
                }
            }
            NotifyDataSetChanged();
        }

        private List<AccountCardModel> GetAccountCardModelListFromLocal(List<SummaryDashBoardDetails> accountList)
        {
            List<AccountCardModel> returnAccountCardModelList = new List<AccountCardModel>();
            AccountCardModel cardModel;

            if (accountList != null && accountList.Count > 0)
            {
                foreach (SummaryDashBoardDetails summaryDashBoardDetails in accountList)
                {
                    int accountType = Int32.Parse(summaryDashBoardDetails.AccType);
                    cardModel = new AccountCardModel();
                    cardModel.AccountNumber = summaryDashBoardDetails.AccNumber;
                    if (summaryDashBoardDetails.AmountDue != null)
                    {
                        cardModel.AccountType = accountType;
                        cardModel.AccountName = summaryDashBoardDetails.AccName;
                        cardModel.IsNegativeAmount = (accountType == 2) ? false : AccountModelFormatter.IsNegativeAmount(summaryDashBoardDetails.AmountDue);
                        cardModel.IsZeroAmount = AccountModelFormatter.IsAmountCleared(summaryDashBoardDetails.AmountDue);
                        cardModel.BillDueAmount = AccountModelFormatter.GetFormatAmount(summaryDashBoardDetails.AmountDue);
                        cardModel.BillDueNote = AccountModelFormatter.GetBillDueNote(accountType,
                            summaryDashBoardDetails.AmountDue, summaryDashBoardDetails.BillDueDate, summaryDashBoardDetails.IsTaggedSMR);
                        cardModel.IsTaggedSMR = summaryDashBoardDetails.IsTaggedSMR;
                        if (summaryDashBoardDetails.SmartMeterCode == "0")
                        {
                            cardModel.SmartMeterCode = Int32.Parse(summaryDashBoardDetails.SmartMeterCode);
                        }
                        else
                        {
                            cardModel.SmartMeterCode = 3;
                        }
                    }
                    returnAccountCardModelList.Add(cardModel);
                }
            }
            return returnAccountCardModelList;
        }

        private List<AccountCardModel> GetAccountCardModelList(List<SummaryDashBoardDetails> accountList)
        {
            List<AccountCardModel> returnAccountCardModelList = new List<AccountCardModel>();
            AccountCardModel model;

            if (accountList != null && accountList.Count > 0)
            {
                foreach (SummaryDashBoardDetails summaryDashBoardDetails in accountList)
                {
                    int accountType = Int32.Parse(summaryDashBoardDetails.AccType);
                    model = new AccountCardModel();
                    model.AccountName = summaryDashBoardDetails.AccName;
                    model.AccountNumber = summaryDashBoardDetails.AccNumber;
                    model.IsNegativeAmount = (accountType == 2) ? false : AccountModelFormatter.IsNegativeAmount(summaryDashBoardDetails.AmountDue);
                    model.IsZeroAmount = AccountModelFormatter.IsAmountCleared(summaryDashBoardDetails.AmountDue);
                    model.BillDueAmount = summaryDashBoardDetails.AmountDue;
                    model.BillDueNote = summaryDashBoardDetails.BillDueDate;
                    model.AccountType = accountType;
                    model.IsTaggedSMR = summaryDashBoardDetails.IsTaggedSMR;
                    if (summaryDashBoardDetails.SmartMeterCode == "0")
                    {
                        model.SmartMeterCode = Int32.Parse(summaryDashBoardDetails.SmartMeterCode);
                    }
                    else
                    {
                        model.SmartMeterCode = 3;
                    }
                    returnAccountCardModelList.Add(model);
                }
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
            DisplayMetrics displayMetrics = new DisplayMetrics();
            int deviceWidth = (int) (parentGroup.Context.Resources.DisplayMetrics.WidthPixels / parentGroup.Context.Resources.DisplayMetrics.Density);
            foreach (AccountCardModel cardModel in accountCardModel)
            {
                float scale = parentGroup.Context.Resources.DisplayMetrics.Density;
                int width = viewListener.GetDeviceHorizontalScaleInPixel(0.9f);
                LinearLayout.LayoutParams layoutParams;
                if (cardList.Count > 1)
                {
                    layoutParams = new LinearLayout.LayoutParams(width,
                    LinearLayout.LayoutParams.WrapContent);
                    layoutParams.BottomMargin = (int)(10 * scale + 0.5f);
                    if (position == 0)
                    {
                        layoutParams.LeftMargin = viewListener.GetDeviceHorizontalScaleInPixel(0.05f);
                        layoutParams.RightMargin = viewListener.GetDeviceHorizontalScaleInPixel(0.025f);
                        viewHolder.linearLayout.LayoutParameters = layoutParams;
                    }
                    else
                    {
                        layoutParams.RightMargin = viewListener.GetDeviceHorizontalScaleInPixel(0.025f);
                        layoutParams.RightMargin = viewListener.GetDeviceHorizontalScaleInPixel(0.025f);
                        viewHolder.linearLayout.LayoutParameters = layoutParams;
                    }
                }
                else
                {
                    layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                    LinearLayout.LayoutParams.WrapContent);
                    layoutParams.LeftMargin = viewListener.GetDeviceHorizontalScaleInPixel(0.05f);
                    layoutParams.RightMargin = viewListener.GetDeviceHorizontalScaleInPixel(0.05f);
                    viewHolder.linearLayout.LayoutParameters = layoutParams;
                }

                CoordinatorLayout shimmerLayoutContainer = (CoordinatorLayout)LayoutInflater.From(parentGroup.Context).Inflate(Resource.Layout.account_card_shimmer_layout, parentGroup, false);
                TextView accountNameShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.accountNameShimmer) as TextView;
                TextView accountNumberShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.accountNumberShimmer) as TextView;
                TextView billDueAmountShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.billDueAmountShimmer) as TextView;
                TextView billDueNoteShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.billDueDateShimmer) as TextView;

                TextViewUtils.SetMuseoSans500Typeface(accountNameShimmer, billDueAmountShimmer);
                TextViewUtils.SetMuseoSans300Typeface(accountNumberShimmer, billDueNoteShimmer);

                ShimmerLoadingLayout.GetInstance().AddViewWithShimmer(parentGroup.Context,viewHolder.linearLayout,CreateAccountCard(cardModel),
                    shimmerLayoutContainer,
                    () =>
                    {
                        return MyTNBAccountManagement.GetInstance().HasUpdatedBillingDetails(cardModel.AccountNumber);
                    });
            }
        }

        private bool IsOwnedSMR(string accountNumber)
        {
            foreach (SMRAccount smrAccount in UserSessions.GetSMRAccountList())
            {
                if (smrAccount.accountNumber == accountNumber)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetAccountIcon(int AccountType, int SmartMeterCode, bool IsTaggedSMR, string AccountNumber)
        {
            int iconResource;
            if (AccountType == 2)
            {
                iconResource = Resource.Drawable.ic_display_r_eleaf;
            }
            else
            {
                if (SmartMeterCode != 0)
                {
                    iconResource = Resource.Drawable.ic_display_smart_meter;
                }
                else
                {

                    if (IsOwnedSMR(AccountNumber) && IsTaggedSMR)
                    {
                        
                        iconResource = Resource.Drawable.smr_48_x_48;
                    }
                    else
                    {
                        iconResource = Resource.Drawable.ic_display_normal_meter;
                    }
                }
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

            if (cardModel.IsZeroAmount)
            {
                billDueAmount.SetTextColor(parentGroup.Context.Resources.GetColor(Resource.Color.all_cleared_amount, null));
            }
            if (cardModel.IsNegativeAmount)
            {
                billDueAmount.SetTextColor(parentGroup.Context.Resources.GetColor(Resource.Color.freshGreen, null));
            }

            accountTypeIcon.SetImageResource(GetAccountIcon(cardModel.AccountType, cardModel.SmartMeterCode, cardModel.IsTaggedSMR, cardModel.AccountNumber));

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

        public List<string> GetFirstCardModelList()
		{
            List<string> accountNumberList = new List<string>();
            if (cardList.Count > 0)
			{
				foreach(AccountCardModel cardModel in cardList.ToArray()[0])
				{
					accountNumberList.Add(cardModel.AccountNumber);
				}
			}
            return accountNumberList;
        }

        public List<string> GetAccountCardNumberListByPosition(int position)
        {
            List<string> accountNumberList = new List<string>();
            if (cardList.Count > 0)
            {
                foreach (AccountCardModel cardModel in cardList.ToArray()[position])
                {
                    if (!MyTNBAccountManagement.GetInstance().HasUpdatedBillingDetails(cardModel.AccountNumber))
                    {
                        accountNumberList.Add(cardModel.AccountNumber);
                    }
                }
            }
            return accountNumberList;
        }
    }
}
