namespace myTNB.Mobile.API.Models.Rating.PostSubmitRating
{
    public class PostSubmitRatingResponse : BaseResponse<PostSubmitRatingModel> { }

    public class PostSubmitRatingModel
    {
        public string TransactionId { set; get; }
        public int Rating { set; get; }
    }
}