namespace TF.WeiXinVote.Controllers
{
    using Senparc.Weixin.MP.AdvancedAPIs;
    using Senparc.Weixin.MP.Containers;
    using SoMain.Common.DataAttribute;
    using System;
    using System.Web.Mvc;
    using TF.WeiXinVote.Data;

    public class InitController : Controller
    {
        public ActionResult API(){
            return base.View();
        } 
            

        public ActionResult Index()
        {

            //var voteinfo = SqlFactory.GetSqlhelper().CreateWhere<VoteInfos>()
            //    .Where(o => o.ID == 2).Single();

            //try
            //{
            //    string accessTokenOrAppId = AccessTokenContainer.TryGetAccessToken(voteinfo.CorpID, voteinfo.Secret, false);
            //    var rules = AutoReplyApi.GetCurrentAutoreplyInfo(accessTokenOrAppId);
            //    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rules));
            //}
            //catch (Exception ex)
            //{
            //    return Content(ex.Message);
            //}
            
            return base.Content("");
        }

        public ActionResult Set()
        {
            base.Session["openid"] = "111111";
            return base.Content(base.Session["openid"].ToString());
        }
    }
}

