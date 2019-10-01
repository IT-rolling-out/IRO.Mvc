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
        static readonly IDictionary<string, string> _cachedPages = new ConcurrentDictionary<string, string>();

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
        

        /// <summary>
        /// Return id of page where you can open exception.
        /// Example: 'https://yourhost.com/DevExceptionsPage/jr2DkM230d
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string AddException(string htmlText)
        {
            if (_cachedPages.Count > 1000)
            {
                _cachedPages.Clear();
            }
            var id = TextExtensions.Generate(10);
            _cachedPages.Add(id, htmlText);
            return id;
        }
    }
}
