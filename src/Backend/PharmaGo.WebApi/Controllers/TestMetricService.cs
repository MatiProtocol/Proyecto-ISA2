using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Metrics;
using InstrumentationInterface;
using PharmaGo.IDataAccess;

public class TestMetricService : BackgroundService
{
    private readonly Counter<long> _testCounter;
    private readonly Meter _meter = new Meter("PharmaGo.CustomMetrics");
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICustomMetrics _customMetrics;

    public TestMetricService(IServiceScopeFactory serviceScopeFactory, ICustomMetrics customMetrics)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _customMetrics = customMetrics;
        _testCounter = _meter.CreateCounter<long>("test_counter");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _testCounter.Add(1);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var databaseSizeService = scope.ServiceProvider.GetRequiredService<IDatabaseSizeService>();
                var databaseSizeKb = await databaseSizeService.GetSpaceUsedAsync();
                if (databaseSizeKb.HasValue)
                {
                    _customMetrics.SetDatabaseSize(databaseSizeKb.Value);
                }
            }

            await Task.Delay(60000, stoppingToken);
        }
    }
}