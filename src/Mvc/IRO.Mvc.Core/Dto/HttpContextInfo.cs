using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace IRO.Mvc.Core.Dto
{
    public class HttpContextInfo
    {
        public int Id { get; set; }

        public RequestInfo Request { get; set; } = new RequestInfo();

        public ResponseInfo Response { get; set; } = new ResponseInfo();

        public class RequestInfo
        {
            public string Path { get; set; }

            public string Method { get; set; }

            public string ContentType { get; set; }

            public long ContentLength { get; set; }

            public string BodyText { get; set; }

            public IDictionary<string, StringValues> Headers { get; set; }

            public IDictionary<string, string> Cookies { get; set; }

            public IDictionary<string, StringValues> QueryParameters { get; set; }

            public IDictionary<string, StringValues> FormParameters { get; set; }

            
        }

        public class ResponseInfo
        {
            public int StatusCode { get; set; }

            public string ContentType { get; set; }

            public long ContentLength { get; set; }

            public string BodyText { get; set; }

            public IDictionary<string, StringValues> Headers { get; set; }
        }

    }
}
