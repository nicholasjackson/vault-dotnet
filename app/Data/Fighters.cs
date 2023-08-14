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
      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        _logger.Log(LogLevel.Information, "Configuring with con string {conn}", _config.GetConnectionString("Fighters"));

        // fetch the connection string and configure
        optionsBuilder.UseNpgsql(_config.GetConnectionString("Fighters"));
      }

     public Fighters (DbContextOptions<Fighters> options,
       ILogger<Fighters> logger,
       IConfiguration config)
       : base(options)
     {
       _logger = logger;
       _config = config;

       _logger.Log(LogLevel.Information,"Fighters DBContext created");
     }

     public DbSet<VaultDotNet.Models.Fighter> Fighter { get; set; } = default!;
    }
}
