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
            title:        null,
            altText:      null,
            description:  null,
            lastModified: null
        },
        media: null,
        dropzone: null
    },
    methods: {
        openItem: function (media) {
            piranha.preview.media = media;
            piranha.preview.show();
        },
        //TODO: Rename loadAndOpen?
        open: function (mediaId) {
            piranha.preview.load(mediaId);
            piranha.preview.show();
        },
        load: function (mediaId) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/media/" + mediaId)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.media = result;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        saveMeta: function (media) {
            var self = this;

            var model = {
                id: media.id,
                title: media.title,
                altText: media.altText,
                description: media.description,
                properties: media.properties
            };

            fetch(piranha.baseUrl + "manager/api/media/meta/save", {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(model)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                piranha.notifications.push(result);

                if (result.type === "success") {
                    self.close();
                }
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        show: function () {
            $("#previewModal").modal("show");
        },
        close: function () {
            $("#previewModal").modal("hide");
            setTimeout(function () {
                piranha.preview.clear();
            }, 300)
        },
        clear: function () {
            this.media = this.empty;
        }
    },
    created: function () {
        this.clear();
    },
    mounted: function () {
        this.dropzone = piranha.dropzone.init("#media-update-container", {
            uploadMultiple: false
        });
        this.dropzone.on("complete", function (file) {
            setTimeout(function () {
                piranha.preview.dropzone.removeFile(file);
            }, 3000)
        })
        this.dropzone.on("queuecomplete", function () {
            piranha.preview.load(piranha.preview.media.id);
            piranha.media.refresh();
        })
    }
});
