/*global
    piranha, summernote
*/

piranha.summernote = {};

piranha.summernote.linkmodal = new Vue({
    el: "#summernotelink",
    data: {
        url: null,
        text: null,
        target: null,
        callback: null,
        zindex: 3010
    },
    methods: {
        open: function (url, text, target, callback) {
            this.url = url;
            this.text = text;
            this.target = target;
            this.callback = callback;

            $("#summernotelink").modal("show");
        },
        selectPage: function () {
            var self = this;

            this.zindex = 2000; // BS doesn't support multiple modals out of the box
            piranha.pagepicker.open(function (data) {
                self.url = data.permalink;
                self.zindex = 3010;
            });
        },
        selectPost: function () {
            var self = this;

            this.zindex = 2000; // BS doesn't support multiple modals out of the box
            piranha.postpicker.open(function (data) {
                self.url = data.permalink;
                self.zindex = 3010;
            });
        },
        selectMedia: function () {
            var self = this;

            this.zindex = 2000; // BS doesn't support multiple modals out of the box
            piranha.mediapicker.openCurrentFolder(function (data) {
                self.url = data.publicUrl;
                self.zindex = 3010;
            }, null);
        },
        save: function () {
            if (this.callback) {
                this.callback({
                    url: this.url,
                    text: this.text,
                    target: this.target
                });
            }
            $("#summernotelink").modal("hide");
        }
    }
});

var ImageButton = function (context) {
    var ui = $.summernote.ui;

    // create button
    var button = ui.button({
        contents: '<i class="note-icon-picture"/>',
        tooltip: 'Image',
        click: function () {
            context.invoke('saveRange');
            piranha.mediapicker.open(function (img) {
                context.invoke('restoreRange');
                context.invoke('insertImage', img.publicUrl, img.filename);
            }, 'image');
        }
    });

    return button.render();
}

var LinkButton = function (context) {
    var ui = $.summernote.ui;

    // create button
    var button = ui.button({
        contents: '<i class="note-icon-link"/>',
        tooltip: 'Link',
        click: function () {
            var range = context.invoke('createRange');
            context.invoke('saveRange');

            piranha.summernote.linkmodal.open('', range.toString(), '', function (data) {
                context.invoke('restoreRange');
                context.invoke('pasteHTML', '<a href="' + data.url + '">' + data.text + '</a>');
                console.log(data);
            });
        }
    });

    return button.render();
}

//
// Create a new inline editor
//
piranha.editor.addInline = function (id, toolbarId, cb) {
    $("#" + id).summernote({
        toolbar: [
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['media', ['piranhalink', 'piranhaimage']],
            ['view', ['codeview']],
        ],
        buttons: {
            piranhaimage: ImageButton,
            piranhalink: LinkButton
        },
        popover: {
            image: [],
            link: [],
            air: []
        },
        codemirror: {
            lineNumbers: true,
            mode: "text/html",
            lineWrapping: true,
            extraKeys: { "Ctrl-Space": "autocomplete" }
        },
        callbacks: {
            onBlur: function () {
                if (cb)
                    cb($('#' + id).summernote('code'));
            },
            onBlurCodeview: function () {
                if (cb)
                    cb($('#' + id).summernote('code'));
            }
        }
    });
};

//
// Remove the editor instance with the given id.
//
piranha.editor.remove = function (id) {
    $("#" + id).summernote("destroy");
};
