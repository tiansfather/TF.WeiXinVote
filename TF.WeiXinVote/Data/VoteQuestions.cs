namespace TF.WeiXinVote.Data
{
    using SoMain.Common.DataAttribute;
    using System;
    using System.Runtime.CompilerServices;

    [PrimaryKey("ID", true)]
    public class VoteQuestions
    {
        public int ID { get; set; }

        public string Person { get; set; }

        public string QuestionAnswers { get; set; }

        public string QuestionCorrect { get; set; }

        public string QuestionTitle { get; set; }

        public int VoteID { get; set; }
    }
}

