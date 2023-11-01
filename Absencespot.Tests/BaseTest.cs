using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Absencespot.DependencyInjection;
using System;
using System.IO;

namespace Absencespot.Tests
{
    public class BaseTest
    {
        protected IServiceProvider ServiceProvider;
        public BaseTest()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var services = new ServiceCollection();
            services.AddPersistence(configuration);
            services.AddServices(configuration);

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}