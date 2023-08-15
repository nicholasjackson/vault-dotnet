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
  using Polly.Caching;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.AspNetCore.Razor.Language;

  public class VaultCacheItem {
  /// <summary>
  /// TTL is the time to live or the lease duration for the secret
  /// this is represented in seconds.
  /// A value for 0 indicates that this secret has no TTL.
  /// </summary>
  public int TTL {get; set;}

  /// <summary>
  /// CreatedAt is the time that the secret was created
  /// </summary>
  public DateTime CreatedAt {get;set;}

  /// <summary>
  /// LeaseID is the lease of the secret if it is renewable
  /// </summary>
  public string? LeaseID {get; set;}

  /// <summary>
  /// Data for the secret
  /// </summary>
  public JObject? Data {get; set;}
  /// <summary>
  /// ConfigKeys contains a list of config keys that reference this secret
  /// the key is the config key name, and the value the secret data element
  /// </summary>
  public Dictionary<string,string>? ConfigKeys {get; set;}
}

  public class VaultConfigurationProvider : ConfigurationProvider
  {
    private ILogger _logger;
    private VaultClient? _vaultClient;

    private Dictionary<string, VaultCacheItem> _secretCache;

    internal VaultConfigurationSource ConfigurationSource { get; private set; }

    public VaultConfigurationProvider(VaultConfigurationSource source)
    {
      ConfigurationSource = source ?? throw new ArgumentNullException(nameof(source));
      _logger = ConfigurationSource.Logger;
      _secretCache = new Dictionary<string, VaultCacheItem>();
    }

    /// <summary>
    /// Cache holds the cached secrets
    /// </summary>
    public Dictionary<string,VaultCacheItem> Cache() {
      return _secretCache;
    }

    public override void Load()
    {
      _logger.LogInformation("Created VaultConfiguraionProvider");

      if (_vaultClient == null)
      {
        _vaultClient = CreateVaultClient(ConfigurationSource.Config);
        var response = _vaultClient.Auth.TokenLookUpSelf();

        _logger.LogDebug("lease duration {duration}, renewable {renewable}", response.LeaseDuration, response.Renewable);
      }

      LoadVaultData(ConfigurationSource.Config.Secrets).Wait();
    }

    public async Task RenewSecret(string secretPath) {
      var cacheItem = _secretCache[secretPath];

      // remove the cache item so the secret is refetched
      _secretCache.Remove(secretPath);

      // fetch the updated secret
      // we really should check to see if the secret can have it's lease renewed
      // rather than recreating it, this is just for proof of concept
      var secret = await FetchSecret(secretPath);

      // update the data items
      foreach(var item in cacheItem.ConfigKeys){
          _logger.LogDebug("Update Config data {key}, {secret}",item.Key, item.Value);
          Data[item.Key] = (string)secret.Data[item.Value]; 
          // add the config key to the secrets references so we can track 
          // renewals
          secret.ConfigKeys.Add(item.Key,item.Value);
      }
    }

    private async Task LoadVaultData(Dictionary<string, VaultSecret> secrets)
    {
      foreach (var secret in secrets)
      {
        try
        {
          // fetch the secret from Vault
          var item = await FetchSecret(secret.Value.Secret);
          // update the config data
          Data.Add(secret.Key, (string)item.Data[secret.Value.Key]);
          // add the config key to the secrets references so we can track 
          // renewals
          item.ConfigKeys.Add(secret.Key,secret.Value.Key);
        }
        catch (VaultApiException e)
        {
          _logger.LogError("unable to read secret {e}", e);
        }
      }
    }

    private async Task<VaultCacheItem?> FetchSecret(string secret)
    {
      // is it in the cache?
      if (_secretCache.ContainsKey(secret))
      {
        return _secretCache[secret];
      }

      // not in the cache fetch from vault
      var retryPolicy = Policy.Handle<RateLimitRejectedException>().WaitAndRetryAsync
      (
        retryCount: 3,
        retryNumber => TimeSpan.FromMilliseconds(100)
      );

      return await retryPolicy.ExecuteAsync(async () =>
      {
        try
        {
          var vaultSecret = await _vaultClient.ReadAsync<Object>(secret);

          var cacheItem = new VaultCacheItem();
          cacheItem.LeaseID = vaultSecret.LeaseID;
          cacheItem.TTL = vaultSecret.LeaseDuration;
          cacheItem.CreatedAt = DateTime.Now;
          cacheItem.Data =vaultSecret.Data as JObject;
          cacheItem.ConfigKeys = new Dictionary<string, string>();

          // cache the data
          _secretCache[secret] = cacheItem;

          return cacheItem;
        }
        catch (RateLimitRejectedException e)
        {
          _logger.LogDebug("rate limited getting secret {secret}", secret);
          await Task.Delay(e.RetryAfter);
          throw;
        }
      });
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

        // we need to track the token for the Vault client as it can have a TTL
        // if the token can be renewed it should be renewed, if the token expires
        // we need to re-authenticate to Vault to get a new token 
        // this is pure happy path code at the moment
        var response = client.Auth.AppRoleLogin(
          new AppRoleLoginRequest(roleID, secretID),
            config.Auth.MountPath);

        client.SetToken(response.ResponseAuth.ClientToken);
      }

      return client;
    }
  }
}