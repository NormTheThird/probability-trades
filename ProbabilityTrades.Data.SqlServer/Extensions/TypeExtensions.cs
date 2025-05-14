namespace ProbabilityTrades.Data.SqlServer.Extensions;

public static class TypeExtensions
{
    public static SqlDbType ConvertToSqlDbType(this Type giveType)
    {
        var typeMap = new Dictionary<Type, SqlDbType>();

        typeMap[typeof(Guid)] = SqlDbType.UniqueIdentifier;
        typeMap[typeof(string)] = SqlDbType.NVarChar;
        typeMap[typeof(char[])] = SqlDbType.NVarChar;
        typeMap[typeof(int)] = SqlDbType.Int;
        typeMap[typeof(Int32)] = SqlDbType.Int;
        typeMap[typeof(Int16)] = SqlDbType.SmallInt;
        typeMap[typeof(Int64)] = SqlDbType.BigInt;
        typeMap[typeof(byte[])] = SqlDbType.VarBinary;
        typeMap[typeof(bool)] = SqlDbType.Bit;
        typeMap[typeof(DateTime)] = SqlDbType.DateTime2;
        typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
        typeMap[typeof(decimal)] = SqlDbType.Decimal;
        typeMap[typeof(decimal?)] = SqlDbType.Decimal;
        typeMap[typeof(double)] = SqlDbType.Float;
        typeMap[typeof(byte)] = SqlDbType.TinyInt;
        typeMap[typeof(TimeSpan)] = SqlDbType.Time;

        return typeMap[(giveType)];
    }
}