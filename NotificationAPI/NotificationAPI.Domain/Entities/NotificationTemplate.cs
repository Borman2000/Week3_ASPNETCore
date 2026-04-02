using System.ComponentModel.DataAnnotations;

namespace NotificationAPI.Domain.Entities;

public class NotificationTemplate
{
	public static string EMPTY_TEMPLATE_ID = "3d4cfc64-b7d9-4bf5-a89f-871e8443efe7";
	public static string DEFAULT_TEMPLATE_ID = "3d4cfc64-b7d9-4bf5-a89f-871e8443efe8";

	[StringLength(40, ErrorMessage = "Template Id must not exceed 20 characters.")]
	public string Id { get; set; } = string.Empty;
	[StringLength(100, ErrorMessage = "Subject must not exceed 20 characters.")]
	public string Subject { get; set; } = string.Empty;
	[StringLength(200, ErrorMessage = "Body length must not exceed 20 characters.")]
	public string Body { get; set; } = string.Empty;
	public bool IsHtml { get; set; }
}