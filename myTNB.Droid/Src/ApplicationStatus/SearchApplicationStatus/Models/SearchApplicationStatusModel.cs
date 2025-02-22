﻿using System.Collections.Generic;
using myTNB;
using myTNB.Mobile;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.ApplicationStatus.SearchApplicationStatus.Models
{
    public class SearchByModel : SearchType
    {
        public SearchByModel(SearchType searchType)
        {
            SearchTypeId = searchType.SearchTypeId;
            SearchTypeDesc = searchType.SearchTypeDesc;
        }
        public string SearchTypeDisplay { get; set; }


        public SearchByModel() { }
        public bool isChecked { get; set; }
        public bool smrFlag { get; set; }
    }

    public class TypeModel : SearchApplicationTypeModel
    {
        public TypeModel(SearchApplicationTypeModel searhApplicationTypeModel)
        {
            SearchApplicationTypeId = searhApplicationTypeModel.SearchApplicationTypeId;
            SearchApplicationTypeDesc = searhApplicationTypeModel.SearchApplicationTypeDesc;
            SearchApplicationNoInputMask = searhApplicationTypeModel.SearchApplicationNoInputMask;
            SearchTypes = searhApplicationTypeModel.SearchTypes;
            UserRole = searhApplicationTypeModel.UserRole;
            isChecked = false;
        }
        public TypeModel() { }
        public bool isChecked { get; set; }

        public string ApplicationTypeDisplay { get; set; }
    }

    public class SMRTypeModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public bool isChecked { get; set; }
       
    }
}