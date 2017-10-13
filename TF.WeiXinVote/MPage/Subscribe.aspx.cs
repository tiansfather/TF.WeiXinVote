namespace TF.WeiXinVote.MPage
{
    using System;
    using System.IO;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class Subscribe : Page
    {
        protected Literal L_Content;

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = int.Parse(base.Request.QueryString["voteid"]);
            try
            {
                string str = File.ReadAllText(base.Server.MapPath("/MPage/Subscribes/" + num + ".html"));
                this.L_Content.Text = str;
            }
            catch (Exception)
            {
            }
        }
    }
}

