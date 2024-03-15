using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using myTNB.AndroidApp.Src.MyHome.Model;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.MyTNBService.Parser;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyHome.MVP
{
    public class MyHomePaymentDetailsPresenter : MyHomePaymentDetailsContract.IUserActionsListener
    {
        private readonly MyHomePaymentDetailsContract.IView mView;
        private BaseAppCompatActivity mActivity;
        private Context mContext;

        private MyHomePaymentDetailsModel _paymentDetailsModel;
        private AccountChargesResponse _accountChargesResponse;
        private List<AccountChargeModel> _accountChargeModelList;
        private List<AccountChargeModel> _selectedAccountChargesModelList;
        private AccountChargeModel _accountChargeModel;
        private List<CreditCard> _registeredCards;

        public MyHomePaymentDetailsPresenter(MyHomePaymentDetailsContract.IView view, BaseAppCompatActivity activity, Context context)
        {
            this.mActivity = activity;
            this.mContext = context;
            this.mView = view;
            this.mView?.SetPresenter(this);
        }

        public void OnInitialize(Bundle bundle)
        {
            try
            {
                this.mView?.SetUpViews();

                _paymentDetailsModel = new MyHomePaymentDetailsModel();
                _accountChargeModelList = new List<AccountChargeModel>();
                _selectedAccountChargesModelList = new List<AccountChargeModel>();
                _accountChargeModel = new AccountChargeModel();

                if (bundle.ContainsKey(MyHomeConstants.PAYMENT_DETAILS_MODEL))
                {
                    _paymentDetailsModel = JsonConvert.DeserializeObject<MyHomePaymentDetailsModel>(bundle.GetString(MyHomeConstants.PAYMENT_DETAILS_MODEL));
                    this.mView?.UpdateAccountInfoUI();

                    _accountChargesResponse = _paymentDetailsModel.AccountChargesResponse;
                    if (_accountChargesResponse != null)
                    {
                        _accountChargeModelList = BillingResponseParser.GetAccountCharges(_accountChargesResponse.GetData().AccountCharges);
                        _selectedAccountChargesModelList = _accountChargeModelList.GetRange(0, _accountChargeModelList.Count);
                        _accountChargeModel = _selectedAccountChargesModelList[0];
                        this.mView?.UpdateMyChargesUI();
                    }

                    RegisteredCardsResponse registeredCardsResponse = _paymentDetailsModel.RegisteredCardsResponse;
                    if (registeredCardsResponse != null)
                    {
                        if (registeredCardsResponse.GetData() != null)
                        {
                            if (registeredCardsResponse.GetData().Count() > 0)
                            {
                                _registeredCards = new List<CreditCard>();
                                List<CreditCard> cards = registeredCardsResponse.GetData();
                                foreach (CreditCard card in cards)
                                {
                                    _registeredCards.Add(card);
                                }

                                this.mView?.UpdatePaymentsMethodUI();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void Start() { }

        public MyHomePaymentDetailsModel GetMyHomePaymentDetailsModel()
        {
            return _paymentDetailsModel;
        }

        public AccountChargeModel GetAccountChargeModel()
        {
            return _accountChargeModel;
        }

        public List<CreditCard> GetRegisteredCards()
        {
            return _registeredCards;
        }
    }
}

