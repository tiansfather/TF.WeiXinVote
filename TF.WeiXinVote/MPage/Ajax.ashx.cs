namespace TF.WeiXinVote.MPage
{
    using Newtonsoft.Json;
    using SoMain.Common;
    using System;
    using System.Web;
    using System.Web.SessionState;
    using TF.WeiXinVote.Data;
    using TF.WeiXinVote.MPage.Code;

    public class Ajax : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        private void Answer()
        {
            AjaxResult result = new AjaxResult();
            if (HttpContext.Current.Session["BonusEnable"] == null)
            {
                result.ErrCode = -1;
                result.Message = "没有有奖问答机会";
            }
            else
            {
                int primaryKey = int.Parse(HttpContext.Current.Request.Form["questionid"]);
                string str = HttpContext.Current.Request.Form["answer"];
                if (SqlFactory.GetSqlhelper().SingleById<VoteQuestions>(primaryKey).QuestionCorrect == str)
                {
                    HttpContext.Current.Session["BonusGet"] = "1";
                    result.ErrCode = 0;
                    result.Tag = "success";
                }
                else
                {
                    result.ErrCode = 0;
                }
                HttpContext.Current.Session["BonusEnable"] = null;
            }
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(result));
        }

        private void GetPersonDetail()
        {
            string name = HttpContext.Current.Request.QueryString["person"];
            int voteid = int.Parse(HttpContext.Current.Request.QueryString["voteid"]);
            VotePersons persons = SqlFactory.GetSqlhelper().CreateWhere<VotePersons>().Where(o => (o.VoteID == voteid) && (o.RealName == name)).SingleOrDefault();
            if (persons != null)
            {
                HttpContext.Current.Response.Write(persons.Description);
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string str2 = context.Request["action"];
            if (str2 != null)
            {
                if (str2 != "GetPersonDetail")
                {
                    if (str2 != "SubmitVote")
                    {
                        if (str2 == "Answer")
                        {
                            this.Answer();
                            return;
                        }
                        if (str2 == "SubmitBonusInfo")
                        {
                            this.SubmitBonusInfo();
                        }
                        return;
                    }
                }
                else
                {
                    this.GetPersonDetail();
                    return;
                }
                this.SubmitVote();
            }
        }

        private void SubmitBonusInfo()
        {
            AjaxResult result = new AjaxResult();
            int primaryKey = int.Parse(HttpContext.Current.Request.Form["recordid"]);
            string str = HttpContext.Current.Request.Form["realname"];
            string str2 = HttpContext.Current.Request.Form["mobile"];
            VoteBonusRecord poco = SqlFactory.GetSqlhelper().SingleOrDefaultById<VoteBonusRecord>(primaryKey);
            if (poco != null)
            {
                poco.RealName = str;
                poco.Mobile = str2;
                SqlFactory.GetSqlhelper().Save<VoteBonusRecord>(poco);
                result.ErrCode = 0;
            }
            else
            {
                result.ErrCode = -1;
                result.Message = "不存在中奖记录";
            }
            HttpContext.Current.Session["openid"] = poco.OpenId;
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(result));
        }

        private void SubmitVote()
        {
            SqlHelper sqlhelper = SqlFactory.GetSqlhelper();
            AjaxResult result = new AjaxResult();
            string session = Fun.GetSession("openid");
            int primaryKey = int.Parse(HttpContext.Current.Request.Form["voteid"]);
            if (string.IsNullOrEmpty(session))
            {
                result.ErrCode = -1;
                result.Message = "已超期，请重新进入页面";
            }
            else if (sqlhelper.Count<VoteDetail>("where openid='" + session + "' and datediff(dd,votedate,getdate())=0", new object[0]) > 0)
            {
                result.ErrCode = -1;
                result.Message = "您今天已投过票,无法继续投票";
            }
            else
            {
                VoteInfos infos = sqlhelper.SingleById<VoteInfos>(primaryKey);
                TimeSpan span = (TimeSpan) (DateTime.Now - infos.ValidStartDate);
                if (span.TotalDays < 0.0)
                {
                    result.ErrCode = -1;
                    result.Message = "该投票尚未开始";
                }
                else
                {
                    TimeSpan span2 = (TimeSpan) (DateTime.Now - infos.ValidEndDate);
                    if (span2.TotalDays > 1.0)
                    {
                        result.ErrCode = -1;
                        result.Message = "该投票已经结束";
                    }
                    else
                    {
                        string[] strArray = HttpContext.Current.Request.Form["ids"].Split(new char[] { ',' });
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            VotePersons persons = sqlhelper.SingleById<VotePersons>(strArray[i]);
                            VoteDetail detail = new VoteDetail {
                                VoteID = primaryKey,
                                Person = persons.RealName,
                                OpenID = session
                            };
                            sqlhelper.Execute("insert into votedetail(voteid,person,openid)values(@0,@1,@2)", new object[] { primaryKey, persons.RealName, session });
                        }
                        result.ErrCode = 0;
                        result.Message = "提交成功";
                        result.Tag = "enable";
                        HttpContext.Current.Session["BonusEnable"] = "1";
                    }
                }
            }
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(result));
        }

        public bool IsReusable {
            get
            {
                return false;
            }
        }
    }
}

