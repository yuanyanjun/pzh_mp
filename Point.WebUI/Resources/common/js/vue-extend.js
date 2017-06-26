Vue.mixin({
    // 带签名的POST
    signPost: function (url, param, callback) {

        var p = param;
        if ($.isFunction(param)) {
            callback = param;
            p = {};
        }
        var dom = document.querySelector('[name="__RequestVerificationToken"]');
        if (dom) {
            p = $.extend({}, p, {
                __RequestVerificationToken: dom.value
            });
        }

        $.post(url, p, callback);
    }
});
// 自动获取焦点
Vue.directive('focus', {
    inserted: function (el) {
        el.focus()
    }
});
