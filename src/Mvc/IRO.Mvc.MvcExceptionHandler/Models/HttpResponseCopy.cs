using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IRO.Mvc.MvcExceptionHandler.Controllers
{
    class HttpResponseCopy
    {
        public string ContentString { get; set; }

        public int StatusCode { get; set; }

        public KeyValuePair<string, StringValues>[] Headers { get; set; }



        public static HttpResponseCopy FromHttpResponse(HttpResponse httpResponse, string contentsString)
        {
            var res = new HttpResponseCopy();
            res.ContentString = contentsString;
            res.StatusCode = httpResponse.StatusCode;
            res.Headers = httpResponse.Headers.ToArray();
            return res;
        }

        public async Task WriteToHttpResponse(HttpResponse httpResponse)
        {
            httpResponse.StatusCode = StatusCode;
            httpResponse.Headers.Clear();
            foreach (var item in Headers)
            {
                httpResponse.Headers[item.Key] = item.Value;
            }
            await httpResponse.WriteAsync(ContentString);
        }
    }
}