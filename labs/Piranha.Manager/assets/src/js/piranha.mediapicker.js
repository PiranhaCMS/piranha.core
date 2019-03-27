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
        callback: null
    },
    methods: {
        toggle: function () {
            this.listView = !this.listView;
        },
        load: function (id) {
            fetch(piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.mediapicker.currentFolderId = result.currentFolderId;
                    piranha.mediapicker.parentFolderId = result.parentFolderId;
                    piranha.mediapicker.folders = result.folders;
                    piranha.mediapicker.items = result.media;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        open: function (callback) {
            this.callback = callback;

            $("#mediapicker").modal("show");
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;

            $("#mediapicker").modal("hide");
        }
    },
    created: function () {
        this.load();
    }
});
