using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eshop_functions
{
    public class DeliveryOrderProcessorFunction
    {
        [FunctionName("DeliveryOrderProcessorFunction")]
        public void Run([BlobTrigger("prod-file-upload/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");


             
        }
    }
}
