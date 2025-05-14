using ProbabilityTrades.Common.Enumerations;

namespace ProbabilityTrades.Data.SqlServer.Extensions;

public static class DbContextCommandExtensions
{
    public static async Task<int> ExecuteNonQueryAsync(this DbContext context, string rawSql)
        => await ExecuteNonQueryAsync(context, rawSql, new List<SqlParameter>());

    public static async Task<int> ExecuteNonQueryAsync(this DbContext context, string rawSql, List<SqlParameter> parameters)
    {
        var dbConnection = context.Database.GetDbConnection();
        using var command = dbConnection.CreateCommand();
        command.CommandText = rawSql;
        foreach (var parameter in parameters)
            command.Parameters.Add(parameter);

        if (dbConnection.State != System.Data.ConnectionState.Open)
            await dbConnection.OpenAsync();

        var output = GetGeneratedQuery(command);

        var result = await command.ExecuteNonQueryAsync();
        await dbConnection.CloseAsync();

        return result;
    }

    public static string GetGeneratedQuery(this IDbCommand dbCommand)
    {
        var query = dbCommand.CommandText;
        var paramList = "(";
        foreach (var parameter in dbCommand.Parameters)
        {
            var sqlParam = (SqlParameter)parameter;
            var value = sqlParam.Value;
            if (sqlParam.SqlDbType == SqlDbType.NVarChar || sqlParam.SqlDbType == SqlDbType.UniqueIdentifier || sqlParam.SqlDbType == SqlDbType.DateTimeOffset)   
                value = $"'{value}'";
            paramList += $"{sqlParam.ParameterName.Replace("@", "")},";
            query = query.Replace(sqlParam.ParameterName, $"{value}");
        }

        query = query.Replace("VALUES", $"{paramList.Trim(',')}) VALUES");
        return query;
    }

    public static async Task<T> ExecuteScalarAsync<T>(this DbContext context, string rawSql, params object[] parameters)
    {
        var dbConnection = context.Database.GetDbConnection();
        using var command = dbConnection.CreateCommand();
        command.CommandText = rawSql;

        if (parameters != null)
            foreach (var p in parameters)
                command.Parameters.Add(p);

        if (dbConnection.State != System.Data.ConnectionState.Open)
            await dbConnection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        await dbConnection.CloseAsync();

        return result == null ? default : (T)result;
    }


    public static async Task<List<T>> CallStoredProcedureAsync<T>(this DbContext context, string sql) where T : new() 
        => await QueryAsync<T>(context, sql, new List<SqlParameter>(), true);

    public static async Task<List<T>> CallStoredProcedureAsync<T>(this DbContext context, string sql, List<SqlParameter> parameters) where T : new() 
        => await QueryAsync<T>(context, sql, parameters, true);

    public static async Task<List<T>> QueryAsync<T>(this DbContext context, string sql) where T : new()
        => await QueryAsync<T>(context, sql, new List<SqlParameter>(), false);

    public static async Task<List<T>> QueryAsync<T>(this DbContext context, string sql, List<SqlParameter> parameters) where T : new() 
        => await QueryAsync<T>(context, sql, parameters, false);

    private static async Task<List<T>> QueryAsync<T>(this DbContext context, string rawSql, List<SqlParameter> parameters, bool isStoredProcedure) where T : new()
    {
        var dbConnection = context.Database.GetDbConnection();
        using var command = dbConnection.CreateCommand();
        command.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
        command.CommandText = rawSql;

        if (parameters != null)
            foreach (var p in parameters)
                command.Parameters.Add(p);

        if (dbConnection.State != System.Data.ConnectionState.Open)
            await dbConnection.OpenAsync();

        var result = new List<T>();
        using var dbDataReader = await command.ExecuteReaderAsync();
        while (dbDataReader.Read())
        {
            var returnType = new T();
            for (int i = 0; i < dbDataReader.FieldCount; i++)
            {
                var type = returnType.GetType();
                var propertyInfo = type.GetProperty(dbDataReader.GetName(i));
                var value = dbDataReader.GetValue(i);
                if (value.GetType() == typeof(DBNull))
                    value = null;
                else if (value.GetType() == typeof(string))
                {
                    if (value.Equals("Kucoin"))
                    {
                        propertyInfo?.SetValue(returnType, DataSource.Kucoin, null);
                        continue;
                    }
                }

                propertyInfo?.SetValue(returnType, value, null);
            }

            result.Add(returnType);
        }
        await dbConnection.CloseAsync();
        return result;
    }
}