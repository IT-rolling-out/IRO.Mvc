using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace IRO.Mvc.CoolSwagger
{
    public class SwaggerTagNameOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var mi = context.MethodInfo;
            var attr = mi.GetCustomAttribute<SwaggerTagNameAttribute>();
            attr = attr ?? mi.DeclaringType.GetCustomAttribute<SwaggerTagNameAttribute>();
            if (attr != null)
            {
                var tagName = attr.TagName.Trim();
                if (string.IsNullOrWhiteSpace(tagName))
                {
                    throw new Exception($"Tag name can`t be null or whitespace in method '{mi.DeclaringType.Name}.{mi.Name}'.");
                }
                string controllerName = mi.DeclaringType.Name.Replace("Controller", "");
                try
                {
                    operation.Tags.Remove(new OpenApiTag() { Name = controllerName });
                    if (!operation.Tags.Contains(new OpenApiTag() { Name = tagName }))
                        operation.Tags.Add(new OpenApiTag() { Name = tagName });
                }
                catch
                {
                    operation.Tags = new List<OpenApiTag> { new OpenApiTag() { Name = tagName } };
                }
            }

            operation.OperationId = mi.Name;
            var attrOp = mi.GetCustomAttribute<SwaggerOperationAttribute>();
            if (attrOp != null)
            {
                operation.OperationId = attrOp.OperationId;
            }


        }
    }
}
