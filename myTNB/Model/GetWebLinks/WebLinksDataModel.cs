using myTNB.DataManager;

namespace myTNB.Model
{
    public class WebLinksDataModel
    {
        string _code = string.Empty;
        public string Id { set; get; }
        public string Code
        {
            set
            {
                _code = ServiceCall.ValidateResponseItem(value);
            }
            get
            {
                return _code.ToLower();
            }
        }
        public string Title { set; get; }
        public string Url { set; get; }
        public string IsActive { set; get; }
        public string DateCreated { set; get; }
        public string OpenWith { set; get; }
    }
}