namespace Common.JwtHelperService.Models;

public static class ClaimType
{
	public static Scope Users => new("users");
	public static Scope Authors => new("authors");
	public static Scope Books => new("books");
	public static Scope Categories => new("categories");
	public static Scope Notification => new("notification");

	public class Scope(string scopeName)
	{
		public string Read => $"{scopeName}:read";
		public string Create => $"{scopeName}:create";
		public string Update => $"{scopeName}:update";
		public string Delete => $"{scopeName}:delete";
	}
}
