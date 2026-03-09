/*global
    piranha
*/

piranha.mediapicker = new Vue({
    el: "#mediapicker",
    data: {
        search: '',
        folderName: '',
        listView: true,
        currentFolderId: null,
        parentFolderId: null,
        currentDocumentFolderId: null,
        parentDocumentFolderId: null,
        currentImageFolderId: null,
        parentImageFolderId: null,
        currentVideoFolderId: null,
        parentVideoFolderId: null,
        currentFolderBreadcrumb: null,
        folders: [],
        items: [],
        folder: {
            name: null
        },
        filter: null,
        callback: null,
        dropzone: null
    },
    computed: {
        filteredFolders: function () {
            return this.folders.filter(function (item) {
                if (piranha.mediapicker.search.length > 0) {
                    return item.name.toLowerCase().indexOf(piranha.mediapicker.search.toLowerCase()) > -1
                }
                return true;
            });
        },
        filteredItems: function () {
            return this.items.filter(function (item) {
                if (piranha.mediapicker.search.length > 0) {
                    return item.filename.toLowerCase().indexOf(piranha.mediapicker.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        toggle: function () {
            this.listView = !this.listView;
        },
        load: function (id) {
            var self = this;

            var url = piranha.baseUrl + "manager/api/media/list" + (id ? "/" + id : "")+"/?width=210&height=160";
            if (self.filter) {
                url += "&filter=" + self.filter;
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.currentFolderId = result.currentFolderId;
                    self.parentFolderId = result.parentFolderId;
                    self.folders = result.folders;
                    self.items = result.media;
                    self.listView = result.viewMode === "list";
                    self.search = "";
                    self.currentFolderBreadcrumb = result.currentFolderBreadcrumb;

                    //set current folder for filter
                    if (self.filter) {
                        switch (self.filter.toLowerCase()) {
                            case "document":
                                self.currentDocumentFolderId = result.currentFolderId;
                                self.parentDocumentFolderId = result.parentFolderId;
                                break;
                            case "image":
                                self.currentImageFolderId = result.currentFolderId;
                                self.parentImageFolderId = result.parentFolderId;
                                break;
                            case "video":
                                self.currentVideoFolderId = result.currentFolderId;
                                self.parentVideoFolderId = result.parentFolderId;
                                break;
                        }
                    }
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        getThumbnailUrl: function (item) {
            return item.altVersionUrl !== null ? item.altVersionUrl : piranha.baseUrl + "manager/api/media/url/" + item.id + "/210/160";
        },
        refresh: function () {
            piranha.mediapicker.load(piranha.mediapicker.currentFolderId);
        },
        open: function (callback, filter, folderId) {
            this.search = '';
            this.callback = callback;
            this.filter = filter;

            this.load(folderId);

            $("#mediapicker").modal("show");
        },
        openCurrentFolder: function (callback, filter) {
            this.callback = callback;
            this.filter = filter;

            var folderId = this.currentFolderId;
            if (filter) {
                switch (filter.toLowerCase()) {
                    case "document":
                        folderId = this.currentDocumentFolderId? this.currentDocumentFolderId : folderId;
                        break;
                    case "image":
                        folderId = this.currentImageFolderId ? this.currentImageFolderId : folderId;
                        break;
                    case "video":
                        folderId = this.currentVideoFolderId ? this.currentVideoFolderId : folderId;
                        break;
                }
            }
            
            this.load(folderId);

            $("#mediapicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length === 0 && this.filteredFolders.length === 1) {
                this.load(this.filteredFolders[0].id);
                this.search = "";
            }

            if (this.filteredItems.length === 1 && this.filteredFolders.length === 0) {
                this.select(this.filteredItems[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#mediapicker").modal("hide");
        },
        savefolder: function () {
            var self = this;

            if (self.folderName !== "") {
                fetch(piranha.baseUrl + "manager/api/media/folder/save" + (self.filter ? "?filter=" + self.filter : ""), {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify({
                        parentId: self.currentFolderId,
                        name: self.folderName
                    })
                })
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.status.type === "success")
                    {
                        // Clear input
                        self.folderName = null;

                        self.folders = result.folders;
                        self.items = result.media;
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
            }
        }
    },
    mounted: function () {
        var self = this;
        piranha.permissions.load(function () {
            if (piranha.permissions.media.add) {
                self.dropzone = piranha.dropzone.init("#mediapicker-upload-container");
                self.dropzone.on("complete", function (file) {
                    if (file.status === "success") {
                        setTimeout(function () {
                            self.dropzone.removeFile(file);
                        }, 3000)
                    }
                });
                self.dropzone.on("queuecomplete", function () {
                    piranha.mediapicker.refresh();
                });
            }
        });
    }
});

$(document).ready(function() {
    $("#mediapicker").on("shown.bs.modal", function() {
        $("#mediapickerSearch").trigger("focus");
    });
});