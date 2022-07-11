using ClientIPAddresses.DatabaseReader;
using ClientIPAddresses.Interfaces;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<IDatFileReader, DatFileReader>();
builder.Services.AddSingleton<IDatMarshReader, DatMarshReader>();

var app = builder.Build();
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    services.GetRequiredService<IDatMarshReader>();
}

app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features
        .Get<IExceptionHandlerPathFeature>()
        .Error;
    var response = new { error = exception.Message };
    await context.Response.WriteAsJsonAsync(response);
}));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapDefaultControllerRoute();

app.Run();