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
using System.Net;
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

        static readonly IDictionary<string, string> _cachedPages
           = new ConcurrentDictionary<string, string>();

        IHostingEnvironment _hostingEnvironment;

        string HostAddress { get; set; } = "https://localhost:5001";

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
                if (_cachedPages.TryGetValue(id, out var cachedPage))
                {
                    //?Return html from cache.
                    await Response.WriteAsync(cachedPage);
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(HostAddress) && false)
                {
                    //?Send request to get full html and return what needed.
                    var urlToRequest = HostAddress + $"/DevExceptionsPage/NoCache/{id}";
                    var req = WebRequest.Create(urlToRequest);
                    var resp = req.GetResponse();
                    using (var stream = resp.GetResponseStream())
                    {
                        var responseBodyText = new StreamReader(stream).ReadToEnd();
                        if (responseBodyText.Length > 10)
                        {
                            //Save to cache.
                            _cachedPages[id] = responseBodyText;
                            await Response.WriteAsync(responseBodyText);
                            return;
                        }
                    }
                }

                if (_exceptionsDict.ContainsKey(id))
                {
                    //?Execute middleware and save current response to cache.
                    var swapStream = new MemoryStream();
                    var originalResponseBody = HttpContext.Response.Body;
                    HttpContext.Response.Body = swapStream;
                    //Run middleware.
                    await UseDevExceptionPageMiddleware(id);

                    //Read stream.
                    swapStream.Seek(0, SeekOrigin.Begin);
                    var responseBodyText = new StreamReader(swapStream).ReadToEnd();
                    swapStream.Seek(0, SeekOrigin.Begin);

                    //Save to cache.
                    await swapStream.CopyToAsync(originalResponseBody);
                    HttpContext.Response.Body = originalResponseBody;
                    _cachedPages[id] = responseBodyText;
                    await Response.WriteAsync(responseBodyText);
                }
                else
                {
                    await Response.WriteAsync(
                        "Can`t find exception object in cache. Maybe it removed or save on another server node."
                        );
                }

            }
            catch (Exception ex)
            {
                await Response.WriteAsync(
                       $"Error in exception handler: '{ex}'."
                       );
            }
        }

        [Route("NoCache/{id}")]
        [HttpGet]
        public async Task OpenWithoutCache(string id)
        {
            if (!await UseDevExceptionPageMiddleware(id))
            {
                await Response.WriteAsync(
                       "Can't resolve exception.");
            }
        }

        async Task<bool> UseDevExceptionPageMiddleware(string id)
        {
            if (_exceptionsDict.TryGetValue(id, out var data))
            {
                //Run middleware.
                RequestDelegate next = async (ctx) =>
                {
                    var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(data.Item1);
                    exceptionDispatchInfo.Throw();
                };
                var loggerFactory = new LoggerFactory();
                var opt = new DeveloperExceptionPageOptions();
                var diagnosticSource = new DiagnosticListener("");
                var developerExceptionPageMiddleware = new DeveloperExceptionPageMiddleware(
                    next,
                    Options.Create(opt),
                    loggerFactory,
                    _hostingEnvironment,
                    diagnosticSource
                );
                await developerExceptionPageMiddleware.Invoke(data.Item2);
                return true;
            }
            return false;
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
