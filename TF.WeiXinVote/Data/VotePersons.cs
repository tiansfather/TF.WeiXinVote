namespace TF.WeiXinVote.Data
{
    using SoMain.Common.DataAttribute;
    using System;
    using System.Runtime.CompilerServices;

    [PrimaryKey("ID", true)]
    public class VotePersons
    {
        public VotePersons()
        {
            this.CreateTime = DateTime.Now;
        }

        public string Brief { get; set; }

        public DateTime CreateTime { get; set; }

        public string Description { get; set; }

        public int ID { get; set; }

        public int NO { get; set; }

        public string Photo { get; set; }

        public string RealName { get; set; }

        public int VoteID { get; set; }

        public int Votes { get; set; }
    }
}

