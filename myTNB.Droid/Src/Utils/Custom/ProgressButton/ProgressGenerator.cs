using Android.App;
using Android.OS;
using System;


namespace myTNB.AndroidApp.Src.Utils.Custom.ProgressButton
{
    public class ProgressGenerator
    {
        Action action;
        Handler messageHandler = new Handler();
        private int delay = 1000;
        private int Delay

        {
            get
            {
                return this.delay;
            }
            set
            {
                this.delay = value;
            }
        }
        private float progressSlice = 1f;
        public float ProgressSlice
        {
            get
            {
                return this.progressSlice;
            }
            set
            {
                this.progressSlice = value;
            }
        }

        private int counter = 0;

        public int MaxCounter
        {
            set; get;
        }

        public interface OnProgressListener
        {

            void OnComplete();

            void OnProgress(int progress);
        }

        private OnProgressListener mListener;
        public float Progress { get; set; }

        public ProgressGenerator(OnProgressListener listener)
        {
            mListener = listener;
        }
        public void Start(ResendButton button, Activity activity)
        {
            try
            {
                counter = 0;
                if (action != null)
                {
                    messageHandler.RemoveCallbacks(action);
                }
                action = () => UpdateProgress(button, 0);


                activity.RunOnUiThread(() =>
                {
                    try
                    {
                        action = () => UpdateProgress(button, 0);
                        messageHandler.PostDelayed(action, generateDelay());
                    }
                    catch (System.ObjectDisposedException e)
                    {
                        messageHandler = new Handler();
                        messageHandler.PostDelayed(action, generateDelay());
                        Utility.LoggingNonFatalError(e);
                    }
                });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void UpdateProgress(ResendButton button, float progress)
        {

            try
            {
                Progress += progressSlice;
                button.SetProgress(Progress);
                counter++;
                if (counter < MaxCounter)
                {
                    action = () => UpdateProgress(button, Progress);
                    messageHandler.PostDelayed(action, generateDelay());
                    mListener.OnProgress(counter);
                }
                else
                {
                    mListener.OnComplete();
                    if (action != null)
                    {
                        messageHandler.RemoveCallbacks(action);
                    }


                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private int generateDelay()
        {
            return Delay;
        }

        public void OnDestroy()
        {
            if (action != null)
            {
                messageHandler.RemoveCallbacks(action);
            }
        }
    }
}