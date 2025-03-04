using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Repositories;
using StoreManager.Services;
using System.Data;

namespace StoreManager.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDbConnection>(p =>
                new SqlConnection(GetConnectionString()));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DatabaseFixture>();
            services.AddScoped<IOrderService, OrderService>();
        }

        private string GetConnectionString()
        {
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".."));

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return config.GetConnectionString("DefaultConnection");
        }
    }
}