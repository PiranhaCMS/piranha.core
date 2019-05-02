/*global
    piranha
*/

piranha.media = new Vue({
    el: "#media",
    data: {
        listView: true,
        currentFolderId: null,
        parentFolderId: null,
        folders: [],
        items: [],
        folder: {
            name: null
        },
        dropzone: null
    },
    methods: {
        drag: function (event, item) {
            event.dataTransfer.setData("mediaId", item.id);
        },
        dragover: function (event) {
            event.preventDefault();

            var target = event.target.closest(".droppable");
            if (!target.classList.contains("actice")) {
                target.classList.add("active");
            }
        },
        dragleave: function (event) {
            event.preventDefault();

            var target = event.target.closest(".droppable");
            if (!target.classList.contains("actice")) {
                target.classList.remove("active");
            }
        },
        drop: function (event, folder) {
            event.preventDefault();

            var target = event.target.closest(".droppable");
            if (!target.classList.contains("actice")) {
                target.classList.remove("active");
            }

            var mediaId = event.dataTransfer.getData("mediaId");

            console.log("Media ID", mediaId);
            console.log("Folder ID", folder.id);
        },
        toggle: function () {
            this.listView = !this.listView;
        },
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
        refresh: function () {
            piranha.media.load(piranha.media.currentFolderId);
        },
        savefolder: function () {
            fetch(piranha.baseUrl + "manager/api/media/folder/save", {
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
            fetch(piranha.baseUrl + "manager/api/media/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.media.folders = result.folders;
                    piranha.media.items = result.media;

                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        removeFolder: function (id) {
            fetch(piranha.baseUrl + "manager/api/media/folder/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.media.folders = result.folders;

                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                })
                .catch(function (error) { console.log("error:", error ); });
        }
    },
    created: function () {
        this.load();
    },
    mounted: function () {
        this.dropzone = piranha.dropzone.init("#media-upload-container", {
            uploadMultiple: false
        });
        this.dropzone.on("complete", function (file) {
            if (file.status === "success") {
                setTimeout(function () {
                    piranha.media.dropzone.removeFile(file);
                }, 3000)
            }
        })
        this.dropzone.on("queuecomplete", function () {
            piranha.media.refresh();
        })
    }
});
