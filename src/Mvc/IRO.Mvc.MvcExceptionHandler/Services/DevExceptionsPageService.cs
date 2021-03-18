using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Common.Text;
using Microsoft.AspNetCore.Http;

namespace IRO.Mvc.MvcExceptionHandler.Services
{
    public class DevExceptionsPageService
    {
        readonly IDictionary<string, string> _cachedPages = new ConcurrentDictionary<string, string>();

        public async Task ShowExceptionPage(HttpContext ctx)
        {
            var url = ctx.Request.Path.Value;
            var index = url.LastIndexOf("/");
            var id = url.Substring(index + 1);


            try
            {
                if (_cachedPages.TryGetValue(id, out var cachedPage))
                {
                    //?Return html from cache.
                    await ctx.Response.WriteAsync(cachedPage);
                }
                else
                {
                    await ctx.Response.WriteAsync(
                        "Can`t find exception object in cache. Maybe it removed or save on another server node."
                    );
                }

            }
            catch (Exception ex)
            {
                await ctx.Response.WriteAsync(
                    $"Error in exception handler: '{ex}'."
                );
            }
        }


        /// <summary>
        /// Return id of page where you can open exception.
        /// Example: 'https://yourhost.com/DevExceptionsPage/jr2DkM230d
        /// </summary>
        /// <returns></returns>
        public string AddException(string htmlText)
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
