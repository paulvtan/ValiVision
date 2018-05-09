using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public FaceDetection(int numFacesRegisteredRequired)
        {
            // Create a grabber, with analysis type Face[].
            grabber = new FrameGrabber<Face[]>();
            
            //Set NumFacesRegister)edRequired
            grabber.NumFacesRegisteredRequired = numFacesRegisteredRequired;
            var faces = new Faces<Face>();
            grabber.FacesList = faces.FacesList;
            // Create Face API client.
            FaceServiceClient faceServiceClient = new FaceServiceClient(FaceApiSubscriptionKey, FaceApiEndpoint);

            try
            {

                // Set up a listener for when we receive a new result from an API call. 
                grabber.NewFrameProvided += (s, e) =>
                {
                    // Console.WriteLine("New frame acquired at {0}", e.Frame.Metadata.Timestamp);
                };

                // Set up Face API call.
                grabber.AnalysisFunction = async frame =>
                {
                    //Console.WriteLine("Submitting frame acquired at {0}", frame.Metadata.Timestamp);
                    // Encode image and submit to Face API. 
                    IEnumerable<FaceAttributeType> faceAttributes = new FaceAttributeType[]
                    {
                        FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile,
                        FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.FacialHair
                    };
                    return await faceServiceClient.DetectAsync(frame.Image.AsJPEG().AsStream(), returnFaceId: true,
                        returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
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
                    {
                        Console.WriteLine("New result received for frame acquired at {0}. {1} faces detected",
                            e.Frame.Metadata.Timestamp, e.Analysis.Length);
                        //If there's a return result.
                        if (e.Analysis.Length == 1)
                        {
                            try
                            {
                                Console.WriteLine(e.Analysis.First().FaceId);
                                Console.WriteLine(grabber.FacesList.Count);
                                //Add the face to the Faces[]
                                if (grabber.FacesList.Count < grabber.NumFacesRegisteredRequired)
                                {
                                    Console.WriteLine("Register Face to array");
                                    grabber.FacesList.Add(e.Analysis.First());
                                }
                                else // Stop the entire process
                                {
                                    StopProcess();
                                    Console.WriteLine("Stop registering faces to array");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        else if (e.Analysis.Length > 1)
                        {
                            Console.WriteLine("More than one face detected.");
                        }
                    }

                };

                // Tell grabber when to call API.
                // See also TriggerAnalysisOnPredicate
                grabber.TriggerAnalysisOnInterval(TimeSpan.FromMilliseconds(3000));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Call this to stop the process.
        public void StopProcess()
        {
            grabber.StopProcessingAsync().Wait();
            Debug.WriteLine("Stopping process");
        }

        // Start Searching for Faces. (FaceAPI)
        public async Task StartFaceDetectAnalysisAsync()
        {
            // Start running in the background.
            await grabber.StartProcessingCameraAsync();
        }
    }
}
