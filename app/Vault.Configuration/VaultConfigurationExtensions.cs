namespace Vault.Configuration
{
  using System;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Logging;

  public static class VaultConfigurationExtensions
  {
    public static IConfigurationBuilder AddVaultConfiguration(
        this IConfigurationBuilder configuration,
        VaultConfigSection config,
        ILogger<VaultConfigurationProvider> logger)
    {
      if (configuration == null)
      {
        throw new ArgumentNullException(nameof(configuration));
      }

      configuration.Add(new VaultConfigurationSource(config, logger));

      return configuration;
    }
  }
}