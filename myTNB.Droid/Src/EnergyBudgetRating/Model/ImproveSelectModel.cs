using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB_Android.Src.EnergyBudgetRating.Model
{
    public class ImproveSelectModel
    {
        [JsonProperty(PropertyName = "ModelCategories")]
        [AliasAs("ModelCategories")]
        public string ModelCategories { get; set; }

        [JsonProperty(PropertyName = "IconCategories")]
        [AliasAs("IconCategories")]
        public string IconCategories { get; set; }

        [JsonProperty(PropertyName = "IsSelected")]
        [AliasAs("IsSelected")]
        public bool IsSelected { get; set; }
    }
}