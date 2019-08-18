using System;
namespace myTNB
{
    public class AccountStatus
    {
        public AccountStatus()
        {
        }

        private CustomUIView _mainView;

        private void CreateUI()
        {

        }

        public CustomUIView GetUI()
        {
            CreateUI();
            return _mainView;
        }
    }
}
