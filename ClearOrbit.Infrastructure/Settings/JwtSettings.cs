namespace ClearOrbit.Infrastructure.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ClearOrbit";
    public string Audience { get; set; } = "ClearOrbitClients";
    public int ExpiryMinutes { get; set; } = 60;
}