
namespace myTNB.Model
{
    public class ChartDataModel : ChartDataModelBase
    {
        public ChartDataModel()
        {
            ByMonth = new ByMonthModel();
        }
        public ByMonthModel ByMonth { set; get; }

    }
}