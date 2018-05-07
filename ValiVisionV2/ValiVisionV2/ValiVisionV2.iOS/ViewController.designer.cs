// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ValiVisionV2.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton UIButtonTakePhoto { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView UIViewMainScreen { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (UIButtonTakePhoto != null) {
                UIButtonTakePhoto.Dispose ();
                UIButtonTakePhoto = null;
            }

            if (UIViewMainScreen != null) {
                UIViewMainScreen.Dispose ();
                UIViewMainScreen = null;
            }
        }
    }
}