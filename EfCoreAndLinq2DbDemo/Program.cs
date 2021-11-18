using EfCoreAndLinq2DbDemo.Bootstrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using NorthwindDataAccess;

namespace EfCoreAndLinq2DbDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Startup startup = new Startup(ParseParameters(args));
            //Startup startup = new TestStartup(ParseParameters(args));
            IServiceCollection services = new ServiceCollection();
            startup.ConfigureServices(services);
            using var mainContainer = services.BuildServiceProvider();
            using (var mainContainerScope = mainContainer.CreateScope())
            {
                NorthwindLinq2DbCommandProcessor.RegisterProcessor(mainContainerScope.ServiceProvider);
                var runner = mainContainerScope.ServiceProvider.GetService<DemoRunner>();

                await runner.ProblematicQueryDemo();
                //await runner.UpdateLotOfRecordsDemo();
                //await runner.InsertLotOfRecordsDemo();
                //await runner.UpsertDemo();
                //await runner.Linq2DbVsSpDemo();
                //await runner.OptionalParametersDemo();
                //await runner.RowNumberDemo();
                //await runner.PagedResultsWithTotalCountDemo();
            }

            TestStartup.connection?.Close();
            Console.WriteLine("Demo ended!");
            Console.ReadKey();
        }

        private static IConfiguration ParseParameters(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddCommandLine(args);
            return configurationBuilder.Build();
        }
    }
}
