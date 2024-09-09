using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ManagementSystem;

public class EnumSchemeFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;

        var namevalues = new OpenApiArray();
        namevalues.AddRange(Enum.GetNames(context.Type).Select(name => new OpenApiString(name)));

        schema.Extensions.Add("x-enumNames", namevalues);
        schema.Enum.Clear(); 
        foreach (var name in Enum.GetNames(context.Type))
        {
            schema.Enum.Add(new OpenApiString(name));
        }
    }
}
