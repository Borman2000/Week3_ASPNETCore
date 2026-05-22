using System.ComponentModel.DataAnnotations;

namespace NotificationAPI.Domain.Entities;

public class NotificationTemplate
{
	public const string EMPTY_TEMPLATE_ID = "3d4cfc64-b7d9-4bf5-a89f-871e8443efe7";
	public const string DEFAULT_TEMPLATE_ID = "3d4cfc64-b7d9-4bf5-a89f-871e8443efe8";

	[StringLength(40, ErrorMessage = "Template ID must not exceed 40 characters.")]
	public string Id { get; set; } = string.Empty;
	[StringLength(100, ErrorMessage = "Subject must not exceed 100 characters.")]
	public string Subject { get; set; } = string.Empty;
	[StringLength(200, ErrorMessage = "Body length must not exceed 200 characters.")]
	public string Body { get; set; } = string.Empty;
	public bool IsHtml { get; set; }
}
