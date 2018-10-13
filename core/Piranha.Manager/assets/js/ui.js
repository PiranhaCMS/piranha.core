//
// Startup
//

var mdeEditors = new Array();

function RegisterMarkdown (e) {
    var preview = $(e).parent().parent().find('.content-preview');
    var simplemde = new SimpleMDE({ 
        forceSync: true,
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
};

$(document).ready(function () {
    $('.datepicker').datetimepicker({
        format: "YYYY-MM-DD"
    });

    $('.datetimepicker').datetimepicker({
        format: "YYYY-MM-DD HH:mm"
    });

    $('.select2').select2({
        tags: true
    });

    $.each($(".markdown-editor"), function (i, e) {
        RegisterMarkdown(e);
    });

    manager.tools.positionblocks();
    manager.tools.positionButtonsFixed();
});

//
// Tooltips
//
$('body').tooltip({
    selector: '[data-toggle="tooltip"]'
});

//
// Count fields
//
$(document).on('keyup', '.count-me', function () {
    var max = Number($(this).attr('maxlength'));
    var length = $(this).val().length;

    if (length > max) {
        $(this).val($(this).val().substring(0, max));
        length = max;
    }

    $(this).next().text(length + '/' + max);
});

//
// Sortable
//
var sortableBlocks = sortable('.page-blocks-body .sortable', {
    handle: '.sortable-handle',
    items: ':not(.unsortable)'
});
for (var n = 0; n < sortableBlocks.length; n++) {
    sortableBlocks[n].addEventListener('sortupdate', function(e) {
        manager.tools.recalcblocks();
    });
}
var sortableBlockItems = sortable('.block-group-body', {
    handle: '.sortable-handle',
    items: ':not(.unsortable)'
});
for (var n = 0; n < sortableBlockItems.length; n++) {
    sortableBlockItems[n].addEventListener('sortupdate', function(e) {
        manager.tools.recalcblocks();
    });
}
var sortableRegions = sortable('.region-list.sortable', {
    handle: '.sortable-handle'
});
for (var n = 0; n < sortableRegions.length; n++) {
    sortableRegions[n].addEventListener('sortupdate', function(e) {
        // Fix for Chrome
        $.each($(e.target).find('.region-list-item'), function (i, e) {
            $(e).removeClass("sortable-dragging");
            $(e).attr('aria-grabbed', 'false');
            $(e).removeAttr('style');
        });

        manager.tools.recalcregion($(e.target));
    });
}
$(document).on('click', '.dd-toggle span', function() {
    $(this).parent().parent().toggleClass('expanded');
});

//
// Blocks
//
$(document).on('click', '.block-remove', function() {
    $(this).closest('.sortable-item').remove();
    manager.tools.recalcblocks();
});
$(document).on('click', '.block-add-toggle', function(e) {
    e.preventDefault();
    
    var active = $(this).parent().hasClass('active');
    $('.block-add').removeClass('active');

    if (!active) {
        $(this).parent().addClass('active');
        $('.form-dimmer').addClass('active');
    } else {
        $('.form-dimmer').removeClass('active');
    }
});
$(document).on('click', '.block-add-dialog a', function (e) {
    e.preventDefault();

    manager.tools.addblock($(this).parent().parent().parent(), $(this).attr('data-typename'), 'page', $(this).attr('data-includegroups'), $(this).attr('data-grouptype'), function() {
        manager.tools.recalcblocks();
        $('.block-add, .form-dimmer').removeClass('active');
    });
});
$(document).on('click', '.form-dimmer', function (e) {
    e.preventDefault();

    $('.block-add.active').removeClass('active');
    $(this).removeClass('active');
});
$(document).on('click', '.block-html-swap', function (e) {
    e.preventDefault();

    var columns = $(this).parent().parent().find('.block-editor');
    if (columns.length == 2) {
        var col1 = $(columns[0]).html();
        var col2 = $(columns[1]).html();

        $(columns[0]).html(col2);
        $(columns[1]).html(col1);
    }
});
$(document).on('click', '.block-group-info', function (e) {
    e.preventDefault();

    $(this).parent().find('.block-expand').click();
});
$(document).on('click', '.block-group .block-expand', function (e) {
    e.preventDefault();

    $(this).parent().parent().find('.block-info').slideToggle();
    $(this).parent().parent().find('.block-group-body').slideToggle();
    $(this).find('span').toggleClass('fa-angle-down');
    $(this).find('span').toggleClass('fa-angle-up');
});
$(document).on('focus', '.block .empty', function () {
    $(this).removeClass('empty');
    $(this).addClass('check-empty');
});
$(document).on('blur','.block .check-empty', function () {
    if (manager.tools.isempty(this)) {
        $(this).removeClass('check-empty');    
        $(this).addClass('empty');
    }
});
$(window).scroll(function () {
    manager.tools.positionblocks();
});
$(window).resize(function () {
    manager.tools.positionButtonsFixed();
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
// Copy editable data
//
$(document).on('submit', 'form', function (e) {
    $(this).find('.editor-area').each(function () {
        $('#' + $(this).attr('data-id')).val($(this).html());
    });
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

$(document).on('keyup', '.table-filter #media-search', function (e) {
    e.preventDefault();

    var table = $(this).parent().parent().data().table;

    manager.tools.tablesort(table, '', '', '', $(this).val());
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
        parent.append('<ol id="add-page-below-placeholder" class="dd-list hover-expanded"><div class="dd-placeholder"><span>Your new page</span></div></ol>');
        parent.addClass('expanded');
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
        isempty: function(elm) {
            return $(elm).text().replace(/\s/g, '') == '' && $(elm).find('img').length == 0;
        },

        positionblocks: function () {
            var toggles = $('.block-add');
            var middle = $(window).height() / 2;
        
            for (var n = 0; n < toggles.length; n++) {
                var toggle = toggles.get(n);
        
                if (toggle.getBoundingClientRect().y < middle) {
                    $(toggle).removeClass('block-add-above').addClass('block-add-below');
                } else {
                    $(toggle).removeClass('block-add-below').addClass('block-add-above');
                }
            }
        },

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
                url: contentType == 'post' ? '/manager/post/region' : contentType == 'site' ? '/manager/site/region' : '/manager/page/region',
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
                        addFormEditor('#' + this.id);
                    });

                    // Initialize markdown editors.
                    $(res).find('.markdown-editor').each(function () {
                        RegisterMarkdown($('#' + $(this).attr('id')).get(0));
                    });           

                    if (cb)
                        cb();

                    sortable('.sortable');
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
                    if (val)
                        return val.replace(/FieldSets_\d+__/, 'FieldSets_' + n + '__');
                    return val;
                });
                inputs.attr('name', function (i, val) {
                    if (val)
                        return val.replace(/FieldSets\[\d+\]/, 'FieldSets[' + n + ']');
                    return val;
                });
            }
        },

        addblock: function(target, blockType, contentType, includeGroups, groupType, cb) {
            $.ajax({
                url: '/manager/block/create',
                method: 'POST',
                contentType: 'application/json',
                dataType: 'html',
                data: JSON.stringify({
                    TypeName: blockType,
                    BlockIndex: 0,
                    IncludeGroups: includeGroups,
                    GroupType: groupType
                }),
                success: function (res) {
                    $('.page-blocks-body >.block-info').remove();
                    $(res).insertAfter(target);

                    // If the new region contains a html editor, make sure
                    // we initialize it.
                    var editors = $(res).find('.block-editor').each(function () {
                        addInlineEditor('#' + this.id);
                    });

                    // Initialize markdown editors.
                    $(res).find('.markdown-editor').each(function () {
                        RegisterMarkdown($('#' + $(this).attr('id')).get(0));
                    });           

                    if (cb)
                        cb();

                    manager.tools.setupBlockSortable();
                }
            });            
        },
        
        recalcblocks: function () {
            var items = $('.page-blocks-body .sortable >.sortable-item');

            for (var n = 0; n < items.length; n++) {
                var inputs = $(items.get(n)).find('input, textarea, select');

                inputs.attr('id', function (i, val) {
                    if (val)
                        return val.replace(/Blocks_\d+__/, 'Blocks_' + n + '__');
                    return val;
                });
                inputs.attr('name', function (i, val) {
                    if (val)
                        return val.replace(/Blocks\[\d+\]/, 'Blocks[' + n + ']');
                    return val;
                });

                var content = $(items.get(n)).find('[contenteditable=true]');
                content.attr('data-id', function (i, val) {
                    if (val)
                        return val.replace(/Blocks_\d+__/, 'Blocks_' + n + '__');
                    return val;
                });

                var media = $(items.get(n)).find('button');
                media.attr('data-mediaid', function (i, val) {
                    if (val)
                        return val.replace(/Blocks_\d+__/, 'Blocks_' + n + '__');
                    return val;
                });

                var subitems = $(items.get(n)).find('.block-group-body .sortable-item');

                for (var s = 0; s < subitems.length; s++) {
                    var subInputs = $(subitems.get(s)).find('input, textarea, select');

                    subInputs.attr('id', function (i, val) {
                        if (val)
                            return val.replace(/Blocks_\d+__Items_\d+__/, 'Blocks_' + n + '__Items_' + s + '__');
                        return val;
                    });
                    subInputs.attr('name', function (i, val) {
                        if (val)
                            return val.replace(/Blocks\[\d+\].Items\[\d+\]/, 'Blocks[' + n + '].Items[' + s + ']');
                        return val;
                    });
    
                    var subContent = $(subitems.get(s)).find('[contenteditable=true]');
                    subContent.attr('data-id', function (i, val) {
                        if (val)
                            return val.replace(/Blocks_\d+__Items_\d+__/, 'Blocks_' + n + '__Items_' + s + '__');
                        return val;
                    });
    
                    var subContent = $(subitems.get(s)).find('button');
                    subContent.attr('data-mediaid', function (i, val) {
                        if (val)
                            return val.replace(/Blocks_\d+__Items_\d+__/, 'Blocks_' + n + '__Items_' + s + '__');
                        return val;
                    });                    
                }
            }
            manager.tools.setupBlockSortable();
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
        },

        setupBlockSortable: function() {
            sortable('.page-blocks-body .sortable');
            sortable('.block-group-body');
        },

        positionButtonsFixed: function() {
            var diff = 1420 - $(window).width();

            $('.buttons-fixed').css({ left: Math.min(1152, 1152 - diff) });
        }
    }
};