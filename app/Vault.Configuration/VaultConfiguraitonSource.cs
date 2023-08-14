namespace Vault.Configuration
{
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Logging;

  public sealed class VaultConfigurationSource : IConfigurationSource
  {
    public VaultConfigurationSource(VaultConfigSection config, ILogger logger)
    {
      Logger = logger;
      Config = config;
    }

    public VaultConfigSection Config { get; }
    public ILogger Logger { get; }

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new VaultConfigurationProvider(this);
  }
}