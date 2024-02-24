using Absencespot.ApiFunctions.Middlewares;
using Absencespot.ApiFunctions.Middlewares.JwtAuthentication;
using Absencespot.Clients.GoogleCalendar.Options;
using Absencespot.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication(builder =>
                {
                    builder.UseMiddleware<ExceptionHandlerMiddleware>();
                    builder.UseWhen<JwtAuthenticationMiddleware>((context) =>
                    {
                        string[] allowedAnonymous = new string[] { "RegisterAsync", "LoginAsync" };
                        return !allowedAnonymous.Contains(context.FunctionDefinition.Name);
                    });
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                         .AddEnvironmentVariables();
                 })
                .ConfigureServices((context, services) =>
                {
                    StripeConfiguration.ApiKey = context.Configuration["Stripe:ApiKey"];
                    services.AddOptions<JwtAuthenticationOptions>()
                        .Bind(context.Configuration.GetSection("Jwt"));
                    services.AddOptions<GoogleAuthOptions>()
                        .Bind(context.Configuration.GetSection("GoogleAuth"));

                    services.AddPersistence(context.Configuration);
                    services.AddServices(context.Configuration);
                    services.AddClients(context.Configuration);
                    // services.AddHttpContextAccessor();
                })
                .Build();

            host.Run();
        }
    }
}