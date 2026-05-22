using Microsoft.Extensions.Logging;
using NotificationAPI.Application.Common.Exceptions;
using NotificationAPI.Domain;
using NotificationAPI.Domain.Entities;
using NotificationAPI.Domain.Interfaces;
using Scriban;
using Scriban.Runtime;
using Template = Scriban.Template;

namespace NotificationAPI.Infrastructure.Services;

public interface ITemplateService
{
    Task<RenderedTemplate> RenderAsync(string templateId, Dictionary<string, object> data);
}

public class TemplateService : ITemplateService
{
    private readonly ITemplateRepository _repository;
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(ITemplateRepository repository, ILogger<TemplateService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<RenderedTemplate> RenderAsync(string templateId, Dictionary<string, object> data)
    {
        var template = await _repository.GetByIdAsync(templateId);
        if (template == null)
        {
			template = await _repository.GetByIdAsync(NotificationTemplate.DEFAULT_TEMPLATE_ID);

            if (template == null)
                throw new TemplateNotFoundException(templateId);

            _logger.LogWarning("Template with ID {TemplateId} not found. Using default template.", templateId);
        }

        // Create a script object with the template data
        var scriptObject = new ScriptObject();
        foreach (var kvp in data)
        {
            scriptObject.Add(kvp.Key, kvp.Value);
        }

        // Add helper functions
        scriptObject.Import("format_currency", new Func<decimal, string, string>(FormatCurrency));
        scriptObject.Import("format_date", new Func<DateTime, string, string>(FormatDate));

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        // Render subject and body
        var subjectTemplate = Template.Parse(template.Subject);
        var bodyTemplate = Template.Parse(template.Body);

        var renderedSubject = await subjectTemplate.RenderAsync(context);
        var renderedBody = await bodyTemplate.RenderAsync(context);

        return new RenderedTemplate
        {
            Subject = renderedSubject,
            Body = renderedBody,
            IsHtml = template.IsHtml
        };
    }

    private string FormatCurrency(decimal amount, string currency)
    {
        return currency.ToUpper() switch
        {
            "USD" => $"${amount:N2}",
            "EUR" => $"{amount:N2} EUR",
            "GBP" => $"{amount:N2} GBP",
            _ => $"{amount:N2} {currency}"
        };
    }

    private string FormatDate(DateTime date, string format)
    {
        return date.ToString(format);
    }
}
