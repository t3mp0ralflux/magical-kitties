using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MagicalKitties.Api.Mapping;

public sealed class EnumSchemaTransformer() : IOpenApiSchemaTransformer
{
    // Found answer at https://github.com/dotnet/aspnetcore/issues/61303 to determine why Enums were just integers.
    
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        Type type = context.JsonTypeInfo.Type;
        
        if (type.IsEnum)
        {
            schema.Type = JsonSchemaType.Integer | JsonSchemaType.String;

            schema.Enum ??= new List<JsonNode>();
            
            schema.Enum.Clear();
            
            foreach (string name in Enum.GetNames(type))
            {
                schema.Enum.Add(name);
            }
        }

        return Task.CompletedTask;
    }
}