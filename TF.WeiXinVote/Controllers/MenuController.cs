namespace Senparc.Weixin.MP.Sample.Controllers
{
    using Senparc.Weixin.Entities;
    using Senparc.Weixin.Exceptions;
    using Senparc.Weixin.MP;
    using Senparc.Weixin.MP.CommonAPIs;
    using Senparc.Weixin.MP.Entities;
    using Senparc.Weixin.MP.Entities.Menu;
    using System;
    using System.Web.Mvc;

    public class MenuController : Controller
    {
        [HttpPost]
        public ActionResult CreateMenu(string token, GetMenuResultFull resultFull, MenuMatchRule menuMatchRule)
        {
            bool flag = (menuMatchRule != null) && !menuMatchRule.CheckAllNull();
            string str = "使用接口："+ (flag ? "个性化菜单接口" : "普通自定义菜单接口")+"。";
            try
            {
                WxJsonResult result = null;
                IButtonGroupBase buttonData = null;
                if (flag)
                {
                    ConditionalButtonGroup menu = CommonApi.GetMenuFromJsonResult(resultFull, new ConditionalButtonGroup()).menu as ConditionalButtonGroup;
                    menu.matchrule = menuMatchRule;
                    result = CommonApi.CreateMenuConditional(token, menu, 0x2710);
                    str = str + "menuid："+(result as CreateMenuConditionalResult).menuid+"。";
                }
                else
                {
                    buttonData = CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
                    result = CommonApi.CreateMenu(token, buttonData, 0x2710);
                }
                var data = new {
                    Success = result.errmsg == "ok",
                    Message = "菜单更新成功。" + str
                };
                return base.Json(data);
            }
            catch (Exception exception)
            {
                var type2 = new {
                    Success = false,
                    Message = "更新失败："+exception.Message+"。"+str
                };
                return base.Json(type2);
            }
        }

        public ActionResult DeleteMenu(string token)
        {
            try
            {
                WxJsonResult result = CommonApi.DeleteMenu(token);
                var data = new {
                    Success = result.errmsg == "ok",
                    Message = result.errmsg
                };
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var type2 = new {
                    Success = false,
                    Message = exception.Message
                };
                return base.Json(type2, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMenu(string token)
        {
            try
            {
                GetMenuResult menu = CommonApi.GetMenu(token);
                if (menu == null)
                {
                    return base.Json(new { error = "菜单不存在或验证失败！" }, JsonRequestBehavior.AllowGet);
                }
                return base.Json(menu, JsonRequestBehavior.AllowGet);
            }
            catch (WeixinMenuException exception)
            {
                return base.Json(new { error = "菜单不存在或验证失败：" + exception.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception2)
            {
                return base.Json(new { error = "菜单不存在或验证失败：" + exception2.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetToken(string appId, string appSecret)
        {
            try
            {
                AccessTokenResult data = CommonApi.GetToken(appId, appSecret, "client_credential");
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return base.Json(new { error = "执行过程发生错误！" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Index()
        {
            GetMenuResult model = new GetMenuResult(new ButtonGroup());
            for (int i = 0; i < 3; i++)
            {
                SubButton button = new SubButton();
                for (int j = 0; j < 5; j++)
                {
                    SingleClickButton item = new SingleClickButton();
                    button.sub_button.Add(item);
                }
            }
            return base.View(model);
        }
    }
}

