using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VaultDotNet.Models;

namespace VaultDotNet.Data
{
  public class Fighters : DbContext
  {
    private readonly ILogger<Fighters> _logger;
    private readonly IConfiguration _config;

    // override OnConfiguring so that we can fetch the connection string every time
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

      // substitute the username and password with the vault values
      var username = _config["db_username"];
      var password = _config["db_password"];
      var connStr = $"server=localhost;username={username};password={password};database=Fighters-6558502e-64dd-43fe-99d8-b519b4408469";

      _logger.Log(LogLevel.Information, "Configuring with con string {conn}", connStr);

      // fetch the connection string and configure
      optionsBuilder.UseNpgsql(connStr);
    }

    public Fighters(DbContextOptions<Fighters> options,
      ILogger<Fighters> logger,
      IConfiguration config)
      : base(options)
    {
      _logger = logger;
      _config = config;

      _logger.Log(LogLevel.Information, "Fighters DBContext created");
    }

    public DbSet<VaultDotNet.Models.Fighter> Fighter { get; set; } = default!;
  }
}
