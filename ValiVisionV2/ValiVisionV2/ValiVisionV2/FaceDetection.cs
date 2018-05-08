using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                IEnumerable<FaceAttributeType> faceAttributes = new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.FacialHair };
                return await faceServiceClient.DetectAsync(frame.Image.AsJPEG().AsStream(), returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
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

                //If there's a return result.
                if (e.Analysis.Length == 1)
                {
                    try
                    {
                        Console.WriteLine(e.Analysis.First().FaceAttributes.Gender);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
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
