using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vault.Configuration;
using VaultDotNet.Data;

var builder = WebApplication.CreateBuilder(args);
///builder.Services.AddDbContext<Fighters>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("Fighters") ?? throw new InvalidOperationException("Connection string 'Fighters' not found.")));

builder.Services.AddDbContext<Fighters>();

//using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
//    .SetMinimumLevel(LogLevel.Trace)
//    .AddConsole());
//var logger = loggerFactory.CreateLogger<VaultConfigurationProvider>();
//
//builder.Services.Configure<VaultConfigSection>(options => builder.Configuration.GetSection("Vault").Bind(options));

//var config = new VaultConfigSection();
//builder.Configuration.GetSection("Vault").Bind(config);
//
//builder.Configuration.AddVaultConfiguration(config, logger);

builder.Services.AddDbContext<Fighters>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Fighters") ?? throw new InvalidOperationException("Connection string 'Fighters' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Run the database migrations
using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider
      .GetRequiredService<Fighters>();
  dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Fighters}/{action=Index}/{id?}");
app.Run();
