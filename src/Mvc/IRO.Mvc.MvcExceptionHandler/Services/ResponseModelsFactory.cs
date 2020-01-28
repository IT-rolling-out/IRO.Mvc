using Microsoft.AspNetCore.Http;
using IRO.Common.Collections;
using IRO.Mvc;
using IRO.Mvc.MvcExceptionHandler.Models;
using System;
using System.Threading.Tasks;
using IRO.Mvc.Core;
using IRO.Mvc.MvcExceptionHandler.Controllers;
using Microsoft.Extensions.Logging;

namespace IRO.Mvc.MvcExceptionHandler.Services
{
    public class ResponseModelsFactory
    {
        readonly ILogger _logger;

        public ResponseModelsFactory(ILogger<ResponseModelsFactory> logger)
        {
            _logger = logger;
        }

        public async Task<string> CreateDebugUrl(ErrorContext errorContext)
        {
            var ctx = errorContext.HttpContext;
            var htmlText = await ctx.ExecuteDevExceptionPage(errorContext.OriginalException);

            var methodPath = "DevExceptionsPage/" + DevExceptionsPageController.AddException(
                htmlText
                );
            var host = errorContext.Configs.Host ?? "";
            if (!host.EndsWith("/"))
                host += "/";
            return host + methodPath;
        }

        public async Task<ErrorDTO> CreateErrorData(ErrorContext errorContext)
        {
            var errorInfo = errorContext.ErrorInfo;
            var err = new ErrorDTO();
            err.ErrorKey = errorInfo.ErrorKey;
            err.InfoUrl = errorContext.Configs.ErrorDescriptionUrlHandler?.GenerateUrl(errorInfo.ErrorKey);

            if (errorContext.Configs.IsDebug)
            {
                if (errorContext.OriginalException != null)
                {
                    err.Message = errorContext.OriginalException.Message;
                    err.StackTrace = errorContext.OriginalException.ToString();
                    try
                    {
                        err.DebugUrl = await CreateDebugUrl(errorContext);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("Error resolving dev exception page message.\n{0}", ex);
                    }
                }
                err.RequestInfo = CreateRequestInfo(errorContext.HttpContext);
            }
            return err;
        }

        public RequestInfoDTO CreateRequestInfo(HttpContext httpContext)
        {
            var requestInfo = new RequestInfoDTO();
            var req = httpContext.Request;

            try
            {
                requestInfo.QueryParameters = req.Query.PairToDictionary();
            }
            catch { }

            try
            {
                requestInfo.Headers = req.Headers.PairToDictionary();
            }
            catch { }

            try
            {
                requestInfo.Cookies = req.Cookies.PairToDictionary();
            }
            catch { }

            try
            {
                if (req.HasFormContentType)
                {
                    requestInfo.FormParameters = req.Form.PairToDictionary();
                }
            }
            catch { }

            try
            {
                requestInfo.ContentLength = req.ContentLength ?? 0;
                var lengthInKB = requestInfo.ContentLength / 1024;
                if (lengthInKB <= 500)
                {
                    requestInfo.BodyText = httpContext.GetRequestBodyText();
                }
            }
            catch { }

            requestInfo.RequestPath = req.Path;
            requestInfo.ContentType = req.ContentType;
            return requestInfo;
        }
    }
}
