namespace TF.WeiXinVote.Data
{
    using SoMain.Common;
    using System;

    public class SqlFactory
    {
        public static SqlHelper GetSqlhelper() => 
            SqlHelper.OpenFromConnStr("conn");
    }
}

