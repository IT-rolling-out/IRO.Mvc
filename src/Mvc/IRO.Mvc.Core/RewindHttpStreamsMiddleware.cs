using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IRO.Mvc.Core
{
    /// <summary>
    /// Enable buffering on request and response streams.
    /// </summary>
    public class RewindHttpStreamsMiddleware
    {
        /// <summary>
        /// Current name used as key to save request body text in HttpContext.Items.
        /// </summary>
        internal const string RewindHttpName = "RewindHttp";

        private readonly RequestDelegate next;

        public RewindHttpStreamsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Request buf.
            context.Request.EnableBuffering();

            //Response buf.
            Stream originalBody = context.Response.Body;
            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    context.Items[RewindHttpName] = true;
                    await next(context);

                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }

            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}