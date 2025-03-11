using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace dbpooling
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            var connString = config["ConnectionString"];

            using var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContextPool<AppDbContext>(options =>
                        options.UseSqlServer(connString));

                    //services.AddDbContext<AppDbContext>(options =>
                    //    options.UseSqlServer("Data Source=localhost;Initial Catalog=dbAbsensi;User Id=sa;Password=sysdev123;TrustServerCertificate=True;"));

                    services.AddLogging(config =>
                    {
                        config.AddConsole();
                        config.SetMinimumLevel(LogLevel.Information);
                    });
                })
                .Build();

            // Get the service provider and run database operations
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Simulate multiple parallel database operations
            await SimulateDbContextPooling(host.Services, 20);
        }
        
        // Simulate parallel database operations to test DbContext pooling
        static async Task SimulateDbContextPooling(IServiceProvider services, int taskCount)
        {
            Console.WriteLine($"\nStarting {taskCount} parallel database tasks...\n");

            var tasks = Enumerable.Range(1, taskCount).Select(async i =>
            {
                using var scope = services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                Console.WriteLine($"Task {i}: Using DbContext Instance");

                dbContext.Users.Add(new User { Name = $"User {i}" });
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"Task {i}: Data inserted.");
            });

            await Task.WhenAll(tasks);

            Console.WriteLine("\nAll tasks completed.");
        }
    }
}
