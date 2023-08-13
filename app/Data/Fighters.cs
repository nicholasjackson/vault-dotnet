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
        public Fighters (DbContextOptions<Fighters> options)
            : base(options)
        {
        }

        public DbSet<VaultDotNet.Models.Fighter> Fighter { get; set; } = default!;
    }
}
