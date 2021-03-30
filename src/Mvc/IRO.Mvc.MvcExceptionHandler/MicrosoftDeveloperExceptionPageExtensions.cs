using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace IRO.Mvc.MvcExceptionHandler
{
    public static class MicrosoftDeveloperExceptionPageExtensions
    {
        /// <summary>
        /// Execute DevExceptionPage on current context and return html.
        /// </summary>
        public static async Task<string> ExecuteDevExceptionPage(this HttpContext currentHttpContext, Exception exception, HttpContext originalHttpContext = null)
        {
            //Add this headers to make excpetion page always pretty.
            currentHttpContext.Request.Headers[HeaderNames.Accept] =
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            currentHttpContext.Request.Headers[HeaderNames.UserAgent] =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36";

            originalHttpContext ??= currentHttpContext;
            var swapStream = new MemoryStream();
            var originalResponseBody = currentHttpContext.Response.Body;
            try
            {
                currentHttpContext.Response.Body = swapStream;

                //Run middleware.
                RequestDelegate next = async (ctx) =>
                {
                    var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                    exceptionDispatchInfo.Throw();
                };
                var loggerFactory = new LoggerFactory();
                var opt = new DeveloperExceptionPageOptions();
                var diagnosticSource = new DiagnosticListener("");
                var hostingEnvironment = currentHttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
                var developerExceptionPageMiddleware = new DeveloperExceptionPageMiddleware(
                    next,
                    Options.Create<DeveloperExceptionPageOptions>(opt),
                    loggerFactory,
                    hostingEnvironment,
                    diagnosticSource,
                    new List<IDeveloperPageExceptionFilter>()
                );
                await developerExceptionPageMiddleware.Invoke(originalHttpContext);

                //Read stream.
                swapStream.Seek(0, SeekOrigin.Begin);
                var responseBodyText = new StreamReader(swapStream).ReadToEnd();
                swapStream.Seek(0, SeekOrigin.Begin);

                //Save to cache.
                return responseBodyText;
            }
            finally
            {
                //await swapStream.CopyToAsync(originalResponseBody);
                currentHttpContext.Response.Body = originalResponseBody;
            }



        }
    }
}
