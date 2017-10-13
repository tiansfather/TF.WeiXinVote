<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyBonus.aspx.cs" Inherits="TF.WeiXinVote.MPage.MyBonus" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=0">
    <title>我的奖品</title>
    <link rel="stylesheet" href="//cdn.bootcss.com/weui/1.1.1/style/weui.min.css">
<link rel="stylesheet" href="//cdn.bootcss.com/jquery-weui/1.0.1/css/jquery-weui.min.css">
</head>
<body>
    <h1 style="text-align:center;color:green">我的奖品</h1>
          <asp:Repeater runat="server" ID="DataList"  OnItemDataBound="DataList_ItemDataBound">
          <ItemTemplate>
          
              <div class="weui-form-preview">
  <div class="weui-form-preview__hd">
    <label class="weui-form-preview__label"><%#Eval("Bonus.Title") %></label>
    <em class="weui-form-preview__value"><%#Eval("usedate")==null?"<font color='red'>未兑换</font>":"<font color='green'>已兑换</font>" %></em>
  </div>
  <div class="weui-form-preview__bd">
      
    <div class="weui-form-preview__item">
      <label class="weui-form-preview__label">姓名</label>
      <span class="weui-form-preview__value"><%#Eval("realname") %></span>
    </div>
    <div class="weui-form-preview__item">
      <label class="weui-form-preview__label">手机</label>
      <span class="weui-form-preview__value"><%#Eval("mobile") %></span>
    </div>
      <div class="weui-form-preview__item">
      <label class="weui-form-preview__label">中奖日期</label>
      <span class="weui-form-preview__value"><%#Convert.ToDateTime( Eval("createtime")).ToString("yyyy-MM-dd") %></span>
    </div>
      <asp:PlaceHolder runat="server" ID="PH_Bonus">
          <div class="weui-form-preview__item">
      <label class="weui-form-preview__label">领奖地址</label>
      <span class="weui-form-preview__value"><%#Eval("Bonus.Location") %></span>
    </div>
      </asp:PlaceHolder>
    
      <asp:PlaceHolder runat="server" ID="PH_RedPack">
          <div class="weui-form-preview__item">
      <label class="weui-form-preview__label">领取请点击</label>
      <span class="weui-form-preview__value"><a href="RedPack.aspx?code=<%#Eval("addinfo") %>" style="color:red;font-size:16px;font-weight:bold">领取红包</a></span>
    </div>
      </asp:PlaceHolder>
  </div>
  <%--<div class="weui-form-preview__ft">
    <a class="weui-form-preview__btn weui-form-preview__btn_default" href="javascript:">辅助操作</a>
    <button type="submit" class="weui-form-preview__btn weui-form-preview__btn_primary" href="javascript:">操作</button>
  </div>--%>
</div>
          </ItemTemplate>
          </asp:Repeater>
</body>
</html>
