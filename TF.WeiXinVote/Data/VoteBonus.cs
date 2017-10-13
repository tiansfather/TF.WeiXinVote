namespace TF.WeiXinVote.Data
{
    using SoMain.Common.DataAttribute;
    using System;
    using System.Runtime.CompilerServices;
    using SoMain.Common;
    [PrimaryKey("ID", true)]
    public class VoteBonus
    {
        public string BonusTip { get; set; }

        public int ID { get; set; }

        public string Location { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public int VoteID { get; set; }

        [Ignore]
        public int AllBonusedNumber
        {
            get
            {
                return SqlFactory.GetSqlhelper().CreateWhere<VoteBonusRecord>()
                    .Where(o => o.BonusID == ID)
                    .Count();
            }
        }

        [Ignore]
        public int TodayBonusedNumber
        {
            get
            {
                return SqlFactory.GetSqlhelper().CreateWhere<VoteBonusRecord>()
                    .Where(o => o.BonusID == ID)
                    .Where(o=>o.CreateTime> Convert.ToDateTime( DateTime.Now.ToString("yyyy-MM-dd")))
                    .Count();
            }
        }
    }
}

