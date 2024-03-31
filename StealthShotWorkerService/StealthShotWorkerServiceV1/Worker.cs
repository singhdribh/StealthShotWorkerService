using System.Diagnostics;

namespace StealthShotWorkerServiceV1
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
                StartAppAndV2ServiceIfNotRunning();
                await Task.Delay(new TimeSpan(hours: 0, minutes: 1, seconds: 0), stoppingToken);
            }
        }

        private void StartAppAndV2ServiceIfNotRunning()
        {
            Process[] processCollection = Process.GetProcesses().OrderBy(x => x.ProcessName).ToArray();
            if (!processCollection.Any(x => x.ProcessName == (_configuration["ClientApp:AppName"] ?? "")))
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = _configuration["ClientApp:FileName"] ?? "",
                    WorkingDirectory = _configuration["ClientApp:WorkingDirectory"] ?? ""
                };
                Process.Start(startInfo);
            }
            if (!processCollection.Any(x => x.ProcessName == (_configuration["ServiceV2:ServiceName"] ?? "")))
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = _configuration["ServiceV2:FileName"] ?? "",
                    WorkingDirectory = _configuration["ServiceV2:WorkingDirectory"] ?? ""
                };
                Process.Start(startInfo);
            }
        }


    }
}
