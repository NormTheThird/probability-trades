namespace ProbabilityTrades.API.Services;

public static class TokenService
{
    public static string GenerateJwtToken(UserAuthenticationModel userAuthModel, string key)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        var claims = new List<Claim>
{
            new Claim(ClaimTypes.NameIdentifier, userAuthModel.Id.ToString()),
            new Claim(ClaimTypes.Name, userAuthModel.Username),
            new Claim(JwtRegisteredClaimNames.Sub, userAuthModel.Username),
            new Claim(JwtRegisteredClaimNames.Email, userAuthModel.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in userAuthModel.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(600),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string GenerateRefreshToken(int length)
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_!";
        return new string(Enumerable.Repeat(chars, length).Select(_ => _[random.Next(_.Length)]).ToArray());
    }

    public static string HashRefreshToken(string refreshToken, string key)
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.Default.GetBytes(refreshToken + key));

        var sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("x2"));

        return sb.ToString();
    }
}