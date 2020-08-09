using System;
using System.Threading.Tasks;
using DollaryDooMiner.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DollaryDooMiner
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;

        private static async Task Main(string[] args)
        {
            RegisterServices();
            var scope = _serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ConsoleApplication>().Run();
            DisposeServices();
        }

        private static void RegisterServices()
        {
            // TODO: This is my dodgy attempt at dependency injection with a console app. Review this later. The guts of things are in ConsoleApplication.
            var services = new ServiceCollection();
            services.AddSingleton<IMiner, Miner>();
            services.AddSingleton<ConsoleApplication>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (_serviceProvider == null)
                return;

            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
