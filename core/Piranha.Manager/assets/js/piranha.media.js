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

    self.init = function (e) {
        self.mediaId = e.data('mediaid');
        self.mediaName = e.data('medianame');
    };

    self.load = function (e, folderId) {
        $.ajax({
            url: baseUrl + 'manager/media/modal/' + folderId,
            success: function (data) {
                $('#modalMedia .modal-body').html(data);
            }
        });
    };

    self.set = function (e) {
        $('#' + self.mediaId).val(e.data('id'));
        $('#' + self.mediaName).text(e.data('name'));
        $('#' + self.mediaName).data('filename', e.data('name'));
        $('#' + self.mediaName).data('url', e.data('url'));
        $('#' + self.mediaName).data('contenttype', e.data('contenttype'));
        $('#' + self.mediaName).data('filesize', e.data('filesize'));
        $('#' + self.mediaName).data('modified', e.data('modified'));
    };

    self.remove = function (e) {
        $('#' + self.mediaId).val('');
        $('#' + self.mediaName).text('');
        $('#' + self.mediaName).data('filename', '');
        $('#' + self.mediaName).data('url', '');
        $('#' + self.mediaName).data('contenttype', '');
        $('#' + self.mediaName).data('filesize', '');
        $('#' + self.mediaName).data('modified', '');
    };
};

$(document).on('click', '#modalMedia .modal-body a', function () {
    var button = $(this);

    if (button.data('type') == 'folder') {
        piranha.media.load(button, button.data('folderid'));
    } else {
        piranha.media.set(button);
        $('#modalMedia').modal('hide');
    }
    return false;
});

$(document).on('click', '.btn-media-clear', function () {
    piranha.media.init($(this));
    piranha.media.remove($(this));
});

$('#modalMedia').on('show.bs.modal', function (event) {
    piranha.media.init($(event.relatedTarget));
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

