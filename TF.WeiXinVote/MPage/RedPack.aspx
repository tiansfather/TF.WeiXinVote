<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RedPack.aspx.cs" Inherits="TF.WeiXinVote.MPage.RedPack" %>


<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=0">
    <title>恭喜获得现金红包</title>
    <link rel="stylesheet" href="//cdn.bootcss.com/weui/0.4.3/style/weui.min.css">
    <link rel="stylesheet" href="//cdn.bootcss.com/jquery-weui/0.8.0/css/jquery-weui.min.css">
    <script src="//cdn.bootcss.com/jquery/1.11.0/jquery.min.js"></script>
    <script src="https://cdn.bootcss.com/lrsjng.jquery-qrcode/0.14.0/jquery-qrcode.js"></script>
</head>
<body ontouchstart>
    
        <div class="page">
            <div class="weui_msg">
                <div class="weui_icon_area"><i class="weui_icon_success weui_icon_msg"></i></div>
                <div class="weui_text_area">
                    <h2 class="weui_msg_title">恭喜获得现金红包</h2>
                    <p class="weui_msg_desc">
                        将在3秒内跳转至领取页面 <br />
                        <%--<div id="qrcode" style="text-align:center;padding-top:10px;">
                           
</div>--%>
                    </p>
                </div>
            </div>
        </div>
    <script>
        //location.href = "http://<%=Request.Url.Host%>/Pay/Index?returnurl=/Pay/SendRedPack?code=<%=Request.QueryString["code"] %>";
        $(function () {
            //jQuery('#qrcode').qrcode({ text: "http://<%=Request.Url.Host%>/Pay/Index?returnurl=/Pay/SendRedPack?code=<%=Request.QueryString["code"] %>" });
            setTimeout(function () {
                location.href = "http://<%=Request.Url.Host%>/Pay/Index?returnurl=/Pay/SendRedPack?code=<%=Request.QueryString["code"] %>" }, 3000);
        })
    </script>
</body>
</html>
