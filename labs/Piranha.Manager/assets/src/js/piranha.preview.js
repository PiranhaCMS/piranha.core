piranha.preview = new Vue({
    el: "#previewModal",
    data: {
        empty: {
            filename:     null,
            contentType:  null,
            publicUrl:    null,
            size:         null,
            width:        null,
            height:       null,
            lastModified: null
        },
        media: null
    },
    methods: {
        open: function (mediaId) {
            piranha.preview.load(mediaId);
            piranha.preview.show();
        },
        load: function (mediaId) {
            fetch(piranha.baseUrl + "manager/api/media/" + mediaId)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.preview.media = result;
                })
                .catch(function (error) { console.log("error:", error ); });
        },        
        show: function () {
            $("#previewModal").modal("show");
        },
        close: function () {
            console.log("click");
            $("#previewModal").modal("hide");
            piranha.preview.clear();
        },
        clear: function () {
            this.media = this.empty;
        }
    },
    computed: {
        size: function () {
            // Convert bytes to a more readable string
            // Source: stackoverflow.com/q/15900485
            var decimals = 2;
            var bytes = this.media.size;
            if (bytes == 0) return '0 Bytes';

            var k = 1024,
                dm = decimals <= 0 ? 0 : decimals || 2,
                sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
                i = Math.floor(Math.log(bytes) / Math.log(k));

            return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
        }
    },
    created: function () {
        this.clear();
    }
});
