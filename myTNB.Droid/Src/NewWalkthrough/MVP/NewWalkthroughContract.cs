using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughContract
    {
        public interface IView
        {
            string GetAppString(int id);
        }

        public interface IPresenter
        {
            List<NewWalkthroughModel> GenerateNewWalkthroughList(string currentAppNavigation);
        }
    }
}