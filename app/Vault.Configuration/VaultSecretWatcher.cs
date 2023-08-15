namespace Vault.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Primitives;

  public class VaultSecretWatcher : IHostedService,IDisposable
  {
    private readonly ILogger _logger;
    private IEnumerable<VaultConfigurationProvider> _configProviders;

    private Timer? _timer = null;

    public VaultSecretWatcher(IConfiguration configuration, ILogger<VaultSecretWatcher> logger)
    {
      _logger = logger;
      _configProviders = ((IConfigurationRoot)configuration).Providers.OfType<VaultConfigurationProvider>().ToList()!;
    }
    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Vault Secret Watcher started.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        _logger.LogDebug("Timed Hosted Service is working.");

        // check the config providers for cached secrets, check if they need to be
        // updated
        foreach(var provider in _configProviders) {
          // get a copy of the cache as we are going to mutate this we will
          // no longer be able to itterate over it
          var cache = new Dictionary<string,VaultCacheItem>(provider.Cache());
          foreach(var item in cache) {
            _logger.LogDebug("Checking exprity for secret {secret}", item.Key);

            // generate a random jitter for the secret renewal, this will be a
            // random interval 25% of the lease plus 50% of the lease to ensure that
            // all secrets are renewed in time but not all at the same time that could
            // cause load on the vault server
            var rnd = new Random();
            var jitter = rnd.Next(item.Value.TTL/4);
            var renewalSeconds = (item.Value.TTL/2) + jitter;

            if(item.Value.CreatedAt.AddSeconds(renewalSeconds) < DateTime.Now) {
              _logger.LogDebug("Renew secret {secret}", item.Key);
              provider.RenewSecret(item.Key).Wait();
            }
          }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
  }
}