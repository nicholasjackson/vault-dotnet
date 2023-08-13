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

        public Fighters (DbContextOptions<Fighters> options,
          ILogger<Fighters> logger)
          : base(options)
        {
          _logger = logger;
        }

        public DbSet<VaultDotNet.Models.Fighter> Fighter { get; set; } = default!;
    }
}
