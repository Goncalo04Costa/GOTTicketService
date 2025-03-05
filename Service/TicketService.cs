using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;

namespace GOTinforcavado.Services
{
    public class TicketService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/ticket";

        public TicketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, ticket);

            if (!response.IsSuccessStatusCode)
            {
                // Captura o erro retornado pela API
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao criar ticket. Código: {response.StatusCode}, Mensagem: {errorMessage}");
            }

            // Tenta converter a resposta para um objeto Ticket, evitando erro se o JSON for inválido
            var ticketResponse = await response.Content.ReadFromJsonAsync<Ticket>();

            if (ticketResponse == null)
            {
                throw new Exception("Erro ao processar a resposta da API. O ticket retornado é nulo.");
            }

            return ticketResponse;
        }


        public async Task<Ticket> GetTicketByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Ticket>($"{BaseUrl}/{id}");
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Ticket>>(BaseUrl);
        }

        public async Task<Ticket> UpdateTicketAsync(string id, Ticket updatedTicket)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", updatedTicket);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Ticket>();
        }

        public async Task<Ticket> UpdateTicketStatusAsync(string id, EstadoTarefa status)
        {
            var response = await _httpClient.PatchAsJsonAsync($"{BaseUrl}/{id}/status", status);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Ticket>();
        }

        public async Task<List<Ticket>> SearchTicketsByCodeAsync(string codigo)
        {
            return await _httpClient.GetFromJsonAsync<List<Ticket>>($"{BaseUrl}/search/{codigo}");
        }

        public async Task UploadFilesAsync(string ticketId, List<IBrowserFile> files)
        {
            var content = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                content.Add(new StreamContent(stream), "files", file.Name);
            }

            var response = await _httpClient.PostAsync($"{BaseUrl}/{ticketId}/upload", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
