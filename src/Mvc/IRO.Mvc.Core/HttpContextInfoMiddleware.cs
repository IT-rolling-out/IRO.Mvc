using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IRO.Common.Collections;
using IRO.Mvc.Core.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IRO.Mvc.Core
{
    public static class HttpContextInfoMiddleware
    {
        /// <summary>
        /// Return all data that can resolve.
        /// </summary>
        public static async Task<HttpContextInfo> ResolveInfo(this HttpContext httpContext)
        {
            var dto = new HttpContextInfo();
            var request = httpContext.Request;
            dto.Request.Method = request.Method;
            try
            {
                var dict = new Dictionary<string, IEnumerable<string>>();
                foreach (var item in request.Query)
                {
                    dict[item.Key] = item.Value;
                }
                dto.Request.QueryParameters = dict;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            try
            {
                var dict = new Dictionary<string, IEnumerable<string>>();
                foreach (var item in request.Headers)
                {
                    dict[item.Key] = item.Value;
                }
                dto.Request.Headers = dict;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            try
            {
                dto.Request.Cookies = (IDictionary<string, string>)request.Cookies.PairToDictionary<string, string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            try
            {
                if (request.HasFormContentType)
                {
                    var dict = new Dictionary<string, IEnumerable<string>>();
                    foreach (var item in request.Form)
                    {
                        dict[item.Key] = item.Value;
                    }
                    dto.Request.Headers = dict;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            try
            {
                dto.Request.ContentLength = request.ContentLength.GetValueOrDefault();
                dto.Request.BodyText = await httpContext.GetRequestBodyText();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            dto.Request.Path = (string)request.Path;
            dto.Request.ContentType = request.ContentType;
            


            var resp = httpContext.Response;
            try
            {
                var dict = new Dictionary<string, IEnumerable<string>>();
                foreach (var item in resp.Headers)
                {
                    dict[item.Key] = item.Value;
                }
                dto.Response.Headers = dict;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            try
            {
                dto.Response.ContentLength = resp.ContentLength.GetValueOrDefault();
                dto.Response.BodyText =await httpContext.GetResponseBodyText();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            dto.Response.ContentType = resp.ContentType;
            dto.Response.StatusCode = resp.StatusCode;
            return dto;
        }


    }

}
