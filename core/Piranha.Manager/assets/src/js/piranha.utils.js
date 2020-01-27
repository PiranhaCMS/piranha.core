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
    },
    strLength: function (str) {
        return str != null ? str.length : 0;
    }
};

Date.prototype.addDays = function(days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}
Date.prototype.toDateString = function(days) {
    var date = new Date(this.valueOf());

    return date.getFullYear() + "-" +
        String(date.getMonth() + 1).padStart(2, '0') + "-" +
        String(date.getDate()).padStart(2, '0');
}

$(document).ready(function () {
    $('.block-header .danger').hover(
        function() {
            $(this).closest('.block').addClass('danger');
        },
        function() {
            $(this).closest('.block').removeClass('danger');
        });

    $('form.validate').submit(
        function (e) {
            if ($(this).get(0).checkValidity() === false) {
                e.preventDefault();
                e.stopPropagation();

                $(this).addClass('was-validated');
            }
        }
    );
});

$(document).on('mouseenter', '.block-header .danger', function() {
    $(this).closest('.block').addClass('danger');
});

$(document).on('mouseleave', '.block-header .danger', function() {
    $(this).closest('.block').removeClass('danger');
});

$(document).on('shown.bs.collapse', '.collapse', function () {
	$(this).parent().addClass('active');
});

$(document).on('hide.bs.collapse', '.collapse', function () {
	$(this).parent().removeClass('active');
});

$(window).scroll(function () {
    var scroll = $(this).scrollTop();

    $(".app .component-toolbar").each(function () {
        var parent = $(this).parent();
        var parentTop = parent.offset().top;

        if (scroll > parentTop) {
            var parentHeight = parent.outerHeight();
            var bottom = parentTop + parentHeight;

            if (scroll > bottom) {
                $(this).css({ "top": parentHeight + "px" })
            } else {
                $(this).css({ "top": scroll - parentTop + "px" })
            }
        } else {
            $(this).removeAttr("style");
        }
    });
});