/*global
    piranha
*/

piranha.media = new Vue({
    el: "#media",
    data: {
        loading: true,
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

            var target = $(event.target).closest(".droppable");
            if (!target.hasClass("active")) {
                target.addClass("active");
            }
        },
        dragleave: function (event) {
            event.preventDefault();

            var target = $(event.target).closest(".droppable");
            if (target.hasClass("active")) {
                target.removeClass("active");
            }
        },
        drop: function (event, folderId) {
            event.preventDefault();

            var target = $(event.target).closest(".droppable");
            if (target.hasClass("active")) {
                target.removeClass("active");
            }

            var mediaId = event.dataTransfer.getData("mediaId");

            fetch(piranha.baseUrl + "manager/api/media/move/" + mediaId + "/" + (folderId ? folderId : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.type === "success") {
                        piranha.media.refresh();
                    }
                    // Push status to notification hub
                    piranha.notifications.push(result);
                })
                .catch(function (error) { console.log("error:", error); });
        },
        showList: function () {
            this.listView = true;
        },
        showGallery: function () {
            this.listView = false;
        },
        load: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : "") + "/?width=210&height=160")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.currentFolderId = result.currentFolderId;
                    self.parentFolderId = result.parentFolderId;
                    self.folders = result.folders;
                    self.items = result.media;
                    self.listView = result.viewMode === "list";
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        getThumbnailUrl: function (item) {
            return item.altVersionUrl !== null ? item.altVersionUrl : piranha.baseUrl + "manager/api/media/url/" + item.id + "/210/160";
        },
        refresh: function () {
            piranha.media.load(piranha.media.currentFolderId);
        },
        savefolder: function () {
            // Validate form
            var form = document.getElementById("mediaFolderForm");
            if (form.checkValidity() === false) {
                form.classList.add("was-validated");
                return;
            }

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
    updated: function () {
        if (this.loading) {
            if (piranha.permissions.media.add) {
                this.dropzone = piranha.dropzone.init("#media-upload-container", {
                    uploadMultiple: false
                });
                this.dropzone.on("complete", function (file) {
                    if (file.status === "success") {
                        setTimeout(function () {
                            piranha.media.dropzone.removeFile(file);
                        }, 3000)
                    }
                });
                this.dropzone.on("queuecomplete", function () {
                    piranha.media.refresh();
                });
            }

            this.loading = false;
        }
    }
});

$(document).on("shown.bs.modal","#mediaFolderModal", function (event) {
    $("#mediaFolderName").focus();
});