using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace pluralsightfuncs
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            //métodos autorizados, no meu caso "post"
            //Quando o AuthorizationLevel está como Function um código secreto será necessário para
            //chamar essa função quando implantada no Azure. Se eu não quisésse isso e quisésse disponibilizá-lo publicamente,
            //poderia simplesmente usar o AuthorizationLevel.Anonymous, mas com essa opção precisária compartilhar a
            //URL completa dessa função,
            //incluindo o código secreto com o meu provedor de pagamentos para permitir que ele chame a minha função.
            //Modelo de rota opcional, por padrão, a URL para uma função do Azure acionada por HTPP incluirá
            // /api e em seguida, o nome da função, no meu caso OnPaymentReceived

            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("orders")] IAsyncCollector<Order> orderQueue,
            ILogger log)
        {
            log.LogInformation("Received a payment");
            //Sempre que essa função for chamada eu gravo na fila orders a message contendo
            //os detalhes do pedido
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
             var order = JsonConvert.DeserializeObject<Order>(requestBody);
            await orderQueue.AddAsync(order);

            log.LogInformation($"Order {order.OrderId} received from {order.Email} for product {order.ProductId}");

            return new OkObjectResult("Thank you for your purchase");
        }

        public class Order
        {
            public string OrderId { get; set; }
            public string ProductId { get; set; }
            public string Email { get; set; }
            public decimal Price { get; set; }
            public DateTime purchaseDate { get; set; } = DateTime.UtcNow;
        }
    }
}
