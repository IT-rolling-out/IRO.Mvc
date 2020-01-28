using System;
using IRO.Mvc.MvcExceptionHandler.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IRO.Mvc.MvcExceptionHandler
{
    public static class MvcExceptionHandlerExtensions
    {
        /// <summary>
        /// Must be registered after UseDeveloperExceptionPage, but before anything else.
        /// </summary>
        public static IApplicationBuilder UseMvcExceptionHandler(this IApplicationBuilder app, Action<MvcExceptionHandlerSetup> setupAction)
        {
            var responseModelsFactory = app.ApplicationServices.GetRequiredService<ResponseModelsFactory>();
            var logger = app.ApplicationServices.GetRequiredService<ILogger<ExHandlerMiddleware>>();
            var middleware = new ExHandlerMiddleware(setupAction, responseModelsFactory, logger);
            app.Use(middleware.RequestProcessing);
            return app;
        }

        public static IServiceCollection AddMvcExceptionHandler(this IServiceCollection serv)
        {
            var responseModelsFactory = serv.AddTransient<ResponseModelsFactory>();
            return serv;
        }
    }
}
