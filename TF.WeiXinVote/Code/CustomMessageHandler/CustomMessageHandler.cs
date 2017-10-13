/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：CustomMessageHandler.cs
    文件功能描述：微信公众号自定义MessageHandler


    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.Context;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.Sample.CommonService.Utilities;
using System.Xml.Linq;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.Threading.Tasks;
using TF.WeiXinVote.Data;
using Senparc.Weixin.MP.AdvancedAPIs.AutoReply;
using Senparc.Weixin.MP.Containers;

namespace Senparc.Weixin.MP.Sample.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */
        private string agentToken;
        private string agentUrl;
        private string appId;
        private string appSecret;
        private VoteInfos voteinfo;
        private GetCurrentAutoreplyInfoResult rules;



        /// <summary>
        /// 模板消息集合（Key：checkCode，Value：OpenId）
        /// </summary>
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
            this.voteinfo = HttpContext.Current.Session["voteinfo"] as VoteInfos;
            if (this.voteinfo != null)
            {
                this.appId = this.voteinfo.CorpID;
                this.appSecret = this.voteinfo.Secret;
            }
            this.WeixinContext.ExpireMinutes = 3.0;
            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                this.appId = postModel.AppId;
            }
            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }

        public CustomMessageHandler(RequestMessageBase requestMessage)
            : base(requestMessage)
        {
        }

        public ResponseMessageNews GetVoteUrlResponse(string openid)
        {
            ResponseMessageNews news = base.CreateResponseMessage<ResponseMessageNews>();
            Article item = new Article
            {
                Title = this.voteinfo.VoteTitle,
                Description = this.voteinfo.VoteTitle,
                Url = string.Concat(new object[] { 
                    this.voteinfo.HostUrl,
                    "/WeiXin/VoteRedirect?voteid=",
                    this.voteinfo.ID,
                    "&openid=",
                    openid
                })
            };
            news.Articles.Add(item);
            return news;
        }
        public override void OnExecuting()
        {
            try
            {
                string accessTokenOrAppId = AccessTokenContainer.TryGetAccessToken(this.appId, this.appSecret, false);
                this.rules = AutoReplyApi.GetCurrentAutoreplyInfo(accessTokenOrAppId);
            }
            catch (Exception ex)
            {
                
            }
            
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            ResponseMessageText text = base.CreateResponseMessage<ResponseMessageText>();
            string fromUserName = requestMessage.FromUserName;
            if (requestMessage.Content == "投票")
            {
                return this.GetVoteUrlResponse(fromUserName);
            }
            else
            {
                return AutoReply(requestMessage);
            }
            text.Content = "无效命令";
            return text;

            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();

            if (requestMessage.Content == null)
            {

            }
            else if (requestMessage.Content == "约束")
            {
                responseMessage.Content =
                    @"您正在进行微信内置浏览器约束判断测试。您可以：
<a href=""http://sdk.weixin.senparc.com/FilterTest/"">点击这里</a>进行客户端约束测试（地址：http://sdk.weixin.senparc.com/FilterTest/），如果在微信外打开将直接返回文字。
或：
<a href=""http://sdk.weixin.senparc.com/FilterTest/Redirect"">点击这里</a>进行客户端约束测试（地址：http://sdk.weixin.senparc.com/FilterTest/Redirect），如果在微信外打开将重定向一次URL。";
            }
            else if (requestMessage.Content == "托管" || requestMessage.Content == "代理")
            {
                //开始用代理托管，把请求转到其他服务器上去，然后拿回结果
                //甚至也可以将所有请求在DefaultResponseMessage()中托管到外部。

                DateTime dt1 = DateTime.Now; //计时开始

                var agentXml = RequestDocument.ToString();

                #region 暂时转发到SDK线上Demo

                agentUrl = "http://sdk.weixin.senparc.com/weixin";
                agentToken = WebConfigurationManager.AppSettings["WeixinToken"];//Token

                //修改内容，防止死循环
                var agentDoc = XDocument.Parse(agentXml);
                agentDoc.Root.Element("Content").SetValue("代理转发文字：" + requestMessage.Content);
                agentDoc.Root.Element("CreateTime").SetValue(DateTimeHelper.GetWeixinDateTime(DateTime.Now));//修改时间，防止去重
                agentDoc.Root.Element("MsgId").SetValue("123");//防止去重
                agentXml = agentDoc.ToString();

                #endregion

                var responseXml = MessageAgent.RequestXml(this, agentUrl, agentToken, agentXml);
                //获取返回的XML
                //上面的方法也可以使用扩展方法：this.RequestResponseMessage(this,agentUrl, agentToken, RequestDocument.ToString());

                /* 如果有WeiweihiKey，可以直接使用下面的这个MessageAgent.RequestWeiweihiXml()方法。
                 * WeiweihiKey专门用于对接www.weiweihi.com平台，获取方式见：https://www.weiweihi.com/ApiDocuments/Item/25#51
                 */
                //var responseXml = MessageAgent.RequestWeiweihiXml(weiweihiKey, RequestDocument.ToString());//获取Weiweihi返回的XML

                DateTime dt2 = DateTime.Now; //计时结束

                //转成实体。
                /* 如果要写成一行，可以直接用：
                 * responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
                 * 或
                 *
                 */
                var msg = string.Format("\r\n\r\n代理过程总耗时：{0}毫秒", (dt2 - dt1).Milliseconds);
                var agentResponseMessage = responseXml.CreateResponseMessage();
                if (agentResponseMessage is ResponseMessageText)
                {
                    (agentResponseMessage as ResponseMessageText).Content += msg;
                }
                else if (agentResponseMessage is ResponseMessageNews)
                {
                    (agentResponseMessage as ResponseMessageNews).Articles[0].Description += msg;
                }
                return agentResponseMessage;//可能出现多种类型，直接在这里返回
            }
            else if (requestMessage.Content == "测试" || requestMessage.Content == "退出")
            {
                /*
                * 这是一个特殊的过程，此请求通常来自于微微嗨（http://www.weiweihi.com）的“盛派网络小助手”应用请求（https://www.weiweihi.com/User/App/Detail/1），
                * 用于演示微微嗨应用商店的处理过程，由于微微嗨的应用内部可以单独设置对话过期时间，所以这里通常不需要考虑对话状态，只要做最简单的响应。
                */
                if (requestMessage.Content == "测试")
                {
                    //进入APP测试
                    responseMessage.Content = "您已经进入【盛派网络小助手】的测试程序，请发送任意信息进行测试。发送文字【退出】退出测试对话。10分钟内无任何交互将自动退出应用对话状态。";
                }
                else
                {
                    //退出APP测试
                    responseMessage.Content = "您已经退出【盛派网络小助手】的测试程序。";
                }
            }
            else if (requestMessage.Content == "AsyncTest")
            {
                //异步并发测试（提供给单元测试使用）
                DateTime begin = DateTime.Now;
                int t1, t2, t3;
                System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
                System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
                DateTime end = DateTime.Now;
                var thread = System.Threading.Thread.CurrentThread;
                responseMessage.Content = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
                        thread.ManagedThreadId,
                        HttpContext.Current != null ? HttpContext.Current.ApplicationInstance.GetHashCode() : -1,
                        begin,
                        end,
                        t2 - t1
                        );
            }
            else if (requestMessage.Content.ToUpper() == "OPEN")
            {
                var openResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageNews>();
                openResponseMessage.Articles.Add(new Article()
                {
                    Title = "开放平台微信授权测试",
                    Description = @"点击进入Open授权页面。

授权之后，您的微信所收到的消息将转发到第三方（盛派网络小助手）的服务器上，并获得对应的回复。

测试完成后，您可以登陆公众号后台取消授权。",
                    Url = "http://sdk.weixin.senparc.com/OpenOAuth/JumpToMpOAuth"
                });
                return openResponseMessage;
            }
            else if (requestMessage.Content == "错误")
            {
                var errorResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                //因为没有设置errorResponseMessage.Content，所以这小消息将无法正确返回。
                return errorResponseMessage;
            }
            else if (requestMessage.Content == "容错")
            {
                Thread.Sleep(1500);//故意延时1.5秒，让微信多次发送消息过来，观察返回结果
                var faultTolerantResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                faultTolerantResponseMessage.Content = string.Format("测试容错，MsgId：{0}，Ticks：{1}", requestMessage.MsgId,
                    DateTime.Now.Ticks);
                return faultTolerantResponseMessage;
            }
            else if (requestMessage.Content.ToUpper() == "TM")//异步模板消息设置
            {
                var openId = requestMessage.FromUserName;
                var checkCode = Guid.NewGuid().ToString("n").Substring(0, 3);//为了防止openId泄露造成骚扰，这里启用验证码
                TemplateMessageCollection[checkCode] = openId;
                responseMessage.Content = string.Format(@"新的验证码为：{0}，请在网页上输入。网址：http://sdk.weixin.senparc.com/AsyncMethods", checkCode);
            }
            else if (requestMessage.Content.ToUpper() == "OPENID") //返回OpenId及用户信息
            {
                var openId = requestMessage.FromUserName;//获取OpenId
                var userInfo = AdvancedAPIs.UserApi.Info(appId, openId, Language.zh_CN);

                responseMessage.Content = string.Format(
                    "您的OpenID为：{0}\r\n昵称：{1}\r\n性别：{2}\r\n地区（国家/省/市）：{3}/{4}/{5}\r\n关注时间：{6}\r\n关注状态：{7}",
                    requestMessage.FromUserName, userInfo.nickname, (Sex)userInfo.sex, userInfo.country, userInfo.province, userInfo.city, DateTimeHelper.GetDateTimeFromXml(userInfo.subscribe_time), userInfo.subscribe);
            }
            else if (requestMessage.Content.ToUpper() == "EX")
            {
                var ex = new WeixinException("openid:" + requestMessage.FromUserName + ":这是一条测试异常信息");//回调过程在global的ConfigWeixinTraceLog()方法中
                responseMessage.Content = "请等待异步模板消息发送到此界面上（自动延时数秒）。\r\n当前时间：" + DateTime.Now.ToString();
            }
            else if (requestMessage.Content.ToUpper() == "MUTE")
            {
                return new SuccessResponseMessage();
                base.TextResponseMessage = "success";
                responseMessage = null;
            }
            else
            {
                var result = new StringBuilder();
                result.AppendFormat("您刚才发送了文字信息：{0}\r\n\r\n", requestMessage.Content);

                if (CurrentMessageContext.RequestMessages.Count > 1)
                {
                    result.AppendFormat("您刚才还发送了如下消息（{0}/{1}）：\r\n", CurrentMessageContext.RequestMessages.Count,
                        CurrentMessageContext.StorageData);
                    for (int i = CurrentMessageContext.RequestMessages.Count - 2; i >= 0; i--)
                    {
                        var historyMessage = CurrentMessageContext.RequestMessages[i];
                        result.AppendFormat("{0} 【{1}】{2}\r\n",
                            historyMessage.CreateTime.ToShortTimeString(),
                            historyMessage.MsgType.ToString(),
                            (historyMessage is RequestMessageText)
                                ? (historyMessage as RequestMessageText).Content
                                : "[非文字类型]"
                            );
                    }
                    result.AppendLine("\r\n");
                }

                result.AppendFormat("如果您在{0}分钟内连续发送消息，记录将被自动保留（当前设置：最多记录{1}条）。过期后记录将会自动清除。\r\n",
                    WeixinContext.ExpireMinutes, WeixinContext.MaxRecordCount);
                result.AppendLine("\r\n");
                result.AppendLine(
                    "您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：http://sdk.weixin.senparc.com");

                responseMessage.Content = result.ToString();
            }
            return responseMessage;
        }

        


        
        

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }

        public IResponseMessageBase AutoReply(RequestMessageText requestMessage)
        {
            IResponseMessageBase base2 = null;
            try
            {
                string content = requestMessage.Content;
                if (this.rules == null)
                {
                    return base2;
                }
                foreach (KeywordAutoReplyInfo_Item item in this.rules.keyword_autoreply_info.list)
                {
                    ReplyListInfoItem item3;
                    ResponseMessageNews news;
                    bool flag = false;
                    foreach (KeywordListInfoItem item2 in item.keyword_list_info)
                    {
                        if (((item2.match_mode != AutoReplyMatchMode.equal) || (content != item2.content)) && ((item2.match_mode != AutoReplyMatchMode.contain) || !content.Contains(item2.content)))
                        {
                            continue;
                        }
                        flag = true;
                        break;
                    }
                    if (flag)
                    {
                        item3 = item.reply_list_info[0];
                        switch (item3.type)
                        {
                            case AutoReplyType.text:
                                {
                                    ResponseMessageText text = base.CreateResponseMessage<ResponseMessageText>();
                                    text.Content = item3.content;
                                    base2 = text;
                                    break;
                                }
                            case AutoReplyType.news:
                                {
                                    news = base.CreateResponseMessage<ResponseMessageNews>();
                                    foreach (NewsInfoItem item4 in item3.news_info.list)
                                    {
                                        Article article = new Article
                                        {
                                            Title = item4.title,
                                            Description = item4.digest,
                                            PicUrl = item4.cover_url,
                                            Url = item4.content_url
                                        };
                                        news.Articles.Add(article);
                                    }
                                    base2 = news;
                                    break;
                                }
                        }
                    }
                    continue;

                }
                if ((base2 == null) && !string.IsNullOrEmpty(this.rules.message_default_autoreply_info.content))
                {
                    ResponseMessageText text2 = base.CreateResponseMessage<ResponseMessageText>();
                    text2.Content = this.rules.message_default_autoreply_info.content;
                    base2 = text2;
                }
            }
            catch
            {
            }
            return base2;
        }
    }
}
