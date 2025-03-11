using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dbpooling
{
    class AppDbContext : DbContext
    {
        private static int _instanceCount = 0;
        private readonly int _instanceId;

        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : base(options)
        {
            _instanceId = Interlocked.Increment(ref _instanceCount);
            logger.LogInformation($"DbContext Instance {_instanceId} Created");
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=dbAbsensi;User Id=sa;Password=sysdev123;TrustServerCertificate=True;Pooling=true;Min Pool Size=5;Max Pool Size=7;Connection Lifetime=60;");
        //        //optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=dbAbsensi;User Id=sa;Password=sysdev123;TrustServerCertificate=True;");
        //    }
        //}

        public override void Dispose()
        {
            Console.WriteLine($"DbContext Instance {_instanceId} Disposed");
            base.Dispose();
        }

    }
}
