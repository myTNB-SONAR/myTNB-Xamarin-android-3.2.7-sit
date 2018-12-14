namespace myTNB.Model
{
    public class ChartModel : ChartModelBase
    {
        public ChartModel()
        {
            data = new ChartDataModel();
        }

        public ChartModel(BaseModel model) : this()
        {
            isError = model.isError;
            status = model.status;
            message = model.message;
            __type = model.__type;
        }

        public ChartDataModel data { set; get; }
    }
}