using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebAdvert.Api.Services;

namespace WebAdvert.Api.HealthChecks
{
    public class StorageHealthChecks : IHealthCheck
    {
        private readonly IAdvertStorageService _advertStorageService;
        public StorageHealthChecks(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            bool isStorageOk = await _advertStorageService.CheckAdvertTableAsync();
            if (isStorageOk)
                return HealthCheckResult.Healthy();
            else
                return HealthCheckResult.Unhealthy();
        }
    }
}
