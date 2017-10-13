namespace TF.WeiXinVote.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;
    using TF.WeiXinVote.Data;
    using TF.WeiXinVote.MPage.Code;

    public class UseController : Controller
    {
        public ActionResult Index(string mobile = "")
        {
            List<VoteBonusRecord> model = new List<VoteBonusRecord>();
            if (!string.IsNullOrEmpty(mobile))
            {
                model = SqlFactory.GetSqlhelper().CreateWhere<VoteBonusRecord>().Where(o => o.UseDate == null).Where(o => o.Mobile == mobile).Select();
            }
            return base.View(model);
        }

        [HttpPost]
        public ActionResult Submit(int id)
        {
            AjaxResult data = new AjaxResult();
            try
            {
                VoteBonusRecord poco = SqlFactory.GetSqlhelper().SingleById<VoteBonusRecord>(id);
                poco.UseDate = new DateTime?(DateTime.Now);
                SqlFactory.GetSqlhelper().Save<VoteBonusRecord>(poco);
                data.ErrCode = 0;
            }
            catch (Exception exception)
            {
                data.ErrCode = -1;
                data.Message = exception.Message;
            }
            return base.Json(data);
        }
    }
}

