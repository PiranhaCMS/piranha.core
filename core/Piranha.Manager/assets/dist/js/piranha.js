
//
// Setting up a common event bus
// for all Vue apps in Piranha
//
Vue.prototype.eventBus = new Vue();
/*global
    piranha
*/

piranha.accessibility = new function () {
    "use strict";

    var self = this;

    self.focus = null;

    //
    // Removes the currently focused block
    //
    self.removeBlock = function (e) {
        var block = $(":focus").parents(".block");

        if (block.length === 1) {
            // Check if we have an active editor
            if (tinymce) {
                var editor = tinymce.activeEditor;

                if (editor && editor.inline) {
                    editor.destroy();
                }
            }

            // Remove focused block
            block.find(".block-remove").click();
        }
    };

    //
    // Store focus poing when modal is opened.
    //
    $(document).on("show.bs.modal", ".modal", function() {
        self.focus = $(":focus");
    });

    //
    // Restore previous focus point when modal i closed.
    //
    $(document).on("hidden.bs.modal", ".modal", function() {
        if (self.focus) {
            self.focus.focus();
            self.focus = null;
        }
    });

    //
    // Keyboard Shortcuts
    //
    $(window).on("keydown", function (e) {
        var menu = $(".main-nav");

        // Function key
        if (e.altKey) {
            // Pressed 'c', toggle content switcher
            if (e.keyCode === 67) {
                $("#contentSelector").click();
            }
            // Pressed backspace, check for focused block
            else if (e.keyCode === 8) {
                self.removeBlock(e);
            }
        }
    });
}

/*global
    piranha
*/

piranha.alert = new Vue({
    el: "#alert",
    data: {
        title: null,
        body: null,
        confirmCss: null,
        confirmIcon: null,
        confirmText: null,
        cancelCss: null,
        cancelIcon: null,
        cancelText: null,
        onConfirm: null,
        onCancel: null,
        verifyPhrase: null,
        verifyPlaceholder: null,
        verifyText: null,
        verifyInput: null,
    },
    methods: {
        open: function (options) {
            if (options) {
                this.title = options.title;
                this.body = options.body;
                this.confirmCss = options.confirmCss ? options.confirmCss : "btn-success";
                this.confirmIcon = options.confirmIcon;
                this.confirmText = options.confirmText ? options.confirmText : piranha.resources.texts.ok;
                this.cancelCss = options.cancelCss ? options.cancelCss : "btn-secondary";
                this.cancelIcon = options.cancelIcon;
                this.cancelText = options.cancelText ? options.cancelText : piranha.resources.texts.cancel;
                this.onConfirm = options.onConfirm;
                this.onCancel = options.onCancel;
                this.verifyPhrase = options.verifyPhrase;
                this.verifyPlaceholder = options.verifyPlaceholder;
                this.verifyText = options.verifyText;

                $("#alert").modal("show");
            }
        },
        confirm: function () {
            if (this.onConfirm) {
                this.onConfirm();
                this.clear();
            }
            $("#alert").modal("hide");
        },
        cancel: function () {
            if (this.onCancel) {
                this.onCancel();
                this.clear();
            }
            $("#alert").modal("hide");
        },
        canConfirm: function () {
            return !this.verifyPhrase || this.verifyPhrase === this.verifyInput;
        },
        clear: function () {
            this.onCancel = null;
            this.onConfirm = null;
            this.verifyInput = null;
        }
    }
});
/*global
    piranha
*/

piranha.archivepicker = new Vue({
    el: "#archivepicker",
    data: {
        search: '',
        sites: [],
        items: [],
        currentSiteId: null,
        currentSiteTitle: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredItems: function () {
            var self = this;

            return this.items.filter(function (item) {
                if (self.search.length > 0) {
                    return item.title.toLowerCase().indexOf(self.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        load: function (siteId) {
            var url = piranha.baseUrl + "manager/api/page/archives" + (siteId ? "/" + siteId : "");
            var self = this;

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.currentSiteId = result.siteId;
                    self.currentSiteTitle = result.siteTitle;
                    self.sites = result.sites;
                    self.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            this.load(piranha.archivepicker.currentSiteId);
        },
        open: function (callback, siteId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId);

            $("#archivepicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length == 1) {
                this.select(this.filteredItems[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#archivepicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#archivepicker").on("shown.bs.modal", function() {
        $("#archivepickerSearch").trigger("focus");
    });
});
// Prevent Dropzone from auto discoveringr all elements:
Dropzone.autoDiscover = false;

/*global
    piranha
*/

piranha.dropzone = new function () {
    var self = this;

    self.init = function (selector, options) {
        if (!options) options = {};

        var defaultOptions = {
            paramName: 'Uploads',
            url: piranha.baseUrl + "manager/api/media/upload",
            headers: piranha.utils.antiForgeryHeaders(false),
            thumbnailWidth: 70,
            thumbnailHeight: 70,
            previewsContainer: selector + " .media-list",
            previewTemplate: document.querySelector( "#media-upload-template").innerHTML,
            uploadMultiple: true,
            timeout: piranha.xhrTimeout * 1000,
            init: function () {
                var self = this;

                // Default addedfile callback
                if (!options.addedfile) {
                    options.addedfile = function (file) { }
                }

                // Default removedfile callback
                if (!options.removedfile) {
                    options.removedfile = function (file) { }
                }

                // Default error callback
                if (!options.error) {
                    options.error = function (file) { }
                }

                // Default complete callback
                if (!options.complete) {
                    options.complete = function (file) {
                        //console.log(file)
                        if (file.status !== "success" && file.xhr.responseText !== "" ) {
                            var response = JSON.parse(file.xhr.responseText);
                            file.previewElement.querySelector("[data-dz-errormessage]").innerText = response.body;
                        }
                    }
                }

                // Default queuecomplete callback
                if (!options.queuecomplete) {
                    options.queuecomplete = function () {}
                }

                self.on("addedfile", options.addedfile);
                self.on("removedfile", options.removedfile);
                self.on("complete", options.complete);
                self.on("queuecomplete", options.queuecomplete);
            }
        };

        var config = Object.assign(defaultOptions, options);

        return new Dropzone(selector + " form", config);
    }
};
/*global
    piranha
*/

piranha.permissions = {
    loaded: false,
    aliases: {
        edit: false,
        delete: false
    },
    comments: {
        approve: false,
        delete: false
    },
    media: {
        add: false,
        addFolder: false,
        delete: false,
        deleteFolder: false,
        edit: false
    },
    pages: {
        add: false,
        delete: false,
        edit: false,
        preview: false,
        publish: false,
        save: false
    },
    posts: {
        add: false,
        delete: false,
        edit: false,
        preview: false,
        publish: false,
        save: false
    },
    sites: {
        add: false,
        delete: false,
        edit: false,
        save: false
    },

    load: function (cb) {
        var self = this;

        if (!this.loaded) {
            fetch(piranha.baseUrl + "manager/api/permissions")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.aliases = result.aliases;
                    self.comments = result.comments;
                    self.media = result.media;
                    self.pages = result.pages;
                    self.posts = result.posts;
                    self.sites = result.sites;
                    self.loaded = true;

                    if (cb)
                        cb();
                })
                .catch(function (error) {
                    console.log("error:", error );

                    if (cb)
                        cb();
                });
        } else {
            if (cb)
                cb();
        }
    }
};
/*global
    piranha
*/

piranha.utils = {
    getOrigin() {
        return window.location.origin;
    },
    formatUrl: function (str) {
        return str.replace("~/", piranha.baseUrl);
    },
    isEmptyHtml: function (str) {
        return str == null || str.replace(/(<([^>]+)>)/ig,"").replace(/\s/g, "") == "" && str.indexOf("<img") === -1;
    },
    isEmptyText: function (str) {
        return str == null || str.replace(/\s/g, "") == "" || str.replace(/\s/g, "") == "<br>";
    },
    strLength: function (str) {
        return str != null ? str.length : 0;
    },
    antiForgery: function () {
        const cookies = document.cookie.split(";");
        for (let i = 0; i < cookies.length; i++) {
            let c = cookies[i].trim().split("=");
            if (c[0] === piranha.antiForgery.cookieName) {
                return c[1];
            }
        }
        return "";
    },
    antiForgeryHeaders: function (isJson) {
        var headers = {};

        if (isJson === undefined || isJson === true)
        {
            headers["Content-Type"] = "application/json";
        }
        headers[piranha.antiForgery.headerName] = piranha.utils.antiForgery();

        return headers;
    }    
};

Date.prototype.addDays = function(days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}
Date.prototype.toDateString = function(days) {
    var date = new Date(this.valueOf());

    return date.getFullYear() + "-" +
        String(date.getMonth() + 1).padStart(2, '0') + "-" +
        String(date.getDate()).padStart(2, '0');
}

$(document).ready(function () {
    $('.block-header .danger').hover(
        function() {
            $(this).closest('.block').addClass('danger');
        },
        function() {
            $(this).closest('.block').removeClass('danger');
        });

    $('form.validate').submit(
        function (e) {
            if ($(this).get(0).checkValidity() === false) {
                e.preventDefault();
                e.stopPropagation();

                $(this).addClass('was-validated');
            }
        }
    );
});

$(document).on('mouseenter', '.block-header .danger', function() {
    $(this).closest('.block').addClass('danger');
});

$(document).on('mouseleave', '.block-header .danger', function() {
    $(this).closest('.block').removeClass('danger');
});

$(document).on('shown.bs.collapse', '.collapse', function () {
	$(this).parent().addClass('active');
});

$(document).on('hide.bs.collapse', '.collapse', function () {
	$(this).parent().removeClass('active');
});

// Fix scroll prevention for multiple modals in bootstrap
$(document).on('hidden.bs.modal', '.modal', function () {
    $('.modal:visible').length && $(document.body).addClass('modal-open');
});

$(window).scroll(function () {
    var scroll = $(this).scrollTop();

    $(".app .component-toolbar").each(function () {
        var parent = $(this).parent();
        var parentTop = parent.offset().top;

        if (scroll > parentTop) {
            var parentHeight = parent.outerHeight();
            var bottom = parentTop + parentHeight;

            if (scroll > bottom) {
                $(this).css({ "top": parentHeight + "px" })
            } else {
                $(this).css({ "top": scroll - parentTop + "px" })
            }
        } else {
            $(this).removeAttr("style");
        }
    });
});
/*global
    piranha
*/

piranha.blockpicker = new Vue({
    el: "#blockpicker",
    data: {
        filter: "",
        categories: [],
        index: 0,
        callback: null
    },
    computed: {
        filteredCategories: function () {
            var self = this;
            return this.categories.filter(function (category) {
                var items = self.filterBlockTypes(category);

                if (items.length > 0) {
                    return true;
                }
                return false;
            });
        }
    },
    methods: {
        open: function (callback, index, parentType) {
            var self = this;

            var url = piranha.baseUrl + "manager/api/content/blocktypes";

            if (piranha.pageedit)
            {
                url += "/page/" + piranha.pageedit.typeId;
            }
            else if (piranha.postedit)
            {
                url += "/post/" + piranha.postedit.typeId;
            }

            fetch(url + (parentType != null ? "/" + parentType : ""))
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.typeCount > 1) {
                        // Several applicable block types, open modal
                        self.filter = "";
                        self.index = index;
                        self.callback = callback;
                        self.categories = result.categories;

                        $("#blockpicker").modal("show");
                    } else {
                        // There's only one valid block type, select it
                        callback(result.categories[0].items[0].type, index);
                    }
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        select: function (item) {
            this.callback(item.type, this.index);

            this.index = 0;
            this.callback = null;

            $("#blockpicker").modal("hide");
        },
        selectSingleItem: function () {
            var categories = this.filteredCategories;

            if (categories.length === 1) {
                var items = this.filterBlockTypes(categories[0]);

                if (items.length === 1) {
                    this.select(items[0]);
                }
            }
        },
        filterBlockTypes: function (category) {
            var self = this;
            return category.items.filter(function (item) {
                return item.name.toLowerCase().indexOf(self.filter.toLowerCase()) > -1;
            });
        }
    },
    created: function () {
    }
});

$(document).ready(function() {
    $("#blockpicker").on("shown.bs.modal", function() {
        $("#blockpickerSearch").trigger("focus");
    });
});
/*global
    piranha
*/

piranha.notifications = new Vue({
    el: "#notification-hub",
    data: {
        items: [],
    },
    methods: {
        unauthorized: function() {
            this.push({
                type: "danger",
                body: "Request sender could not be verified by the server.",
                hide: true
            });
        },
        push: function (notification) {

            notification.style = {
                visible: false,
                'notification-info': notification.type === "info",
                'notification-danger': notification.type === "danger",
                'notification-success': notification.type === "success",
                'notification-warning': notification.type === "warning"
            };

            piranha.notifications.items.push(notification);

            setTimeout(function () {
                notification.style.visible = true;

                if (notification.hide)
                {
                    setTimeout(function () {
                        notification.style.visible = false;

                        setTimeout(function () {
                            piranha.notifications.items.shift();
                        }, 200);
                    }, 5000);
                }
            }, 200);
        }
    }
});
/*global
    piranha
*/

piranha.contentpicker = new Vue({
    el: "#contentpicker",
    data: {
        search: '',
        groups: [],
        items: [],
        currentGroupId: null,
        currentGroupTitle: null,
        currentGroupIcon: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredItems: function () {
            var self = this;

            return this.items.filter(function (item) {
                if (self.search.length > 0) {
                    return item.title.toLowerCase().indexOf(self.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        bind: function (result, partial) {
            this.currentGroupId = result.group.id;
            this.currentGroupTitle = result.group.title;
            this.currentGroupIcon = result.group.icon;
            this.types = result.types;
            this.items = result.items.map(function (i) {
                var type = result.types.find(function (t) {
                    return t.id === i.typeId;
                });

                i.type = type.title || i.typeId;

                return i;
            });

            if (!partial)
            {
                // Only bind groups if this is a full reload
                this.groups = result.groups;
            }
        },
        load: function (groupId, partial) {
            var url = piranha.baseUrl + "manager/api/content/" + (groupId ? groupId + "/" : "") + "list";
            var self = this;

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result, partial);
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        loadGroups: function () {

        },
        refresh: function () {
            this.load(piranha.contentpicker.currentGroupId, true);
        },
        open: function (groupId, callback) {
            this.search = '';
            this.callback = callback;

            this.load(groupId);

            $("#contentpicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length == 1) {
                this.select(this.filteredItems[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#contentpicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#contentpicker").on("shown.bs.modal", function() {
        $("#contentpickerSearch").trigger("focus");
    });
});
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
/*global
    piranha
*/

piranha.pagepicker = new Vue({
    el: "#pagepicker",
    data: {
        search: '',
        sites: [],
        items: [],
        currentSiteId: null,
        currentSiteTitle: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredItems: function () {
            let self = this;
            
            if (self.search && self.search.length < 1) {
                return this.items;   
            }
            
            let items = Object.assign([], this.items);
            let searchTerm = self.search ? self.search.toLowerCase() : "";
            
            let filterRecursive = function(arr) {
                return arr.reduce(function(acc, item){
                    let newItem = Object.assign({}, item);
                    
                    if (newItem.items) {
                        newItem.items = filterRecursive(item.items);
                        newItem.isExpanded = newItem.items.length > 0;
                    }
                    
                    if (newItem.title && (newItem.title.toLowerCase().indexOf(searchTerm) > -1 || newItem.isExpanded)) {
                        acc.push(newItem);
                    }
                    
                    return acc;
                }, []);
            };
            
            return filterRecursive(items);
        }
    },
    methods: {
        load: function (siteId) {
            var url = piranha.baseUrl + "manager/api/page/sitemap" + (siteId ? "/" + siteId : "");
            var self = this;

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.currentSiteId = result.siteId;
                    self.currentSiteTitle = result.siteTitle;
                    self.sites = result.sites;
                    self.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            this.load(piranha.pagepicker.currentSiteId);
        },
        open: function (callback, siteId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId);

            $("#pagepicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length == 1) {
                this.select(this.filteredItems[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#pagepicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#pagepicker").on("shown.bs.modal", function() {
        $("#pagepickerSearch").trigger("focus");
    });
});
/*global
    piranha
*/

piranha.postpicker = new Vue({
    el: "#postpicker",
    data: {
        search: '',
        sites: [],
        archives: [],
        posts: [],
        currentSiteId: null,
        currentArchiveId: null,
        currentSiteTitle: null,
        currentArchiveTitle: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredPosts: function () {
            return this.posts.filter(function (post) {
                if (piranha.postpicker.search.length > 0) {
                    return post.title.toLowerCase().indexOf(piranha.postpicker.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        load: function (siteId, archiveId) {
            var url = piranha.baseUrl + "manager/api/post/modal";

            if (siteId) {
                url += "?siteId=" + siteId;
                if (archiveId) {
                    url += "&archiveId=" + archiveId;
                }
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.postpicker.sites = result.sites;
                    piranha.postpicker.archives = result.archives;
                    piranha.postpicker.posts = result.posts;

                    piranha.postpicker.currentSiteId = result.siteId;
                    piranha.postpicker.currentArchiveId = result.archiveId;

                    piranha.postpicker.currentSiteTitle = result.siteTitle;
                    piranha.postpicker.currentArchiveTitle = result.archiveTitle;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            this.load(this.currentSiteId, this.currentArchiveId);
        },
        open: function (callback, siteId, archiveId, currentPostId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId, archiveId);

            $("#postpicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredPosts.length == 1) {
                this.select(this.filteredPosts[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#postpicker").modal("hide");
        }
    }
});

$(document).ready(function() {
    $("#postpicker").on("shown.bs.modal", function() {
        $("#postpickerSearch").trigger("focus");
    });
});
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
                headers: piranha.utils.antiForgeryHeaders(),
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

/*global
    piranha
*/

piranha.languageedit = new Vue({
    el: "#languageedit",
    data: {
        loading: true,
        items: [],
        originalDefault: null,
        currentDefault: null,
        showDefaultInfo: false,
        currentDelete: null,
        showDeleteInfo: false,
    },
    methods: {
        bind: function (result) {
            for (var n = 0; n < result.items.length; n++)
            {
                result.items[n].errorTitle = false;

                if (result.items[n].isDefault) {
                    this.originalDefault = this.currentDefault = result.items[n];
                }
            }
            this.items = result.items;
            this.showDefaultInfo = false;
            this.showDeleteInfo = false;
            this.currentDelete = null;
            this.loading = false;

        },
        load: function () {
            var self = this;

            self.loading = true;
            fetch(piranha.baseUrl + "manager/api/language")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) {
                    console.log("error:", error );
                    self.loading = false;
                });
        },
        save: function () {
            // Validate form
            if (this.validate()) {
                var self = this;

                self.loading = true;
                fetch(piranha.baseUrl + "manager/api/language", {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify({
                        items: JSON.parse(JSON.stringify(self.items))
                    })
                })
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    if (result.status.type === "success") {
                        self.bind(result);
                    }
                    
                    if (result.status !== 400) {
                        // Refresh language list
                        self.refreshLanguageList();
                        // Push status to notification hub
                        piranha.notifications.push(result.status);
                    } else {
                        // Unauthorized request
                        piranha.notifications.unauthorized();
                        self.loading = false;
                    }
                })
                .catch(function (error) {
                    console.log("error:", error);
                });
            }
        },
        remove: function (item) {
            var self = this;

            self.loading = true;
            fetch(piranha.baseUrl + "manager/api/language/" + item.id, {
                method: "delete",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(item)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.status.type === "success") {
                    self.bind(result);
                }

                if (result.status !== 400) {
                    // Refresh language list
                    self.refreshLanguageList();
                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                } else {
                    // Unauthorized request
                    piranha.notifications.unauthorized();
                    self.loading = false;
                }
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        open: function () {
            this.load();
            $("#languageedit").modal("show");
        },
        close: function () {
            $("#languageedit").modal("hide");
        },
        addItem: function () {
            this.items.push({
                id: "00000000-0000-0000-0000-000000000000",
                title: "",
                culture: "",
                isDefault: false
            });
        },
        setDefault: function (item) {
            if (!item.isDefault) {
                for (var n = 0; n < this.items.length; n++) {
                    if (this.items[n].id != item.id) {
                        this.items[n].isDefault = false;
                    }
                }
                item.isDefault = true;
                this.currentDefault = item;
                if (this.originalDefault != item) {
                    this.showDefaultInfo = true;
                }
            }
        },
        setDefaultConfirm: function (item) {
            this.showDefaultInfo = false;
        },
        setDefaultCancel: function (items) {
            this.setDefault(this.originalDefault);
            this.currentDefault = this.originalDefault;
            this.showDefaultInfo = false;
        },
        removeItem: function (item) {
            this.currentDelete = item;
            this.showDeleteInfo = true;
        },
        removeConfirm: function () {
            this.remove(this.currentDelete);
        },
        removeCancel: function () {
            this.currentDelete = null;
            this.showDeleteInfo = false;
        },
        validate: function (item) {
            isValid = true;

            for (var n = 0; n < this.items.length; n++) {
                if (this.items[n].title === null || this.items[n].title === "")
                {
                    this.items[n].errorTitle = true;
                    isValid = false;
                }
                else
                {
                    this.items[n].errorTitle = false;
                }
                Vue.set(this.items, n, this.items[n]);
            }
            return isValid;
        },
        refreshLanguageList() {
            if (piranha.siteedit) {
                piranha.siteedit.refreshLanguageList();
            }
        }
    }
});
/*global
    piranha
*/

piranha.resources = new function() {
    "use strict";

    var self = this;

    this.texts = {};

    this.init = function (texts)
    {
        self.texts = texts;
    };
};
/*global
    piranha, tinymce
*/

piranha.editor = {
    editors:[],

    addInline: function (id, toolbarId) {
        console.log("No HTML editor registered.")
    },
    addInlineMarkdown: function (id, value, update) {
        var preview = $("#" + id).parent().find(".markdown-preview");
        var simplemde = new SimpleMDE({
            element: document.getElementById(id),
            status: false,
            spellChecker: true,
            hideIcons: ["preview", "guide"],
            initialValue: value,
            toolbar: [
                "bold", "italic", "heading", "|", "quote", "unordered-list", "ordered-list", "|", "link",
                {
                    name: "image",
                    action: function customFunction(editor) {
                        piranha.mediapicker.openCurrentFolder(function(media) {
                            var cm = editor.codemirror;
                            var active = simplemde.getState(cm).image;

                            var startPoint = cm.getCursor("start");
                            var endPoint = cm.getCursor("end");

                            if (active) {
                                text = cm.getLine(startPoint.line);
                                cm.replaceRange("![" + media.filename + "](" + media.publicUrl + ")",
                                    {
                                        line: startPoint.line,
                                        ch: 0
                                    });
                            } else {
                                cm.replaceSelection("![" + media.filename + "](" + media.publicUrl + ")");
                            }
                            cm.setSelection(startPoint, endPoint);
                            cm.focus();
                        }, "Image");
                    },
                    className: "fa fa-picture-o",
                    title: "Image"
                },
                "side-by-side", "fullscreen"
            ],
            renderingConfig: {
                singleLineBreaks: false
            }
        });
        simplemde.codemirror.on("change", function() {
            preview.html(simplemde.markdown(simplemde.value()));
            update(simplemde.value());
        });
        setTimeout(function() {
            preview.html(simplemde.markdown(simplemde.value()));
            simplemde.codemirror.refresh();
        }.bind(simplemde), 0);

        this.editors[id] = simplemde;
    },
    remove: function (id) {
        console.log("No HTML editor registered.")
    },
    removeMarkdown: function (id) {
        var simplemde = this.editors[id];

        if (simplemde != null) {
            var index = this.editors.indexOf(simplemde);

            simplemde.toTextArea();
            this.editors.splice[index, 1];
        }
    },
    refreshMarkdown: function () {
        for (var key in this.editors) {
            if (this.editors.hasOwnProperty(key)) {
                this.editors[key].codemirror.refresh();
            }
        }
    }
};

$(document).on('focusin', function (e) {
    if ($(e.target).closest(".tox-tinymce-inline").length) {
        e.stopImmediatePropagation();
    }
});
Vue.component("page-item", {
  props: ["item"],
  methods: {
    toggleItem: function (item) {
      item.isExpanded = !item.isExpanded;
    }
  },
  template: "\n<li :data-id=\"item.id\" class=\"dd-item\" :class=\"{ expanded: item.isExpanded || item.items.length === 0 }\">\n    <div class=\"sitemap-item expanded\">\n        <div class=\"link\">\n            <span class=\"actions\">\n                <a v-if=\"item.items.length > 0 && item.isExpanded\" v-on:click.prevent=\"toggleItem(item)\" class=\"expand\" href=\"#\"><i class=\"fas fa-minus\"></i></a>\n                <a v-if=\"item.items.length > 0 && !item.isExpanded\" v-on:click.prevent=\"toggleItem(item)\" class=\"expand\" href=\"#\"><i class=\"fas fa-plus\"></i></a>\n            </span>\n            <a href=\"#\" v-on:click.prevent=\"piranha.pagepicker.select(item)\">\n                {{ item.title }}\n            </a>\n        </div>\n        <div class=\"type d-none d-md-block\">\n            {{ item.typeName }}\n        </div>\n    </div>\n    <ol class=\"dd-list\" v-if=\"item.items.length > 0\">\n        <page-item v-for=\"child in item.items\" v-bind:key=\"child.id\" v-bind:item=\"child\">\n        </page-item>\n    </ol>\n</li>\n"
});