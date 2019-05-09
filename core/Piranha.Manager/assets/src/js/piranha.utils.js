/*global
    piranha
*/

piranha.utils = {
    formatUrl: function (str) {
        return str.replace("~/", piranha.baseUrl);
    },
    isEmptyHtml: function (str) {
        return str == null || str.replace(/(<([^>]+)>)/ig,"").replace(/\s/g, "") == "" && str.indexOf("<img") === -1;
    },
    isEmptyText: function (str) {
        return str == null || str.replace(/\s/g, "") == "" || str.replace(/\s/g, "") == "<br>";
    }
};

$(document).ready(function () {
    $('.block-header .danger').hover(
        function() {
            $(this).closest('.block').addClass('danger');
        },
        function() {
            $(this).closest('.block').removeClass('danger');
        });
});

$(document).on('mouseenter', '.block-header .danger', function() {
    $(this).closest('.block').addClass('danger');
});

$(document).on('mouseleave', '.block-header .danger', function() {
    $(this).closest('.block').removeClass('danger');
});