namespace TF.WeiXinVote.Data
{
    using SoMain.Common.DataAttribute;
    using System;
    using System.Runtime.CompilerServices;

    [PrimaryKey("ID", true)]
    public class DbUser
    {
        public bool Enable { get; set; }

        public int ID { get; set; }

        public int LimitVoteID { get; set; }

        [FieldLength(50)]
        public string PassWord { get; set; }

        [FieldLength(50)]
        public string UserName { get; set; }

        public DbUserType UserType { get; set; }
    }
}

