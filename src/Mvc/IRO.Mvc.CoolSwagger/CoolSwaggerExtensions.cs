using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using IRO.FileIO.ImprovedFileOperations;
using Microsoft.OpenApi.Models;

namespace IRO.Mvc.CoolSwagger
{
    public static class CoolSwaggerExtensions
    {
        public static void AddSwaggerTagNameOperationFilter(this SwaggerGenOptions opt)
        {
            opt.OperationFilter<SwaggerTagNameOperationFilter>();
        }

        /// <summary>
        /// Use custom summory.
        /// </summary>
        /// <param name="opt"></param>
        public static void UseCoolSummaryGen(this SwaggerGenOptions opt)
        {
            opt.OperationFilter<SummaryOperationFilter>();
        }

        public static void UseDefaultIdentityAuthScheme(this SwaggerGenOptions opt)
        {
            opt.OperationFilter<IdentityAuthOperationFilter>(new string[] { }, "Bearer");

            opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\".",
                Name = "Authorization",
                //In = "header",
                //Type = "apiKey"
            });
        }

        public static void SwaggerDocAdditional(this SwaggerGenOptions opt, Action<OpenApiDocument> action)
        {
            opt.DocumentFilter<AdditionalSettingsDocumentFilter>(action);
        }
    }
}
