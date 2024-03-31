using System.Diagnostics;

namespace StealthShotWorkerServiceV2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                StartV1ServiceIfNotRunning();
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void StartV1ServiceIfNotRunning()
        {
            Process[] processCollection = Process.GetProcesses().OrderBy(x => x.ProcessName).ToArray();
            if (!processCollection.Any(x => x.ProcessName == (_configuration["ServiceV1:ServiceName"] ?? "")))
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = _configuration["ServiceV1:FileName"] ?? "",
                    WorkingDirectory = _configuration["ServiceV1:WorkingDirectory"] ?? ""
                };
                Process.Start(startInfo);
            }
        }


    }
}
