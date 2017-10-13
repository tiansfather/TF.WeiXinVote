using Senparc.Weixin;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TF.WeiXinVote.Data;

namespace TF.WeiXinVote.Controllers
{
    public class PayController : BaseController
    {
        private string appId = "wxc6fb4171e583c51b";
        private string secret = "b0470ac6a8392bbc814c0d224cb1e040";
        private string mchId = "1307757901";
        private string key = "zhejiangtaizhoujingshuiqiweixin1";
        public ActionResult BaseCallback(string code, string state, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return base.RedirectError("您拒绝了授权！");
                }
                if (state != (base.Session["State"] as string))
                {
                    return base.RedirectError("验证失败！请从正规途径进入！");
                }
                OAuthAccessTokenResult result = OAuthApi.GetAccessToken(this.appId, this.secret, code, "authorization_code");
                if (result.errcode != ReturnCode.请求成功)
                {
                    return base.RedirectError("错误：" + result.errmsg);
                }
                
                base.SetCookie("payopenid", result.openid);
                return this.Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }


        }

        public ActionResult Test()
        {
            return Content(Request.Cookies["payopenid"].Value);
        }

        public ActionResult Index(string returnUrl)
        {
            if ((base.Request.Cookies["payopenid"] != null) && !string.IsNullOrEmpty(base.Request.Cookies["payopenid"].Value))
            {
                return this.Redirect(returnUrl);
            }
            string state = "TF-" + DateTime.Now.Millisecond;
            base.Session["State"] = state;
            string url = OAuthApi.GetAuthorizeUrl(this.appId, "http://"+Request.Url.Host+"/Pay/BaseCallback?returnUrl=" + returnUrl.UrlEncode(), state, OAuthScope.snsapi_base, "code", true);
            return this.Redirect(url);
        }

        public ActionResult SendRedPack(string code)
        {
            var record = SqlFactory.GetSqlhelper().CreateWhere<VoteBonusRecord>()
                .Where(o => o.AddInfo == code)
                .Where(o => o.UseDate == null)
                .FirstOrDefault();
            if (record == null)
            {
                return Redirect("/MPage/Info.aspx?info=未找到红包信息");
            }

            var openid = "oI6q_t7v86pILbZKUoFrB-TkovB4";
            openid = Request.Cookies["payopenid"].Value;
            string nonceStr;//随机字符串
            string paySign;//签名
            var sendNormalRedPackResult = RedPackApi.SendNormalRedPack(
                appId, mchId, key,
                @"D:\certjsq\apiclient_cert.p12",     //证书物理地址
                openid,   //接受收红包的用户的openId
                "新路桥人家园",             //红包发送者名称
                Request.UserHostAddress,      //IP
                100,                          //付款金额，单位分
                "感谢投票",                 //红包祝福语
                "新路桥人创业之星评选",                   //活动名称
                "",                   //备注信息
                out nonceStr,
                out paySign,
                null,                         //场景id（非必填）
                null,                         //活动信息（非必填）
                null                          //资金授权商户号，服务商替特约商户发放时使用（非必填）
                );
            if(sendNormalRedPackResult.result_code== "SUCCESS")
            {
                record.UseDate = DateTime.Now;
                SqlFactory.GetSqlhelper().Update(record);
                return Redirect("/MPage/Info.aspx?icon_type=success&info=您已成功领取红包");
            }
            else
            {
                return Redirect("/MPage/Info.aspx?info=" + sendNormalRedPackResult.err_code_des);
            }
            //SerializerHelper serializerHelper = new SerializerHelper();
            //return Content(serializerHelper.GetJsonString(sendNormalRedPackResult));
        }
    }
}
