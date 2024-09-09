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
            //m�todos autorizados, no meu caso "post"
            //Quando o AuthorizationLevel est� como Function um c�digo secreto ser� necess�rio para
            //chamar essa fun��o quando implantada no Azure. Se eu n�o quis�sse isso e quis�sse disponibiliz�-lo publicamente,
            //poderia simplesmente usar o AuthorizationLevel.Anonymous, mas com essa op��o precis�ria compartilhar a
            //URL completa dessa fun��o,
            //incluindo o c�digo secreto com o meu provedor de pagamentos para permitir que ele chame a minha fun��o.
            //Modelo de rota opcional, por padr�o, a URL para uma fun��o do Azure acionada por HTPP incluir�
            // /api e em seguida, o nome da fun��o, no meu caso OnPaymentReceived

            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("orders")] IAsyncCollector<Order> orderQueue,
            ILogger log)
        {
            log.LogInformation("Received a payment");
            //Sempre que essa fun��o for chamada eu gravo na fila orders a message contendo
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
