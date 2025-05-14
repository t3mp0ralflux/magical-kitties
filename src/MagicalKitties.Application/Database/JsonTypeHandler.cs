using System.Collections;
using System.Data;
using System.Text.Json;
using Dapper;

namespace MagicalKitties.Application.Database;

public class JsonTypeHandler : SqlMapper.ITypeHandler
{
    private readonly JsonSerializerOptions _options = new(JsonSerializerOptions.Web);

    public void SetValue(IDbDataParameter parameter, object value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }

    public object? Parse(Type destinationType, object value)
    {
        string parsedValue = value.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(parsedValue) && typeof(IEnumerable).IsAssignableFrom(destinationType))
        {
            parsedValue = "[]";
        }
        else if (string.IsNullOrWhiteSpace(parsedValue) && destinationType.IsClass)
        {
            return null;
        }
        
        return JsonSerializer.Deserialize(parsedValue, destinationType, _options);;
    }
}