using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaultDotNet.Data;

var builder = WebApplication.CreateBuilder(args);
///builder.Services.AddDbContext<Fighters>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("Fighters") ?? throw new InvalidOperationException("Connection string 'Fighters' not found.")));

builder.Services.AddDbContext<Fighters>();


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
