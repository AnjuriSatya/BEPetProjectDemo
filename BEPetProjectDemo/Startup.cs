using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using System;
using BEPetProjectDemo.Domain;
using BEPetProjectDemo.DAL;
[assembly: FunctionsStartup(typeof(BEPetProjectDemo.Startup))]
namespace BEPetProjectDemo
{
    public class Startup : FunctionsStartup
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables()
            .Build();
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(s =>
            {
                var connectionString = Configuration["CosmosDBConnectionString"];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Please specify a valid CosmosDBConnection in the local.settings.json file or your Azure Functions Settings.");
                }
                return new CosmosClientBuilder(connectionString)
                    .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase, IgnoreNullValues = true })
                    .Build();
        
            });
            builder.Services.PatientDomainServices();
            builder.Services.PatientDALServices();
        }
 
    }
}