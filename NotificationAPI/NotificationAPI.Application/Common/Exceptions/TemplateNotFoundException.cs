namespace NotificationAPI.Application.Common.Exceptions;

public class TemplateNotFoundException : Exception
{
	public TemplateNotFoundException() : base()
	{
	}

	public TemplateNotFoundException(string templateId) : base($"Template with id \"{templateId}\" was not found.")
	{
	}

	public TemplateNotFoundException(string message, Exception exp) : base(message, exp)
	{
	}

	public TemplateNotFoundException(string name, object key)
		: base($"Entity \"{name}\" ({key}) was not found.")
	{

	}
}