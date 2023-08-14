namespace Vault.Configuration
{
  public class VaultConfigSection
  {
    public string? Server { get; set; }
    public VaultAuthSection? Auth { get; set; }
    public Dictionary<string, VaultSecret>? Secrets { get; set; }
  }

  public class VaultAuthSection
  {
    public string? Type { get; set; }
    public string? MountPath { get; set; }
    public Dictionary<string, string>? Config { get; set; }
  }
}

public class VaultSecret
{
  public string? Secret { get; set; }
  public string? Key { get; set; }
}