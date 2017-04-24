//
// Startup
//
$(document).ready(function () {
    $('.datepicker').datetimepicker({
        format: "YYYY-MM-DD"
    });

    //$('[data-toggle="tooltip"]').tooltip()
});

//
// Tooltips
//
$('body').tooltip({
    selector: '[data-toggle="tooltip"]'
});

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
// Handle region items
//
$(document).on('click', '.addRegionItem', function () {
    var btn = $(this);

    manager.tools.addregion(
        btn.data('targetid'), 
        btn.data('pagetypeid'),
        btn.data('regiontypeid'),
        btn.data('regionindex'),
        btn.data('itemindex'),
        function () {
            btn.data('itemindex', btn.data('itemindex') + 1);
        });
});
$(document).on('click', '.region-actions .delete', function () {
    manager.tools.removeregion($(this));
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
        },

        addregion: function (targetId, pageTypeId, regionTypeId, regionIndex, itemIndex, cb) {
            $.ajax({
                url: '/manager/page/region',
                method: 'POST',
                contentType: 'application/json',
                dataType: 'html',
                data: JSON.stringify({
                    PageTypeId: pageTypeId,
                    RegionTypeId: regionTypeId,
                    RegionIndex: regionIndex,
                    ItemIndex: itemIndex
                }),
                success: function (res) {
                    $(targetId).append(res);

                    if (cb)
                        cb();
                }
            });
        },

        removeregion: function (button) {
            var region = button.parent().parent().parent();
            var list = button.parent().parent().parent().parent();

            // Remove the region
            region.remove();

            // Recalculate indexes
            manager.tools.recalcregion(list);
        },

        recalcregion: function (region) {
            var items = region.find('.region-list-item');

            for (var n = 0; n < items.length; n++) {
                var inputs = $(items.get(n)).find('input, textarea');

                $(items.get(n)).find('input').attr('id', function (i, val) {
                    return val.replace(/FieldSets_\d+__/, 'FieldSets_' + n + '__');
                });
                $(items.get(n)).find('input').attr('name', function (i, val) {
                    return val.replace(/FieldSets\[\d+\]/, 'FieldSets[' + n + ']');
                });
            }
        }
    }
};