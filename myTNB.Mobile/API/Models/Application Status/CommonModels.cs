namespace myTNB.Mobile
{
    public class Details
    {
        public string Status { set; get; }
        public string StatusCode { set; get; }
        public string Type { set; get; }
        public string TypeCode { set; get; }
        public string RefCode { set; get; }
        public string AccountNumber { set; get; }
        public string ApplicationNumber { set; get; }
        public string SRNumber { set; get; }
        public ApplicationDate ApplicationDate { set; get; }
    }

    public class ApplicationDate
    {
        public int Month { set; get; }
        public string Year { set; get; }
        public string FormattedDate { set; get; }
    }

    public class Progress
    {
        public string Status { set; get; }
        public string StateCode { set; get; }
    }
}