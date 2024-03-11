using System.Security.Claims;
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
                var user = OcsHelper.SignCheck(request);
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
}
