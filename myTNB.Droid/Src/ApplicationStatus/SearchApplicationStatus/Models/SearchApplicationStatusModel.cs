using System.Collections.Generic;
using myTNB;
using myTNB.Mobile;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Models
{
    public class SearchByModel : SearchType
    {
        public SearchByModel(SearchType searchType)
        {
            SearchTypeId = searchType.SearchTypeId;
            SearchTypeDesc = searchType.SearchTypeDesc;
        }

        public SearchByModel() { }
        public bool isChecked { get; set; }
    }

    public class TypeModel : SearhApplicationTypeModel
    {


        public TypeModel(SearhApplicationTypeModel searhApplicationTypeModel)
        {
            SearchApplicationTypeId = searhApplicationTypeModel.SearchApplicationTypeId;
            SearchApplicationTypeDesc = searhApplicationTypeModel.SearchApplicationTypeDesc;
            SearchApplicationNoInputMask = searhApplicationTypeModel.SearchApplicationNoInputMask;
            SearchTypes = searhApplicationTypeModel.SearchTypes;
            UserRole = searhApplicationTypeModel.UserRole;
            isChecked = false;
        }
        public TypeModel(){}
        public bool isChecked { get; set; }
    }
}