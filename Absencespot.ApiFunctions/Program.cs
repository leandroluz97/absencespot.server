using Absencespot.ApiFunctions.Middlewares;
using Absencespot.ApiFunctions.Middlewares.GoogleAuthentication;
using Absencespot.ApiFunctions.Middlewares.JwtAuthentication;
using Absencespot.ApiFunctions.Middlewares.MicrosoftAuthentication;
using Absencespot.Clients.GoogleCalendar.Options;
using Absencespot.DependencyInjection;
using Absencespot.Dtos;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
                    string[] allowedAnonymous = new string[] { "RegisterAsync", "LoginAsync", "GetCalendarContent" };
                    builder.UseMiddleware<ExceptionHandlerMiddleware>();
                    //builder.UseWhen<JwtAuthenticationMiddleware>((context) =>
                    //{
                    //    return !allowedAnonymous.Contains(context.FunctionDefinition.Name);
                    //});
                    builder.UseWhen<MicrosoftAuthenticationMiddleware>((context) =>
                    {
                        string headers = context.BindingContext.BindingData["Headers"] as string;
                        var deserializedHeader = JsonSerializer.Deserialize<Dictionary<string, string>>(headers);
                        var hasXMicrosoftAuth = deserializedHeader.ContainsKey("X-MICROSOFT-AUTH");

                        return !allowedAnonymous.Contains(context.FunctionDefinition.Name) && hasXMicrosoftAuth;
                    });
                    builder.UseWhen<GoogleAuthenticationMiddleware>((context) =>
                    {
                        string headers = context.BindingContext.BindingData["Headers"] as string;
                        var deserializedHeader =  JsonSerializer.Deserialize<Dictionary<string, string>>(headers);
                        var hasXGoogleAuth = deserializedHeader.ContainsKey("X-GOOGLE-AUTH");

                        return !allowedAnonymous.Contains(context.FunctionDefinition.Name) && hasXGoogleAuth;
                    });
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
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
                    services.AddOptions<MicrosoftAuthOptions>()
                        .Bind(context.Configuration.GetSection("MicrosoftAuth"));

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