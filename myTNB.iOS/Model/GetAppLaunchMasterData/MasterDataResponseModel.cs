namespace myTNB.Model
{
    public class MasterDataResponseModel : BaseModel
    {
        public MasterDataResponseModel()
        {
        }

        public MasterDataResponseModel(BaseModel model) : this()
        {
            isError = model.isError;
            status = model.status;
            message = model.message;
            __type = model.__type;
            StatusCode = model.StatusCode;
        }

        public MasterDataModel data { get; set; }
    }
}