namespace ProbabilityTrades.Domain.Services;

public abstract class BaseApplicationService : BaseService
{
    internal readonly IConfiguration _configuration;
    internal readonly ApplicationDbContext _db;

    public BaseApplicationService(IConfiguration configuration, ApplicationDbContext db)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
}

public abstract class BaseCurrencyHistoryService : BaseService
{
    internal readonly CurrencyHistoryDbContext _db;

    public BaseCurrencyHistoryService(CurrencyHistoryDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
}

public abstract class BaseService
{
    internal static string EncryptString(string value, string keyString)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var valueBytes = Encoding.UTF8.GetBytes(value);
        var valueHash = MD5.Create().ComputeHash(valueBytes);

        using var crypt = Aes.Create();
        crypt.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(keyString));
        crypt.IV = new byte[16];

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write);
        cryptoStream.Write(valueBytes, 0, valueBytes.Length);
        cryptoStream.Write(valueHash, 0, valueHash.Length);
        cryptoStream.FlushFinalBlock();

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    internal static string DecryptString(string value, string keyString)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        using var crypt = Aes.Create();
        crypt.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(keyString));
        crypt.IV = new byte[16];

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Write);
        var valueBytes = Convert.FromBase64String(value);
        cryptoStream.Write(valueBytes, 0, valueBytes.Length);
        cryptoStream.FlushFinalBlock();

        var encryptedValueBytes = memoryStream.ToArray();
        var encryptedValueLength = encryptedValueBytes.Length - 16;
        if (encryptedValueLength < 0)
            throw new Exception("Invalid Hash Length");

        var encryptedValueHash = new byte[16];
        Array.Copy(encryptedValueBytes, encryptedValueLength, encryptedValueHash, 0, 16);

        var decryptHash = MD5.Create().ComputeHash(encryptedValueBytes, 0, encryptedValueLength);
        if (encryptedValueHash.SequenceEqual(decryptHash) == false)
            throw new Exception("Invalid Hash");

        return Encoding.UTF8.GetString(encryptedValueBytes, 0, encryptedValueLength);
    }

    public string GetSupportMessage() => "Please contact support if you have any questions.";

    public string GetTableName(string baseCurrency) => $"{baseCurrency.ToLower().FirstCharToUpper()}History";

    //public static object GetPropertyValue(object instance, string strPropertyName)
    //{
    //    Type type = instance.GetType();
    //    System.Reflection.PropertyInfo propertyInfo = type.GetProperty(strPropertyName);
    //    return propertyInfo.GetValue(instance, null);
    //}
    //public static int[] GetMovingAveragePossibleValues => new[] { 3, 5, 8, 9, 13, 21, 34, 50, 55, 89, 144, 233 };

}