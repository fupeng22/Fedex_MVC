using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FedexSystem.Filter
{
    public class RequiresLoginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在action method之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;
            if (filterContext.HttpContext.Session["Global_UserName"] == null)
            {
                filterContext.HttpContext.Response.Redirect("~/Login/Index" + "?URLRet=" + redirectOnSuccess + "&comment=2");
            }
        }
    }
}