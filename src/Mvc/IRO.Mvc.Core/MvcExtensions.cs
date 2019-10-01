using System.IO;
using IRO.Common.Services;
using Microsoft.AspNetCore.Http;

namespace IRO.Mvc.Core
{
    public static class MvcExtensions
    {
        /// <summary>
        /// Current name used as key to save request body text in HttpContext.Items.
        /// </summary>
        public const string RequestBodyTextItemName = "RequestBodyText";

        /// <summary>
        /// Current name used as key to save request body text in HttpContext.Items.
        /// </summary>
        public const string ResponseBodyTextItemName = "ResponseBodyText";

        /// <summary>
        /// Read request contetn to string and then return cached value.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetRequestBodyText(this HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(RequestBodyTextItemName, out var cachedText))
            {
                return (string)cachedText;
            }
            string text = ReadStream(httpContext.Request.Body);
            httpContext.Items[RequestBodyTextItemName] = text;
            return text;
        }

        public static string GetResponseBodyText(this HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(ResponseBodyTextItemName, out var cachedText))
            {
                return (string)cachedText;
            }
            string text = ReadStream(httpContext.Response.Body);
            httpContext.Items[ResponseBodyTextItemName] = text;
            return text;
        }

        static string ReadStream(Stream stream)
        {
            var position = stream.Position;
            stream.Position = 0;
            var responseBody = new StreamReader(stream).ReadToEnd();
            stream.Position = position;
            return responseBody;
        }
    }
}
