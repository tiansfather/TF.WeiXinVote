namespace TF.WeiXinVote.MPage.Code
{
    using SoMain.Common.Weixin;
    using System;

    public class WeAPI : WxMpApi
    {
        private string _corpid;
        private string _secret;

        public WeAPI(string corpid, string secret)
        {
            this._corpid = corpid;
            this._secret = secret;
        }

        public override string GetAppid() {
            return this._corpid;
        } 
            

        public override string GetSecret() {
            return this._secret;
        }
            
    }
}

