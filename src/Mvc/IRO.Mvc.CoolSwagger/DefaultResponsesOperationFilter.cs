using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IRO.Mvc.CoolSwagger
{
    public class DefaultResponsesOperationFilter : IOperationFilter
    {
        readonly IEnumerable<ResponseDescription> _responses;

        public DefaultResponsesOperationFilter(IEnumerable<ResponseDescription> responses)
        {
            _responses = responses;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var resp in _responses)
            {
                try
                {
                    context.SchemaRepository.TryLookupByType(resp.Type, out var scheme);
                    if (scheme == null)
                    {
                        context.SchemaGenerator.GenerateSchema(resp.Type, context.SchemaRepository);
                        context.SchemaRepository.TryLookupByType(resp.Type, out scheme);
                    }
                    var openApiResponse = new OpenApiResponse
                    {
                        Description = resp.Description
                    };
                    openApiResponse.Content["application/json"] = new OpenApiMediaType()
                    {
                        Schema = scheme
                    };
                    operation.Responses.Add(resp.StatusCode.ToString(), openApiResponse);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                }
            }

        }
    }
}