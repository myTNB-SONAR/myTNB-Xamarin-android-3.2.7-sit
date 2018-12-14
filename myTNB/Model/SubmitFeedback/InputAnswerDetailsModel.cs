
using myTNB.Enums;

namespace myTNB.Model
{
    public class InputAnswerDetailsModel
    {
        public string WLTYQuestionId { get; set; }
        public string RatingInput { get; set; }
        public string MultilineInput { get; set; }

        public InputAnswerDetailsModel()
        {
            WLTYQuestionId = string.Empty;
            RatingInput = string.Empty;
            MultilineInput = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:myTNB.Model.InputAnswerDetailsModel"/> class.
        /// </summary>
        /// <param name="input">Input <see cref="T:myTNB.Model.FeedbackQuestionModel"/>.</param>
        public InputAnswerDetailsModel(FeedbackQuestionModel input): this()
        {
            WLTYQuestionId = input.WLTYQuestionId;

            switch(input.Kind)
            {
                case QuestionTypeEnum.Rating:
                    RatingInput = !string.IsNullOrEmpty(input.Answer) ? input.Answer : string.Empty;
                    break;
                case QuestionTypeEnum.MultilineComment:
                    MultilineInput = !string.IsNullOrEmpty(input.Answer) ? input.Answer : string.Empty;
                    break;
            }
        }
    }
}
