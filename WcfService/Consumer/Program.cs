using ServiceReference1;
using System.Threading.Tasks;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elastic.Apm.Api;
using Elastic.Apm.SqlClient;
using Elastic.Apm.NetCoreAll;

namespace Consumer
{
    internal static class Program
    {
        /*public static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)
                  .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<IStartup>())
                  .UseAllElasticApm();
          private static void Main(String[] args)
          {
              int i = 0;
              var service = new Service1Client();
              callAPMBuilder(args);
              while (i < 100)
              {
                  Thread.Sleep(1000);
                  Console.Write(service.HelloWorld());
                  i++;
              }  
          }
          private static async void callAPMBuilder(String[] args)
          {
              var host = CreateHostBuilder(args).Build();
              await host.RunAsync();
          }*/

        private static async Task Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args);

            await hostBuilder.RunConsoleAsync();
           
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             // .ConfigureServices((context, services) => { services.AddHostedService<IService1>(); })
             .ConfigureServices((context, services) => { services.AddHostedService<HostedService>(); })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(options => options.IncludeScopes = true);
                })
                //seElasticApm(new HttpDiagnosticsSubscriber(), new SqlClientDiagnosticSubscriber());
          .UseAllElasticApm();
            
    }
}