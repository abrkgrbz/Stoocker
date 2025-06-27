using Microsoft.OpenApi.Models;

namespace Stoocker.API.Filter
{
    public class SwaggerAreaOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
        {
            var areaAttribute = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Mvc.AreaAttribute>()
                .FirstOrDefault();

            if (areaAttribute != null)
            {
                operation.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = $"{areaAttribute.RouteValue} Area" }
                };
            }

            
        }
    }
}
