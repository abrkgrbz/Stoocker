using Microsoft.AspNetCore.Authorization;

namespace Stoocker.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)] 
    public class RequirePermissionAttribute: AuthorizeAttribute
    {
        public string Permission { get; }
        public bool RequireAll { get; set; } = true;

        public RequirePermissionAttribute(string permission) : base("PermissionPolicy")
        {
            Permission = permission;
        }

        public RequirePermissionAttribute(params string[] permissions) : base("PermissionPolicy")
        {
            Permission = string.Join(",", permissions);
        }
    }
}
