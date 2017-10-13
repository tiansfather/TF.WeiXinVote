namespace TF.WeiXinVote.MPage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using TF.WeiXinVote.Data;

    public class BonusGet : Page
    {
        protected VoteBonus bonus;
        protected VoteBonusRecord record;

        protected VoteBonusRecord GetBonus(string openid, out VoteBonus bonus)
        {
            //所有当日中奖数未超过每日限量的奖品
            List<VoteBonus> source = SqlFactory.GetSqlhelper().CreateWhere<VoteBonus>().Select().Where(o=>o.TodayBonusedNumber<o.Number).ToList();
            if (source.Count == 0)
            {
                bonus = null;
                return null;
            }

            int num = new Random().Next(0, source.Count<VoteBonus>());
            bonus = source[num];
            //获取之前的领取记录，读取姓名和手机
            var lastrecord = SqlFactory.GetSqlhelper().CreateWhere<VoteBonusRecord>()
                .Where(o => o.OpenId == openid).OrderBy(o => o.CreateTime, SoMain.Common.AscDesc.Desc).FirstOrDefault();
            VoteBonusRecord poco = new VoteBonusRecord {
                OpenId = openid,
                BonusID = bonus.ID,
                RealName=lastrecord?.RealName,
                Mobile=lastrecord?.Mobile
            };
            //如果是现金红包，则生成随机兑换码
            if (poco.Bonus.Title == "现金红包")
            {
                var code = getStr(false, 8);
                poco.AddInfo = code;
            }
            SqlFactory.GetSqlhelper().Save<VoteBonusRecord>(poco);
            HttpContext.Current.Session["BonusGet"] = null;
            return poco;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Session["openid"] == null)
            {
                base.Response.Redirect("Info.aspx?info=参数错误");
            }
            if (HttpContext.Current.Session["BonusGet"] == null)
            {
                base.Response.Redirect("Info.aspx?info=没有中奖信息");
            }
            this.record = this.GetBonus(this.Session["openid"].ToString(), out this.bonus);
            if (record == null)
            {
                base.Response.Redirect("Info.aspx?info=谢谢参与，今天奖品已经发放结束，请明天赶早投票");
            }else if (record.Bonus.Title == "现金红包")
            {
                base.Response.Redirect("RedPack.aspx?code="+record.AddInfo);
            }
        }

        public static string getStr(bool b, int n)
        {
            string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (b)
            {
                str = str + "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                builder.Append(str.Substring(random.Next(0, str.Length), 1));
            }
            return builder.ToString();
        }
    }
}

