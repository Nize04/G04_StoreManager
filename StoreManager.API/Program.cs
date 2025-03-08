using Microsoft.AspNetCore.HttpOverrides;
using StoreManager.API.Configurations;
using StoreManager.API.MapperProfile;
using StoreManager.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddHttpContextAccessor();
builder.ConfigureDependency(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Session";
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always; // ? Fix for Dev Mode
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// ? Use a better method to configure authentication
new AuthenticationConfiguration(builder.Configuration).ConfigureServices(builder.Services);

builder.Services.AddControllers();

var app = builder.Build();
UserRequestHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// ? Move this up (Before Routing)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

// ? Use Session before Authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Exception handling middleware
app.UseMiddleware<StoreManager.API.Middleware.ExceptionHandlingMiddleware>();

app.MapControllers(); // Map API controllers

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS

app.Run();