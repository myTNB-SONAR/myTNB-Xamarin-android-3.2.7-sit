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
            StatusCode = model.StatusCode;
            RefreshTitle = model.RefreshTitle;
            RefreshMessage = model.RefreshMessage;
            RefreshBtnText = model.RefreshBtnText;
        }

        public ChartDataModel data { set; get; }
        public new string RefreshTitle { set; get; }
        public new string RefreshMessage { set; get; }
        public new string RefreshBtnText { set; get; }
    }
}