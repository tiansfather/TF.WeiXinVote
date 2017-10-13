namespace TF
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UserAttribute : FilterAttribute, IAuthorizationFilter
    {
        public const string LoginUrl = "/Admin/Login";

        protected void CookieSet(string cookiename, string cookievalue, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(cookiename) {
                Value = cookievalue,
                Expires = expires
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private void GotoLogin(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ContentResult result = new ContentResult {
                    Content = $"<script>top.location.href = '{"/Admin/Login"}';</script>",
                    ContentType = "text/html"
                };
                filterContext.Result = result;
            }
            else
            {
                JsonResult result2 = new JsonResult {
                    Data = new { 
                        ok = false,
                        jumpUrl = "/Admin/Login",
                        errCode = 1,
                        errMsg = "超时，请重新登入."
                    }
                };
                filterContext.Result = result2;
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session["user"] == null)
            {
                this.GotoLogin(filterContext);
            }
        }
    }
}

