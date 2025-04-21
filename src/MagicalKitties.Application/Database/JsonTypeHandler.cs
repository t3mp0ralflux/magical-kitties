using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;

namespace MagicalKitties.Application.Database;

public class JsonTypeHandler : SqlMapper.ITypeHandler
{
    public void SetValue(IDbDataParameter parameter, object value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }

    public object? Parse(Type destinationType, object value)
    {
        return JsonSerializer.Deserialize(value as string ?? string.Empty, destinationType, JsonSerializerOptions.Web);
    }
}