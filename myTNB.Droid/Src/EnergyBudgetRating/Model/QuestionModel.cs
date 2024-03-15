using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.EnergyBudgetRating.Model
{
    public class QuestionModel
    {
        [JsonProperty(PropertyName = "WLTYQuestionId")]
        [AliasAs("WLTYQuestionId")]
        public string WLTYQuestionId { get; set; }

        [JsonProperty(PropertyName = "ModelCategories")]
        [AliasAs("ModelCategories")]
        public string ModelCategories { get; set; }

        [JsonProperty(PropertyName = "IconCategories")]
        [AliasAs("IconCategories")]
        public string IconCategories { get; set; }

        [JsonProperty(PropertyName = "IconPosition")]
        [AliasAs("IconPosition")]
        public int IconPosition { get; set; }

        [JsonProperty(PropertyName = "IsSelected")]
        [AliasAs("IsSelected")]
        public bool IsSelected { get; set; }
    }
}