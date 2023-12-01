using Cartao.Corban.Models.Dto;
using RabbitMQ.Client;

namespace Cartao.Corban.Interfaces
{
    public interface IBrokerConsumerService
    {
        Task ExecutaHngFire();
        Task Consumir();
    }
}
