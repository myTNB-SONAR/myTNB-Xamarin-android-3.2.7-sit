using System.Collections.Generic;
using myTNB_Android.Src.AppLaunch.Models;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughPresenter
    {
        List<NewWalkthroughModel> newWalkthroughList = new List<NewWalkthroughModel>();
        NewWalkthroughContract.IView mView;
        public NewWalkthroughPresenter(NewWalkthroughContract.IView view)
        {
            this.mView = view;
            newWalkthroughList = new List<NewWalkthroughModel>();
        }

        public List<NewWalkthroughModel> GenerateNewWalkthroughList(string currentAppNavigation)
        {
            newWalkthroughList = new List<NewWalkthroughModel>();

            if (!string.IsNullOrEmpty(currentAppNavigation) && currentAppNavigation == AppLaunchNavigation.Walkthrough.ToString())
            {
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = this.mView.GetAppString(Resource.String.walkthrough_install_title_1),
                    Description = this.mView.GetAppString(Resource.String.walkthrough_install_msg_1),
                    Image = "walkthrough_img_install_1"
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = this.mView.GetAppString(Resource.String.walkthrough_install_title_2),
                    Description = this.mView.GetAppString(Resource.String.walkthrough_install_msg_2),
                    Image = "walkthrough_img_install_2"
                });

                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = this.mView.GetAppString(Resource.String.walkthrough_install_title_3),
                    Description = this.mView.GetAppString(Resource.String.walkthrough_install_msg_3),
                    Image = "walkthrough_img_install_3"
                });
            }
            else
            {
                newWalkthroughList.Add(new NewWalkthroughModel()
                {
                    Title = this.mView.GetAppString(Resource.String.walkthrough_update_title_1),
                    Description = this.mView.GetAppString(Resource.String.walkthrough_update_msg_1),
                    Image = "walkthrough_img_update_1"
                });
            }

            return newWalkthroughList;
        }
    }
}
