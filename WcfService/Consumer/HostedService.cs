using Elastic.Apm;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    internal class HostedService : IHostedService
    {
        private readonly IApmAgent _apmAgent;
        private readonly ILogger _logger;

        public HostedService(IApmAgent apmAgent, ILogger<HostedService> logger) => (_apmAgent, _logger) = (apmAgent, logger);

        public async Task StartAsync(CancellationToken cancellationToken) =>
            await _apmAgent.Tracer.CaptureTransaction("Console .Net Core Example", "background", async () =>
            {
                Console.WriteLine("HostedService running");

                _logger.LogError("This is a sample error log message, with a sample value: {intParam}", 42);

                // We test the ApmErrorLogger with this code - this covers multiple scopes
                using (_logger.BeginScope("foo"))
                {
                    _logger.LogError("Yet another sample error log");

                    using (_logger.BeginScope("bar"))
                    {
                        _logger.LogError("And a 3. sample error log");
                    }
                }

                var fooScope = _logger.BeginScope("foo");
                int i = 0;
                var service = new Service1Client();
                while (i < 10)
                {

                    Thread.Sleep(1000);
                    _logger.LogInformation("Response from API:" + (service.HelloWorld()));
                    //ogger.("Response from API:" + (service.HelloWorld()));
                    //ogger.("Response from API:" + (service.HelloWorld()));
                    i++;
                }
                // Make sure Agent.Tracer.CurrentTransaction is not null
                var currentTransaction = Agent.Tracer.CurrentTransaction;
                if (currentTransaction == null) throw new Exception("Agent.Tracer.CurrentTransaction returns null");

                var httpClient = new HttpClient();
                return await httpClient.GetAsync("https://elastic.co", cancellationToken);
            });

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
