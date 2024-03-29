﻿using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using IRO.Mvc.MvcExceptionHandler.Models;
using IRO.Mvc.MvcExceptionHandler.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IRO.Mvc.MvcExceptionHandler
{
    class ExHandlerMiddleware
    {
        const string ExText = "Exception while processing request exception! Contact developer of lib.";
        readonly IExceptionHandlerConfigs _configs;
        readonly MvcExceptionHandlerSetup _setup;
        readonly IErrorInfoResolver _errorInfoResolver;
        readonly ResponseModelsFactory _responseModelsFactory;
        readonly JsonSerializerSettings _jsonSerializerSettings;
        readonly ILogger _logger;
        readonly DevExceptionsPageService _devExceptionsPageService;

        public ExHandlerMiddleware(
            Action<MvcExceptionHandlerSetup> setupAction, 
            ResponseModelsFactory responseModelsFactory,
            ILogger<ExHandlerMiddleware> logger,
            DevExceptionsPageService devExceptionsPageService
            )
        {
            _responseModelsFactory = responseModelsFactory;
            _logger = logger;
            _devExceptionsPageService = devExceptionsPageService;
            _setup = new MvcExceptionHandlerSetup();
            setupAction(_setup);
            _configs = _setup.CreateConfigs();

            var erroInfoRegistry = new ErrorInfoRegistry(
                _setup.KeyValidator,
                _setup.KeyGenerator,
                _setup.DefaultHttpCode
                );
            _setup.ActionToRegisterExceptions(erroInfoRegistry);
            _errorInfoResolver = erroInfoRegistry.Build();
            _jsonSerializerSettings = _setup.JsonSerializerSettings;
        }

        public async Task RequestProcessing(HttpContext ctx, Func<Task> next)
        {
            var url = ctx.Request.Path.Value;
            if (url.StartsWith("/DevExceptionsPage"))
            {
                await _devExceptionsPageService.ShowExceptionPage(ctx);
                return;
            }

            ErrorInfo ? errorInfo = null;
            Exception exception = null;
            Exception innerException = null;
            bool exceptionProcessedHere;
            ExceptionDispatchInfo exceptionDispatchInfo = null;

            try
            {
                await next();
                //exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            }
            catch (Exception ex)
            {
                exception = ex;
                exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
            }

            try
            {
                //Error by inner exception.  
                if (exception != null)
                {

                    innerException = _configs.InnerExceptionsResolver(exception);
                    errorInfo = _errorInfoResolver.TryGetByException(innerException.GetType());
                }

                //Error by http code.
                if (errorInfo == null && _configs.CanBindByHttpCode)
                {
                    bool bodyHasContent = ctx.Response.ContentLength > 0;
                    if (bodyHasContent)
                        return;
                    var httpCode = ctx.Response.StatusCode;
                    errorInfo = _errorInfoResolver.TryGetByHttCode(httpCode);
                }

                if (errorInfo == null)
                {
                    exceptionProcessedHere = false;
                }
                else
                {
                    await OnError(
                        ctx,
                        errorInfo.Value,
                        exception,
                        innerException
                        );
                    exceptionProcessedHere = true;
                }
            }
            catch (Exception ex)
            {
                //If exception in current middleware.
                var ownException = new ErrorHandlerException(ExText, ex);
                _setup.OwnExceptionsHandler?.Invoke(ownException);
                throw ownException;
            }

            //Give it to else middleware.
            if (!exceptionProcessedHere && exception != null)
            {
                exceptionDispatchInfo.Throw();
            }
        }

        public async Task OnError(
            HttpContext httpContext,
            ErrorInfo errorInfo,
            Exception exception = null,
            Exception innerException = null
            )
        {
            var errorContext = new ErrorContext()
            {
                Configs = _configs,
                ErrorInfo = errorInfo,
                HttpContext = httpContext,
                InnerException = innerException,
                OriginalException = exception
            };

            try
            {
                if (_setup.FilterBeforeDTO != null)
                {
                    var handled = await _setup.FilterBeforeDTO?.Invoke(errorContext);
                    if (handled)
                        return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FilterBeforeDTO.", ex);
            }

            var errorDTO = errorContext.ResponseDTO = await _responseModelsFactory.CreateErrorData(errorContext);

            try
            {
                if (_setup.FilterAfterDTO != null)
                {
                    var handled = await _setup.FilterAfterDTO?.Invoke(errorContext);
                    if (handled)
                        return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FilterAfterDTO.", ex);
            }

            var jsonResponse = JsonConvert.SerializeObject(errorDTO, _jsonSerializerSettings);
            httpContext.Response.StatusCode = errorContext.ErrorInfo.HttpCode ?? 500;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(jsonResponse);
        }


    }
}
