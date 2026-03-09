Vue.component("folder-item", {
  props: ["item", "selected"],
  template: "\n<li class=\"dd-item expanded\" :class=\"{ active: item.id === selected, expanded: item.isExpanded || item.items.length === 0 }\" :data-id=\"item.id\">\n    <a v-if=\"!item.edit\" class=\"droppable\" v-on:click.prevent=\"piranha.media.load(item.id)\" href=\"#\" draggable=\"true\" v-on:dragstart=\"piranha.media.drag($event, item)\" v-on:dragover=\"piranha.media.dragover\" v-on:dragleave=\"piranha.media.dragleave\" v-on:drop=\"piranha.media.drop($event, item.id)\">\n        <i class=\"fas fa-folder\"></i>{{ item.name }}\n        <span class=\"badge badge-light float-right\">{{ item.mediaCount }}</span>\n    </a>\n    <form v-else v-on:submit.prevent=\"piranha.media.updateFolder()\" class=\"d-flex\">\n        <i class=\"fas fa-folder\"></i>\n        <input :id=\"'folder-' + item.id\" type=\"text\" v-on:keyup.esc=\"piranha.media.cancelEditFolder()\" v-model=\"piranha.media.currentFolderName\" class=\"form-control form-control-sm\">\n    </form>\n    <ol v-if=\"selected === item.id && piranha.media.isAdding\" class=\"dd-list\">\n        <form v-on:submit.prevent=\"piranha.media.addFolder()\" class=\"d-flex\">\n            <i class=\"fas fa-folder\"></i><input id=\"add-folder\" type=\"text\" v-on:keyup.esc=\"piranha.media.isAdding = false\" v-model=\"piranha.media.folder.name\" class=\"form-control form-control-sm\">\n        </form>\n    </ol>\n    <ol v-if=\"item.items.length > 0\" class=\"dd-list\">\n        <folder-item v-for=\"child in item.items\" v-bind:key=\"child.id\" v-bind:selected=\"selected\" v-bind:item=\"child\"></folder-item>\n    </ol>\n</li>\n"
});
/*global
    piranha
*/

piranha.media = new Vue({
    el: "#media",
    data: {
        loading: true,
        listView: true,
        currentFolder: null,
        currentFolderId: null,
        currentFolderName: null,
        parentFolderId: null,
        folders: [],
        items: [],
        structure: [],
        rootCount: null,
        totalCount: null,
        canDelete: false,
        isAdding: false,
        newFolderName: null,
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

            this.initFolders(result.structure);
            this.folders = result.folders;

            this.items = result.media.map(function (i) {
                i.selected = false
                return i;
            });

            this.structure = result.structure;
            this.rootCount = result.rootCount;
            this.totalCount = result.totalCount;
            this.canDelete = result.canDelete;
            this.listView = result.viewMode === "list";
        },
        initFolders: function (folders) {
            for (var n = 0; n < folders.length; n++) {
                folders[n].edit = false;

                if (folders[n].id === this.currentFolderId) {
                    this.currentFolder = folders[n];
                }

                if (folders[n].items.length > 0) {
                    this.initFolders(folders[n].items);
                }
            }
        },
        editFolder: function () {
            this.currentFolder.edit = true;

            this.$nextTick(function () {
                document.getElementById("folder-" + this.currentFolderId).focus();
            });
        },
        editAddFolder: function () {
            this.isAdding = true;

            this.$nextTick(function () {
                document.getElementById("add-folder").focus();
            });
        },
        cancelEditFolder: function () {
            this.currentFolder.edit = false;
            this.currentFolderName = this.currentFolder.name;
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

            var dropped = event.dataTransfer.getData("mediaId");
            if (dropped === folderId) {
                return;
            }

            var selections = this.items.filter(i => i.selected).map(i => i.id);
            if (!selections.includes(dropped)) {
                selections = [];
                selections.push(dropped);
            }

            fetch(piranha.baseUrl + "manager/api/media/move/" + (folderId || ""), {
                method: "POST",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(selections)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.type === "success") {
                    piranha.media.refresh();
                }

                if (result.status !== 400) {
                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                } else {
                    // Unauthorized request
                    piranha.notifications.unauthorized();
                }
            })
            .catch(function (error) { console.log("error:", error); });
        },
        onItemClick: function (event, media) {
            if (event.shiftKey) {
                media.selected = !media.selected;
            } else {
                piranha.preview.openItem(media);
            }
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
            this.saveFolder(null, null, {
                parentId: this.currentFolderId,
                name: this.folder.name
            });
            this.isAdding = false;
        },
        updateFolder: function () {
            this.saveFolder(null, null, {
                id: this.currentFolderId,
                name: this.currentFolderName
            });
        },
        saveFolder: function (modal, form, folder) {
            var self = this;

            // Validate form
            if (form != null) {
                var form = document.getElementById(form);
                if (form.checkValidity() === false) {
                    form.classList.add("was-validated");
                    return;
                }
            }

            fetch(piranha.baseUrl + "manager/api/media/folder/save", {
                method: "post",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(folder)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status.type === "success")
                {
                    // Close modal
                    if (modal != null) {
                        $(modal).modal('hide')
                    }

                    // Clear modal
                    self.folder.name = null;
                    self.items = result.media;

                    // Refresh
                    self.refresh();
                }

                if (result.status !== 400) {
                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                } else {
                    // Unauthorized request
                    piranha.notifications.unauthorized();
                }
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        remove: function (id) {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deleteMediaSelectionConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/media/delete", {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders(),
                        body: JSON.stringify([id])
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        // Refresh
                        self.refresh();

                        if (result.status !== 400) {
                            // Push status to notification hub
                            piranha.notifications.push(result);
                        } else {
                            // Unauthorized request
                            piranha.notifications.unauthorized();
                        }
                    })
                    .catch(function (error) { console.log("error:", error); });
                }
            });
        },
        removeSelection: function () {
            var self = this;
            var selections = this.items.filter(i => i.selected).map(i => i.id);

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deleteMediaSelectionConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/media/delete", {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders(),
                        body: JSON.stringify(selections)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        // Refresh
                        self.refresh();

                        if (result.status !== 400) {
                            // Push status to notification hub
                            piranha.notifications.push(result.status);
                        } else {
                            // Unauthorized request
                            piranha.notifications.unauthorized();
                        }
                    })
                    .catch(function (error) { console.log("error:", error); });
                }
            });
        },
        removeFolder: function (id) {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deleteMediaSelectionConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/media/folder/delete", {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders(),
                        body: JSON.stringify(id)
                    })
                        .then(function (response) { return response.json(); })
                        .then(function (result) {
                            self.bind(result);

                            history.pushState({ folderId: id }, "", piranha.baseUrl + "manager/media" + (id ? "/" + id : ""));
                            document.title = result.currentFolderName ? result.currentFolderName : "Media";

                            if (result.status !== 400) {
                                // Push status to notification hub
                                piranha.notifications.push(result.status);
                            } else {
                                // Unauthorized request
                                piranha.notifications.unauthorized();
                            }
                        })
                        .catch(function (error) { console.log("error:", error); });
                }
            });
        }
    },
    computed: {
        hasSelection: function () {
            return this.items.some(function (item) {
                return item.selected;
            });
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