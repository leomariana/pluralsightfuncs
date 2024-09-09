using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using static pluralsightfuncs.OnPaymentReceived;
using System.Security.Cryptography;
namespace pluralsightfuncs
{
    public class GenerateLicenseFile
    {
        [FunctionName("GenerateLicenseFile")]
        public void Run([QueueTrigger("orders", Connection = "AzureWebJobsStorage")] Order order,
            [Blob("licenses/{rand-guid}.lic")] TextWriter outputBlob, ILogger log)
        {
            outputBlob.WriteLine($"OrderId:{order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"ProductId: {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate: {order.purchaseDate}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");
            log.LogInformation($"C# Queue trigger function processed: {order}");
        }
    }
}
 