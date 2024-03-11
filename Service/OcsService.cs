using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleApi.Service;

public class OcsService(ILogger logger)
{
    public void SignRequest(HttpRequestMessage requestMessage, string user = "")
    {
        var appSecret = Environment.GetEnvironmentVariable("APP_SECRET");
        var appId = Environment.GetEnvironmentVariable("APP_ID");
        var appVersion = Environment.GetEnvironmentVariable("APP_VERSION");
        
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{appSecret}"));
        requestMessage.Headers.Add("AUTHORIZATION-APP-API", authValue);
        requestMessage.Headers.Add("EX-APP-ID", appId);
        requestMessage.Headers.Add("EX-APP-VERSION", appVersion);
        requestMessage.Headers.Add("OCS-APIRequest", "true");
    }

public async Task<HttpResponseMessage> OcsCallAsync(
    HttpMethod method,
    string path,
    object json_data = null,
    string user = "")
{
    using var client = new HttpClient();
    var requestMessage = new HttpRequestMessage(method, GetNcUrl(logger) + path);
    if (json_data != null)
    {
        var jsonContent = JsonSerializer.Serialize(json_data);
        requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
    }

    SignRequest(requestMessage, user);
    logger.LogInformation("Request: {RequestMethod} {RequestUri} {RequestHeaders}", requestMessage.Method, requestMessage.RequestUri, requestMessage.Headers.ToString());
    if (json_data != null)
    {
        logger.LogInformation("Request body: {RequestBody}", await requestMessage.Content.ReadAsStringAsync());
    }
    return await client.SendAsync(requestMessage);
}

public async Task CallTopMenuApi()
{
    var jsonData = new
    {
        name = "first_menu",
        displayName = "HausKreisFinder",
        icon = "img/houseIcon.svg",
        adminRequired = "0",
    };
    
    var response = await OcsCallAsync(HttpMethod.Post, "/ocs/v1.php/apps/app_api/api/v1/ui/top-menu", jsonData);

    if (response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        logger.LogInformation("Success: " + responseBody);
    }
    else
    {
         logger.LogInformation("error: " + response.StatusCode);  
    }
}
public async Task SetState()
{
    var jsonData = new
    {
        type = "top_menu",
        name = "first_menu",
        key = "ui_example_state",
        value =  new [] { "init state", "no state"}
    };
    
    var response = await OcsCallAsync(HttpMethod.Post, "/ocs/v1.php/apps/app_api/api/v1/ui/initial-state", jsonData);

    if (response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        logger.LogInformation("Success: " + responseBody);
    }
    else
    {
         logger.LogInformation("error: " + response.StatusCode);  
    }
}

public async Task RegisterFrontendJS()
{
    var jsonData = new
    {
        type= "top_menu",
        name= "first_menu",
        path= "assets/index"
    };

    var response = await OcsCallAsync(HttpMethod.Post, "/ocs/v1.php/apps/app_api/api/v1/ui/script", jsonData);

    if (response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        logger.LogInformation("Success: " + responseBody);
    }
    else
    {
         logger.LogInformation("error: " + response.StatusCode);  
    }
}

public async Task RegisterFrontendCSS()
{
    var jsonData = new
    {
        type= "top_menu",
        name= "first_menu",
        path= "assets/index"
    };

    var response = await OcsCallAsync(HttpMethod.Post, "/ocs/v1.php/apps/app_api/api/v1/ui/style", jsonData);

    if (response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        logger.LogInformation("Success: " + responseBody);
    }
    else
    {
         logger.LogInformation("error: " + response.StatusCode);  
    }
}


public static string GetNcUrl(ILogger logger)
{
    var ncUrl = Environment.GetEnvironmentVariable("NEXTCLOUD_URL");

    if (string.IsNullOrEmpty(ncUrl))
    {
        throw new InvalidOperationException("The NEXTCLOUD_URL environment variable is not set.");
    }

    // Removes "/index.php" or "/" from the end of the URL if present
    ncUrl = ncUrl.TrimEnd('/');
    if (ncUrl.EndsWith("/index.php", StringComparison.OrdinalIgnoreCase))
    {
        ncUrl = ncUrl.Substring(0, ncUrl.Length - "/index.php".Length);
    }
     logger.LogInformation("NC URL: " + ncUrl);  
    return ncUrl;
}

}