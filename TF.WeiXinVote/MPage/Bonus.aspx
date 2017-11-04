<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bonus.aspx.cs" Inherits="TF.WeiXinVote.MPage.Bonus" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=0">
    <title>有奖问答</title>
    <link rel="stylesheet" href="//cdn.bootcss.com/weui/1.1.1/style/weui.min.css">
<link rel="stylesheet" href="//cdn.bootcss.com/jquery-weui/1.0.1/css/jquery-weui.min.css">
    <style>
        #detailcontent{line-height:30px;text-indent: 2em;}
        .weui_msg_desc{text-align: center;
    color: red;
    font-size: 22px;}
        .num{font-size:16px;color:red;}
    </style>
</head>
<body>
    <header class="demos-header" style="color:#3cc51f;">
      <h1 class="demos-title" style="text-align:center">恭喜获得有奖问答机会</h1>
        <%--<h3 style="text-align:center">回答正确百分百有奖</h3>--%>
    </header>
    <div style="overflow:auto;margin-top:10px;padding:10px">
        <a href="javascript:;" class="weui-btn weui-btn_mini weui-btn_warn" style="float:right" onclick="reject();">主动放弃机会</a>
    </div>
    <div class="weui-cells__title" style="font-size:20px;"><%=question.QuestionTitle %></div>
    <div class="weui-cells weui-cells_radio">
        <%
            var answers = question.QuestionAnswers.Split(new string[] { "\n" }, StringSplitOptions.None);
            for(var i=0;i<answers.Length;i++) { 
             %>
      <label class="weui-cell weui-check__label" for="x<%=i %>">
        <div class="weui-cell__bd">
          <p><%=answers[i] %></p>
        </div>
        <div class="weui-cell__ft">
          <input type="radio" class="weui-check" name="v" value="<%=i+1 %>" id="x<%=i %>">
          <span class="weui-icon-checked"></span>
        </div>
      </label>
        <%} %>
    </div>

    <div style="padding:15px;">
        <a href="javascript:;" class="weui-btn weui-btn_primary" onclick="dosubmit()">提交</a>
        <a href="javascript:;" class="weui-btn weui-btn_default" onclick="view('<%=question.Person %>');">查阅"<%=question.Person %>"资料</a>
    </div>
    <div id="persondetail" class="weui-popup__container">
      <div class="weui-popup__overlay"></div>
      <div class="weui-popup__modal">
        <div class="toolbar">
          <div class="toolbar-inner">
            <a href="javascript:;" class="picker-button close-popup">关闭</a>
            <h1 class="title"></h1>
              
          </div>
        </div>
          <div class="modal-content" style="padding-left:1rem;padding-right:1rem;background:#fff;">
                <div id="detailcontent"></div>
            </div>
          
      </div>
    </div>
    <script src="//cdn.bootcss.com/jquery/1.11.0/jquery.min.js"></script>
<script src="//cdn.bootcss.com/jquery-weui/1.0.1/js/jquery-weui.min.js"></script>
    <script>
        function reject() {
            location.href = "index.aspx?voteid=<%=voteinfo.ID%>";
            <%--$.confirm({
                title: '确认放弃',
                text: '确认放弃问答机会？回答正确百分百有奖哦',
                onOK: function () {
                    //点击确认
                    location.href = "index.aspx?voteid=<%=voteinfo.ID%>";
                },
                onCancel: function () {
                }
            });--%>
        }

        function view(person) {
            $.showLoading("正在加载...");
            $("#persondetail .title").html(person);
            $("#detailcontent").html('');
            $.get("Ajax.ashx", { action: "GetPersonDetail", person: person ,voteid:<%=voteinfo.ID%>}, function (data) {
                $.hideLoading();
                $("#detailcontent").html(data);
                $("#persondetail").popup();
            })
        }

        function dosubmit(){
            var v=$("input[name='v']:checked").val();
            if(!v){
                $.toptip('请先选择答案', 'error');
                return;
            }
            $.showLoading("正在提交...");
            $.post("Ajax.ashx", { action: "Answer", questionid: <%=question.ID%>,voteid:<%=voteinfo.ID%>,answer:v }, function (data) {
                $.hideLoading();
                var d=$.parseJSON(data);
                if(d.ErrCode==0){
                    $.toast("提交成功");
                    if(d.Tag=="success"){
                        setTimeout(function(){location.href="BonusGet.aspx"},2000);
                    }else{
                        setTimeout(function(){location.href="Info.aspx?info="+encodeURIComponent("真可惜，没答对，明天还有机会")},2000);
                    }
                    
                }else{
                    $.toptip(d.Message, 'error');
                }
            })
        }
    </script>
    </body>
    </html>