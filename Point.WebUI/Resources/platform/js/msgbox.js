var Msg = (function ($) {
    var defaults = {
        width: "450",
        type: "confirm",//弹出类型有 alert,confirm,prompt,confirm2,warn
        title: "提示",
        txt: '',
        zindex: '10000',
        confirmsuretxt: '确定',
        confirmcaneltxt: '取消',
        confirmsurecb: null,
        confirmcanelcb: null,
        alertsurecb: null,
        autohide: ''//仅仅对于prompt弹出方式自动隐藏
    };

    var msg = function (opts) {
        this.time = new Date().getTime();
        this.opt = $.extend({}, defaults, opts || {});
        init.call(this);
    };
    //初始化HTML
    var init = function () {
        var buff = [];
        var opt = this.opt;
        buff.push('<div class="msg-box" id="MsgBox_', this.time, '" style="display:none;z-index:' + (opt.zindex || '') + '">');
        buff.push('<div class="msg-cover" id="MsgCover_', this.time, '"></div><div class="msg-cont" id="MsgCont_', this.time, '"><div class="msg-title">');
        buff.push('<span class="title-bar" id="MsgTitle_', this.time, '"></span><div class="msg-close" id="MsgClose_', this.time, '"></div></div>');
        buff.push('<div class="msg-container"><div class="txt" id="MsgTxt_', this.time, '"></div>');
        buff.push('<div class="btn-box" id="MsgBtnBox_', this.time, '"></div>');
        buff.push('</div></div><div class="msg-prop" id="MsgProp_', this.time, '"><div class="msg-bg"></div><div id="MsgProp_txt_', this.time, '"></div></div></div>');
        $("body").append(buff.join(""));
        bindHtml.call(this);
    }

    //修改HTML
    var bindHtml = function () {
        var opt = this.opt;
        var ow = opt.width == "" ? defaults.width : opt.width;
        $("#MsgCont_" + this.time).css({
            'margin-left': -ow / 2,
            'width': ow
        });
        $("#MsgProp_" + this.time).css({
            'margin-left': -ow / 2,
            'width': ow
        });
        $("#MsgTitle_" + this.time).html(opt.title);

        $("#MsgTxt_" + this.time).html(opt.type == "warn" ? '<i class="msg-warn-icon"></i>' + opt.txt : opt.txt);
        $("#MsgProp_txt_" + this.time).html(opt.txt);
        bindType.call(this);
    }
    //类型判断
    var bindType = function () {
        var _type = this.opt.type;
        var _that = this;
        var _btnBox = $("#MsgBtnBox_" + this.time);
        var _msgbox = $("#MsgCont_" + this.time);
        var _propbox = $("#MsgProp_" + this.time);
        switch (_type) {
            case "alert": {
                _propbox.hide();
                _msgbox.show();
                var str = '<a class="btn msg-alert" href="javascript:void(0);" id="MsgAlertSure' + this.time + '">确定</a>';
                _btnBox.html(str);
            }
                break;
            case "confirm": {
                _propbox.hide();
                _msgbox.show();
                var str = '<a class="btn msg-sure" href="javascript:void(0);" id="MsgConfirmSure_' + this.time + '">' + this.opt.confirmsuretxt + '</a> <a class="btn msg-refuse" href="javascript:void(0);" id="MsgConfirmCanel_' + this.time + '">' + this.opt.confirmcaneltxt + '</a>';
                _btnBox.html(str);
            }
                break;
            case "confirm2": {
                _propbox.hide();
                _msgbox.show();
                var str = '<a class="btn msg-sure" href="javascript:void(0);" id="MsgConfirmSure_' + this.time + '">' + this.opt.confirmsuretxt + '</a> <a class="btn msg-canel" href="javascript:void(0);" id="MsgConfirmCanel_' + this.time + '">' + this.opt.confirmcaneltxt + '</a>';
                _btnBox.html(str);
            }
                break;
            case "warn": {
                _propbox.hide();
                _msgbox.show();
                var str = '<a class="btn msg-warn" href="javascript:void(0);" id="MsgConfirmSure_' + this.time + '">' + this.opt.confirmsuretxt + '</a> <a class="btn msg-canel" href="javascript:void(0);" id="MsgConfirmCanel_' + this.time + '">' + this.opt.confirmcaneltxt + '</a>';
                _btnBox.html(str);
            }
                break;
            case "prompt": {
                $("#MsgCover_" + this.time).css({
                    'background': "none"
                });
                _propbox.show();
                _msgbox.hide();
                if (_that.opt.autohide && _that.opt.autohide != "") {
                    setTimeout(function () {
                        _that.hide();
                    }, parseInt(_that.opt.autohide));
                }
            }
                break;
        }
        bindEvent.call(this);
    }
    //点击事件
    var bindEvent = function () {
        var _that = this;
        //关闭按钮
        $("#MsgClose_" + this.time).bind("click", function () {
            _that.hide();
        });
        $("#MsgAlertSure" + this.time).bind("click", function () {
            if ($.isFunction(_that.opt.alertsurecb)) {
                _that.opt.alertsurecb();
            }
            _that.hide();
        });
        $("#MsgConfirmSure_" + this.time).bind("click", function () {
            if ($.isFunction(_that.opt.confirmsurecb)) {
                _that.opt.confirmsurecb();
            }
            _that.hide();
        });
        $("#MsgConfirmCanel_" + this.time).bind("click", function () {
            if ($.isFunction(_that.opt.confirmcanelcb)) {
                _that.opt.confirmcanelcb();
            }
            _that.hide();
        });
    };
    msg.prototype = {
        constructor: msg,
        show: function (param) {
            this.opt = $.extend({}, defaults, this.opt, param || {});
            var _msg = $("#MsgBox_" + this.time);
            var _msgCont = $("#MsgCont_" + this.time);
            bindHtml.call(this);
            if (this.opt.type != "prompt") {
                _msgCont.hide();
            }
            _msg.show();
            $("#MsgCover_" + this.time).animate({
                'opacity': '.86',
                'filter': 'alpha(opacity = 86)'
            }, 350, 'linear');
            if (this.opt.type != "prompt") {
                setTimeout(function () {
                    _msgCont.show();
                }, 350);
            }
        },
        hide: function () {
            var _msg = $("#MsgBox_" + this.time);
            $("#MsgCover_" + this.time).animate({
                'opacity': '0',
                'filter': 'alpha(opacity = 0)'
            }, 0);
            this.opt.confirmsurecb = null;
            this.opt.confirmcanelcb = null;
            this.opt.alertsurecb = null;
            _msg.hide();
            _msg.remove();
        }
    };
    function Dom(opts, types) {
        var that = this;
        that.types = types;
        that.dom = new msg({
            width: opts.width,
            type: that.types,//弹出类型有 alert,confirm,prompt,confirm2
            title: opts.title,
            txt: typeof opts === 'string' ? opts : opts.txt,
            zindex: opts.zindex,
            confirmsuretxt: opts.confirmsuretxt,
            confirmcaneltxt: opts.confirmcaneltxt,
            confirmsurecb: opts.confirmsurecb,
            confirmcanelcb: opts.confirmcanelcb,
            alertsurecb: opts.alertsurecb,
            autohide: opts.autohide//仅仅对于prompt弹出方式自动隐藏
        });
        that.dom.show();
        return this;
    }
    Dom.prototype = {
        constructor: Dom,
        hide: function () {
            var that = this;
            that.dom.hide();
            that.dom = null;
            return that;
        }
    };
    var msgbox = {
        alert: function (opts) {
            return new Dom(opts, 'alert');
        },
        confirm: function (opts) {
            return new Dom(opts, 'confirm');
        },
        confirm2: function (opts) {
            return new Dom(opts, 'confirm2');
        },
        prompt: function (opts) {
            return new Dom(opts, 'prompt');
        },
        warn: function (opts) {
            return new Dom(opts, 'warn');
        }
    };
    return msgbox;
})(window.jQuery);



var TipMsg = {
    show: function (msg, width) {
        Msg.prompt({ txt: msg, autohide: 1000, width: width || 200 });
    }
    
}
