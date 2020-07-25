using System;
using System.Collections.Generic;

namespace myTNB.Model
{
    public class GetSearchForAccountResponseModel
    {
        public List<GetSearchForAccountModel> d { set; get; } = new List<GetSearchForAccountModel>();
    }

    public class GetSearchForAccountModel
    {
        public string __type { set; get; } = string.Empty;
        public string ContractAccount { set; get; } = string.Empty;
        public string IC { set; get; } = string.Empty;
        public string FullName { set; get; } = string.Empty;
    }
}
