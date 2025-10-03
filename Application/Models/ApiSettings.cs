using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class ApiSettings
{
	public const string Section = "ApiSettings";

	public string CorrelationName { get; set; } = string.Empty;

	[Required(AllowEmptyStrings = false)]
	public string Name { get; set; }
	[Required(AllowEmptyStrings = false)]
	[RegularExpression(@"^v([0-9]{1,3}\.[0-9]{1,3}(\.[0-9]{1,3}$)?)")]
	public string Version { get; set; }		// expected version format: v1.32.1, v1.32, v100.3.879 etc.
	[Range(10, 50, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
	public int MaxPageSize { get; set; }
}