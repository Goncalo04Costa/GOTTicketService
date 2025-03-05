using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GOTinforcavado.Services;
using Blazored.SessionStorage;

namespace GOTinforcavado
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredSessionStorage();

            // Configura o HttpClient com BaseAddress condicional para desenvolvimento ou produção
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.IsDevelopment()
                    ? "https://localhost:7111/" // URL do backend em desenvolvimento
                    : "https://localhost:7111/") // URL do backend em produção
            });

            // Registra o TicketService
            builder.Services.AddScoped<TicketService>();

            await builder.Build().RunAsync();
        }
    }
}
