//
// Startup
//

var mdeEditors = new Array();

$(document).ready(function () {
    $('.datepicker').datetimepicker({
        format: "YYYY-MM-DD"
    });

    $('.datetimepicker').datetimepicker({
        format: "YYYY-MM-DD hh:mm"
    });

    $('.select2').select2({
        tags: true
    });

    $.each($(".markdown-editor"), function (i, e) {
        var preview = $(e).parent().parent().find('.content-preview');
        var simplemde = new SimpleMDE({ 
            element: e,
            status: false,
            spellChecker: false,
            hideIcons: ['preview', 'guide'],
            toolbar: [
                'bold', 'italic', 'heading', '|', 'quote', 'unordered-list', 'ordered-list', '|', 'link', 
                {
                    name: 'image',
                    action: function customFunction (editor) {
                        piranha.media.initCallback = function () {
                            console.log('setting media filter');
                            piranha.media.mediaFilter = 'image';
                        };

                        // Show modal
                        $('#modalMedia').modal('show');

                        piranha.media.callback = function (data) {
                            var cm = editor.codemirror;
                            var active = simplemde.getState(cm).image;

                            var startPoint = cm.getCursor('start');
                            var endPoint = cm.getCursor('end');

                            if (active) {
                                text = cm.getLine(startPoint.line);
                                cm.replaceRange('![' + data.name + '](' + data.url + ')', {
                                    line: startPoint.line,
                                    ch: 0
                                });
                            } else {
                                cm.replaceSelection('![' + data.name + '](' + data.url + ')');
                            }
                            cm.setSelection(startPoint, endPoint);
                            cm.focus();
                        };
                    },
                    className: 'fa fa-picture-o',
                    title: 'Image'
                },
                'side-by-side', 'fullscreen'
            ],
            renderingConfig: {
                singleLineBreaks: false
            }
        });
        mdeEditors[$(e).attr("id")] = simplemde;

        simplemde.codemirror.on('change', function () {
            preview.html(simplemde.markdown(simplemde.value()));
        });
    });
});

//
// Tooltips
//
$('body').tooltip({
    selector: '[data-toggle="tooltip"]'
});

//
// Sortable
//
$('.sortable').sortable({
    handle: '.sortable-handle'
}).bind('sortupdate', function(e, ui) {
    manager.tools.recalcregion($(ui.item).parent());
});
$(document).on('click', '.dd-toggle span', function() {
    $(this).parent().parent().toggleClass('expanded');
});

//
// Sortable fix for FF
//
$(document).on('focus', '.region-list-item input, .region-list-item textarea', function () {
    $(this).closest('.region-list-item').attr('draggable', false);
}).on('blur', '.region-list-item input, .region-list-item textarea', function () {
    $(this).closest('.region-list-item').attr('draggable', true);
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

    // Refresh SimpleMDE editors inside tab
    $.each($(target).find(".markdown-editor"), function (i, e) {
        mdeEditors[$(e).attr("id")].codemirror.refresh();
    });

    return false;
});

//
// Refresh SimpleMDE editors inside a collapsible panel
//
$('.region-list-body').on('shown.bs.collapse', function () {
    $.each($(this).find('.markdown-editor'), function (i, e) {
        mdeEditors[$(e).attr('id')].codemirror.refresh();
    });
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
        btn.data('contenttype'),
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
// Toggle menu style
//
$(document).on('click', '.navmenu-brand', function () {
    $('body').toggleClass('collapsed');
    return false;
});


//
// Confirm Delete
//
$(document).on('click', 'a.confirm-delete, button.confirm-delete', function (e) {
    e.preventDefault();
    var data = $(this).data();
    var title = data.title || 'Delete Confirmation';
    var message = data.message || '<p>Are you sure to delete?</p>';
    var url = data.posturl || $(this).attr('href');
    var $modal = $('<div class="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true"><div class="modal-dialog modal-sm"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button><h4 class="modal-title">' + title + '</h4></div><div class="modal-body">' + message + '</div><div class="modal-footer"><form method="post" class="form-delete" style="display:inline;" action="' + url + '"><button class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button> <button class="btn btn-danger btn-labeled" type="submit"><span class="btn-label"><i class="glyphicon glyphicon-trash"></i></span>Yes</button></form></div></div></div></div>').appendTo('body');
    $modal.modal('show');
    $modal.on('hidden.bs.modal', function () { $modal.remove(); });
    return false;
});


//
// Table filters
//
$(document).on('click', '.table-filter button', function (e) {
    e.preventDefault();

    var data = $(this).data();
    var table = $(this).parent().data().table;

    manager.tools.tablesort(table, data.filter, $('#blog-type-filter').val(), $('#blog-category-filter').val(), $('#blog-search').val());

    $(this).parent().find('button').removeClass('btn-primary');
    $(this).addClass('btn-primary');
});

$(document).on('change', '.table-filter select', function (e) {
    e.preventDefault();

    var data = $(this).parent().find('button.btn-primary').data();
    var table = $(this).parent().data().table;
    
    manager.tools.tablesort(table, data.filter, $('#blog-type-filter').val(), $('#blog-category-filter').val(), $('#blog-search').val());
});

$(document).on('keyup', '.table-filter #blog-search', function (e) {
    e.preventDefault();

    var data = $(this).parent().find('button.btn-primary').data();
    var table = $(this).parent().data().table;

    manager.tools.tablesort(table, data.filter, $('#blog-type-filter').val(), $('#blog-category-filter').val(), $('#blog-search').val());
});

//
// Add page
//
$(document).on('click', 'a.add-after, a.add-below', function (e) {
    e.preventDefault();
    var data = $(this).data();
    var title = data.title || 'Choose page type';
    var url = data.posturl || $(this).attr('href');

    var items = "";
    for (var n = 0; n < manager.pageTypes.length; n++) {
        items += '<li class="list-group-item"><a href="' + url.replace("pageType", manager.pageTypes[n].id) + '">' + manager.pageTypes[n].title + '</a></li>';
    }

    var $modal = $('<div class="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true"><div class="modal-dialog modal-sm"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button><h4 class="modal-title">' + title + '</h4></div><div class="modal-body"><ul class="list-group">' + items + '</ul></div><div class="modal-footer"><button class="btn btn-default" data-dismiss="modal" aria-hidden="true">Cancel</button></div></div></div></div>').appendTo('body');
    $modal.modal('show');
    $modal.on('hidden.bs.modal', function () { $modal.remove(); });
    return false;
});

$(document).on('mouseenter', 'a.add-after', function (e) {
    $(this).parent().parent().append('<div id="add-page-placeholder" class="dd-placeholder""><span>Your new page</span></div>');
});
$(document).on('mouseleave', 'a.add-after', function (e) {
    $('#add-page-placeholder').remove();
});
$(document).on('mouseenter', 'a.add-below', function (e) {
    var parent = $(this).parent().parent();
    var list = parent.children('ol');
    if (list.length > 0) {
        var isExpanded = parent.hasClass("expanded");

        list.children('li:first-child').before('<div id="add-page-below-placeholder" class="dd-placeholder"><span>Your new page</span></div>');
        
        if (!isExpanded) {
            $('#add-page-below-placeholder').addClass('hover-expanded');
            parent.addClass('expanded');
        }
    } else {
        parent.append('<ol id="add-page-below-placeholder" class="dd-list"><div class="dd-placeholder"><span>Your new page</span></div></ol>');
    }
});
$(document).on('mouseleave', 'a.add-below', function (e) {
    var elm = $('#add-page-below-placeholder');

    if (elm.hasClass('hover-expanded'))
        elm.parent().parent().removeClass('expanded');
    elm.remove();
});
$(document).on('mouseenter', '.sitemap-item .remove', function (e) {
    $(this).parent().parent().addClass('hover-remove');
});
$(document).on('mouseleave', '.sitemap-item .remove', function (e) {
    $(this).parent().parent().removeClass('hover-remove');    
});
        
var manager = {
    pageTypes: [],

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

        addregion: function (targetId, pageTypeId, regionTypeId, regionIndex, itemIndex, contentType, cb) {
            $.ajax({
                url: contentType == 'post' ? '/manager/post/region' : '/manager/page/region',
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

                    // If the new region contains a html editor, make sure
                    // we initialize it.
                    var editors = $(res).find('.editor').each(function () {
                        tinyMCE.execCommand('mceAddEditor', false, this.id);
                    });

                    if (cb)
                        cb();

                    $('.sortable').sortable('destroy');
                    $('.sortable').sortable({
                        handle: '.sortable-handle'
                    }).bind('sortupdate', function(e, ui) {
                        manager.tools.recalcregion($(ui.item).parent());
                    });
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
                var inputs = $(items.get(n)).find('input, textarea, select');

                inputs.attr('id', function (i, val) {
                    return val.replace(/FieldSets_\d+__/, 'FieldSets_' + n + '__');
                });
                inputs.attr('name', function (i, val) {
                    return val.replace(/FieldSets\[\d+\]/, 'FieldSets[' + n + ']');
                });
            }
        },

        tablesort: function(table, status, type, category, search) {
            $.each($(table).find('tr'), function (i, e) {
                if (i > 0) {
                    var row = $(e);
                    var show = true;
                    
                    if (status != '' && !row.hasClass(status))
                        show = false;
                    if (type != '' && type != row.data().posttype)
                        show = false;
                    if (category != '' && category != row.data().category)
                        show = false;
                    if (search != '') {
                        if (!row.first('td').text().toLowerCase().includes(search))
                            show = false;
                    }

                    if (show)
                        row.show();
                    else row.hide();
                }
            });        
        }
    }
};