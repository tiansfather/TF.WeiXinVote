namespace TF.WeiXinVote.MPage
{
    using System;
    using System.Web.UI;

    public class Info : Page
    {
        protected string icon_type;
        protected string info;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.info = base.Request.QueryString["info"];
            this.icon_type = base.Request.QueryString["icon_type"];
            if (string.IsNullOrEmpty(this.icon_type))
            {
                this.icon_type = "warn";
            }
        }
    }
}

