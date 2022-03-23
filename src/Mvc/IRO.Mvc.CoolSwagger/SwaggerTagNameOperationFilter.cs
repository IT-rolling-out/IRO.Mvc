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
            MethodInfo methodInfo = context.MethodInfo;
            SwaggerTagNameAttribute tagNameAttribute = methodInfo.GetCustomAttribute<SwaggerTagNameAttribute>();
            if (tagNameAttribute == null)
            {
                Type reflectedType = methodInfo.ReflectedType;
                tagNameAttribute =
                    reflectedType != null ?
                    reflectedType.GetCustomAttribute<SwaggerTagNameAttribute>(false) :
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
            SwaggerOperationAttribute customAttribute = methodInfo.GetCustomAttribute<SwaggerOperationAttribute>();
            if (customAttribute == null)
                return;
            operation.OperationId = customAttribute.OperationId;
        }
    }
}
