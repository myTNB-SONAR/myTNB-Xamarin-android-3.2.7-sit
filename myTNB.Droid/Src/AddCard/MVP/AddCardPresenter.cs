namespace myTNB.Android.Src.AddCard.MVP
{
    public class AddCardPresenter : AddCardContract.IUserActionsListener
    {
        private AddCardContract.IView mView;

        public AddCardPresenter(AddCardContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {

        }
    }
}