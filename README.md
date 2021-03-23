# Text Recognition Using Azure Cognitive Services
Samples using Azure storages, Azure cognitive services, Azure Functions with blob triggers and CosmosDB

![alt text](https://github.com/shuvo009/text-recognition-using-azure-cognitive-services/blob/main/img/SystemDiagram.PNG "Project Diagram")

# Getting started

## Prerequisites
* .NET Core 3.1
* Azure Cognitive Services
* Azure Storage Account
* Azure Function
* CosmosDB

### Running the Sample
1.  Load TextRecognition solution in Visual Studio.
2.  Create `local.settings.json` file at root of directory.
3.  Add following JSON at `local.settings.json`
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "CosmosDBConnection": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "COMPUTER_VISION_SUBSCRIPTION_KEY": "",
    "COMPUTER_VISION_ENDPOINT": ""
  }
}
```
4.  Create a Container `image-text` at Azure Storage
5.  Hit `F5`
6.  Upload a File at Azure Storage

## Resources
* https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-output?tabs=csharp
* https://github.com/lbugnion/sample-azure-cognitive1/blob/master/Doc/text-recognition.md
* https://docs.microsoft.com/en-us/samples/azure-samples/cognitive-services-dotnet-sdk-samples/cognitive-services-dotnet-sdk-samples/
