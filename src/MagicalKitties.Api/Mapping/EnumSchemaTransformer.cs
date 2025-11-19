using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MagicalKitties.Api.Mapping;

public sealed class EnumSchemaTransformer() : IOpenApiSchemaTransformer
{
    // Found answer at https://github.com/dotnet/aspnetcore/issues/61303 to determine why Enums were just integers.
    
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        Type type = context.JsonTypeInfo.Type;
        
        if (type.IsEnum)
        {
            schema.Type = "integer / string";
            
            schema.Enum.Clear();
            
            foreach (string name in Enum.GetNames(type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }

        return Task.CompletedTask;
    }
}