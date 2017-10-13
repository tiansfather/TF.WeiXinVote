namespace TF.WeiXinVote.MPage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using TF.WeiXinVote.Data;
    using TF.WeiXinVote.MPage.Code;

    public class MyBonus : Page
    {
        protected Repeater DataList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Session["openid"] == null)
            {
                Fun.Error("参数错误");
            }
            string openid = this.Session["openid"].ToString();
            var enumerable = SqlFactory.GetSqlhelper().CreateWhere<VoteBonusRecord>().Where(o => o.OpenId == openid).Select();
            this.DataList.DataSource = enumerable;
            this.DataList.DataBind();
        }
        

        protected void DataList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var record = e.Item.DataItem as VoteBonusRecord;
            if (record.Bonus.Title == "现金红包")
            {
                (e.Item.FindControl("PH_Bonus") as PlaceHolder).Visible = false;
            }
            if (record.Bonus.Title != "现金红包" || record.UseDate!=null)
            {
                (e.Item.FindControl("PH_RedPack") as PlaceHolder).Visible = false;
            }
        }
    }
}

