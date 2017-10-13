<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="TF.WeiXinVote.MPage.Index" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=0">
    <title><%=voteinfo.VoteTitle %></title>
    <link rel="stylesheet" href="//cdn.bootcss.com/weui/0.4.3/style/weui.min.css">
    <link rel="stylesheet" href="//cdn.bootcss.com/jquery-weui/0.8.0/css/jquery-weui.min.css">
    <script src="//cdn.bootcss.com/jquery/1.11.0/jquery.min.js"></script>
<script src="//cdn.bootcss.com/jquery-weui/0.8.0/js/jquery-weui.min.js"></script>
    <style>
        #detailcontent{margin-top:10px;line-height:30px;text-indent: 2em;}
        .weui_msg_desc{text-align: center;
    color: red;
    font-size: 22px;}
        .num{font-size:16px;color:red;}
    </style>
</head>
<body>
    <asp:Literal runat="server" ID="L_Content"></asp:Literal>
    <div class="weui_panel weui_panel_access">
  <div class="weui_panel_hd" style="font-size:16px"><%=voteinfo.VoteTitle %></div>
<asp:PlaceHolder runat="server" ID="PH_NoVote" Visible="false">
    <p class="weui_msg_desc"><asp:Literal runat="server" ID="L_Msg"></asp:Literal></p>
    
</asp:PlaceHolder>
  <div class="weui_panel_bd">
      <asp:Repeater runat="server" ID="DataList">
          <ItemTemplate>
              <div  class="weui_media_box weui_media_appmsg item" vote="0" personid="<%#Eval("id") %>">
                  <div class="weui_media_hd" style="height:80px;">
                    <img class="weui_media_appmsg_thumb" src="<%#Eval("Photo") %>" alt="">
                  </div>
                  <div class="weui_media_bd">
                    <h4 class="weui_media_title"><%#Eval("NO") %>.<%#Eval("realname") %></h4>
                    <p class="weui_media_desc"><%#Eval("brief") %></p>
                      <p class="button_sp_area">
                          <span style="line-height: 1.9;    font-size: 14px;    padding: 0 .25em 0 0;    display: block;float:left;margin-top:15px;">目前得票:<i class="num"><%#GetVoteCount(Eval("realname").ToString()) %></i></span>
                          
                          <a href="javascript:loadperson(<%#Eval("id") %>,'<%#Eval("realname") %>');" class="weui_btn weui_btn_mini weui_btn_default">了解详情</a>
                          
                          <a href="javascript:voteFor(<%#Eval("id") %>);" class="weui_btn weui_btn_mini weui_btn_primary votebtn" <%if(!canvote){ %>style="visibility:hidden"<%} %>>支持1票</a>
                        </p>
                  </div>
                </div>
          </ItemTemplate>
      </asp:Repeater>
    
  </div>
</div>
    <%if(canvote){ %>
    <div class="button_sp_area" style="padding:1rem;">
    <a href="javascript:submitVote();" class="weui_btn weui_btn_warn">确认提交</a>
    </div>
    <%} %>
    <!--弹出对话框-->
    <div id="persondetail" class='weui-popup-container ' style="z-index: 10000">
        <div class="weui-popup-modal">
            <div class="toolbar">
                <div class="toolbar-inner">
                    <a href="javascript:;" class="picker-button close-popup">返回</a>
                    <h1 class="title"></h1>
                </div>
            </div>
            <div class="modal-content" style="padding-left:1rem;padding-right:1rem;background:#fff;">
                <div id="detailcontent"></div>
            </div>
        </div>
    </div>

<script>
    function voteFor(personid) {
        var item = $(".item[personid='" + personid + "']");
        var obj = item.find(".votebtn");
        var isvote = item.attr("vote");
        if (isvote == "1") {
            //已投票的，允许取消投票
            item.attr("vote", 0);
            $(obj).html("支持1票").removeClass("weui_btn_warn").addClass("weui_btn_primary");
        } else {
            //未投票的，需要判断已投票数量是否超过10个
            var votecount = $(".item[vote=1]").size();
            if (votecount >= 10) {
                $.toptip('您一天只能投10票', 'error');
            } else {
                item.attr("vote", 1);
                $(obj).html("已支持").addClass("weui_btn_warn").removeClass("weui_btn_primary");
            }
        }
    }

    function loadperson(id,realname) {
        $.showLoading("正在加载...");
        $("#persondetail .title").html(realname);
        $("#detailcontent").html('');
        $.get("Ajax.ashx", { action: "GetPersonDetail", person: realname,voteid:<%=voteinfo.ID%> }, function (data) {
            $.hideLoading();
            $("#detailcontent").html(data);
            $("#persondetail").popup();
        })        
    }

    function submitVote() {
        var voteids = [];
        $(".item[vote=1]").each(function () {
            voteids.push($(this).attr("personid"));            
        })
        if (voteids.length == 0) {
            $.toptip('请先投票再提交,谢谢', 'error');
        } else if (voteids.length > 10) {
            $.toptip('您一天只能投10票', 'error');
        } else {
            var ids = voteids.join(",");
            $.showLoading("正在提交...");
            $.post("Ajax.ashx", { action: "SubmitVote", ids: ids,voteid:<%=voteinfo.ID%> }, function (data) {
                $.hideLoading();
                var d=$.parseJSON(data);
                if(d.ErrCode==0){
                    $.toast("操作成功");
                    if(d.Tag=="enable"){
                        setTimeout(function(){location.href="Bonus.aspx?voteid=<%=voteinfo.ID%>"},2000);
                    }else{
                        setTimeout(function(){location.href="Index.aspx?voteid=<%=voteinfo.ID%>"},2000);
                    }
                    
                }else{
                    $.toptip(d.Message, 'error');
                }
            })
        }
    }
</script>
</body>
</html>
