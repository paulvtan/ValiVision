using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;
using UIKit;
using ValiVisionV2.iOS.Helper;
using ValiVisionV2.iOS.VideoFrameAnalyzer;

namespace ValiVisionV2.iOS
{
    public partial class ViewController : UIViewController
    {
        private static UIView uiViewMainScreenLocal;
        private static UIImageView uiImageViewCardOverlay;
        private static UIButton uiButtonProceed;
        private static CameraControl cameraControl { get; set; }
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            uiViewMainScreenLocal = UIViewMainScreen;
            cameraControl = new CameraControl(UIViewMainScreen);
            uiButtonProceed = UIButtonProceed;
            uiButtonProceed.Hidden = true;
            cameraControl.SetupLiveCameraStream();
            cameraControl.TurnPreviewOn(true);
            RegisterInitialUserFace(5);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        // This method will try to register initial 5 guide to be used in verificiation later.
        public void RegisterInitialUserFace(int numFacesRegisteredRequired)
        {
            //Set up frameGrabber and start FaceAPI.
            var frameGrabber = new FaceDetection(numFacesRegisteredRequired);
            frameGrabber.StartFaceDetectAnalysisAsync();
        }

        //Proceed to Id Card Scanning
        public static void StartIdCardScanning(List<Face> faceses)
        {
            int frameNumber = 1;
            foreach (var face in faceses)
            {
                DisplayFaceApiResult.DisplayFaceDetectionResult(frameNumber, face);
                frameNumber += 1;
            }

        }

    }
}