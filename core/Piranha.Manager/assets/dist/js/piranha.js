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
            thumbnailWidth: 70,
            thumbnailHeight: 70,
            previewsContainer: selector + " .media-list",
            previewTemplate: document.querySelector( "#media-upload-template").innerHTML,
            uploadMultiple: true,
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
    formatUrl: function (str) {
        return str.replace("~/", piranha.baseUrl);
    },
    isEmptyHtml: function (str) {
        return str == null || str.replace(/(<([^>]+)>)/ig,"").replace(/\s/g, "") == "" && str.indexOf("<img") === -1;
    },
    isEmptyText: function (str) {
        return str == null || str.replace(/\s/g, "") == "" || str.replace(/\s/g, "") == "<br>";
    }
};

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

            fetch(piranha.baseUrl + "manager/api/content/blocktypes" + (parentType != null ? "/" + parentType : ""))
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

piranha.mediapicker = new Vue({
    el: "#mediapicker",
    data: {
        search: '',
        folderName: '',
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
            if (this.filter) {
                url += "&filter=" + this.filter;
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

            this.load(this.currentFolderId);

            $("#mediapicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredItems.length == 0 && this.filteredFolders.length == 1) {
                this.load(this.filteredFolders[0].id);
                this.search = "";
            }

            if (this.filteredItems.length == 1 && this.filteredFolders.length == 0) {
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
                    headers: {
                        "Content-Type": "application/json",
                    },
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

                    // Push status to notification hub
                    piranha.notifications.push(result.status);
                })
                .catch(function (error) {
                    console.log("error:", error);
                });
            }
        }
    },
    mounted: function () {
        piranha.permissions.load(function () {
            if (piranha.permissions.media.add) {
                this.dropzone = piranha.dropzone.init("#mediapicker-upload-container");
                this.dropzone.on("complete", function (file) {
                    if (file.status === "success") {
                        setTimeout(function () {
                            piranha.mediapicker.dropzone.removeFile(file);
                        }, 3000)
                    }
                });
                this.dropzone.on("queuecomplete", function () {
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
            fetch(piranha.baseUrl + "manager/api/media/" + mediaId)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.preview.media = result;
                })
                .catch(function (error) { console.log("error:", error ); });
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
            spellChecker: false,
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
/*global
    piranha
*/

Vue.component("page-item", {
    props: ["item"],
    methods: {
        toggleItem: function (item) {
            item.isExpanded = !item.isExpanded;
        }
    },
    template:
        "<li :data-id='item.id' class='dd-item' :class='{ expanded: item.isExpanded || item.items.length === 0 }'>" +
        "  <div class='sitemap-item expanded'>" +
        "    <div class='link'>" +
        "      <span class='actions'>" +
        "        <a v-if='item.items.length > 0 && item.isExpanded' v-on:click.prevent='toggleItem(item)' class='expand' href='#'><i class='fas fa-minus'></i></a>" +
        "        <a v-if='item.items.length > 0 && !item.isExpanded' v-on:click.prevent='toggleItem(item)' class='expand' href='#'><i class='fas fa-plus'></i></a>" +
        "      </span>" +
        "      <a href='#' v-on:click.prevent='piranha.pagepicker.select(item)'>" +
        "        {{ item.title }}" +
        "      </a>" +
        "    </div>" +
        "    <div class='type d-none d-md-block'>" +
        "      {{ item.typeName }}" +
        "    </div>" +
        "  </div>" +
        "  <ol class='dd-list' v-if='item.items.length > 0' class='dd-list'>" +
        "    <page-item v-for='child in item.items' v-bind:key='child.id' v-bind:item='child'>" +
        "    </page-item>" +
        "  </ol>" +
        "</li>"
});
