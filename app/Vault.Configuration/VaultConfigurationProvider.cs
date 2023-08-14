namespace Vault.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using Microsoft.AspNetCore.DataProtection;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Logging;
  using Polly;
  using Polly.RateLimit;
  using Vault.Client;
  using Vault.Model;
  using Newtonsoft.Json.Linq;

  public class VaultConfigurationProvider : ConfigurationProvider
  {
    private ILogger _logger;
    private VaultClient? _vaultClient;

    public VaultConfigurationProvider(VaultConfigurationSource source)
    {
      ConfigurationSource = source ?? throw new ArgumentNullException(nameof(source));
      _logger = ConfigurationSource.Logger;
    }

    internal VaultConfigurationSource ConfigurationSource { get; private set; }

    public override void Load()
    {
      _logger.LogInformation("abcx");

      if (_vaultClient == null)
      {
        _vaultClient = CreateVaultClient(ConfigurationSource.Config);
        var response = _vaultClient.Auth.TokenLookUpSelf();

        _logger.LogDebug("lease duration {duration}, renewable {renewable}", response.LeaseDuration, response.Renewable);
      }

      LoadVaultData(_vaultClient, ConfigurationSource.Config.Secrets).Wait();
    }

    private async Task LoadVaultData(VaultClient vaultClient, Dictionary<string, VaultSecret> secrets)
    {
      foreach (var secret in secrets)
      {
        try
        {
          // fetch the secret from Vault
          var vaultSecret = await vaultClient.ReadAsync<Object>(secret.Value.Secret);
          var data = vaultSecret.Data as JObject;
          Data.Add(secret.Key, (string)data[secret.Key]);
        }
        catch (VaultApiException e)
        {
          _logger.LogError("unable to read secret {e}", e);
        }
      }
    }

    private VaultClient CreateVaultClient(VaultConfigSection config)
    {
      var clientConfig = new VaultConfiguration(config.Server);
      var client = new VaultClient(clientConfig);

      if (config.Auth.Type == "AppRole")
      {
        _logger.LogDebug("Authenticating with App Role");

        string roleID;
        string secretID;

        // load the secret id from a file
        using (var streamReader = new StreamReader(config.Auth.Config["RoleID"], Encoding.ASCII))
        {
          roleID = streamReader.ReadToEnd();
        }

        // load the role id from a file
        using (var streamReader = new StreamReader(config.Auth.Config["SecretID"], Encoding.ASCII))
        {
          secretID = streamReader.ReadToEnd();
        }

        var response = client.Auth.AppRoleLogin(
          new AppRoleLoginRequest(roleID, secretID),
            config.Auth.MountPath);

        client.SetToken(response.ResponseAuth.ClientToken);
      }

      return client;
    }
  }
}