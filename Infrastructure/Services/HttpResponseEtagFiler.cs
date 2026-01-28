using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Infrastructure.Services;

public class HttpResponseEtagFiler : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var isGetOrHeadRequest = HttpMethods.IsGet(context.HttpContext.Request.Method) || HttpMethods.IsHead(context.HttpContext.Request.Method);
		if (!isGetOrHeadRequest)
		{
			return await next(context);
		}

		var result = await next(context);

		if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
		{
			// generates ETag from the entire response Content
			var etag = ETagService.ComputeWithHashFunction(result);

			if (context.HttpContext.Request.Headers.ContainsKey(HeaderNames.IfNoneMatch))
			{
				// fetch etag from the incoming request header
				var incomingEtag = context.HttpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString();

				// if both the etags are equal
				// raise a 304 Not Modified Response
				if (incomingEtag.Equals(etag))
				{
					context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotModified;
					result = new StatusCodeResult((int)HttpStatusCode.NotModified);
				}
			}

			// add ETag response header
			context.HttpContext.Response.Headers[HeaderNames.ETag] = new[] { etag };
		}

		return result;
	}
}