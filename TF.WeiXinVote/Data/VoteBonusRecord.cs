namespace TF.WeiXinVote.Data
{
    using SoMain.Common.DataAttribute;
    using System;
    using System.Runtime.CompilerServices;

    [PrimaryKey("ID", true)]
    public class VoteBonusRecord
    {
        public VoteBonusRecord()
        {
            this.CreateTime = DateTime.Now;
        }

        public int BonusID { get; set; }

        public DateTime CreateTime { get; set; }

        public int ID { get; set; }

        [FieldLength(50)]
        public string Mobile { get; set; }

        [FieldLength(50)]
        public string OpenId { get; set; }

        [FieldLength(50)]
        public string RealName { get; set; }

        public DateTime? UseDate { get; set; }
        /// <summary>
        /// 额外数据
        /// </summary>
        public string AddInfo { get; set; }

        [Ignore]
        public VoteBonus Bonus
        {
            get
            {
                return SqlFactory.GetSqlhelper().CreateWhere<VoteBonus>()
                    .Where(o => o.ID == BonusID)
                    .SingleOrDefault();
            }
        }
    }
}

