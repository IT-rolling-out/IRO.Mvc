using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using IRO.Mvc.Core.Dto;
using IRO.Mvc.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IRO.Mvc.Core
{
    public static class RequestsLoggingMiddlewareExtension
    {
        static IRequestsLoggingService _logService;
        static string _pathToLogs;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void UseAllRequestsLogging(this IApplicationBuilder app, string pathToLogs = "logs")
        {
            
            _pathToLogs = "/" + pathToLogs;
            _logService = app.ApplicationServices.GetService<IRequestsLoggingService>() ??
                          new InMemoryRequestsLoggingService();
            AddHttpCallsLogs(app);
            app.Use(async (ctx, next) =>
            {
                try
                {
                    var path = ctx.Request.Path.Value;
                    if (ctx.Request.Path.Value.StartsWith(_pathToLogs))
                    {
                        var reqQuery = ctx.Request.Query;
                        var resp = ctx.Response;
                       
                        List<HttpContextInfo> records;
                        if (reqQuery.TryGetValue("id", out var idStr))
                        {
                            var id = Convert.ToInt32(idStr);
                            var rec = await _logService.GetRecord(id);
                            records = new List<HttpContextInfo>{ rec };
                        }
                        else
                        {
                            records = (await _logService.GetAllRecords()).ToList();

                        }
                        records.Reverse();
                        resp.ContentType = "application/json";
                        resp.StatusCode = 500;
                        var logRecordsStr = JsonConvert.SerializeObject(records, Formatting.Indented);
                        await resp.WriteAsync(logRecordsStr);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    await next();
                }
            });

        }

        static void AddHttpCallsLogs(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                try
                {
                    await next();
                }
                finally
                {
                    try
                    {
                        if (!ctx.Request.Path.Value.StartsWith(_pathToLogs))
                        {
                            var info = await ctx.ResolveInfo();
                            await _logService.SaveLogRecord(info);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            });

        }
    }
}
