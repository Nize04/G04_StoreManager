using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace MyAttributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeJwtAttribute : ActionFilterAttribute
    {
        private ITokenService? _tokenService;
        private IRoleService? _roleService;
        private readonly HashSet<string>? _roles;
        private readonly bool _forAnyUser;

        public AuthorizeJwtAttribute(params string[] roles)
        {
            _roles = new HashSet<string>(roles);
            _roles.Add("Admin");
            _forAnyUser = false;
        }

        public AuthorizeJwtAttribute()
        {
            _forAnyUser = true;
        }

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            _tokenService = serviceProvider.GetRequiredService<ITokenService>();
            _roleService = serviceProvider.GetRequiredService<IRoleService>();

            string token = context.HttpContext.Request.Cookies["jwtToken"]!;

            if (string.IsNullOrEmpty(token) || !await _tokenService.IsTokenValidAsync(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            if (!_forAnyUser)
            {
                int accountId = int.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                IEnumerable<Role> roles = await _roleService.GetRolesByAccountIdAsync(accountId);
                IEnumerable<string> roleNames = roles.Select(role => role.RoleName);

                if (!_roles!.Any(requiredRole => roleNames.Contains(requiredRole)))
                {
                    context.Result = new ForbidResult("You don't have access.");
                    return;
                }
            }


            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            await base.OnActionExecutionAsync(context, next);
        }
    }
}