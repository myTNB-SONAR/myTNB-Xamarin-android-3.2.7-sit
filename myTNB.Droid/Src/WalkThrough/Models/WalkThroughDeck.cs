namespace myTNB.Droid.Models
{
    public class Deck
    {
        public int imageId;
        public string heading;
        public string content;
        public string imageUrl;
        public string Heading { get { return heading; } }
        public string Content { get { return content; } }
        public int ImageId { get { return imageId; } }
        public string ImageUrl { get { return imageUrl; } }
    }

    public class WalkThroughDeck
    {
        static Deck[] builtInFlashCards = {
            new Deck { imageId = 1, imageUrl = "", heading = "",
                            content = "Content One" },
            new Deck { imageId = 2, imageUrl = "", heading = "Heading Two" ,
                            content = "Content Two" }
        };

        private Deck[] mDeck;

        public WalkThroughDeck() { mDeck = builtInFlashCards; }

        public WalkThroughDeck(Deck[] param)
        {
            mDeck = param;
        }

        public Deck this[int i] { get { return mDeck[i]; } }

        public int NumCards { get { return mDeck.Length; } }
    }
}