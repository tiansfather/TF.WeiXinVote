<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Info.aspx.cs" Inherits="TF.WeiXinVote.MPage.Info" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=0">
    <title><%=icon_type=="success"?"成功":"错误" %></title>
    <link rel="stylesheet" href="//cdn.bootcss.com/weui/0.4.3/style/weui.min.css">
    <link rel="stylesheet" href="//cdn.bootcss.com/jquery-weui/0.8.0/css/jquery-weui.min.css">

</head>
<body ontouchstart>
    
        <div class="page">
            <div class="weui_msg">
                <div class="weui_icon_area"><i class="weui_icon_<%=icon_type %> weui_icon_msg"></i></div>
                <div class="weui_text_area">
                    <h2 class="weui_msg_title"><%=info %></h2>
                    <p class="weui_msg_desc"></p>
                </div>
            </div>
        </div>
    
</body>
</html>
