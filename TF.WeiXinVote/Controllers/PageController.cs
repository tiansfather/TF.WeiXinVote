using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TF.WeiXinVote.Controllers
{
    public class PageController : Controller
    {
        //
        // GET: /Page/

        public ActionResult Show(int id)
        {
            var page = Data.SqlFactory.GetSqlhelper().CreateWhere<Data.VotePages>()
                .Where(o => o.ID == id).Single();
            return View(page);
        }

    }
}
