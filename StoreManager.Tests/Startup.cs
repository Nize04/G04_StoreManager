using Microsoft.Data.SqlClient;
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
            services.AddScoped<IDbConnection>(p => new SqlConnection(@"Data Source=.;Initial Catalog=G04_StoreManagerDatabase;Integrated Security=True; trustServerCertificate = true"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DatabaseFixture>();
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}