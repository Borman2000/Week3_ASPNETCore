using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Yarp.ReverseProxy.Swagger;

namespace Yarp.Gateway.Extensions;

public static class SwaggerExtensions
{
//    public static void ConfigureSwaggerEndpoints(this SwaggerUIOptions options, ReverseProxyDocumentFilterConfig config)
//    {
//        if (config.Swagger.IsCommonDocument)
//        {
//            var name = config.Swagger.CommonDocumentName;
//            options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
//        }
//        else
//        {
//            foreach (var cluster in config.Clusters)
//            {
//                var name = cluster.Key;
//                foreach (var dest in config.Clusters[name].Destinations.Values)
//                {
//	                foreach (var swagger in dest.Swaggers ?? Enumerable.Empty<ReverseProxyDocumentFilterConfig.Cluster.Destination.Swagger>())
//	                {
//		                var url = dest.Address;
//		                if(dest.Address.EndsWith("/") && swagger.MetadataPath.StartsWith("/"))
//			                url += swagger.MetadataPath.Substring(1);
//		                else
//			                url += swagger.MetadataPath;
//
//		                options.SwaggerEndpoint(url, name);
//	                }
//                }
////                options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
//            }
//        }
//    }

    public static void ConfigureSwaggerEndpoints(
	    this SwaggerUIOptions options,
	    ReverseProxyDocumentFilterConfig config
    )
    {
	    if (config.Swagger.IsCommonDocument)
	    {
		    var name = config.Swagger.CommonDocumentName;
		    options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
	    }
	    else
	    {
		    foreach (var cluster in config.Clusters)
		    {
			    var name = cluster.Key;
			    options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
		    }
	    }
    }

    public static void ConfigureSwaggerDocs(this SwaggerGenOptions options, ReverseProxyDocumentFilterConfig config)
    {
        if (config.Swagger.IsCommonDocument)
        {
            var name = config.Swagger.CommonDocumentName;
            options.SwaggerDoc(name, new OpenApiInfo {Title = name, Version = name});
        }
        else
        {
            foreach (var cluster in config.Clusters)
            {
                var name = cluster.Key;
                options.SwaggerDoc(name, new OpenApiInfo {Title = name, Version = name});
            }
        }
    }
}