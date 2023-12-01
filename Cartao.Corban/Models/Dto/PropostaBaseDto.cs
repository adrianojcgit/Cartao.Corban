namespace Cartao.Corban.Models.Dto
{
    public class PropostaBaseDto
    {
        public List<PropostaDto> Propostas { get; set; }
        public PropostaBaseDto(List<PropostaDto> propostas)
        {
            Propostas = propostas;
        }
    }
}
