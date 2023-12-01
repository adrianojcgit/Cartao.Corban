namespace Cartao.Corban.Models.Dto
{
    public class PropostaDto
    {
        public PropostaDto()
        {
            Enderecos = new List<EnderecoDto>();
            Faturamentos = new List<FaturamentoDto>();
            ContasBancos = new List<ContaBancoDto>();
        }
        public string NumeroProposta { get; set; }
        public int IdHtml { get; set; }
        public string? CodGuid { get; set; }
        public string? CodInterno { get; set; }
        public string? ClienteOrigem { get; set; }
        public string? CNPJ { get; set; }
        public string? NomeEmpresarial { get; set; }
        public string? NomeFantasia { get; set; }
        public DateTime DataImportacao { get; set; } = DateTime.UtcNow.AddHours(-3);
        public string? PorteEmpresa { get; set; }
        public decimal? FatBrutoAnual { get; set; }
        public bool Ativo { get; set; }
        public List<EnderecoDto> Enderecos { get; set; }
        public List<FaturamentoDto> Faturamentos { get; set; }
        public List<ContaBancoDto> ContasBancos { get; set; }
    }
}
