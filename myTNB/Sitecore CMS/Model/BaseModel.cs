using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class BaseModel
    {
        public string Status { get; set; } = "Failed";
        public List<object> Data { get; set; }
    }
}