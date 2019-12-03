using System;
using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    #region GetUserRewards
    public class GetUserRewardsResponseModel
    {
        public GetUserRewardsDataModel d { set; get; }
    }

    public class GetUserRewardsDataModel : BaseModelV2
    {
        public RewardsDataModel data { set; get; }
    }

    public class RewardsDataModel
    {
        public List<RewardsItemModel> UserRewards { set; get; } = new List<RewardsItemModel>();
    }

    public class RewardsItemModel
    {
        //public int Id { set; get; }
        public string Email { set; get; }
        public string RewardId { set; get; }
        public bool Read { set; get; }
        public DateTime? ReadDate { set; get; } = null;
        public bool Favourite { set; get; }
        public DateTime? FavUpdatedDate { set; get; } = null;
        public bool Redeemed { set; get; }
        public DateTime? RedeemedDate { set; get; } = null;
    }
    #endregion

    #region AddUpdateRewards
    public class UpdateRewardsResponseModel
    {
        public UpdateRewardsDataModel d { set; get; }
    }

    public class UpdateRewardsDataModel : BaseModelV2 { }
    #endregion
}