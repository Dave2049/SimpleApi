using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using SimpleApi.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Optionally, instantiate Startup class and call ConfigureServices
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();   
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    // Log the request here
    // You can access the request details using the 'context' object
    // For example, you can log the request method and path
    var requestMethod = context.Request.Method;
    var requestQuery = context.Request.QueryString.Value;
    var requestPath = context.Request.Path;
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    

    logger.LogInformation("Request: {RequestMethod} {RequestPath} {RequestQuery}", requestMethod, requestPath, requestQuery);
    
    await next.Invoke();
});
// Optionally, call the Configure method on the Startup class
startup.Configure(app, app.Environment);

app.Run();


