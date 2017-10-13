namespace TF.WeiXinVote.MPage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using TF.WeiXinVote.Data;
    using TF.WeiXinVote.MPage.Code;

    public partial class Bonus : Page
    {
        protected VoteQuestions question;
        protected VoteInfos voteinfo;

        protected void Page_Load(object sender, EventArgs e)
        {
            int num;
            if (this.Session["BonusEnable"] == null)
            {
                base.Response.Redirect("Info.aspx?info=没有有奖问答机会");
            }
            if (!int.TryParse(base.Request.QueryString["voteid"], out num))
            {
                Fun.Error("参数不正确");
            }
            else
            {
                Fun.ValidVoteID(num, out this.voteinfo);
            }
            List<VoteQuestions> source = SqlFactory.GetSqlhelper().CreateWhere<VoteQuestions>().Select();
            int num2 = new Random().Next(0, source.Count<VoteQuestions>());
            this.question = source[num2];
            if (this.question.QuestionAnswers == null)
            {
                this.question.QuestionAnswers = "";
            }
        }
    }
}

