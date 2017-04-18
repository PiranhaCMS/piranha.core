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
        $('input[name="' + self.mediaId + '"]').val(e.data('id'));
        $('input[name="' + self.mediaName + '"]').val(e.data('name'));
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

$('#modalMedia').on('show.bs.modal', function (event) {
    piranha.media.init($(event.relatedTarget));
    piranha.media.load($(event.relatedTarget), '');
});
