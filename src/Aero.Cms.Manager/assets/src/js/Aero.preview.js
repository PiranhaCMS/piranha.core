/*global
    Aero
*/

Aero.preview = new Vue({
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
            Aero.preview.media = media;
            Aero.preview.show();
        },
        //TODO: Rename loadAndOpen?
        open: function (mediaId) {
            Aero.preview.load(mediaId);
            Aero.preview.show();
        },
        load: function (mediaId) {
            var self = this;

            fetch(Aero.baseUrl + "manager/api/media/" + mediaId)
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

            fetch(Aero.baseUrl + "manager/api/media/meta/save", {
                method: "post",
                headers: Aero.utils.antiForgeryHeaders(),
                body: JSON.stringify(model)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                Aero.notifications.push(result);

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
                Aero.preview.clear();
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
        this.dropzone = Aero.dropzone.init("#media-update-container", {
            uploadMultiple: false
        });
        this.dropzone.on("complete", function (file) {
            setTimeout(function () {
                Aero.preview.dropzone.removeFile(file);
            }, 3000)
        })
        this.dropzone.on("queuecomplete", function () {
            Aero.preview.load(Aero.preview.media.id);
            Aero.media.refresh();
        })
    }
});
