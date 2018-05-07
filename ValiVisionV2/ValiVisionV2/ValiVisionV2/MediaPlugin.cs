using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
using Xamarin.Forms;

namespace ValiVisionV2
{
    //This cross platform plugin allows you to take photos and video or pick them from a gallery.
    public class MediaPlugin
    {
        public MediaPlugin()
        {
            try
            {
                Debug.WriteLine("Media Plugin Initialized.");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }


        public async void TakePhoto()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Debug.WriteLine("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Test",
                    SaveToAlbum = true,
                    CompressionQuality = 75,
                    CustomPhotoSize = 50,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 2000,
                    DefaultCamera = CameraDevice.Front
                });

                if (file == null)
                    return;
                file.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

    }
}
