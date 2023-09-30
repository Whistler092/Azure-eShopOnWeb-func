using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

using Azure.Storage.Blobs;

namespace eshop_functions
{
    public static class OrderItemsFunctions
    {
        [FunctionName("OrderItemsReserver")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            List<ItemDto> data = JsonConvert.DeserializeObject<List<ItemDto>>(requestBody);
            var itemId = data[0]?.itemId;
            var quantity = data[0]?.quantity;


            string responseMessage = string.IsNullOrEmpty(itemId)
                ? "Missed ItemId"
                : $"ItemId {itemId} with Quantity {quantity}. Updated";

            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            string fileName = $"{Guid.NewGuid()}.txt";
            string fileContent = JsonConvert.SerializeObject(data);
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

            return new OkObjectResult(responseMessage);
        }

        public class ItemDto
        {
            public string itemId { get; set; }
            public int quantity { get; set; }
        }
    }
}
