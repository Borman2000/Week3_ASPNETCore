namespace AuthAPI.Infrastructure;

public record JwtSettings
{
	public string SecretKey { get; init; } = string.Empty;
	public string Issuer { get; init; } = string.Empty;
	public string Audience { get; init; } = string.Empty;
	public int AccessTokenExpirationMinutes => 15;
	public int RefreshTokenExpirationDays => 7;
}
