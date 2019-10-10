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
