using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;



using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
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
        int INTIATE_ACCOUNT_PER_CARD = 3;
        int MAX_ACCOUNT_PER_CARD = 5;
        Filter accountsFilter;
        static HomeMenuFragment viewListener;
        private ISharedPreferences mSharedPref;

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

        public void UpdatedCardList()
        {
            accountModelList = new List<AccountCardModel>();
            for (int i = 0; i < accountCardModelList.Count; i++)
            {
                AccountCardModel model = accountCardModelList[i];
                accountModelList.Add(model);
            }
            viewListener.OnUpdateAccountListChanged(false);
        }

        public void SetAccountCards(List<SummaryDashBoardDetails> accountList)
        {

            accountCardModelList = GetAccountCardModelList(accountList);
            UpdatedCardList();
            NotifyDataSetChanged();
        }

        public void SetAccountCardsFromLocal(List<SummaryDashBoardDetails> accountList)
        {

            accountCardModelList = GetAccountCardModelListFromLocal(accountList);
            UpdatedCardList();
            NotifyDataSetChanged();
        }

        public void UpdateAccountCards(List<SummaryDashBoardDetails> accountList)
        {
            try
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public override int ItemCount => accountCardModelList.Count;

        public Filter Filter => GetFilter();

        private Filter GetFilter()
        {
            if (accountsFilter == null)
            {
                accountsFilter = new AccountCardsFilter(viewListener, this, accountCardModelList);
            }
            return accountsFilter;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AccountsContainerViewHolder viewHolder = holder as AccountsContainerViewHolder;
            viewHolder.IsRecyclable = false;
            AccountCardModel cardModel = accountModelList[position];
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.WrapContent);
            if (accountCardModelList != null && accountCardModelList.Count == 1)
            {
                layoutParams.TopMargin = (int)DPUtils.ConvertDPToPx(8f);
            }
            viewHolder.linearLayout.LayoutParameters = layoutParams;

            CoordinatorLayout shimmerLayoutContainer = (CoordinatorLayout)LayoutInflater.From(parentGroup.Context).Inflate(Resource.Layout.account_card_shimmer_layout, parentGroup, false);
            LinearLayout accountCard = shimmerLayoutContainer.FindViewById<LinearLayout>(Resource.Id.accountCard);
            ShimmerFrameLayout accountContainer = accountCard.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmerConstainer);
            LinearLayout.LayoutParams layout = accountContainer.LayoutParameters as LinearLayout.LayoutParams;
            if (accountCardModelList != null && accountCardModelList.Count == 1)
            {
                layout.BottomMargin = (int)DPUtils.ConvertDPToPx(4f);
                accountContainer.LayoutParameters = layout;
            }
            TextView accountNameShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.accountNameShimmer) as TextView;
            TextView accountNumberShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.accountNumberShimmer) as TextView;
            TextView billDueAmountShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.billDueAmountShimmer) as TextView;
            TextView billDueNoteShimmer = shimmerLayoutContainer.FindViewById(Resource.Id.billDueDateShimmer) as TextView;

            TextViewUtils.SetMuseoSans500Typeface(accountNameShimmer, billDueAmountShimmer);
            TextViewUtils.SetMuseoSans300Typeface(accountNumberShimmer, billDueNoteShimmer);
            TextViewUtils.SetTextSize12(accountNameShimmer, accountNumberShimmer, billDueAmountShimmer, billDueNoteShimmer);

            ShimmerLoadingLayout.GetInstance().AddViewWithShimmer(parentGroup.Context, viewHolder.linearLayout, CreateAccountCard(cardModel),
                shimmerLayoutContainer,
                () =>
                {
                    return MyTNBAccountManagement.GetInstance().HasUpdatedBillingDetails(cardModel.AccountNumber);
                });
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

        private CoordinatorLayout CreateAccountCard(AccountCardModel cardModel)
        {
            CoordinatorLayout card = (CoordinatorLayout)LayoutInflater.From(parentGroup.Context).Inflate(Resource.Layout.card_layout, parentGroup, false);
            LinearLayout accountCard = card.FindViewById<LinearLayout>(Resource.Id.accountCard);
            ConstraintLayout accountContainer = accountCard.FindViewById<ConstraintLayout>(Resource.Id.accountContainer);
            LinearLayout.LayoutParams layout = accountContainer.LayoutParameters as LinearLayout.LayoutParams;
            if (accountCardModelList != null && accountCardModelList.Count == 1)
            {
                layout.BottomMargin = (int)DPUtils.ConvertDPToPx(4f);
                accountContainer.LayoutParameters = layout;
            }
            card.SetOnClickListener(new OnAccountCardClickListener(cardModel.AccountNumber));
            ImageView accountTypeIcon = card.FindViewById(Resource.Id.accountIcon) as ImageView;
            TextView accountName = card.FindViewById(Resource.Id.accountName) as TextView;
            TextView accountNumber = card.FindViewById(Resource.Id.accountNumber) as TextView;
            TextView billDueAmount = card.FindViewById(Resource.Id.billDueAmount) as TextView;
            TextView billDueNote = card.FindViewById(Resource.Id.billDueDate) as TextView;
            TextViewUtils.SetTextSize12(accountName, accountNumber, billDueAmount, billDueNote);

            accountName.Id = cardModel.Id + 1;
            accountNumber.Id = cardModel.Id + 2;
            billDueAmount.Id = cardModel.Id + 3;
            billDueNote.Id = cardModel.Id + 4;
            card.Id = cardModel.Id + 5;

            accountName.Text = cardModel.AccountName;
            accountNumber.Text = cardModel.AccountNumber;
            billDueAmount.Text = cardModel.BillDueAmount;
            billDueNote.Text = cardModel.BillDueNote;

            Paint mPaint = new Paint();
            mPaint.AntiAlias = true;
            mPaint.TextSize = DPUtils.ConvertDPToPx(12f);
            Typeface plain = Typeface.CreateFromAsset(parentGroup.Context.Assets, "fonts/" + TextViewUtils.MuseoSans500);
            mPaint.SetTypeface(plain);

            if (!string.IsNullOrEmpty(cardModel.BillDueAmount) && !string.IsNullOrEmpty(cardModel.AccountName))
            {
                float dueAmountWidth = mPaint.MeasureText(cardModel.BillDueAmount, 0, cardModel.BillDueAmount.Length);
                float accountNameWidth = mPaint.MeasureText(cardModel.AccountName, 0, cardModel.AccountName.Length);

                LinearLayout.LayoutParams layoutParams = accountName.LayoutParameters as LinearLayout.LayoutParams;

                int nameWidth = (int)((parentGroup.Context.Resources.DisplayMetrics.WidthPixels) - dueAmountWidth - DPUtils.ConvertDPToPx(48f));

                if (accountNameWidth > nameWidth)
                {
                    layoutParams.Width = nameWidth;
                }

                accountName.LayoutParameters = layoutParams;
            }

            /*if (cardModel.IsZeroAmount)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                {
                    billDueAmount.SetTextColor(parentGroup.Context.Resources.GetColor(Resource.Color.all_cleared_amount, null));
                }
                else
                {
                    billDueAmount.SetTextColor(parentGroup.Context.Resources.GetColor(Resource.Color.all_cleared_amount));
                }
            }*/
            if (cardModel.IsNegativeAmount)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                {
                    billDueAmount.SetTextColor(parentGroup.Context.Resources.GetColor(Resource.Color.light_green_blue, null));
                }
                else
                {
                    billDueAmount.SetTextColor(parentGroup.Context.Resources.GetColor(Resource.Color.light_green_blue));
                }
            }

            if (cardModel.AccountType == 2)
            {
                accountTypeIcon.Visibility = ViewStates.Visible;
                accountTypeIcon.SetImageResource(Resource.Drawable.re_meter_dashboard);
            }
            else if (cardModel.SmartMeterCode.Equals(3))
            {
                accountTypeIcon.Visibility = ViewStates.Visible;
                accountTypeIcon.SetImageResource(Resource.Drawable.smart_meter_icon);
            }
            else
            {
                accountTypeIcon.Visibility = ViewStates.Gone;
            }

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

        public List<string> GetAccountCardNumberLists()
        {
            List<string> accountNumberList = new List<string>();
            if (accountModelList.Count > 0)
            {
                foreach (AccountCardModel cardModel in accountModelList)
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
