namespace Vault.Configuration
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Vault options class.
  /// </summary>
  public class VaultOptions
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VaultOptions"/> class.
    /// </summary>
    /// <param name="reloadOnChange">Reload secrets if changed in Vault.</param>
    /// <param name="reloadCheckIntervalSeconds">Interval in seconds to check Vault for any changes.</param>
    public VaultOptions(
        bool reloadOnChange = false,
        int reloadCheckIntervalSeconds = 300)
    {
      this.ReloadOnChange = reloadOnChange;
      this.ReloadCheckIntervalSeconds = reloadCheckIntervalSeconds;
    }

    /// <summary>
    /// Gets a value indicating whether gets value indicating that secrets should be re-read when they are changed in Vault.
    /// In this case Reload token will be triggered.
    /// </summary>
    public bool ReloadOnChange { get; }

    /// <summary>
    /// Gets interval in seconds to check Vault for any changes.
    /// </summary>
    public int ReloadCheckIntervalSeconds { get; }
  }
}