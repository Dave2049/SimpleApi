using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

//app.UseHttpsRedirection();

app.MapGet("/heartbeat", () =>
{
    return new { status = "ok" };
})
.WithName("heartbeat")
.WithOpenApi();

app.MapPut("/enabled", async (HttpRequest request, int enabled) =>
{
    OcsHelper.SignCheck(request);
     var logger = app.Services.GetRequiredService<ILogger<Program>>();
    if (enabled == 1)
    {
        // Enabled logic here
        await OcsHelper.CallTopMenuApi(logger);
        return new { error = "" };
    }
    else if (enabled == 0)
    {
        // Disabled logic here
        return new { error = "" };
    }
    else
    {
        // Invalid value logic here
        return new { error = "invalid" };
    }
})
.WithName("enabled")
.WithOpenApi();

app.UseStaticFiles(); // Serves files from wwwroot by default



app.Run();

