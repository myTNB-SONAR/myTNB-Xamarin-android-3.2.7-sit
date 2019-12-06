﻿using System.Collections.Generic;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response
{
	public class GetUserRewardsResponse
    {

		[JsonProperty("d")]
		public AddUpdateRewardData Data { get; set; }

		public class AddUpdateRewardData
		{
			[JsonProperty(PropertyName = "ErrorCode")]
			public string ErrorCode { get; set; }

			[JsonProperty(PropertyName = "ErrorMessage")]
			public string ErrorMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayMessage")]
			public string DisplayMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayType")]
			public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "data")]
            public UserRewards Data { get; set; }
        }

        public class UserRewards
        {
            [JsonProperty(PropertyName = "UserRewards")]
            public List<AddUpdateRewardModel> CurrentList { get; set; }
        }
    }
}