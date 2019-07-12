using Dadayun.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Dadayun.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                //logging
                .ConfigureLogging((hostContext, factory) =>
                {
                    factory.AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                    .AddConsole();
                })

                //host config
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                    ////command line
                    //if (args != null)
                    //{
                    //    config.AddCommandLine(args);
                    //}
                })
                //app config
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var env = hostContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                })
                //service
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.Configure<DadayunClientOptions>(hostContext.Configuration.GetSection("DadayunClientOptions"));
                    services.Configure<TokenOptions>(hostContext.Configuration.GetSection("DadayunClientOptions:TokenOptions"));
                    services.AddDadayunClient(hostContext.Configuration.GetValue<string>("DadayunClientOptions:EndPoint"));

                    services.AddTransient<BaseSample>();
                    services.AddTransient<FormSample>();

                    services.AddHostedService<SampleBackgroundService>();
                });

            //console 
            await builder.RunConsoleAsync();
        }
    }

    public class SampleBackgroundService : BackgroundService
    {
        private readonly BaseSample baseSample;
        private readonly FormSample formSample;
        public SampleBackgroundService(BaseSample baseSample, FormSample formSample)
        {
            this.baseSample = baseSample;
            this.formSample = formSample;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await baseSample.RunSample();

            await formSample.RunSample();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
