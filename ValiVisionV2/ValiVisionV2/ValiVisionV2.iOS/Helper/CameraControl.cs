using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using UIKit;

namespace ValiVisionV2.iOS.Helper
{
    public class CameraControl
    {
        AVCaptureSession captureSession;
        AVCaptureDeviceInput captureDeviceInput;
        AVCaptureStillImageOutput stillImageOutput;
        AVCaptureVideoPreviewLayer videoPreviewLayer;
        UIView currentUIView;

        public CameraControl(UIView currentUIView)
        {
            currentUIView = this.currentUIView;
            DebugHelper.DisplayAnnouncement("CameraControl Object Initialized");
        }

        //If the Camera is Not Authorized - Request It
        async Task AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        //Toggle Camera previw on.
        public void TurnPreviewOn(UIView backgrounUIView, bool option)
        {
            try
            {
                //Setup the video preview.
                if (backgrounUIView != null)
                {
                    //If toggle on is 'true' and preview has not been initialized
                    if (option && this.videoPreviewLayer == null)
                    {
                        this.videoPreviewLayer = new AVCaptureVideoPreviewLayer(this.captureSession)
                        {
                            //Mirror the video
                            Frame = backgrounUIView.Bounds,
                            Connection =
                            {
                                AutomaticallyAdjustsVideoMirroring = false,
                                VideoMirrored = false
                            }
                        };
                        videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                        backgrounUIView.Layer.AddSublayer(videoPreviewLayer);
                        DebugHelper.DisplayActionResult("Live preview on.");
                    }
                    else if (option)
                    {
                        DebugHelper.DisplayActionResult("Preview is already on.");
                        return;
                    }
                    else
                    {
                        if (this.videoPreviewLayer != null)
                        {
                            backgrounUIView.Layer.Sublayers.Last().RemoveFromSuperLayer();
                            DebugHelper.DisplayActionResult("Live camera preview off");
                        }
                        else
                        {
                            DebugHelper.DisplayActionResult("No live camera preview was on");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.DisplayError(e);
            }
        }

        public void SetupLiveCameraStream()
        {
            try
            {
                AVCaptureDevice frontCamera = null;
                //Set Orientation to Front Camera
                foreach (var device in AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video))
                {
                    if (device.Position == AVCaptureDevicePosition.Front)
                    {
                        frontCamera = device;
                    }
                }
                
                ConfigureCameraForDevice(frontCamera);
                captureSession = new AVCaptureSession();
                var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
                captureDeviceInput = AVCaptureDeviceInput.FromDevice(frontCamera);
                captureSession.AddInput(captureDeviceInput);

                var dictionary = new NSMutableDictionary();
                dictionary[AVVideo.CodecKey] = new NSNumber((int) AVVideoCodec.JPEG);
                stillImageOutput = new AVCaptureStillImageOutput()
                {
                    OutputSettings = new NSDictionary()
                };

                captureSession.AddOutput(stillImageOutput);
                captureSession.StartRunning();

                DebugHelper.DisplayAnnouncement("CameraStream activated");
            }
            catch (Exception e)
            {
                DebugHelper.DisplayError(e);
            }
        }

        //Configuring Extra Settings for Camera
        void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
            }
        }
    }
}
