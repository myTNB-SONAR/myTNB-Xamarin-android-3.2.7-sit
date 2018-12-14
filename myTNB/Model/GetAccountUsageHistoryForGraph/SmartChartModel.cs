using System;
namespace myTNB.Model
{
    public class SmartChartModel : ChartModelBase
    {
        public SmartChartModel()
        {
            data = new SmartChartDataModel();
        }

        public SmartChartModel(BaseModel model) : this()
        {
            isError = model.isError;
            status = model.status;
            message = model.message;
            __type = model.__type;
            StatusCode = model.StatusCode;
        }

        public SmartChartDataModel data { set; get; }
    }
}
