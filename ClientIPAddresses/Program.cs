using ClientIPAddresses.DatabaseReader;
using ClientIPAddresses.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IDatFileReader, DatFileReader>();

var app = builder.Build();
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    services.GetRequiredService<IDatFileReader>();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapDefaultControllerRoute();

app.Run();