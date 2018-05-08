﻿using System;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Threading;
using UIKit;
using ValiVisionV2.iOS.Helper;
using ValiVisionV2.VideoFrameAnalyzer;

namespace ValiVisionV2.iOS
{
    public partial class ViewController : UIViewController
    {
        private CameraControl cameraControl;
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            cameraControl = new CameraControl(UIViewMainScreen);
            cameraControl.SetupLiveCameraStream();
            cameraControl.TurnPreviewOn(true);
            new FaceDetection();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


    }
}