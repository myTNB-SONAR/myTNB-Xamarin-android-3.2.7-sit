using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using myTNB_Android.Src.ViewReceipt.Model;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ViewReceipt.Api
{
    public interface GetMultiReceiptByTransId
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_BillingSSP.asmx/GetMultiReceiptByTransId")]
        Task<GetMultiReceiptByTransIdResponse> GetMultiReceiptByTransId([Body] GetReceiptRequest getCards);
    }
}