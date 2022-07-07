using ClientIPAddresses.DatabaseReader;
using ClientIPAddresses.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IDatFileReader, DatFileReader>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapDefaultControllerRoute();

app.Run();