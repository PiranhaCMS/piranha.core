//
// Copyright (c) 2018 Håkan Edling
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.
// 
// http://github.com/piranhacms/piranha.core
// 

if (typeof(piranha)  == 'undefined')
    piranha = {};

piranha.page = new function() {
    'use strict';

    var self = this;

    self.pageId = '';
    self.pageTitle = '';
    self.siteId = '';
    self.pageUrlId = '';
    self.callback = null;

    self.init = function (e) {
        self.pageId = e.data('pageid');
        self.pageTitle = e.data('pagetitle');
        self.siteId = e.data('siteid');
    };

    self.load = function (e, site) {
        if (!site)
            site = self.siteId;

        $.ajax({
            url: baseUrl + 'manager/page/modal' + (site ? '/' + site : ''),
            success: function (data) {
                $('#modalPage .modal-body').html(data);
            }
        });
    };

    self.set = function (e) {
        if (!self.callback) {
            if (self.pageUrlId) {
                $('#' + self.pageUrlId).val(e.data('url'));
            } else {
                if (self.pageId)
                    $('#' + self.pageId).val(e.data('id'));
                $('#' + self.pageTitle).text(e.data('title'));
            }
        } else {
            self.callback({
                id:  e.data('id'),
                title: e.data('title'),
                url: e.data('url')
            });
            self.callback = null;
        }
    };

    self.remove = function (e) {
        $('#' + self.pageId).val('');
        $('#' + self.pageTitle).text('');
    };
};

$(document).on('click', '#modalPage .modal-body a', function () {
    var button = $(this);

    if (button.data('type') == 'reload') {
        piranha.page.load(button, button.data('siteid'));
    } else {
        piranha.page.set(button);
        $('#modalPage').modal('hide');
    }
    return false;
});

$(document).on('submit', '#modalPage form', function (e) {
    e.preventDefault();

    var form = $('#modalPage form');
    var formData = new FormData(form.get(0));

    $.ajax({
        url: $(this).attr('action'),
        type: 'POST',
        data: formData,
        contentType: false,       
        cache: false,             
        processData: false,                                  
        success: function (data) {
            $('#modalPage .modal-body').html(data);
        },
        error: function (a, b, c) {
            console.log(a)
            console.log(b)
            console.log(c)
        }
    }); 
});

$(document).on('click', '.btn-page-clear', function () {
    piranha.page.init($(this));
    piranha.page.remove($(this));
});

$('#modalPage').on('show.bs.modal', function (event) {
    piranha.page.init($(event.relatedTarget));
    piranha.page.load($(event.relatedTarget));
});
