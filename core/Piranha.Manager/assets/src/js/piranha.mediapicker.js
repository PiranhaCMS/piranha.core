/*global
    piranha
*/

piranha.mediapicker = new Vue({
    el: "#mediapicker",
    data: {
        listView: true,
        currentFolderId: null,
        parentFolderId: null,
        folders: [],
        items: [],
        folder: {
            name: null
        },
        filter: null,
        callback: null,
        dropzone: null
    },
    methods: {
        toggle: function () {
            this.listView = !this.listView;
        },
        load: function (id) {
            var url = piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : "");
            if (this.filter) {
                url += "?filter=" + this.filter;
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.mediapicker.currentFolderId = result.currentFolderId;
                    piranha.mediapicker.parentFolderId = result.parentFolderId;
                    piranha.mediapicker.folders = result.folders;
                    piranha.mediapicker.items = result.media;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            piranha.mediapicker.load(piranha.mediapicker.currentFolderId);
        },
        open: function (callback, filter, folderId) {
            this.callback = callback;
            this.filter = filter;

            this.load(folderId);

            $("#mediapicker").modal("show");
        },
        openCurrentFolder: function (callback, filter) {
            this.callback = callback;
            this.filter = filter;

            this.load(this.currentFolderId);

            $("#mediapicker").modal("show");
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;

            $("#mediapicker").modal("hide");
        }
    },
    mounted: function () {
        this.dropzone = piranha.dropzone.init("#mediapicker-upload-container");
        this.dropzone.on("complete", function (file) {
            if (file.status === "success") {
                setTimeout(function () {
                    piranha.mediapicker.dropzone.removeFile(file);
                }, 3000)
            }
        })
        this.dropzone.on("queuecomplete", function () {
            piranha.mediapicker.refresh();
        })
    }
});
