
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.App;
using Android.Widget;
using myTNB.Android.Src.SSMR.SubmitMeterReading.Listener;
using Java.Util.Concurrent;
using Android.Hardware.Camera2;
using Android.Graphics;
using Java.Lang;
using System.Collections.Generic;
using Android.Media;
using Android.Hardware.Camera2.Params;
using Java.Util;
using Java.IO;
using Orientation = Android.Content.Res.Orientation;
using Android.Content;
using myTNB.Android.Src.Utils;
using Android.Text;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterTakePhotoFragment : AndroidX.Fragment.App.Fragment
    {
        // Camera state: Showing camera preview.
        public const int STATE_PREVIEW = 0;

        // Camera state: Waiting for the focus to be locked.
        public const int STATE_WAITING_LOCK = 1;

        // Camera state: Waiting for the exposure to be precapture state.
        public const int STATE_WAITING_PRECAPTURE = 2;

        //Camera state: Waiting for the exposure state to be something other than precapture.
        public const int STATE_WAITING_NON_PRECAPTURE = 3;

        // Camera state: Picture was taken.
        public const int STATE_PICTURE_TAKEN = 4;

        public const int MAX_CAMERA_ZOOM = 40;
        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();

        CameraStateListener mCameraStateChange;
        public Semaphore mCameraOpenCloseLock = new Semaphore(1);
        public CameraDevice mCameraDevice;
        private AutoFitTextureView mTextureView;
        private Size mPreviewSize;
        public CaptureRequest.Builder mPreviewRequestBuilder;
        private ImageReader mImageReader;
        public CameraCaptureSession mCaptureSession;
        public CaptureRequest mPreviewRequest;
        public CameraCaptureListener mCaptureCallback;
        public int mState = STATE_PREVIEW;
        // A {@link Handler} for running tasks in the background.
        public Handler mBackgroundHandler;
        // An additional thread for running tasks that shouldn't block the UI.
        private HandlerThread mBackgroundThread;
        // TextureView.ISurfaceTextureListener handles several lifecycle events on a TextureView
        private Camera2SurfaceTextureListener mSurfaceTextureListener;
        private CaptureRequest.Builder stillCaptureBuilder;

        // This a callback object for the {@link ImageReader}. "onImageAvailable" will be called when a
        // still image is ready to be saved.
        private ImageAvailableListener mOnImageAvailableListener;

        // This is the output file for our picture.
        public File mFile;

        // Orientation of the camera sensor
        private int mSensorOrientation;
        // Max preview width that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_WIDTH = 1920;

        // Max preview height that is guaranteed by Camera2 API
        private static readonly int MAX_PREVIEW_HEIGHT = 1080;

        // Whether the current camera device supports Flash or not.
        private bool mFlashSupported;
        // ID of the current {@link CameraDevice}.
        private string mCameraId;
        // CameraDevice.StateListener is called when a CameraDevice changes its state
        private CameraStateListener mStateCallback;
        public ImageView galleryPreview;
        public CameraCharacteristics characteristics;
        public float maximumZoomLevel;
        SeekBar seekBar;
        Rect zoomArea;
        CropAreaView cropAreaView;
        TextView takePhotoNoteView;

        public static SubmitMeterTakePhotoFragment NewInstance()
        {
            return new SubmitMeterTakePhotoFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mCameraStateChange = new CameraStateListener(this);
            mSurfaceTextureListener = new Camera2SurfaceTextureListener(this);

            // fill ORIENTATIONS list
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.SubmitMeterTakePhotoFragmentLayout , container, false);
        }

        public class OnSeekbarChangeListener : Java.Lang.Object, SeekBar.IOnSeekBarChangeListener
        {
            SubmitMeterTakePhotoFragment mFragment;
            public OnSeekbarChangeListener(SubmitMeterTakePhotoFragment fragment)
            {
                mFragment = fragment;
            }
            public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
            {
                if (mFragment.mPreviewRequestBuilder != null)
                {
                    int mProgress = progress;
                    int maxzoom = (int)mFragment.characteristics.Get(CameraCharacteristics.ScalerAvailableMaxDigitalZoom) * 5;
                    if (maxzoom < MAX_CAMERA_ZOOM)
                    {
                        maxzoom = MAX_CAMERA_ZOOM;
                    }
                    Rect rect = (Rect)mFragment.characteristics.Get(CameraCharacteristics.SensorInfoActiveArraySize);

                    int minW = (int)(rect.Width() / maxzoom);
                    int minH = (int)(rect.Height() / maxzoom);
                    int difW = rect.Width() - minW;
                    int difH = rect.Height() - minH;
                    int cropW = difW / 100 * (int)mProgress;
                    int cropH = difH / 100 * (int)mProgress;

                    mFragment.zoomArea = new Rect(cropW, cropH, rect.Width() - cropW, rect.Height() - cropH);
                    mFragment.mPreviewRequestBuilder.Set(CaptureRequest.ScalerCropRegion, mFragment.zoomArea);

                    try
                    {
                        if (mFragment.mCaptureSession != null)
                        {
                            mFragment.mCaptureSession.SetRepeatingRequest(mFragment.mPreviewRequestBuilder.Build(), mFragment.mCaptureCallback, null);
                        }
                    }
                    catch (CameraAccessException e)
                    {
                        e.PrintStackTrace();// printStackTrace();
                    }
                    catch (NullPointerException ex)
                    {
                        ex.PrintStackTrace();
                    }
                }
            }

            public void OnStartTrackingTouch(SeekBar seekBar)
            {
                //throw new System.NotImplementedException();
            }

            public void OnStopTrackingTouch(SeekBar seekBar)
            {
                //throw new System.NotImplementedException();
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            //base.OnViewCreated(view, savedInstanceState);
            mTextureView = (AutoFitTextureView)view.FindViewById(Resource.Id.texture_view_autofit);
            ImageView captureImage = (ImageView)view.FindViewById(Resource.Id.imageTakePhoto);
            galleryPreview = (ImageView)view.FindViewById(Resource.Id.imageGallery);
            galleryPreview.SetScaleType(ImageView.ScaleType.CenterCrop);
            galleryPreview.SetBackgroundDrawable(Activity.GetDrawable(Resource.Drawable.meter_capture_holder_inactive));
            seekBar = (SeekBar)view.FindViewById(Resource.Id.seekBar);

            captureImage.Click += delegate
            {
                TakePicture();
            };

            galleryPreview.Click += delegate
            {
                ((SubmitMeterTakePhotoActivity)Activity).ShowGallery();
            };

            galleryPreview.SetImageBitmap(null);

            seekBar.SetOnSeekBarChangeListener(new OnSeekbarChangeListener(this));
            LinearLayout linearLayout = view.FindViewById<LinearLayout>(Resource.Id.cropAreaContainer);
            linearLayout.Alpha = 0.8f;
            linearLayout.AddView(new CropAreaView(Context, this.Activity));

            takePhotoNoteView = view.FindViewById<TextView>(Resource.Id.take_photo_note);
            takePhotoNoteView.Text = Utility.GetLocalizedLabel("SSMRCaptureMeter", "singleTakePhotoDescription");
            takePhotoNoteView.BringToFront();

            TextViewUtils.SetMuseoSans300Typeface(takePhotoNoteView);

            try
            {
                ((SubmitMeterTakePhotoActivity)Activity).EnableMoreMenu();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            mFile = new File(Activity.GetExternalFilesDir(null), "pic.jpg");
            mCaptureCallback = new CameraCaptureListener(this);
            mOnImageAvailableListener = new ImageAvailableListener(this, mFile);
        }

        public override void OnResume()
        {
            base.OnResume();
            StartBackgroundThread();

            // When the screen is turned off and turned back on, the SurfaceTexture is already
            // available, and "onSurfaceTextureAvailable" will not be called. In that case, we can open
            // a camera and start preview from here (otherwise, we wait until the surface is ready in
            // the SurfaceTextureListener).
            if (mTextureView.IsAvailable)
            {
                OpenCamera(mTextureView.Width, mTextureView.Height);
            }
            else
            {
                mTextureView.SurfaceTextureListener = mSurfaceTextureListener;
            }
        }

        public override void OnPause()
        {
            CloseCamera();
            StopBackgroundThread();
            base.OnPause();
        }

        // Closes the current {@link CameraDevice}.
        private void CloseCamera()
        {
            try
            {
                mCameraOpenCloseLock.Acquire();
                if (null != mCaptureSession)
                {
                    mCaptureSession.Close();
                    mCaptureSession = null;
                }
                if (null != mCameraDevice)
                {
                    mCameraDevice.Close();
                    mCameraDevice = null;
                }
                if (null != mImageReader)
                {
                    mImageReader.Close();
                    mImageReader = null;
                }
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                mCameraOpenCloseLock.Release();
            }
        }

        public void CreateCameraPreviewSession()
        {
            try
            {
                SurfaceTexture texture = mTextureView.SurfaceTexture;
                if (texture == null)
                {
                    throw new IllegalStateException("texture is null");
                }

                // We configure the size of default buffer to be the size of camera preview we want.
                texture.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);

                // This is the output Surface we need to start preview.
                Surface surface = new Surface(texture);

                // We set up a CaptureRequest.Builder with the output Surface.
                mPreviewRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                mPreviewRequestBuilder.AddTarget(surface);

                // Here, we create a CameraCaptureSession for camera preview.
                List<Surface> surfaces = new List<Surface>();
                surfaces.Add(surface);
                surfaces.Add(mImageReader.Surface);
                mCameraDevice.CreateCaptureSession(surfaces, new CameraCaptureSessionCallback(this), null);

            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private void StartBackgroundThread()
        {
            mBackgroundThread = new HandlerThread("CameraBackground");
            mBackgroundThread.Start();
            mBackgroundHandler = new Handler(mBackgroundThread.Looper);
        }

        // Stops the background thread and its {@link Handler}.
        private void StopBackgroundThread()
        {
            mBackgroundThread.QuitSafely();
            try
            {
                mBackgroundThread.Join();
                mBackgroundThread = null;
                mBackgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }

        // Capture a still picture. This method should be called when we get a response in
        // {@link #mCaptureCallback} from both {@link #lockFocus()}.
        public void CaptureStillPicture()
        {
            try
            {
                var activity = Activity;
                if (null == activity || null == mCameraDevice)
                {
                    return;
                }
                // This is the CaptureRequest.Builder that we use to take a picture.
                //if (stillCaptureBuilder == null)
                //    stillCaptureBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                var stillCaptureBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                stillCaptureBuilder.AddTarget(mImageReader.Surface);

                // Use the same AE and AF modes as the preview.
                //stillCaptureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);

                stillCaptureBuilder.Set(CaptureRequest.ScalerCropRegion, zoomArea);
                //SetAutoFlash(stillCaptureBuilder);

                // Orientation
                int rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
                stillCaptureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                mCaptureSession.StopRepeating();
                mCaptureSession.Capture(stillCaptureBuilder.Build(), new CameraCaptureStillPictureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Retrieves the JPEG orientation from the specified screen rotation.
        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (ORIENTATIONS.Get(rotation) + mSensorOrientation + 270) % 360;
        }

        // Unlock the focus. This method should be called when still image capture sequence is
        // finished.
        public void UnlockFocus()
        {
            try
            {
                // Reset the auto-focus trigger
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
                //SetAutoFlash(mPreviewRequestBuilder);
                if (mCaptureSession != null)
                {
                    mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback,
                        mBackgroundHandler);
                    // After this, the camera will go back to the normal state of preview.
                    mState = STATE_PREVIEW;
                    mCaptureSession.SetRepeatingRequest(mPreviewRequest, mCaptureCallback,
                            mBackgroundHandler);
                }
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void RunPrecaptureSequence()
        {
            try
            {
                // This is how to tell the camera to trigger.
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);
                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                mState = STATE_WAITING_PRECAPTURE;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        // Opens the camera specified by {@link Camera2BasicFragment#mCameraId}.
        public void OpenCamera(int width, int height)
        {
            //if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.Camera) != Permission.Granted)
            //{
            //    RequestCameraPermission();
            //    return;
            //}
            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);
            var activity = Activity;
            var manager = (CameraManager)activity.GetSystemService(Android.Content.Context.CameraService);
            try
            {
                if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }
                manager.OpenCamera(mCameraId, mCameraStateChange, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
            }
        }

        // Sets up member variables related to camera.
        private void SetUpCameraOutputs(int width, int height)
        {
            var activity = Activity;
            var manager = (CameraManager)activity.GetSystemService(Android.Content.Context.CameraService);
            try
            {
                for (var i = 0; i < manager.GetCameraIdList().Length; i++)
                {
                    var cameraId = manager.GetCameraIdList()[i];
                    characteristics = manager.GetCameraCharacteristics(cameraId);
                    int maxZoom = (int)characteristics.Get(CameraCharacteristics.ScalerAvailableMaxDigitalZoom) * 5;
                    if (maxZoom < MAX_CAMERA_ZOOM)
                    {
                        maxZoom = MAX_CAMERA_ZOOM;
                    }
                    seekBar.Max = maxZoom;

                    // We don't use a front facing camera in this sample.
                    var facing = (Integer)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (facing != null && facing == (Integer.ValueOf((int)LensFacing.Front)))
                    {
                        continue;
                    }

                    var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (map == null)
                    {
                        continue;
                    }

                    // For still image captures, we use the largest available size.
                    Size largest = (Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)),
                        new CompareSizesByArea());
                    mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, /*maxImages*/2);
                    mImageReader.SetOnImageAvailableListener(mOnImageAvailableListener, mBackgroundHandler);

                    // Find out if we need to swap dimension to get the preview size relative to sensor
                    // coordinate.
                    var displayRotation = activity.WindowManager.DefaultDisplay.Rotation;
                    //noinspection ConstantConditions
                    mSensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
                    bool swappedDimensions = false;
                    switch (displayRotation)
                    {
                        case SurfaceOrientation.Rotation0:
                        case SurfaceOrientation.Rotation180:
                            if (mSensorOrientation == 90 || mSensorOrientation == 270)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        case SurfaceOrientation.Rotation90:
                        case SurfaceOrientation.Rotation270:
                            if (mSensorOrientation == 0 || mSensorOrientation == 180)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        default:
                            //Log.Error(TAG, "Display rotation is invalid: " + displayRotation);
                            break;
                    }

                    Point displaySize = new Point();
                    activity.WindowManager.DefaultDisplay.GetSize(displaySize);
                    var rotatedPreviewWidth = width;
                    var rotatedPreviewHeight = height;
                    var maxPreviewWidth = displaySize.X;
                    var maxPreviewHeight = displaySize.Y;

                    if (swappedDimensions)
                    {
                        rotatedPreviewWidth = height;
                        rotatedPreviewHeight = width;
                        maxPreviewWidth = displaySize.Y;
                        maxPreviewHeight = displaySize.X;
                    }

                    if (maxPreviewWidth > MAX_PREVIEW_WIDTH)
                    {
                        maxPreviewWidth = MAX_PREVIEW_WIDTH;
                    }

                    if (maxPreviewHeight > MAX_PREVIEW_HEIGHT)
                    {
                        maxPreviewHeight = MAX_PREVIEW_HEIGHT;
                    }

                    // Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
                    // bus' bandwidth limitation, resulting in gorgeous previews but the storage of
                    // garbage capture data.
                    mPreviewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))),
                        rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
                        maxPreviewHeight, largest);

                    // We fit the aspect ratio of TextureView to the size of preview we picked.
                    var orientation = Resources.Configuration.Orientation;
                    if (orientation == Orientation.Landscape)
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
                    }
                    else
                    {
                        //mTextureView.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
                    }

                    // Check if the flash is supported.
                    var available = (Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
                    if (available == null)
                    {
                        mFlashSupported = false;
                    }
                    else
                    {
                        mFlashSupported = (bool)available;
                    }

                    mCameraId = cameraId;
                    return;
                }
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (NullPointerException e)
            {
                // Currently an NPE is thrown when the Camera2API is used but not supported on the
                // device this code runs.
                //ErrorDialog.NewInstance(GetString(Resource.String.camera_error)).Show(ChildFragmentManager, FRAGMENT_DIALOG);
            }
        }

        public class CompareSizesByArea : Java.Lang.Object, IComparator
        {
            public int Compare(Object lhs, Object rhs)
            {
                var lhsSize = (Size)lhs;
                var rhsSize = (Size)rhs;
                // We cast here to ensure the multiplications won't overflow
                return Long.Signum((long)lhsSize.Width * lhsSize.Height - (long)rhsSize.Width * rhsSize.Height);
            }
        }

        public void ConfigureTransform(int viewWidth, int viewHeight)
        {
            Activity activity = Activity;
            if (null == mTextureView || null == mPreviewSize || null == activity)
            {
                return;
            }
            var rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, mPreviewSize.Height, mPreviewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = Math.Max((float)viewHeight / mPreviewSize.Height, (float)viewWidth / mPreviewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            }
            else if ((int)SurfaceOrientation.Rotation180 == rotation)
            {
                matrix.PostRotate(180, centerX, centerY);
            }
            mTextureView.SetTransform(matrix);
        }

        private static Size ChooseOptimalSize(Size[] choices, int textureViewWidth,
            int textureViewHeight, int maxWidth, int maxHeight, Size aspectRatio)
        {
            // Collect the supported resolutions that are at least as big as the preview Surface
            var bigEnough = new List<Size>();
            // Collect the supported resolutions that are smaller than the preview Surface
            var notBigEnough = new List<Size>();
            int w = aspectRatio.Width;
            int h = aspectRatio.Height;

            for (var i = 0; i < choices.Length; i++)
            {
                Size option = choices[i];
                if ((option.Width <= maxWidth) && (option.Height <= maxHeight) &&
                       option.Height == option.Width * h / w)
                {
                    if (option.Width >= textureViewWidth &&
                        option.Height >= textureViewHeight)
                    {
                        bigEnough.Add(option);
                    }
                    else
                    {
                        notBigEnough.Add(option);
                    }
                }
            }

            // Pick the smallest of those big enough. If there is no one big enough, pick the
            // largest of those not big enough.
            if (bigEnough.Count > 0)
            {
                return (Size)Collections.Min(bigEnough, new CompareSizesByArea());
            }
            else if (notBigEnough.Count > 0)
            {
                return (Size)Collections.Max(notBigEnough, new CompareSizesByArea());
            }
            else
            {
                //Log.Error(TAG, "Couldn't find any suitable preview size");
                return choices[0];
            }
        }

        // Initiate a still image capture.
        private void TakePicture()
        {
            LockFocus();
        }

        // Lock the focus as the first step for a still image capture.
        private void LockFocus()
        {
            if (mPreviewRequestBuilder != null)
            {
                try
                {
                    mPreviewRequestBuilder.Set(CaptureRequest.ScalerCropRegion, zoomArea);
                    // Tell #mCaptureCallback to wait for the lock.
                    mState = STATE_WAITING_LOCK;
                    mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback,
                            mBackgroundHandler);
                }
                catch (CameraAccessException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        public void ShowToast(string text)
        {
            //if (Activity != null)
            //{
            //    Activity.RunOnUiThread(new ShowToastRunnable(Activity.ApplicationContext, text));
            //}
        }

        private class ShowToastRunnable : Java.Lang.Object, IRunnable
        {
            private string text;
            private Context context;

            public ShowToastRunnable(Context context, string text)
            {
                this.context = context;
                this.text = text;
            }

            public void Run()
            {
                Toast.MakeText(context, text, ToastLength.Short).Show();
            }
        }

        public void SetImageGallery(Bitmap myBitmap)
        {
            if (Activity != null)
            {
                Activity.RunOnUiThread(new UpdateImageView(this, myBitmap));
            }
        }

        public void UpdateImage(Bitmap myBitmap)
        {
            galleryPreview.SetImageBitmap(myBitmap);
        }

        public void ResetZoom()
        {
            seekBar.SetProgress(0, false);
        }

        public void UpdateTakePhotoNote(string takePhotoNote)
        {
            takePhotoNoteView.Text = takePhotoNote;
        }

        public void UpdateTakePhotoFormattedNote(ISpanned takePhotoFormattedNote)
        {
            takePhotoNoteView.TextFormatted = takePhotoFormattedNote;
        }

        public class UpdateImageView : Java.Lang.Object, IRunnable
        {
            SubmitMeterTakePhotoFragment owner;
            Bitmap myBitmap;
            public UpdateImageView(SubmitMeterTakePhotoFragment o, Bitmap m)
            {
                this.owner = o;
                this.myBitmap = m;
            }

            public void Run()
            {
                //owner.CloseCamera();
                //owner.StopBackgroundThread();
                ((SubmitMeterTakePhotoActivity)owner.Activity).AddCapturedImage(myBitmap);
            }
        }

        public class CropAreaView : View
        {
            public Rect cropAreaRect;
            public int canvasHeight;
            public Activity mOwnerActivity;
            public CropAreaView(Context context, Activity ownerActivity) : base(context)
            {
                mOwnerActivity = ownerActivity;
            }

            public CropAreaView(Context context, IAttributeSet attrs) : base(context, attrs)
            {
            }

            public CropAreaView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
            {
            }

            protected override void OnDraw(Canvas canvas)
            {
                base.OnDraw(canvas);

                Paint rectPaint = new Paint(PaintFlags.AntiAlias);
                rectPaint.Color = Color.ParseColor("#49494a");
                rectPaint.SetStyle(Paint.Style.Fill);
                canvas.DrawPaint(rectPaint);

                rectPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
                ((SubmitMeterTakePhotoActivity)this.mOwnerActivity).mCropAreaHeight = canvas.Height;
                int height = canvas.Height;
                int width = canvas.Width;
                int left = (int)(width - (width * .809)); 
                int top = (int)(height - (height * .70));
                int right = (int)(width - (width * .191));
                int bottom = (int)(height - (height * .35));
                canvas.DrawRect(0, top, width, bottom, rectPaint);
                cropAreaRect = new Rect(left,top,right,bottom);

                rectPaint.SetXfermode(null);
                rectPaint.Color = Color.White;
                rectPaint.SetStyle(Paint.Style.Stroke);
                rectPaint.StrokeWidth = 10;
                canvas.DrawRoundRect(-10, top, width + 10, bottom, 0, 0, rectPaint);
            }

            public Rect GetCropAreaRect()
            {
                return cropAreaRect;
            }
        }
    }
}
