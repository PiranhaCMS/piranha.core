/*global
    piranha
*/

piranha.preview = new Vue({
    el: "#previewModal",
    data: {
        empty: {
            filename:     null,
            type:         null,
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
    created: function () {
        this.clear();
    }
});
