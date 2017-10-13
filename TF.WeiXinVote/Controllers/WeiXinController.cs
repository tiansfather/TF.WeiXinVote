namespace TF.WeiXinVote.Controllers
{
    using Senparc.Weixin.MP;
    using Senparc.Weixin.MP.Containers;
    using Senparc.Weixin.MP.Entities.Request;
    using Senparc.Weixin.MP.MvcExtension;
    using Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using TF.WeiXinVote.Data;
    using System.Collections.Generic;
    using System.Linq;
    using Senparc.Weixin.MP.AdvancedAPIs.Media;

    public class WeiXinController : Controller
    {
        private readonly Func<string> _getRandomFileName = () => (DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6));

        //[CompilerGenerated]
        //private static string <.ctor>b__0() => 
        //    (DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6));

        [ActionName("Index"), HttpGet]
        public ActionResult Get(PostModel postModel, string echostr, int id)
        {
            string token = SqlFactory.GetSqlhelper().SingleById<VoteInfos>(id).Token;
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token))
            {
                return base.Content(echostr);
            }
            return base.Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, token) + "。如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
        }

        [ActionName("Index"), HttpPost]
        public ActionResult MiniPost(PostModel postModel, int id)
        {
           
            try
            {
                VoteInfos infos = SqlFactory.GetSqlhelper().SingleById<VoteInfos>(id);
                string token = infos.Token;
                string encodingAESKey = infos.EncodingAESKey;
                string corpID = infos.CorpID;
                base.Session["VoteInfo"] = infos;
                if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token))
                {
                    return new WeixinResult("参数错误！");
                }
                postModel.Token = token;
                postModel.EncodingAESKey = encodingAESKey;
                postModel.AppId = corpID;
                Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler.CustomMessageHandler messageHandlerDocument = new Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler.CustomMessageHandler(base.Request.InputStream, postModel, 10);
                messageHandlerDocument.Execute();
                return new FixWeixinBugWeixinResult(messageHandlerDocument);
            }
            catch (Exception ex)
            {
                
                return null;
            }
            
        }

        public ActionResult VoteRedirect(string openid, int voteid)
        {
            base.Session["openid"] = openid;
            return this.Redirect("/Mpage/Index.aspx?voteid=" + voteid);
        }

        public ActionResult Media(int voteid,string key="")
        {
            var vote = SqlFactory.GetSqlhelper().CreateWhere<VoteInfos>()
                .Where(o => o.ID == voteid).SingleOrDefault();

            var accessToken = AccessTokenContainer.TryGetAccessToken(vote.CorpID, vote.Secret);
            var items = new List<MediaList_News_Item>();
            for(var i = 0; i < 5; i++)
            {
                var result = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.GetNewsMediaList(accessToken, 20*i, 20);
                items.AddRange(result.item);
            }
            if (!string.IsNullOrEmpty(key))
            {
                items=items.Where(o => {
                    var valid = false;
                    foreach (var news_item in o.content.news_item)
                    {
                        if (news_item.title.Contains(key))
                        {
                            valid = true;
                            break;
                        }
                    }
                    return valid;
                }).ToList();
            }
            
            return Json(items.Select(o=>new { o.media_id,list=o.content.news_item.Select(n=>n.title)}), JsonRequestBehavior.AllowGet);
        }
    }
}

