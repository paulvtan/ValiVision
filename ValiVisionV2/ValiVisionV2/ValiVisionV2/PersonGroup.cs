using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;

namespace ValiVisionV2
{
    public class PersonGroup
    {
        //TODO: This class is on hold for now.
        public PersonGroup()
        {
            // Create a new person group.
            try
            {
                var client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(String.Empty);

                // Request Header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", FaceDetection.FaceApiSubscriptionKey);

                // Prediction URL
                string url = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/1";



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
