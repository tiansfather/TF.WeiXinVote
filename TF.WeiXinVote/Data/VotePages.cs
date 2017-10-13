using SoMain.Common.DataAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TF.WeiXinVote.Data
{
    [PrimaryKey("ID")]
    public class VotePages
    {
        public VotePages()
        {
            CreateTime = DateTime.Now;
        }

        public int ID { get; set; }
        public int VoteID { get; set; }
        [FieldLength(100)]
        public string Title { get; set; }
        [Text]
        public string PageContent { get; set; }
        public DateTime CreateTime { get; set; }
    }
}