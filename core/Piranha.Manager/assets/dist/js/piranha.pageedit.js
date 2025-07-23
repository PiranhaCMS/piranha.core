/*global
    piranha
*/

piranha.pageedit = new Vue({
    el: "#pageedit",
    data: {
        loading: true,
        id: null,
        siteId: null,
        parentId: null,
        originalId: null,
        sortOrder: 0,
        typeId: null,
        title: null,
        navigationTitle: null,
        slug: null,
        metaTitle: null,
        metaKeywords: null,
        metaDescription: null,
        metaIndex: null,
        metaFollow: null,
        metaPriority: null,
        ogTitle: null,
        ogDescription: null,
        ogImage: {
            id: null,
            media: null
        },
        excerpt: null,
        isHidden: false,
        published: null,
        publishedTime: null,
        redirectUrl: null,
        redirectType: null,
        enableComments: null,
        closeCommentsAfterDays: null,
        commentCount: null,
        pendingCommentCount: 0,
        state: "new",
        blocks: [],
        regions: [],
        editors: [],
        useBlocks: true,
        usePrimaryImage: true,
        useExcerpt: true,
        useHtmlExcerpt: true,
        permissions: [],
        primaryImage: {
            id: null,
            media: null
        },
        selectedPermissions: [],
        isCopy: false,
        isScheduled: false,
        saving: false,
        savingDraft: false,
        selectedRegion: {
            uid: "uid-blocks",
            name: null,
            icon: null,
        },
        selectedSetting: "uid-settings",
        selectedRoute: null,
        routes: []
    },
    computed: {
        contentRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display != "setting" && item.meta.display != "hidden";
            });
        },
        settingRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display === "setting";
            });
        },
        primaryImageUrl: function () {
            if (this.primaryImage.media != null) {
                return piranha.utils.formatUrl("~/manager/api/media/url/" + this.primaryImage.id + "/448/200");
                //return piranha.utils.formatUrl(this.primaryImage.media.publicUrl);
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        },
        isExcerptEmpty: function () {
            return piranha.utils.isEmptyText(this.excerpt);
        },
        metaPriorityDescription: function() {
            var description = piranha.resources.texts.important;
            if (this.metaPriority <= 0.3)
                description = piranha.resources.texts.low;
            else if (this.metaPriority <= 0.6)
                description =  piranha.resources.texts.medium;
            else if (this.metaPriority <= 0.9)
                description =  piranha.resources.texts.high;

            return description += " (" + this.metaPriority + ")";
        }
    },
    mounted() {
        document.addEventListener("keydown", this.doHotKeys);
    },
    beforeDestroy() {
        document.removeEventListener("keydown", this.doHotKeys);
    },
    methods: {
        bind: function (model) {
            this.id = model.id;
            this.siteId = model.siteId;
            this.parentId = model.parentId;
            this.originalId = model.originalId;
            this.sortOrder = model.sortOrder;
            this.typeId = model.typeId;
            this.title = model.title;
            this.navigationTitle = model.navigationTitle;
            this.slug = model.slug;
            this.metaTitle = model.metaTitle;
            this.metaKeywords = model.metaKeywords;
            this.metaDescription = model.metaDescription;
            this.metaIndex = model.metaIndex;
            this.metaFollow = model.metaFollow;
            this.metaPriority = model.metaPriority;
            this.ogTitle = model.ogTitle;
            this.ogDescription = model.ogDescription;
            this.ogImage = model.ogImage;
            this.excerpt = model.excerpt;
            this.isHidden = model.isHidden;
            this.published = model.published;
            this.publishedTime = model.publishedTime;
            this.redirectUrl = model.redirectUrl;
            this.redirectType = model.redirectType;
            this.enableComments = model.enableComments;
            this.closeCommentsAfterDays = model.closeCommentsAfterDays;
            this.commentCount = model.commentCount;
            this.pendingCommentCount = model.pendingCommentCount;
            this.state = model.state;
            this.blocks = model.blocks;
            this.regions = model.regions;
            this.editors = model.editors;
            this.useBlocks = model.useBlocks;
            this.usePrimaryImage = model.usePrimaryImage;
            this.useExcerpt = model.useExcerpt;
            this.useHtmlExcerpt = model.useHtmlExcerpt;
            this.isCopy = model.isCopy;
            this.isScheduled = model.isScheduled;
            this.selectedRoute = model.selectedRoute;
            this.routes = model.routes;
            this.permissions = model.permissions;
            this.primaryImage = model.primaryImage;
            this.selectedPermissions = model.selectedPermissions;

            if (!this.useBlocks) {
                // First choice, select the first custom editor
                if (this.editors.length > 0) {
                    this.selectedRegion = this.editors[0];
                }

                // Second choice, select the first content region
                else if (this.contentRegions.length > 0) {
                    this.selectedRegion = this.contentRegions[0].meta;
                }
            } else {
                this.selectedRegion = {
                    uid: "uid-blocks",
                    name: null,
                    icon: null,
                };
            }
        },
        load: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        create: function (id, pageType) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/create/" + id + "/" + pageType)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        createrelative: function (id, pageType, after) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/createrelative/" + id + "/" + pageType + "/" + after)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        copy: function (source, siteId) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/copy/" + source + "/" + siteId)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        copyrelative: function (source, id, after) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/copyrelative/" + source + "/" + id + "/" + after)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        doHotKeys(e)
        {
            // CTRL + S
            if (e.keyCode === 83 && e.ctrlKey)
            {
                e.preventDefault();
                this.saveDraft();
            }
        },
        save: function ()
        {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/page/save");
        },
        saveDraft: function ()
        {
            this.savingDraft = true;
            this.saveInternal(piranha.baseUrl + "manager/api/page/save/draft");
        },
        unpublish: function ()
        {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/page/save/unpublish");
        },
        saveInternal: function (route) {
            var self = this;

            var model = {
                id: self.id,
                siteId: self.siteId,
                parentId: self.parentId,
                originalId: self.originalId,
                sortOrder: self.sortOrder,
                typeId: self.typeId,
                title: self.title,
                navigationTitle: self.navigationTitle,
                slug: self.slug,
                metaTitle: self.metaTitle,
                metaKeywords: self.metaKeywords,
                metaDescription: self.metaDescription,
                metaIndex: self.metaIndex,
                metaFollow: self.metaFollow,
                metaPriority: self.metaPriority,
                ogTitle: self.ogTitle,
                ogDescription: self.ogDescription,
                ogImage: {
                    id: self.ogImage.id
                },
                excerpt: self.excerpt,
                isHidden: self.isHidden,
                published: self.published,
                publishedTime: self.publishedTime,
                redirectUrl: self.redirectUrl,
                redirectType: self.redirectType,
                enableComments: self.enableComments,
                closeCommentsAfterDays: self.closeCommentsAfterDays,
                isCopy: self.isCopy,
                blocks: JSON.parse(JSON.stringify(self.blocks)),
                regions: JSON.parse(JSON.stringify(self.regions)),
                selectedRoute: self.selectedRoute,
                selectedPermissions: self.selectedPermissions,
                primaryImage: {
                    id: self.primaryImage.id
                },
            };

            fetch(route, {
                method: "post",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(model)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                var oldState = self.state;

                self.id = result.id;
                self.slug = result.slug;
                self.published = result.published;
                self.publishedTime = result.publishedTime;
                self.state = result.state;
                self.isCopy = result.isCopy;
                self.selectedRoute = result.selectedRoute;

                if (oldState === 'new' && result.state !== 'new') {
                    window.history.replaceState({ state: "created"}, "Edit page", piranha.baseUrl + "manager/page/edit/" + result.id);
                }
                piranha.notifications.push(result.status);

                self.saving = false;
                self.savingDraft = false;

                self.eventBus.$emit("onSaved", self.state)
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        revert: function () {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/revert", {
                method: "post",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(self.id)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.bind(result);

                piranha.notifications.push(result.status);
            })
            .catch(function (error) { 
                console.log("error:", error );
            });
        },
        detach: function () {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/detach", {
                method: "post",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(self.id)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.bind(result);

                piranha.notifications.push(result.status);
            })
            .catch(function (error) { 
                console.log("error:", error );
            });
        },
        remove: function () {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deletePageConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/page/delete", {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders(),
                        body: JSON.stringify(self.id)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        window.location = piranha.baseUrl + "manager/pages";
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
        },
        addBlock: function (type, pos) {
            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pageedit.blocks.splice(pos, 0, result.body);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        moveBlock: function (from, to) {
            this.blocks.splice(to, 0, this.blocks.splice(from, 1)[0])
        },
        collapseBlock: function (block) {
            block.meta.isCollapsed = !block.meta.isCollapsed;
        },
        removeBlock: function (block) {
            var index = this.blocks.indexOf(block);

            if (index !== -1) {
                this.blocks.splice(index, 1);
            }
        },
        updateBlockTitle: function (e) {
            for (var n = 0; n < this.blocks.length; n++) {
                if (this.blocks[n].meta.uid === e.uid) {
                    this.blocks[n].meta.title = e.title;
                    break;
                }
            }
        },
        selectRegion: function (region) {
            this.selectedRegion = region;
            Vue.nextTick(function () {
                piranha.editor.refreshMarkdown();
            });
        },
        selectSetting: function (uid) {
            this.selectedSetting = uid;
            Vue.nextTick(function () {
                piranha.editor.refreshMarkdown();
            });
        },
        isCommentsOpen: function () {
            var date = new Date(this.published + " " + this.publishedTime);
            date = date.addDays(this.closeCommentsAfterDays);

            return date > new Date();
        },
        commentsClosedDate: function () {
            var date = new Date(this.published + " " + this.publishedTime);
            date = date.addDays(this.closeCommentsAfterDays);

            return date.toDateString();
        },
        selectPrimaryImage: function () {
            if (this.primaryImage.media !== null) {
                piranha.mediapicker.open(this.updatePrimaryImage, "Image", this.primaryImage.media.folderId);
            } else {
                piranha.mediapicker.openCurrentFolder(this.updatePrimaryImage, "Image");
            }
        },
        removePrimaryImage: function () {
            this.primaryImage.id = null;
            this.primaryImage.media = null;
        },
        updatePrimaryImage: function (media) {
            if (media.type === "Image") {
                this.primaryImage.id = media.id;
                this.primaryImage.media = media;
            } else {
                console.log("No image was selected");
            }
        },
        onExcerptBlur: function (e) {
            if (this.useHtmlExcerpt) {
                this.excerpt = tinyMCE.activeEditor.getContent();
            } else {
                this.excerpt = e.target.innerHTML;
            }
        }
    },
    created: function () {
    },
    updated: function () {
        if (this.loading)
        {
            sortable("#content-blocks", {
                handle: ".handle",
                items: ":not(.unsortable)"
            })[0].addEventListener("sortupdate", function (e) {
                piranha.pageedit.moveBlock(e.detail.origin.index, e.detail.destination.index);
            });
            piranha.editor.addInline('excerpt-body', 'excerpt-toolbar');
        }
        else {
            sortable("#content-blocks", "disable");
            sortable("#content-blocks", "enable");
        }

        this.loading = false;
    },
    components: {
        datepicker: vuejsDatepicker
    }
});
