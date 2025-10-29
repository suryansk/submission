using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication2.Attributes
{
    /// <summary>
    /// Custom authorization attribute that prevents ViewOnly users from accessing certain endpoints
    /// </summary>
    public class RequireWriteAccessAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            var userType = user.FindFirst("userType")?.Value;
            
            if (userType == "ViewOnlyUser")
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Access Denied: ViewOnly users cannot perform write operations",
                    statusCode = 403,
                    userType = userType
                })
                {
                    StatusCode = 403
                };
                return;
            }
        }
    }
    
    /// <summary>
    /// Custom authorization attribute that requires specific user types
    /// </summary>
    public class RequireUserTypeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _allowedUserTypes;
        
        public RequireUserTypeAttribute(params string[] allowedUserTypes)
        {
            _allowedUserTypes = allowedUserTypes;
        }
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            var userType = user.FindFirst("userType")?.Value;
            
            if (!_allowedUserTypes.Contains(userType))
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = $"Access Denied: This endpoint requires one of the following user types: {string.Join(", ", _allowedUserTypes)}. Current user type: {userType}",
                    statusCode = 403,
                    userType = userType,
                    allowedUserTypes = _allowedUserTypes
                })
                {
                    StatusCode = 403
                };
                return;
            }
        }
    }
    
    /// <summary>
    /// Custom authorization attribute for admin-only operations
    /// </summary>
    public class RequireAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
            var userType = user.FindFirst("userType")?.Value;
            
            if (!roles.Contains("ADMIN") && !roles.Contains("BANK_MANAGER"))
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = "Access Denied: This operation requires Administrator or Bank Manager privileges",
                    statusCode = 403,
                    userType = userType,
                    currentRoles = roles,
                    requiredRoles = new[] { "ADMIN", "BANK_MANAGER" }
                })
                {
                    StatusCode = 403
                };
                return;
            }
        }
    }
}