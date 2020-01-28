using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IRO.Mvc.CoolSwagger
{
    class AdditionalSettingsDocumentFilter : IDocumentFilter
    {
        Action<OpenApiDocument> _action;

        public AdditionalSettingsDocumentFilter(Action<OpenApiDocument> action)
        {
            _action = action;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            _action(swaggerDoc);
        }

    }
}
