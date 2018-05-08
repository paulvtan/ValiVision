using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using ValiVisionV2.VideoFrameAnalyzer;

namespace ValiVisionV2
{
    public class FaceDetection
    {
        public static string FaceApiSubscriptionKey = "84ecc0d7d7f04e13bf6739df09fbeb86";
        public static string FaceApiEndpoint = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";

        public FrameGrabber<Face[]> grabber;

        public FaceDetection()
        {
            // Create a grabber, with analysis type Face[].
            grabber = new FrameGrabber<Face[]>();

            // Create Face API client.
            FaceServiceClient faceServiceClient = new FaceServiceClient(FaceApiSubscriptionKey, FaceApiEndpoint);



            // Set up a listener for when we receive a new result from an API call. 
            grabber.NewFrameProvided += (s, e) =>
            {
                // Console.WriteLine("New frame acquired at {0}", e.Frame.Metadata.Timestamp);
            };

            // Set up Face API call.
            grabber.AnalysisFunction = async frame =>
            {
                Console.WriteLine("Submitting frame acquired at {0}", frame.Metadata.Timestamp);
                // Encode image and submit to Face API. 
                return await faceServiceClient.DetectAsync(frame.Image.AsJPEG().AsStream());
            };

            // Set up a listener for when we receive a new result from an API call. 
            grabber.NewResultAvailable += (s, e) =>
            {
                if (e.TimedOut)
                    Console.WriteLine("API call timed out.");
                else if (e.Exception != null)
                {
                    Console.WriteLine("API call threw an exception.");
                    Console.WriteLine(e.Exception);
                }
                else
                    Console.WriteLine("New result received for frame acquired at {0}. {1} faces detected", e.Frame.Metadata.Timestamp, e.Analysis.Length);
            };

            // Tell grabber when to call API.
            // See also TriggerAnalysisOnPredicate
            grabber.TriggerAnalysisOnInterval(TimeSpan.FromMilliseconds(3000));
        }

        // Start Searching for Faces. (FaceAPI)
        public void StartFaceDetectAnalysis()
        {
            // Start running in the background.
            grabber.StartProcessingCameraAsync().Wait();
        }
    }
}
