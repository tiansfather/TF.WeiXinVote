using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TF.WeiXinVote.MPage
{
    public partial class RedPack : System.Web.UI.Page
    {
        protected string code;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Session["openid"] == null)
            {
                base.Response.Redirect("Info.aspx?info=参数错误");
            }
            code = Request.QueryString["code"];
        }
    }
}