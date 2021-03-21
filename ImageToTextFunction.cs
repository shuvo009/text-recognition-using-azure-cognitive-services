using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TextRecognition.DbModel;

namespace TextRecognition
{
    public static class ImageToTextFunction
    {
        private static readonly string SubscriptionKey =
            Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");

        private static readonly string Endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");

        [FunctionName("ImageToTextFunction")]
        public static async Task Run([BlobTrigger("image-text/{name}", Connection = "AzureWebJobsStorage")]
            Stream blobStream, string name,
            [CosmosDB("ImageToTextDb", "textCollection", ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<TextModel> textItemsOut,
            ILogger log)
        {
            log.LogInformation(
                $"C# Blob trigger function Processed blob\n Name:{name} \n Size: {blobStream.Length} Bytes");

            var client = Authenticate(Endpoint, SubscriptionKey);
            var lines = await ExtractTextFromBlob(client, blobStream, log);
            await textItemsOut.AddAsync(new TextModel
            {
                Lines = lines,
                Id = Guid.NewGuid()
            });
        }


        #region Supported Methods

        private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };
            return client;
        }

        public static async Task<List<string>> ExtractTextFromBlob(IComputerVisionClient client, Stream blobStream,
            ILogger log)
        {
            const int numberOfCharsInOperationId = 36;

            log.LogInformation("Extract text from blob");

            var blobTextHeaders = await client.ReadInStreamAsync(blobStream);

            var operationLocation = blobTextHeaders.OperationLocation;
            var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            var i = 0;
            const int maxRetries = 10;
            ReadOperationResult results;
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
                log.LogInformation("Server status: {0}, waiting {1} seconds...", results.Status, i);
                await Task.Delay(1000);

                if (i == 9)
                    log.LogInformation("Server timed out.");
            } while ((results.Status == OperationStatusCodes.Running ||
                      results.Status == OperationStatusCodes.NotStarted) && i++ < maxRetries);

            var lines = results.AnalyzeResult.ReadResults.SelectMany(x => x.Lines).Select(x => x.Text).ToList();
            return lines;
        }

        #endregion
    }
}