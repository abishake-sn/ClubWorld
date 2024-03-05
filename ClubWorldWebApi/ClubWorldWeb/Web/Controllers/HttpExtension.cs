using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Web.Controllers
{
    public static partial class HttpExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_request"></param>
        /// <returns></returns>
        public static Uri GetUri(this HttpRequest p_request)
        {
            return new Uri(p_request.GetDisplayUrl());
        }
    }
}
