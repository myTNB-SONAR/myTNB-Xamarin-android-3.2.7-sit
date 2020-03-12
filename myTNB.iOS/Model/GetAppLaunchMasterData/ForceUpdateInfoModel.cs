namespace myTNB
{
    public class ForceUpdateInfoModel
    {
        public bool isIOSForceUpdateOn { set; get; }
        public bool isAndroidForceUpdateOn { set; get; }
        public string iOSLastVersion { set; get; }
        public string AndroidLastVersion { set; get; }
        public string iOSLatestVersion { set; get; }
        public string AndroidLatestVersion { set; get; }
        public string ModalTitle { set; get; }
        public string ModalBody { set; get; }
        public string ModalBtnText { set; get; }
    }
}