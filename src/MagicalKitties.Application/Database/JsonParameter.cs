using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace MagicalKitties.Application.Database;

public class JsonParameter : SqlMapper.ICustomQueryParameter
{
    private readonly string _value;

    public JsonParameter(string value)
    {
        _value = value;
    }

    public void AddParameter(IDbCommand command, string name)
    {
        NpgsqlParameter parameter = new(name, NpgsqlDbType.Json)
                                    {
                                        Value = _value
                                    };

        command.Parameters.Add(parameter);
    }
}