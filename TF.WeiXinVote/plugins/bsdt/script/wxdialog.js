/*! zepto.alert 30-08-2014 hihicd@hotmail.com*/
var sjjx = 1; //标识

//判断手机类型
window.onload = function () {
    var u = navigator.userAgent;
    if (u.indexOf('Android') > -1 || u.indexOf('Linux') > -1) {//安卓手机

        sjjx = 1;

    } else if (u.indexOf('iPhone') > -1) {//苹果手机

        sjjx = 2;
    } else if (u.indexOf('Windows Phone') > -1) {//winphone手机

        sjjx = 2;

    }

//    var winheight = document.body.clientHeight - 100;
//    var allheight = document.getElementById("allheight").offsetHeight;

//    if (allheight < winheight) {
//        $("#allheight").css("height", winheight);
//    }
   // document.body.clientHeight = document.body.clientHeight + 100;
}

!function (a, b, c) {
    var d = a(b), e = (a(document), 1),
f = !1, g = function (b) { this.settings = a.extend({}, g.defaults, b), this.init() };
    g.prototype =
 { init: function ()
 { this.create(), this.settings.lock && this.lock(), isNaN(this.settings.time) || null == this.settings.time || this.time() },
 create: function () {
     var cc = '<article class="query-sprite"><div class="btn-sprite flex-sprite"><p class="inner query"></p></div></article>';
        
         var b = null == this.settings.title ? "" : '<div style="padding-top:1em;font-size: 1.2em;" class="rDialog-header-' + this.settings.title + '"></div>',
  c = '<div id="hidebox"><div class="rDialog-wrap"><div class="rDialog-content">' + this.settings.content + '</div><div class="rDialog-footer">'+cc+'</div></div></div>';
         this.dialog = a("<div>").addClass("rDialog").css({ zIndex: this.settings.zIndex + e++ }).html(c).prependTo("body"),
   a.isFunction(this.settings.ok) && this.ok(), a.isFunction(this.settings.cancel) && this.cancel(), this.size(), this.position()
     },
     ok: function () {
         var b = this, d = this.dialog.find(".btn-sprite p.query");
         a("<a>",
   { href: "javascript:;", text: this.settings.okText }).on("click",
   function () { var a = b.settings.ok(); (a == c || a) && b.close() }).addClass("rDialog-ok").prependTo(d)
     },
     cancel: function () {
         var b = this, d = this.dialog.find(".rDialog-footer"); a("<a>",
   { href: "javascript:;", text: this.settings.cancelText }).on("click", function ()
   { var a = b.settings.cancel(); (a == c || a) && b.close() }).addClass("rDialog-cancel").appendTo(d)
     },
     size: function () {
         { var a = this.dialog.find(".rDialog-content"); this.dialog.find(".rDialog-wrap") }
         a.css({ width: this.settings.width, height: this.settings.height })
     },

     position: function () {
         //var a = this, b = d.width(), c = d.height(), e = 0;

         var awidth = document.body.clientWidth, bwidth = awidth - 294, cwidth = bwidth / 2;
         var aheight = window.screen.height, bheight = aheight - 294, cheight = "";
         if (sjjx == 2) {
             cheight = (bheight / 2);
         }
         else {
             cheight = (bheight / 6);
         }
         this.dialog.css({ left: cwidth, top: cheight })
     },

     lock: function () {
         f || (this.lock = a("<div>").css({ zIndex: this.settings.zIndex }).addClass("rDialog-mask"),
      this.lock.appendTo("body"), f = !0)
     },
     unLock: function () { this.settings.lock && f && (this.lock.remove(), f = !1) },
     close: function () { this.dialog.remove(), this.unLock() },
     time: function () { var a = this; this.closeTimer = setTimeout(function () { a.close() }, this.settings.time) }
 },
       g.defaults = { content: "加载中...",
           title: "load", width: "auto", height: "auto",
           ok: null, cancel: null, okText: "确认",
           cancelText: "取消", time: null, lock: !0, zIndex: 9999
       };
    var h = function (a) { new g(a) }; b.rDialog = a.rDialog = a.dialog = h
} (window.jQuery || window.Zepto, window);