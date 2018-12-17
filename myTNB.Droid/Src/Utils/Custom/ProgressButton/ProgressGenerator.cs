using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace myTNB_Android.Src.Utils.Custom.ProgressButton
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
            set;get;
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



            counter = 0;
            if (action != null)
            {
                messageHandler.RemoveCallbacks(action);
            }
            action = () => UpdateProgress(button, 0);


            activity.RunOnUiThread(() => {
                action = () => UpdateProgress(button, 0);
                messageHandler.PostDelayed(action, generateDelay());
            });
        }

        void UpdateProgress(ResendButton button, float progress)
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