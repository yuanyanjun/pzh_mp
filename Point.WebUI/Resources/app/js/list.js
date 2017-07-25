
var postUrl = $('#getArticleListUrl').val();
var filter = {
    pageIndex: 1,
    pageSize: 20,
    ArticleType: null,
    typeIds: $('#ArticleTypeIds').val() || null,
    Keywords: null
}
var getArticleDataList = function () {
    $('#loading').removeClass('hide');
    $.post(postUrl, filter, function (res) {
        $('#loading').addClass('hide');
        if (res) {
            var flag = res.substring(0, 1);
            if (flag == 'N') {
                res = res.substring(2);
                var pos = res.indexOf(',');

                var total = parseInt(res.substring(0, pos), 10);
                res = res.substring(pos + 1);
                $('#articleList').append(res);

                initPager(total);
            } else {
                alert(res.substring(2));
            }
        }
        else {
            $('#empty').removeClass('hide');
        }
    });
}

var initPager = function (total) {

    if (total == 0) {
        $('#empty').removeClass('hide');
    }

    var count = parseInt((total - 1) / filter.pageSize + 1, 10);
    if (count > 1) {
        if (filter.pageIndex == count) {
            $('#nomore').removeClass('hide');
        }
        else {
            $('#loadnext').removeClass('hide').bind('click', function () {
                filter.pageIndex++;
                getArticleDataList();

                $('#loadnext').addClass('hide').unbind('click');
            });
        }
    }
}

$(function () {
    getArticleDataList();

    $('#btnSearch').click(function () {
        var txt = $("#text").val();
        filter.Keywords = encodeURIComponent(txt);

        $('#articleList').html('');

        getArticleDataList();
    });
});