using System.Web;
using System.Web.Mvc;

namespace TF.WeiXinVote
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}