using System;
using System.IO;
using System.Threading.Tasks;
using IRO.Common.Services;
using Microsoft.AspNetCore.Http;

namespace IRO.Mvc.Core
{
    public static class GetBodyExtensions
    {
        /// <summary>
        /// Current name used as key to save request body text in HttpContext.Items.
        /// </summary>
        internal const string RequestBodyTextItemName = "RequestBodyText";

        /// <summary>
        /// Current name used as key to save request body text in HttpContext.Items.
        /// </summary>
        internal const string ResponseBodyTextItemName = "ResponseBodyText";

        /// <summary>
        /// Read request content to string and then return cached value.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static async Task<string> GetRequestBodyText(this HttpContext ctx)
        {
            CheckRewindMiddleware(ctx);
            if (ctx.Items.TryGetValue(RequestBodyTextItemName, out var cachedText))
            {
                return (string)cachedText;
            }
            string text = await ReadStreamAsync(ctx.Request.Body);
            ctx.Items[RequestBodyTextItemName] = text;
            return text;
        }

        /// <summary>
        /// Read request content to string and then return cached value.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static async Task<string> GetResponseBodyText(this HttpContext ctx)
        {
            CheckRewindMiddleware(ctx);
            if (ctx.Items.TryGetValue(ResponseBodyTextItemName, out var cachedText))
            {
                return (string)cachedText;
            }
            string text = await ReadStreamAsync(ctx.Response.Body);
            ctx.Items[ResponseBodyTextItemName] = text;
            return text;
        }

        static async Task<string> ReadStreamAsync(Stream stream)
        {
            var position = stream.Position;
            stream.Position = 0;
            var responseBody = await new StreamReader(stream).ReadToEndAsync();
            stream.Position = position;
            return responseBody;
        }

        static void CheckRewindMiddleware(HttpContext ctx)
        {
            if (!ctx.Items.ContainsKey(RewindHttpStreamsMiddleware.RewindHttpName))
            {
                throw new Exception($"{nameof(RewindHttpStreamsMiddleware)} must be added on start of pipeline." +
                                    $"\nUse 'app.UseMiddleware<{nameof(RewindHttpStreamsMiddleware)}>();'.");
            }
        }
    }
}
