//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using SQLitePCL;

//namespace HabbitHole.Data
//{
//    public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//    {
//                public ApplicationDbContext CreateDbContext(string[] args)
//        {
//            // REQUIRED for SQLite at design time
//            Batteries.Init();

//            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

//            optionsBuilder.UseSqlite(
//                "Data Source=habit_hole.db");

//            return new ApplicationDbContext(optionsBuilder.Options);
//        }
//       // public ApplicationDbContext CreateDbContext(string[] args)
//       // {
//       //     // Build config to read from appsettings.json
//       //     var settingsPath = Environment.GetEnvironmentVariable("JBC_APPSETTINGS_PATH")
//       //                       ?? Path.Combine(Directory.GetCurrentDirectory(), "../JBC.API/appsettings.json");

//       //     builder.Services.AddDbContext<ApplicationDbContext>(options =>
//       //options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//       //     var config = new ConfigurationBuilder()
//       //         .AddJsonFile(settingsPath, optional: false)
//       //         .Build();


//       //     var connectionString = config.GetConnectionString("DefaultConnection");

//       //     var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//       //     optionsBuilder.UseSqlite(connectionString);

//       //     return new ApplicationDbContext(optionsBuilder.Options);
//       // }
//    }
//}
