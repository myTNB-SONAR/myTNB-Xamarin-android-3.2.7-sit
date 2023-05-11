using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Parser;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyHome.MVP
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

                    this.mView?.UpdatePaymentsMethodUI();
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
    }
}

