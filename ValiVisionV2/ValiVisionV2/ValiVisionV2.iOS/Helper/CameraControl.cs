using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using OpenCvSharp;
using UIKit;

namespace ValiVisionV2.iOS.Helper
{
    public class CameraControl
    {

        AVCaptureSession captureSession;
        AVCaptureDeviceInput captureDeviceInput;
        AVCaptureStillImageOutput stillImageOutput;
        AVCaptureVideoPreviewLayer videoPreviewLayer;
        UIView CurrentUIView;

        public OutputRecorder outputRecorder;
        public DispatchQueue queue;

        public CameraControl(UIView currentUIView)
        {
            CurrentUIView = currentUIView;
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
        public void TurnPreviewOn(bool option)
        {
            try
            {
                //Setup the video preview.
                if (CurrentUIView != null)
                {
                    //If toggle on is 'true' and preview has not been initialized
                    if (option && this.videoPreviewLayer == null)
                    {
                        this.videoPreviewLayer = new AVCaptureVideoPreviewLayer(this.captureSession)
                        {
                            Frame = CurrentUIView.Bounds
                        };
                        videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                        CurrentUIView.Layer.AddSublayer(videoPreviewLayer);
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
                            CurrentUIView.Layer.Sublayers.Last().RemoveFromSuperLayer();
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

                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
                frontCamera = devices.FirstOrDefault(device => device.Position == AVCaptureDevicePosition.Front); //Choose the front camera.
                

                ConfigureCameraForDevice(frontCamera);
                captureSession = new AVCaptureSession();
                var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
                captureDeviceInput = AVCaptureDeviceInput.FromDevice(frontCamera);
                captureSession.AddInput(captureDeviceInput);
                var dictionary = new NSMutableDictionary();
                dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);

                //Configuring the ouput for frame to be captured.
                var settings = new CVPixelBufferAttributes
                {
                    PixelFormatType = CVPixelFormatType.CV32BGRA
                };
                using (var output = new AVCaptureVideoDataOutput { WeakVideoSettings = settings.Dictionary })
                {
                    queue = new DispatchQueue("myQueue");
                    outputRecorder = new OutputRecorder();
                    output.SetSampleBufferDelegateQueue(outputRecorder, queue);
                    captureSession.AddOutput(output);
                }

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

    //capture frame class - this acts as a producer thread.
    public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {

            try
            {
                var image = ImageFromSampleBuffer(sampleBuffer);
                // Do something with the image, we just stuff it in our main view.
                //ImageView.BeginInvokeOnMainThread(() => {
                //    TryDispose(ImageView.Image);
                //    ImageView.Image = image;
                //    ImageView.Transform = CGAffineTransform.MakeRotation(NMath.PI / 2);
                //});

                // Set current frame to be this frame.
                ValiVisionV2.VideoFrameAnalyzer.CurrentFrame.Frame = image;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                sampleBuffer.Dispose();
            }
        }

        UIImage ImageFromSampleBuffer(CMSampleBuffer sampleBuffer)
        {
            // Get the CoreVideo image
            using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
            {
                // Lock the base address
                pixelBuffer.Lock(CVOptionFlags.None);
                // Get the number of bytes per row for the pixel buffer
                var baseAddress = pixelBuffer.BaseAddress;
                var bytesPerRow = (int)pixelBuffer.BytesPerRow;
                var width = (int)pixelBuffer.Width;
                var height = (int)pixelBuffer.Height;
                var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
                // Create a CGImage on the RGB colorspace from the configured parameter above
                using (var cs = CGColorSpace.CreateDeviceRGB())
                {
                    using (var context = new CGBitmapContext(baseAddress, width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo)flags))
                    {
                        using (CGImage cgImage = context.ToImage())
                        {
                            pixelBuffer.Unlock(CVOptionFlags.None);

                            // TODO: To be improved later
                            // Rotate the image as it's defaulted to landscape right.
                            return UIImage.FromImage(cgImage, 10, UIImageOrientation.Right);
                        }
                    }
                }
            }
        }

        void TryDispose(IDisposable obj)
        {
            if (obj != null)
                obj.Dispose();
        }
    }
}

