using System.Data;
using System.Text.Json;
using Dapper;

namespace MagicalKitties.Application.Database;

public class JsonTypeHandler : SqlMapper.ITypeHandler
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerOptions.Web);
    public void SetValue(IDbDataParameter parameter, object value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }

    public object? Parse(Type destinationType, object value)
    {
        return JsonSerializer.Deserialize(value as string ?? string.Empty, destinationType, _options);
    }
}