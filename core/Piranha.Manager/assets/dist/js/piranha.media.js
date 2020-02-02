/*global
    piranha
*/

Vue.component("folder-item", {
    props: ["item", "selected"],
    methods: {
        toggleItem: function (item) {
            item.isExpanded = !item.isExpanded;
        }
    },
    template:
        "<li class='dd-item expanded' :class='{ active: item.id === selected, expanded: item.isExpanded || item.items.length === 0 }' :data-id='item.id'>" +
        "  <a class='droppable' v-on:click.prevent='piranha.media.load(item.id)' href='#' v-on:dragover='piranha.media.dragover' v-on:dragleave='piranha.media.dragleave' v-on:drop='piranha.media.drop($event, item.id)'>" +
        "    <i class='fas fa-folder'></i>{{ item.name }}" +
        "    <span class='badge badge-light float-right'>{{ item.mediaCount }}</span>" +
        "  </a>" +
        "  <ol v-if='item.items.length > 0' class='dd-list'>" +
        "    <folder-item v-for='child in item.items' v-bind:key='child.id' v-bind:selected='selected' v-bind:item='child'></folder-item>" +
        "  </ol>" +
        "</li>"
});

/*global
    piranha
*/

piranha.media = new Vue({
    el: "#media",
    data: {
        loading: true,
        listView: true,
        currentFolderId: null,
        currentFolderName: null,
        parentFolderId: null,
        items: [],
        structure: [],
        rootCount: null,
        totalCount: null,
        folder: {
            name: null
        },
        dropzone: null
    },
    methods: {
        bind: function (result) {
            this.currentFolderId = result.currentFolderId;
            this.currentFolderName = result.currentFolderName;
            this.parentFolderId = result.parentFolderId;
            this.items = result.media;
            this.structure = result.structure;
            this.rootCount = result.rootCount;
            this.totalCount = result.totalCount;
            this.listView = result.viewMode === "list";
        },
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
        load: function (id, skipState) {
            var self = this;

            if (!skipState) {
                history.pushState({ folderId: id }, "", piranha.baseUrl + "manager/media" + (id ? "/" + id : ""));
            }

            fetch(piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : "") + "/?width=210&height=160")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);

                    document.title = result.currentFolderName ? result.currentFolderName : "Media";
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        getThumbnailUrl: function (item) {
            return item.altVersionUrl !== null ? item.altVersionUrl : piranha.baseUrl + "manager/api/media/url/" + item.id + "/210/160";
        },
        refresh: function () {
            piranha.media.load(piranha.media.currentFolderId);
        },
        addFolder: function () {
            this.saveFolder("#folderAdd", "folderAddForm", {
                parentId: this.currentFolderId,
                name: this.folder.name
            });
        },
        updateFolder: function () {
            this.saveFolder("#folderEdit", "folderEditForm", {
                id: this.currentFolderId,
                name: this.currentFolderName
            });
        },
        saveFolder: function (panel, form, folder) {
            var self = this;

            // Validate form
            var form = document.getElementById(form);
            if (form.checkValidity() === false) {
                form.classList.add("was-validated");
                return;
            }

            fetch(piranha.baseUrl + "manager/api/media/folder/save", {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(folder)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status.type === "success")
                {
                    // Close modal
                    $(panel).collapse('hide')

                    // Clear modal
                    self.folder.name = null;
                    self.items = result.media;

                    // Refresh
                    self.refresh();
                }

                // Push status to notification hub
                piranha.notifications.push(result.status);
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        remove: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/media/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);

                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        removeFolder: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/media/folder/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);

                    history.pushState({ folderId: id }, "", piranha.baseUrl + "manager/media" + (id ? "/" + id : ""));
                    document.title = result.currentFolderName ? result.currentFolderName : "Media";

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
    },
    mounted: function () {
        var self = this;

        window.onpopstate = function (event) {
            self.load(event.state && event.state.folderId ? event.state.folderId : "", true);
        }
    }
});

$(document).on("shown.bs.modal","#mediaFolderModal", function (event) {
    $("#mediaFolderName").focus();
});