namespace ProbabilityTrades.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Encrypts the specified text using the provided key.
    /// </summary>
    /// <param name="text">The text to encrypt.</param>
    /// <param name="key">
    ///     The encryption key. The size of the key should be:
    ///         - For a 128-bit key, 16 bytes or 128 bits.
    ///         - For a 192-bit key, 24 bytes or 192 bits.
    ///         - For a 256-bit key, 32 bytes or 256 bits.
    /// </param>
    /// <returns>The encrypted text.</returns>
    public static async Task<string> EncryptAsync(this string text, string key)
    {
        return await text.EncryptAsync(key, string.Empty);
    }
    public static async Task<string> EncryptAsync(this string text, string key, string iv)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = string.IsNullOrEmpty(iv) ? new byte[16] : HexStringToByteArray(iv);

        byte[] array;
        using var memoryStream = new MemoryStream();
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            await streamWriter.WriteAsync(text);
        }

        array = memoryStream.ToArray();
        return Convert.ToBase64String(array);
    }

    /// <summary>
    ///     Decrypts the specified encrypted text using the provided key.
    /// </summary>
    /// <param name="text">The encrypted text to decrypt.</param>
    /// <param name="key">
    ///     The decryption key. The size of the key should be:
    ///         - For a 128-bit key, 16 bytes or 128 bits.
    ///         - For a 192-bit key, 24 bytes or 192 bits.
    ///         - For a 256-bit key, 32 bytes or 256 bits.
    /// </param>
    /// <returns>The decrypted text.</returns>
    public static async Task<string> DecryptAsync(this string text, string key)
    {
        return await text.DecryptAsync(key, string.Empty);
    }
    public static async Task<string> DecryptAsync(this string text, string key, string iv)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = string.IsNullOrEmpty(iv) ? new byte[16] : HexStringToByteArray(iv);

        var buffer = Convert.FromBase64String(text);
        using var memoryStream = new MemoryStream(buffer);

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return await streamReader.ReadToEndAsync();
    }

    public static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToUpper() + input[1..]
        };
    }

    public static byte[] GetHash(this string text, string key)
    {
        var encoding = new ASCIIEncoding();
        var textBytes = encoding.GetBytes(text);
        var keyBytes = encoding.GetBytes(key);
        using (HMACSHA256 hash = new HMACSHA256(keyBytes))
            return hash.ComputeHash(textBytes);
    }



    private static byte[] HexStringToByteArray(string hexString)
    {
        int length = hexString.Length;
        byte[] byteArray = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            byteArray[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        }
        return byteArray;
    }
}