using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class OcsHelper
{
    public static void SignRequest(HttpRequestMessage requestMessage, string user = "")
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

    public static string SignCheck(HttpRequest request)
{
    var aaVersion = request.Headers["AA-VERSION"];
    var exAppId = request.Headers["EX-APP-ID"];
    var exAppVersion = request.Headers["EX-APP-VERSION"];
    var authorizationAppApi = request.Headers["AUTHORIZATION-APP-API"];

    var expectedAppId = Environment.GetEnvironmentVariable("APP_ID");
    var expectedAppVersion = Environment.GetEnvironmentVariable("APP_VERSION");

    if (exAppId != expectedAppId)
    {
        throw new ArgumentException($"Invalid EX-APP-ID: {exAppId} != {expectedAppId}");
    }

    if (exAppVersion != expectedAppVersion)
    {
        throw new ArgumentException($"Invalid EX-APP-VERSION: {exAppVersion} != {expectedAppVersion}");
    }

    var decodedAuth = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationAppApi));
    var parts = decodedAuth.Split(':');
    if (parts.Length != 2 || parts[1] != Environment.GetEnvironmentVariable("APP_SECRET"))
    {
        throw new ArgumentException("Invalid APP_SECRET");
    }

    return parts[0]; // Username
}

public static async Task<HttpResponseMessage> OcsCallAsync(
    HttpMethod method, ILogger logger,
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
    return await client.SendAsync(requestMessage);
}

public static async Task CallTopMenuApi(ILogger logger)
{
    var jsonData = new
    {
        name = "unique_name_of_top_menu",
        displayName = "HausKreisFinder",
        icon = "img/houseIcon.svg",
        adminRequired = "0",
    };
    
    var response = await OcsCallAsync(HttpMethod.Post, logger, "/ocs/v1.php/apps/app_api/api/v1/ui/top-menu", jsonData);

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

public class TopMenuRequest
{
    public string Name { get; set; } = "unique_name_of_top_menu";
    public string DisplayName { get; set; } = "Display name";
    public string Icon { get; set; } = "img/icon.svg";
    public string AdminRequired { get; set; } = "0";
}