namespace TF.WeiXinVote.Data
{
    using SoMain.Common.DataAttribute;
    using System;
    using System.Runtime.CompilerServices;

    [PrimaryKey("ID", true)]
    public class VoteDetail
    {
        public VoteDetail()
        {
            this.VoteDate = DateTime.Now;
        }

        public int ID { get; set; }

        public string OpenID { get; set; }

        public string Person { get; set; }

        public DateTime VoteDate { get; set; }

        public int VoteID { get; set; }
    }
}

