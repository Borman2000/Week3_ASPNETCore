using System.Text.Json.Serialization;

namespace AuthAPI.Application.DTOs;

public class UserDto
{
	public string? Id { get; set; } = string.Empty;
	public string? UserName { get; set; } = string.Empty;
	public IList<string>? Roles { get; set; }
//	[JsonIgnore]
	public string? Password { get; set; } = string.Empty;
	public string? Email { get; set; } = string.Empty;
}