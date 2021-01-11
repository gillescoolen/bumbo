using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Bumbo.Data.Models.PayrollServiceIntegration;
using Newtonsoft.Json;

namespace Bumbo.Data
{
    public class PayrollServiceIntegration
    {
        private static readonly string ConnectionString = "Endpoint=sb://soprj6.servicebus.windows.net/;SharedAccessKeyName=PublishVerloning;SharedAccessKey=twRRwoi7V3ZndBMvw2BH3F7HJC/XFf1Q7cRNSVdXBPs=;";
        private static readonly string QueueName = "verloningque";

        /// <summary>
        /// Use this method to send payroll to external servicebus
        /// </summary>
        /// <param name="payroll"></param>
        /// <returns></returns>
        public static async Task Submit(Payroll payroll)
        {
            await using (var client = new ServiceBusClient(ConnectionString))
            {
                var sender = client.CreateSender(QueueName);
                var message = new ServiceBusMessage(JsonConvert.SerializeObject(payroll));
                await sender.SendMessageAsync(message);
            }
        }
    }
}