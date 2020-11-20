using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IRO.Tests.SwashbuckleTesting.Models
{
    public class ServerErrorDto
    {
        public ErrorInternal Error { get; set; }

        public class ErrorInternal
        {
            public string ErrorMsg { get; set; }
        }
    }
}
