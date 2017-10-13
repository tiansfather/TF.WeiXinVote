namespace TF.Controllers
{
    using SoMain.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;
    using TF;
    using TF.WeiXinVote.Data;

    public class AdminController : BaseController
    {
        [User]
        public ActionResult BonusRecord(long page = 1L, string mobile = "")
        {
            Page<VoteBonusRecord> model = SqlFactory.GetSqlhelper().CreateWhere<VoteBonusRecord>().IfTrueWhere(mobile != "", o => o.Mobile == mobile).OrderBy<DateTime>(o => o.CreateTime, AscDesc.Desc).Page(page, 20L);
            return base.View(model);
        }

        [User]
        public ActionResult Index()
        {
            object obj1 = base.Session["user"];
            return base.View();
        }

        public ActionResult Login()
        {
            string str;
            string str2;
            string str3;
            Rsa.CreateKey(out str, out str2, out str3);
            base.ViewData["n"] = str2;
            base.ViewData["e"] = str3;
            base.Session["key"] = str;
            base.ClearCookie("c");
            base.ClearCookie("v");
            base.ClearCookie("m");
            return base.View();
        }

        [HttpPost]
        public ActionResult Login(string user, string pwd, bool rm)
        {
            user = Rsa.Decrypt(base.Session["key"].ToString(), user);
            pwd = Rsa.Decrypt(base.Session["key"].ToString(), pwd);
            pwd = Md5.GetMd5String(pwd);
            DbUser user2 = base.helper.CreateWhere<DbUser>().Where(q => q.Enable).Where(q => q.UserName == user).Where(q => q.PassWord == pwd).FirstOrDefault();
            if (user2 == null)
            {
                return base.Error("用户名或密码错误！");
            }
            base.Session["user"] = user2;
            return base.Success();
        }

        [User]
        public ActionResult Questions()
        {
            DbUser user = base.Session["user"] as DbUser;
            List<VoteQuestions> model = SqlFactory.GetSqlhelper().CreateWhere<VoteQuestions>().Where(o => o.VoteID == user.LimitVoteID).Select();
            return base.View(model);
        }

        [User]
        public ActionResult VoteBonus()
        {
            DbUser user = base.Session["user"] as DbUser;
            List<TF.WeiXinVote.Data.VoteBonus> model = SqlFactory.GetSqlhelper().CreateWhere<TF.WeiXinVote.Data.VoteBonus>().Where(o => o.VoteID == user.LimitVoteID).Select();
            return base.View(model);
        }

        [User]
        public ActionResult VoteBonusAdd() {return base.View();} 
            

        [User, HttpPost, ValidateInput(false)]
        public ActionResult VoteBonusAdd(TF.WeiXinVote.Data.VoteBonus bonus)
        {
            DbUser user = base.Session["user"] as DbUser;
            bonus.VoteID = user.LimitVoteID;
            SqlFactory.GetSqlhelper().Save<TF.WeiXinVote.Data.VoteBonus>(bonus);
            return base.Success();
        }

        [User]
        public ActionResult VoteBonusDel(int id)
        {
            SqlFactory.GetSqlhelper().DeleteById<TF.WeiXinVote.Data.VoteBonus>(id);
            return base.Success();
        }

        [User]
        public ActionResult VoteBonusEdit(int id)
        {
            TF.WeiXinVote.Data.VoteBonus model = SqlFactory.GetSqlhelper().SingleById<TF.WeiXinVote.Data.VoteBonus>(id);
            return base.View(model);
        }

        [HttpPost, ValidateInput(false), User]
        public ActionResult VoteBonusEdit(TF.WeiXinVote.Data.VoteBonus bonus)
        {
            SqlFactory.GetSqlhelper().Save<TF.WeiXinVote.Data.VoteBonus>(bonus);
            return base.Success();
        }

        [User]
        public ActionResult VotePerson()
        {
            DbUser user = base.Session["user"] as DbUser;
            List<VotePersons> model = SqlFactory.GetSqlhelper().CreateWhere<VotePersons>().Where(o => o.VoteID == user.LimitVoteID).OrderBy<int>(o => o.NO, AscDesc.Asc).Select();
            return base.View(model);
        }

        [User]
        public ActionResult VotePersonAdd() { return base.View(); }
            

        [User, ValidateInput(false), HttpPost]
        public ActionResult VotePersonAdd(VotePersons person)
        {
            DbUser user = base.Session["user"] as DbUser;
            person.VoteID = user.LimitVoteID;
            if (string.IsNullOrEmpty(person.RealName))
            {
                return base.Error("请输入姓名");
            }
            SqlFactory.GetSqlhelper().Save<VotePersons>(person);
            return base.Success();
        }

        [User]
        public ActionResult VotePersonDel(int id)
        {
            SqlFactory.GetSqlhelper().DeleteById<VotePersons>(id);
            return base.Success();
        }

        [User]
        public ActionResult VotePersonEdit(int id)
        {
            VotePersons model = SqlFactory.GetSqlhelper().SingleById<VotePersons>(id);
            return base.View(model);
        }

        [User]
        public ActionResult VoteQuestionAdd()
        {
            DbUser user = base.Session["user"] as DbUser;
            List<VotePersons> list = SqlFactory.GetSqlhelper().CreateWhere<VotePersons>().Where(o => o.VoteID == user.LimitVoteID).Select();
            base.ViewData["persons"] = list;
            return base.View();
        }

        [HttpPost, ValidateInput(false), User]
        public ActionResult VoteQuestionAdd(VoteQuestions question)
        {
            DbUser user = base.Session["user"] as DbUser;
            question.VoteID = user.LimitVoteID;
            if (string.IsNullOrEmpty(question.Person))
            {
                return base.Error("请选择关联人员");
            }
            SqlFactory.GetSqlhelper().Save<VoteQuestions>(question);
            return base.Success();
        }

        [User]
        public ActionResult VoteQuestionDel(int id)
        {
            SqlFactory.GetSqlhelper().DeleteById<VoteQuestions>(id);
            return base.Success();
        }

        [User]
        public ActionResult VoteQuestionEdit(int id)
        {
            VoteQuestions model = SqlFactory.GetSqlhelper().SingleById<VoteQuestions>(id);
            DbUser user = base.Session["user"] as DbUser;
            List<VotePersons> list = SqlFactory.GetSqlhelper().CreateWhere<VotePersons>().Where(o => o.VoteID == user.LimitVoteID).Select();
            base.ViewData["persons"] = list;
            return base.View(model);
        }

        #region 页面
        [User]
        public ActionResult VotePage()
        {
            DbUser user = base.Session["user"] as DbUser;
            List<VotePages> model = SqlFactory.GetSqlhelper().CreateWhere<VotePages>().Where(o => o.VoteID == user.LimitVoteID).OrderBy(o => o.ID, AscDesc.Desc).Select();
            return base.View(model);
        }

        [User]
        public ActionResult VotePageAdd() { return base.View(); }


        [User, ValidateInput(false), HttpPost]
        public ActionResult VotePageAdd(VotePages page)
        {
            DbUser user = base.Session["user"] as DbUser;
            page.VoteID = user.LimitVoteID;
            SqlFactory.GetSqlhelper().Save<VotePages>(page);
            return base.Success();
        }

        [User]
        public ActionResult VotePageDel(int id)
        {
            SqlFactory.GetSqlhelper().DeleteById<VotePages>(id);
            return base.Success();
        }

        [User]
        public ActionResult VotePageEdit(int id)
        {
            VotePages model = SqlFactory.GetSqlhelper().SingleById<VotePages>(id);
            return base.View(model);
        }
        #endregion
    }
}

