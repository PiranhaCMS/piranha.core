piranha.media = new Vue({
    el: "#media",
    data: {
        currentFolderId: null,
        parentFolderId: null,
        folders: [],
        items: [],
        folder: {
            name: null
        }
    },
    methods: {
        load: function (id) {
            fetch(piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.media.currentFolderId = result.currentFolderId;
                    piranha.media.parentFolderId = result.parentFolderId;
                    piranha.media.folders = result.folders;
                    piranha.media.items = result.media;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        savefolder: function () {
            fetch(piranha.baseUrl + "manager/api/media/savefolder", {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    parentId: piranha.media.currentFolderId,
                    name: piranha.media.folder.name
                })
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status.type === "success")
                {
                    // Close modal
                    $("#mediaFolderModal").modal("hide");

                    // Clear modal
                    piranha.media.folder.name = null;

                    piranha.media.folders = result.folders;
                    piranha.media.items = result.media;
                }

                // Push status to notification hub
                piranha.notifications.push(result.status);
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        remove: function (id) {
            console.log("Remove media: ", id);
        }
    },
    created: function () {
        this.load();
    }
});
