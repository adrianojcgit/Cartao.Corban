using Cartao.Corban.Interfaces;
using Cartao.Corban.Models.Dto;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Text;
using System.Net.Http.Headers;

namespace Cartao.Corban.Servicos
{
    public class PropostaService : IPropostaService
    {
        private readonly HttpClient _httpClient;

        public PropostaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> AdicionarProposta(PropostaBaseDto propostaDto)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(propostaDto);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                contentString.Headers.ContentType = new
                MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("Proposta", contentString);

                if (!response.IsSuccessStatusCode)
                    return false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
