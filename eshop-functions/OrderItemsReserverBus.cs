using System;
using Azure.Storage.Blobs;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eshop_functions
{
    public class OrderItemsReserverBus
    {
        [FunctionName("OrderItemsReserverBus")]
        public void Run([ServiceBusTrigger("orders-queue", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            string fileName = $"{Guid.NewGuid()}.txt";
            string fileContent = JsonConvert.SerializeObject(myQueueItem);
            Stream myBlob = new MemoryStream();

            // Write the data to the outputBlob stream
            StreamWriter streamWriter = new StreamWriter(myBlob);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            myBlob.Position = 0;

            BlobContainerClient container = new BlobContainerClient(Connection, containerName);
            container.CreateIfNotExists();
            BlobClient blob = container.GetBlobClient(fileName);
            blob.Upload(myBlob);
        }
    }
}
