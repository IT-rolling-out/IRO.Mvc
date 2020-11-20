using System;

namespace IRO.Mvc.CoolSwagger
{
    public class ResponseDescription
    {
        public int StatusCode { get; set; }

        public string Description { get; set; }

        public Type Type { get; set; }
    }
}