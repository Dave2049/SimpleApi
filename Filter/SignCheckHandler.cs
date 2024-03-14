using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

public class SignCheckHandler : AuthorizationHandler<SignCheckRequirement>
{


        private readonly ILogger<SignCheckHandler> _logger;
     public SignCheckHandler(ILogger<SignCheckHandler> logger)
        {
            _logger = logger;
        }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SignCheckRequirement requirement)
    {
        var httpContext = (context.Resource as Microsoft.AspNetCore.Http.DefaultHttpContext);
        var request = httpContext?.Request;

        if (request != null)
        {
            try 
            {
                var user = SignCheck(request);
                 var claims = new[] { new Claim(ClaimTypes.Name, user) };
                 _logger.LogInformation("User {username} is trying to enable the app", user);
            var identity = new ClaimsIdentity(claims, "Custom");
            var principal = new ClaimsPrincipal(identity);

            context.User.AddIdentity(identity); // Adds the custom identity to the current user
          
                context.Succeed(requirement);
            }
            catch (ArgumentException e)
            {
                context.Fail();
            }
        }
        return Task.CompletedTask;
    }

      private string SignCheck(HttpRequest request)
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
}
