using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NigerianFoodPredictor
{
    static class Program
    {
        static void Main()
        {
            Console.Write("Enter image file path: ");
            string imageFilePath = Console.ReadLine();

            MakePredictionRequest(imageFilePath).Wait();

            Console.WriteLine("\n\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers -
            client.DefaultRequestHeaders.Add("Prediction-Key", "1a35b84639d14bb09af9139e754c8f8c");
            

            // Prediction URL - my nigerian food predictor url
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/4d5d3bfa-e477-452d-bef9-29afa749fbc1/image?iterationId=863f397b-6946-41bd-937f-170ad56d2ef7";

            HttpResponseMessage response;
            String response_json;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                response_json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }

            JObject results = JObject.Parse(response_json);
            JArray predictions = (JArray)results.GetValue("Predictions");

            if(predictions.Count == 0)
            {
                Console.WriteLine("I don't know what this is");
            }
            else
            {
                String suggestions = "";
                for (int i=0; i < predictions.Count; i++)
                {
                    JObject prediction = (JObject)predictions[i];
                    var probability = prediction.GetValue("Probability");
                    {
                        var tag = prediction.GetValue("Tag");
                        suggestions += tag + ", ";
                    }
                }

                if(suggestions != "")
                {
                    Console.WriteLine("I think this is " + suggestions.Substring(0,suggestions.LastIndexOf(",")));
                }
                else
                {
                    Console.WriteLine("I don't know what this is");
                }

                
            }
        }
    }

}
