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

piranha.post = new function() {
    'use strict';

    var self = this;

    self.postId = '';
    self.postTitle = '';
    self.siteId = '';
    self.blogId;
    self.postUrlId = '';

    self.init = function (e) {
        self.postId = e.data('postid');
        self.postTitle = e.data('posttitle');
        self.siteId = e.data('siteid');
        self.blogId = e.data('blogid');
    };

    self.load = function (e, site, blog) {
        if (!site)
            site = self.siteId;
        if (!blog)
            blog = self.blogId;

        $.ajax({
            url: baseUrl + 'manager/post/modal' + (site ? '/' + site + (blog ? '/' + blog : '') : ''),
            success: function (data) {
                $('#modalPost .modal-body').html(data);
            }
        });
    };

    self.set = function (e) {
        if (self.postUrlId) {
            $('#' + self.postUrlId).val(e.data('url'));
        } else {
            if (self.postId)
                $('#' + self.postId).val(e.data('id'));
            $('#' + self.postTitle).text(e.data('title'));
        }
    };

    self.remove = function (e) {
        $('#' + self.postId).val('');
        $('#' + self.postTitle).text('');
    };
};

$(document).on('click', '#modalPost .modal-body a', function () {
    var button = $(this);

    if (button.data('type') == 'reload') {
        piranha.post.load(button, button.data('siteid'), button.data('blogid'));
    } else {
        piranha.post.set(button);
        $('#modalPost').modal('hide');
    }
    return false;
});

$(document).on('submit', '#modalPost form', function (e) {
    e.preventDefault();

    var form = $('#modalPost form');
    var formData = new FormData(form.get(0));

    $.ajax({
        url: $(this).attr('action'),
        type: 'POST',
        data: formData,
        contentType: false,       
        cache: false,             
        processData: false,                                  
        success: function (data) {
            $('#modalPost .modal-body').html(data);
        },
        error: function (a, b, c) {
            console.log(a)
            console.log(b)
            console.log(c)
        }
    }); 
});

$(document).on('click', '.btn-post-clear', function () {
    piranha.post.init($(this));
    piranha.post.remove($(this));
});

$('#modalPost').on('show.bs.modal', function (event) {
    piranha.post.init($(event.relatedTarget));
    piranha.post.load($(event.relatedTarget));
});
