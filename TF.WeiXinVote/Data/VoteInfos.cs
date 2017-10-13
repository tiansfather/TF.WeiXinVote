namespace TF.WeiXinVote.Data
{
    using SoMain.Common.DataAttribute;
    using System;
    using System.Runtime.CompilerServices;

    [PrimaryKey("ID", true)]
    public class VoteInfos
    {
        public VoteInfos()
        {
            this.CreateTime = DateTime.Now;
        }

        public string CorpID { get; set; }

        public DateTime CreateTime { get; set; }

        public bool Enable { get; set; }

        public string EncodingAESKey { get; set; }

        public string HostUrl { get; set; }

        public int ID { get; set; }

        public int MPType { get; set; }

        public string Secret { get; set; }

        public string Token { get; set; }

        public DateTime ValidEndDate { get; set; }

        public DateTime ValidStartDate { get; set; }

        public string VoteTitle { get; set; }
    }
}

