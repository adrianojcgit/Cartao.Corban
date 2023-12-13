using Cartao.Corban.Models.Dto;

namespace Cartao.Corban.Interfaces
{
    public interface IPropostaService
    {
        Task<bool> AdicionarProposta(PropostaBaseDto propostaDto);
    }
}
