//
// Panel toggle buttons
//
$(document).on('click', '.panel-heading .btn-toggle', function () {
    var target = $(this).attr('data-target');

    // Remove selecte state
    $(this).siblings('.btn-toggle').removeClass('btn-primary');
    $(this).parent().parent().find('.panel-body').hide();
    $(this).addClass('btn-primary');
    $(target).show();

    return false;
});

//
// Auto grow textareas
//
$('.single-region textarea.raw-text').css({'overflow': 'hidden'}).autogrow({
    vertical: true, 
    horizontal: false
});

//
// Refresh markdown content
//
$(document).on('keyup', '.single-region textarea.raw-text', function() {
    var md = $(this);

    $.ajax({
        url: '/manager/markdown',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify($(this).val()),
        dataType: 'json',
        success: function(data) {
            md.next().html(data.body);
        }
    });
});


//
// Toggle menu style
//
$(document).on('click', '.navmenu-brand', function () {
    $('body').toggleClass('collapsed');
    return false;
});

var manager = {
    tools: {
        markdown: function (str) {
            $.ajax({
                url: '/manager/markdown',
                method: 'POST',
                contentType: 'text/plain',
                data: str,
                success: function (res) {
                    alert(res.Body);
                }
            });
        }
    }
};