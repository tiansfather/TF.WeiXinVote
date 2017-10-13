<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BonusGet.aspx.cs" Inherits="TF.WeiXinVote.MPage.BonusGet" %>


<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=0">
    <title>恭喜中奖</title>
    <link rel="stylesheet" href="//cdn.bootcss.com/weui/1.1.1/style/weui.min.css">
<link rel="stylesheet" href="//cdn.bootcss.com/jquery-weui/1.0.1/css/jquery-weui.min.css">

</head>
<body ontouchstart>
    
        <div class="page">
            
            <div class="weui-msg">
                <div class="weui-msg__icon-area"><i class="weui-icon-success weui-icon_msg"></i></div>
  <div class="weui-msg__text-area">
    <h2 class="weui-msg__title">恭喜您中了<%=bonus.Title %></h2>
    <p class="weui-msg__desc"><%=bonus.BonusTip %></p>
  </div>
                
            </div>
        </div>
 <div class="weui-cells__title">请完善您的信息</div>
        <div class="weui-cells weui-cells_form">
  <div class="weui-cell">
    <div class="weui-cell__hd"><label class="weui-label">姓名</label></div>
    <div class="weui-cell__bd">
      <input class="weui-input" type="text" placeholder="请输入姓名" name="realname" value="<%=record.RealName %>">
    </div>
  </div>
  <div class="weui-cell">
    <div class="weui-cell__hd">
      <label class="weui-label">手机号</label>
    </div>
    <div class="weui-cell__bd">
      <input class="weui-input" type="tel" placeholder="请输入手机号" name="mobile" value="<%=record.Mobile %>">
    </div>
  </div>
</div>

    <div class="button_sp_area" style="padding:1rem;">
        <a href="javascript:submitInfo();;" class="weui-btn weui-btn_primary">提交</a>
    </div>

<script src="//cdn.bootcss.com/jquery/1.11.0/jquery.min.js"></script>
<script src="//cdn.bootcss.com/jquery-weui/1.0.1/js/jquery-weui.min.js"></script>
<script>
    function submitInfo() {
        var recordid = "<%=record.ID%>";
        var realname = $("input[name='realname']").val();
        var mobile = $("input[name='mobile']").val();
        if (!realname) {
            $.toptip('请填写姓名', 'error');
        } else if (!mobile) {
            $.toptip('请填写手机号', 'error');
        } else {
            $.showLoading("正在提交...");
            $.post("Ajax.ashx", { action: "SubmitBonusInfo", recordid: recordid,realname:realname,mobile:mobile }, function (data) {
                $.hideLoading();
                var d=$.parseJSON(data);
                if(d.ErrCode==0){
                    $.toast("操作成功");
                    setTimeout(function () { location.href = "MyBonus.aspx" }, 2000);
                    
                }else{
                    $.toptip(d.Message, 'error');
                }
            })
        }
    }
</script>
</body>
</html>
