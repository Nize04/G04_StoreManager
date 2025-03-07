using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Facade.Interfaces.Trackers;
using StoreManager.Models;
using StoreManager.Repositories;
using StoreManager.Services;
using System.Data;

namespace StoreManager.API.Configurations;

public static class DependencyConfigurator
{
    public static void ConfigureDependency(this WebApplicationBuilder builder, ConfigurationManager configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        configuration.GetConnectionString("ConnectionString");
        builder.Services.AddScoped<IDbConnection>(provider => new SqlConnection(connectionString));
        builder.Services.Configure<AzureStorageSettings>(builder.Configuration.GetSection("AzureStorageSettings"));

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IAccountImageService, AccountImageService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<ISupplierService, SupplierService>();
        builder.Services.AddScoped<ISupplierTransactionService, SupplierTransactionService>();
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        builder.Services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
        builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<ILogger<Program>, Logger<Program>>();
        builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
        builder.Services.AddSingleton<ILoginAttemptTracker, LoginAttemptTracker>();
    }
}