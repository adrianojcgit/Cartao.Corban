using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cartao.Corban.Models.Dto
{
    public class FaturamentoDto
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [DataType(DataType.Currency)]
        public decimal? Valor { get; set; }
    }
}
