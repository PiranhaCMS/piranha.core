//
// Copyright (c) 2017 Håkan Edling
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
// 
// http://github.com/piranhacms/piranha.core
// 

if (typeof(piranha)  == 'undefined')
    piranha = {};

piranha.media = new function() {
    'use strict';

    var self = this;

    self.mediaId = '';
    self.mediaName = '';
    self.mediaUrlId = '';
    self.mediaFilter = '';
    self.currentFolder = '';
    self.callback = null;
    self.initCallback = null;

    self.init = function (e) {
        self.mediaId = e.attr('data-mediaid');
        self.mediaName = e.data('medianame');
        self.mediaUrlId = e.data('mediaurlid');
        self.mediaFilter = e.data('filter');
    };

    self.load = function (e, folderId) {
        $.ajax({
            url: baseUrl + 'manager/media/modal/' + folderId + '?filter=' + self.mediaFilter,
            success: function (data) {
                $('#modalMedia .modal-body').html(data);
                self.currentFolder = folderId;
                self.bindDropzone();
            }
        });
    };

    self.reload = function (e) {
        self.load(e, self.currentFolder);
    };

    self.set = function (e) {
        if (!self.callback) {
            if (self.mediaId)
                $('#' + self.mediaId).val(e.data('id'));
            $('#' + self.mediaName).text(e.data('name'));
            if (self.mediaUrlId) {
                var mediaUrlCtrl = $('#' + self.mediaUrlId);

                if (mediaUrlCtrl.prop('tagName') == 'IMG') {
                    mediaUrlCtrl.attr('src', e.data('url'));
                } else {
                    mediaUrlCtrl.val(e.data('url'));
                }
            }
            $('#' + self.mediaName).data('filename', e.data('name'));
            $('#' + self.mediaName).data('url', e.data('url'));
            $('#' + self.mediaName).data('contenttype', e.data('contenttype'));
            $('#' + self.mediaName).data('filesize', e.data('filesize'));
            $('#' + self.mediaName).data('modified', e.data('modified'));
        } else {
            self.callback({
                id: e.data('id'),
                name: e.data('name'),
                url: e.data('url')
            });
            self.callback = null;
        }
    };

    self.remove = function (e) {
        $('#' + self.mediaId).val('');
        $('#' + self.mediaName).html('&nbsp;');
        $('#' + self.mediaName).data('filename', '');
        $('#' + self.mediaName).data('url', '');
        $('#' + self.mediaName).data('contenttype', '');
        $('#' + self.mediaName).data('filesize', '');
        $('#' + self.mediaName).data('modified', '');
        if (self.mediaUrlId) {
            var mediaUrlCtrl = $('#' + self.mediaUrlId);

            if (mediaUrlCtrl.prop('tagName') == 'IMG') {
                mediaUrlCtrl.attr('src', '/manager/assets/img/block-img-placeholder.png');
            }
        }
};

    self.bindDropzone = function () {
        $("#dropzonemodal").dropzone({
            paramName: 'Uploads',
            url: '/manager/media/modal/add',
            uploadMultiple: true,
            init: function () {
                this.on("queuecomplete", function(file) {
                    piranha.media.reload();
                });
            }
        });    
    };
};

$(document).on('click', '#modalMedia .modal-body a', function () {
    var button = $(this);

    if (button.data('type') == 'folder') {
        piranha.media.load(button, button.data('folderid'), button.data('filter'));
    } else {
        piranha.media.set(button);
        $('#modalMedia').modal('hide');
    }
    return false;
});

$(document).on('submit', '#modalMedia form', function (e) {
    e.preventDefault();

    var form = $('#modalMedia form');
    var formData = new FormData(form.get(0));

    $.ajax({
        url: $(this).attr('action'),
        type: 'POST',
        data: formData,
        contentType: false,       
        cache: false,             
        processData: false,                                  
        success: function (data) {
            $('#modalMedia .modal-body').html(data);
        },
        error: function (a, b, c) {
            console.log(a)
            console.log(b)
            console.log(c)
        }
    }); 
});

$(document).on('click', '.btn-media-clear', function () {
    piranha.media.init($(this));
    piranha.media.remove($(this));
});

$('.modal').on('shown.bs.modal', function (event) {
    $(this).find('input[autofocus]').focus();
});

$('#modalMedia').on('show.bs.modal', function (event) {
    piranha.media.init($(event.relatedTarget));
    if (piranha.media.initCallback) {
        piranha.media.initCallback();
        piranha.media.initCallback = null;
    }
    piranha.media.load($(event.relatedTarget), '');
});

$('#modalImgPreview').on('show.bs.modal', function (event) {
    var link = $(event.relatedTarget);
    var filename = link.data('filename');
    var url = link.data('url');
    var contenttype = link.data('contenttype');
    var filesize = link.data('filesize');
    var modified = link.data('modified');
    var id = link.data('id');
    var parentid = link.data('parentid');

    var modal = $(this);
    modal.find('.modal-title').text(filename)
    modal.find('#btnDownload').attr('href', url);
    modal.find('#previewContentType').text(contenttype);
    modal.find('#previewFilesize').text(filesize);
    modal.find('#previewModified').text(modified);
    modal.find('#previewId').val(id);
    modal.find('#previewParentId').val(parentid);

    if (!id || id == '')
        modal.find('.fileinput').hide();
    else modal.find('.fileinput').show(); 

    if (contenttype.startsWith("image")) {
        modal.find('#previewImage').show();
        modal.find('#previewVideo').hide();

        modal.find('#imgPreview').attr('alt', filename);
        modal.find('#imgPreview').attr('src', url);
    } else if (contenttype.startsWith("video")) {
        modal.find('#previewImage').hide();
        modal.find('#previewVideo').show();

        modal.find('video').attr('src', url);
        modal.find('video').attr('type', contenttype);
    }
});

