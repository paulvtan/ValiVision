using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ProjectOxford.Face.Contract;

namespace ValiVisionV2.iOS.Helper
{
    public class DisplayFaceApiResult
    {
        //TODO: For displaying the result return by Face API.
        public static void DisplayFaceDetectionResult(int frameNumber, Face face)
        {
            Console.WriteLine("");
            Console.WriteLine("Result of Frame " + frameNumber);
            Console.WriteLine("Frame GUID: " + face.FaceId);
            Console.WriteLine("Gender: " + face.FaceAttributes.Gender);
            Console.WriteLine("Age: " + face.FaceAttributes.Age);
            Console.WriteLine("Smile: " + face.FaceAttributes.Smile);
            Console.WriteLine("Glasses: " + face.FaceAttributes.Glasses);
            Console.WriteLine(face.FaceAttributes.Emotion.ToRankedList().First());
            Console.WriteLine("");
        }


    }
}
