using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;

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

    public class SwaggerTagNameOperationFilter2 : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            MethodInfo methodInfo = context.MethodInfo;
            SwaggerTagNameAttribute tagNameAttribute = CustomAttributeExtensions.GetCustomAttribute<SwaggerTagNameAttribute>(methodInfo);
            if (tagNameAttribute == null)
            {
                Type reflectedType = methodInfo.ReflectedType;
                tagNameAttribute =
                    reflectedType != null ?
                    CustomAttributeExtensions.GetCustomAttribute<SwaggerTagNameAttribute>(reflectedType, false) :
                    null;
            }
            if (tagNameAttribute != null)
            {
                string str = tagNameAttribute.TagName.Trim();
                if (string.IsNullOrWhiteSpace(str))
                    throw new Exception("Tag name can`t be null or whitespace in method '" + ((methodInfo).ReflectedType).Name + "." + (methodInfo).Name + "'.");
                OpenApiOperation openApiOperation = operation;
                List<OpenApiTag> openApiTagList = new List<OpenApiTag>
                {
                    new OpenApiTag() { Name = str }
                };
                openApiOperation.Tags = openApiTagList;
            }
            operation.OperationId = (methodInfo).Name;
            SwaggerOperationAttribute customAttribute = CustomAttributeExtensions.GetCustomAttribute<SwaggerOperationAttribute>(methodInfo);
            if (customAttribute == null)
                return;
            operation.OperationId = customAttribute.OperationId;
        }
    }
}
