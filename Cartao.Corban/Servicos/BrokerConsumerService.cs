using Cartao.Corban.Interfaces;
using Cartao.Corban.Models.Dto;
using Hangfire;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;

namespace Cartao.Corban.Servicos
{
    public class BrokerConsumerService : IBrokerConsumerService
    {
        private readonly HttpClient _http;
        public BrokerConsumerService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7232/")
            };
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task ExecutaHngFire()
        {
            RecurringJob.AddOrUpdate("cartao-consignado-esteira", () =>
                                        Consumir(), Cron.MinuteInterval(3), TimeZoneInfo.Local);
        }


        public async Task Consumir()
        {
            var factorySQL = new ConnectionFactory() { HostName = "localhost" };
            var connection = factorySQL.CreateConnection();
            var channel = CreateChannel(connection);
            await ConsumirFila(channel);
        }

        private static IModel CreateChannel(IConnection connection)
        {
            var channel = connection.CreateModel();
            return channel;
        }

        private async Task ConsumirFila(IModel channel)
        {
            await Task.Run(() =>
            {
                channel.ExchangeDeclare("DeadLetterExchange", ExchangeType.Fanout);      // 1o. Declara o Exchange
                channel.QueueDeclare("dlq_proposta", true, false, false, null);
                channel.QueueBind("dlq_proposta", "DeadLetterExchange", "");
                var arguments = new Dictionary<string, object>()
                {
                    {"x-dead-letter-exchange", "DeadLetterExchange" }
                };
                //fim fila dlq
                channel.QueueDeclare(queue: "proposta",
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: arguments);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var proposta = JsonConvert.DeserializeObject<PropostaBaseDto>(message);
                        var result = await AdicionarProposta(proposta);                       

                        if (!result)
                            await FilaObservacao(proposta);

                        channel.BasicAck(ea.DeliveryTag, false);


                    }
                    catch
                    {
                        channel.BasicNack(ea.DeliveryTag, false, false); //Se true então devolve pra fila, então deve ser false.
                    }
                };
                channel.BasicConsume(queue: "proposta", autoAck: false, consumer: consumer);
            });
        }

        private async Task<bool> AdicionarProposta(PropostaBaseDto propostaDto)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(propostaDto);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                contentString.Headers.ContentType = new
                MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await _http.PostAsync("Proposta", contentString);

                if (!response.IsSuccessStatusCode)
                    return false;                    
                
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            
        }


        private async Task FilaObservacao(PropostaBaseDto item)
        {
            await Task.Run(() =>
            {
                //fila dlq
                var factorySQL = new ConnectionFactory() { HostName = "localhost" };
                var connection = factorySQL.CreateConnection();
                var channel = CreateChannel(connection);

                channel.ExchangeDeclare("DeadLetterExchangeObs", ExchangeType.Fanout);      // 1o. Declara o Exchange
                channel.QueueDeclare("dlq_observacao", true, false, false, null);
                channel.QueueBind("dlq_observacao", "DeadLetterExchangeObs", "");

                var arguments = new Dictionary<string, object>()
                {
                    {"x-dead-letter-exchange", "DeadLetterExchangeObs" }
                };
                //fim fila dlq
                channel.QueueDeclare(queue: "observacao",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: arguments);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                string message = JsonConvert.SerializeObject(item);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "observacao",
                                     basicProperties: properties,
                                     body: body,
                                     mandatory: true);

            });
        }
    }
}
