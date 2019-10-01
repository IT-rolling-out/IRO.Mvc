using IRO.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using IRO.Common.Text;
using IRO.Mvc.Core;

namespace IRO.Mvc.MvcExceptionHandler.Controllers
{
    [Route("DevExceptionsPage/")]
    [ApiController]
    public class DevExceptionsPageController : ControllerBase
    {
        static readonly IDictionary<string, Tuple<Exception, HttpContext>> _exceptionsDict
            = new ConcurrentDictionary<string, Tuple<Exception, HttpContext>>();

        static readonly IDictionary<string, string> _cachedHtmlPages
           = new ConcurrentDictionary<string, string>();

        IHostingEnvironment _hostingEnvironment;

        public DevExceptionsPageController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task Open(string id)
        {
            try
            {
                if (_cachedHtmlPages.TryGetValue(id, out var cachedHtml))
                {
                    await Response.WriteAsync(cachedHtml);
                }
                else if (_exceptionsDict.TryGetValue(id, out var data))
                {
                    using (var swapStream = new MemoryStream())
                    {
                        var originalResponseBody = HttpContext.Response.Body;
                        HttpContext.Response.Body = swapStream;

                        //Run middleware.
                        RequestDelegate next = async (ctx) =>
                        {
                            var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(data.Item1);
                            exceptionDispatchInfo.Throw();
                        };
                        var loggerFactory = new LoggerFactory();
                        var opt = new DeveloperExceptionPageOptions();
                        DiagnosticSource diagnosticSource = new DiagnosticListener("");
                        var developerExceptionPageMiddleware = new DeveloperExceptionPageMiddleware(
                            next,
                            Options.Create(opt),
                            loggerFactory,
                            _hostingEnvironment,
                            diagnosticSource
                        );
                        await developerExceptionPageMiddleware.Invoke(data.Item2);

                        //Save to cache.
                        swapStream.Seek(0, SeekOrigin.Begin);
                        string responseBody = new StreamReader(swapStream).ReadToEnd();
                        swapStream.Seek(0, SeekOrigin.Begin);
                        await swapStream.CopyToAsync(originalResponseBody);
                        HttpContext.Response.Body = originalResponseBody;
                        _cachedHtmlPages[id] = responseBody;
                        await Response.WriteAsync(responseBody);
                    }
                }
                else
                {
                    await Response.WriteAsync(
                        "Can`t find exception object in cache. Maybe it removed or save on another server node.");
                }
            }
            catch (Exception ex)
            {
                await Response.WriteAsync(
                       $"Error in exception handler: '{ex}'.");
            }
        }

        [Route("/NotCache/{id}")]
        [HttpGet]
        public async Task OpenNotCache(string id)
        {
            try
            {
                if (_cachedHtmlPages.TryGetValue(id, out var cachedHtml))
                {
                    await Response.WriteAsync(cachedHtml);
                }
                else if (_exceptionsDict.TryGetValue(id, out var data))
                {
                    using (var swapStream = new MemoryStream())
                    {
                        var originalResponseBody = HttpContext.Response.Body;
                        HttpContext.Response.Body = swapStream;

                        //Run middleware.
                        RequestDelegate next = async (ctx) =>
                        {
                            var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(data.Item1);
                            exceptionDispatchInfo.Throw();
                        };
                        var loggerFactory = new LoggerFactory();
                        var opt = new DeveloperExceptionPageOptions();
                        DiagnosticSource diagnosticSource = new DiagnosticListener("");
                        var developerExceptionPageMiddleware = new DeveloperExceptionPageMiddleware(
                            next,
                            Options.Create(opt),
                            loggerFactory,
                            _hostingEnvironment,
                            diagnosticSource
                        );
                        await developerExceptionPageMiddleware.Invoke(data.Item2);

                        //Save to cache.
                        swapStream.Seek(0, SeekOrigin.Begin);
                        string responseBody = new StreamReader(swapStream).ReadToEnd();
                        swapStream.Seek(0, SeekOrigin.Begin);
                        await swapStream.CopyToAsync(originalResponseBody);
                        HttpContext.Response.Body = originalResponseBody;
                        _cachedHtmlPages[id] = responseBody;
                        await Response.WriteAsync(responseBody);
                    }
                }
                else
                {
                    await Response.WriteAsync(
                        "Can`t find exception object in cache. Maybe it removed or save on another server node.");
                }
            }
            catch (Exception ex)
            {
                await Response.WriteAsync(
                       $"Error in exception handler: '{ex}'.");
            }
        }

        /// <summary>
        /// Return id of page where you can open exception.
        /// Example: 'https://yourhost.com/DevExceptionsPage/jr2DkM230d
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string AddException(Exception exception, HttpContext httpContext)
        {
            if (_exceptionsDict.Count > 200)
            {
                _exceptionsDict.Clear();
            }
            var id = TextExtensions.Generate(10);
            var data = new Tuple<Exception, HttpContext>(
                exception,
                httpContext
                );
            _exceptionsDict.Add(id, data);
            return id;
        }
    }
}
