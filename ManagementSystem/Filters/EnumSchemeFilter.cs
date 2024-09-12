using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ManagementSystem.Filters;

//public class EnumSchemeFilter : ISchemaFilter
//{
//    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//    {
//        if (!context.Type.IsEnum) return;

//        var enumStrings = new OpenApiArray();
//        var enumNumbers = new OpenApiArray();

//        foreach (var name in Enum.GetNames(context.Type))
//        {
//            enumStrings.Add(new OpenApiString(name));
//        }

//        foreach (var value in Enum.GetValues(context.Type))
//        {
//            enumNumbers.Add(new OpenApiInteger((int)value));
//        }

//        schema.Extensions.Add("x-enumNames", enumStrings);
//        schema.Enum.Clear();

//        foreach (var enumNumber in enumNumbers)
//        {
//            schema.Enum.Add(enumNumber);
//        }
//    }
//}
