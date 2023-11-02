using Absencespot.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(builder =>
                {
                    //builder.UseMiddleware<ExceptionHandlingMiddleware>();
                    //builder.UseMiddleware<MyCustomMiddleware>();
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
                    services.AddPersistence(context.Configuration);
                    services.AddServices(context.Configuration);
                })
                .Build();

            host.Run();
        }
    }
}