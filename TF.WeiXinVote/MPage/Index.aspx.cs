namespace TF.WeiXinVote.MPage
{
    using SoMain.Common;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using TF.WeiXinVote.Data;
    using TF.WeiXinVote.MPage.Code;

    public class Index : Page
    {
        protected bool canvote = true;
        protected Repeater DataList;
        protected Literal L_Content;
        protected Literal L_Msg;
        protected PlaceHolder PH_NoVote;
        protected VoteInfos voteinfo;

        protected int GetVoteCount(string realname) => 
            SqlFactory.GetSqlhelper().CreateWhere<VoteDetail>().Where(o => o.VoteID == this.voteinfo.ID).Where(o => o.Person == realname).Count();

        protected void Page_Load(object sender, EventArgs e)
        {
            int num;
            this.Session["BonusEnable"] = null;
            if (!int.TryParse(base.Request.QueryString["voteid"], out num))
            {
                Fun.Error("参数不正确");
            }
            else
            {
                Fun.ValidVoteID(num, out this.voteinfo);
            }
            try
            {
                string str = File.ReadAllText(base.Server.MapPath("/MPage/Subscribes/" + num + ".html"));
                this.L_Content.Text = str;
            }
            catch (Exception)
            {
            }
            TimeSpan span = (TimeSpan) (DateTime.Now - this.voteinfo.ValidStartDate);
            if (span.TotalDays < 0.0)
            {
                this.canvote = false;
                this.L_Msg.Text = "投票尚未开始";
                this.PH_NoVote.Visible = true;
            }
            TimeSpan span2 = (TimeSpan) (DateTime.Now - this.voteinfo.ValidEndDate);
            if (span2.TotalDays > 1.0)
            {
                this.canvote = false;
                this.L_Msg.Text = "投票已经结束";
                this.PH_NoVote.Visible = true;
            }
            SqlHelper sqlhelper = SqlFactory.GetSqlhelper();
            if (sqlhelper.Count<VoteDetail>("where openid='" + Fun.GetSession("openid") + "' and datediff(dd,votedate,getdate())=0", new object[0]) > 0)
            {
                this.canvote = false;
                this.L_Msg.Text = "您今天已投过票,无法继续投票";
                this.PH_NoVote.Visible = true;
            }
            List<VotePersons> list = sqlhelper.CreateWhere<VotePersons>().Where(o => o.VoteID == this.voteinfo.ID).OrderBy<int>(o => o.NO, AscDesc.Asc).Select();
            this.DataList.DataSource = list;
            this.DataList.DataBind();
        }
    }
}

