using System.Threading.Tasks;
using IRO.Mvc.Core;
using IRO.Mvc.Core.Dto;
using IRO.Tests.ExceptionHandlerTesting.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IRO.Tests.ExceptionHandlerTesting.Controllers
{

    [Route("err/[action]")]
    [ApiController]
    public class ErrorTestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test1()
        {
            return BadRequest();
        }

        [HttpGet]
        public IActionResult Test2()
        {
            throw new ClientException("Exceptions here!");
        }

        [HttpGet]
        public void Test3()
        {
            Task.Run(() =>
            {
                Task.Run(() =>
                {
                    throw new ClientException();
                }).Wait();
            }).Wait();
        }

        [HttpPost]
        public async Task<HttpContextInfo> TestGetBody()
        {
            await Response.WriteAsync("qqqqq");
            return await HttpContext.ResolveInfo();
        }
    }
}
