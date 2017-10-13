namespace TF.WeiXinVote.MPage.Code
{
    using SoMain.Common.Weixin.Mp.Datas;
    using System;
    using System.Runtime.InteropServices;
    using System.Web;
    using TF.WeiXinVote.Data;

    public class Fun
    {
        public static void Error(string info)
        {
            HttpContext.Current.Response.Redirect("/MPage/Info.aspx?info=" + info);
        }

        public static string GetSession(string key)
        {
            string str = "";
            try
            {
                str = HttpContext.Current.Session[key].ToString();
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static void ValidVoteID(int voteid, out VoteInfos voteinfo)
        {
            voteinfo = SqlFactory.GetSqlhelper().SingleOrDefaultById<VoteInfos>(voteid);
            if (voteinfo == null)
            {
                Error("未找到对应投票");
            }
            WeAPI eapi = new WeAPI(voteinfo.CorpID, voteinfo.Secret);
            if (HttpContext.Current.Session["openid"] == null)
            {
                if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["code"]))
                {
                    if (voteinfo.MPType == 0)
                    {
                        HttpContext.Current.Response.Redirect("/MPage/Subscribe.aspx?voteid=" + voteinfo.ID);
                    }
                    string url = eapi.OAuthApi.GetAuthorizeUrl(HttpContext.Current.Request.Url.ToString(), "", OAuthScope.snsapi_base, "code");
                    HttpContext.Current.Response.Redirect(url);
                }
                else
                {
                    string code = HttpContext.Current.Request.QueryString["code"];
                    OAuthAccessTokenResult token = eapi.OAuthApi.GetToken(code, "authorization_code");
                    if (!string.IsNullOrEmpty(token.openid))
                    {
                        string openid = token.openid;
                        if (eapi.UserApi.Info(openid, "zh_CN").subscribe == 0)
                        {
                            HttpContext.Current.Response.Redirect("Subscribe.aspx?voteid=" + voteid);
                        }
                        HttpContext.Current.Session["openid"] = openid;
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect("Index.aspx?voteid=" + voteid);
                    }
                }
            }
        }
    }
}

