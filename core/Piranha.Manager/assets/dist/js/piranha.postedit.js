/*global
    piranha
*/

piranha.postedit = new Vue({
    el: "#postedit",
    data: {
        loading: true,
        id: null,
        blogId: null,
        typeId: null,
        title: null,
        slug: null,
        metaKeywords: null,
        metaDescription: null,
        excerpt: null,
        published: null,
        redirectUrl: null,
        redirectType: null,
        enableComments: null,
        closeCommentsAfterDays: null,
        commentCount: null,
        pendingCommentCount: 0,
        state: "new",
        categories: [],
        tags: [],
        blocks: [],
        regions: [],
        editors: [],
        useBlocks: true,
        permissions: [],
        primaryImage: {
            id: null,
            media: null
        },
        selectedPermissions: [],
        saving: false,
        savingDraft: false,
        selectedRegion: {
            uid: "uid-blocks",
            name: null,
            icon: null,
        },
        selectedSetting: "uid-settings",
        selectedCategory: null,
        selectedTags: [],
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
                return piranha.utils.formatUrl(this.primaryImage.media.publicUrl);
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        },
        isExcerptEmpty: function () {
            return piranha.utils.isEmptyText(this.excerpt);
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
            this.blogId = model.blogId;
            this.typeId = model.typeId;
            this.title = model.title;
            this.slug = model.slug;
            this.metaKeywords = model.metaKeywords;
            this.metaDescription = model.metaDescription;
            this.excerpt = model.excerpt;
            this.published = model.published;
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
            this.categories = model.categories;
            this.tags = model.tags;
            this.useBlocks = model.useBlocks;
            this.selectedCategory = model.selectedCategory;
            this.selectedTags = model.selectedTags;
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
            }
        },
        load: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/post/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        create: function (id, postType) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/post/create/" + id + "/" + postType)
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
            this.saveInternal(piranha.baseUrl + "manager/api/post/save");
        },
        saveDraft: function ()
        {
            this.savingDraft = true;
            this.saveInternal(piranha.baseUrl + "manager/api/post/save/draft");
        },
        unpublish: function ()
        {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/post/save/unpublish");
        },
        saveInternal: function (route) {
            var self = this;

            var model = {
                id: self.id,
                blogId: self.blogId,
                typeId: self.typeId,
                primaryImage: {
                    id: self.primaryImage.id
                },
                title: self.title,
                slug: self.slug,
                metaKeywords: self.metaKeywords,
                metaDescription: self.metaDescription,
                excerpt: self.excerpt,
                published: self.published,
                redirectUrl: self.redirectUrl,
                redirectType: self.redirectType,
                enableComments: self.enableComments,
                closeCommentsAfterDays: self.closeCommentsAfterDays,
                blocks: JSON.parse(JSON.stringify(self.blocks)),
                regions: JSON.parse(JSON.stringify(self.regions)),
                selectedCategory: self.selectedCategory,
                selectedTags: JSON.parse(JSON.stringify(self.selectedTags)),
                selectedRoute: self.selectedRoute,
                selectedPermissions: self.selectedPermissions
            };

            fetch(route, {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(model)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                var oldState = self.state;

                self.id = result.id;
                self.slug = result.slug;
                self.published = result.published;
                self.state = result.state;
                self.selectedRoute = result.selectedRoute;

                if (oldState === 'new' && result.state !== 'new') {
                    window.history.replaceState({ state: "created"}, "Edit post", piranha.baseUrl + "manager/post/edit/" + result.id);
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

            fetch(piranha.baseUrl + "manager/api/post/revert/" + self.id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);

                    Vue.nextTick(function () {
                        $("#selectedCategory").select2({
                            tags: true,
                            selectOnClose: true,
                            placeholder: piranha.resources.texts.addCategory
                        });
                        $("#selectedTags").select2({
                            tags: true,
                            selectOnClose: true,
                            placeholder: piranha.resources.texts.addTags
                        });
                    });

                    piranha.notifications.push(result.status);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        remove: function () {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deletePostConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/post/delete/" + self.id)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        window.location = piranha.baseUrl + "manager/page/edit/" + self.blogId;
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
        },
        addBlock: function (type, pos) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.blocks.splice(pos, 0, result.body);
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
        },
        selectSetting: function (uid) {
            this.selectedSetting = uid;
        },
        isCommentsOpen: function () {
            var date = new Date(this.published);
            date = date.addDays(this.closeCommentsAfterDays);

            return date > new Date();
        },
        commentsClosedDate: function () {
            var date = new Date(this.published);
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
            this.excerpt = e.target.innerHTML;
        }
    },
    created: function () {
    },
    updated: function () {
        var self = this;

        if (this.loading)
        {
            sortable(".blocks", {
                handle: ".handle",
                items: ":not(.unsortable)"
            })[0].addEventListener("sortupdate", function (e) {
                self.moveBlock(e.detail.origin.index, e.detail.destination.index);
            });
            $("#selectedCategory").select2({
                tags: true,
                selectOnClose: true,
                placeholder: piranha.resources.texts.addCategory
            });
            $("#selectedCategory").on("change", function() {
                var item = $(this).find("option:selected").text();
                self.selectedCategory = item;
            });
            $("#selectedTags").select2({
                tags: true,
                selectOnClose: true,
                placeholder: piranha.resources.texts.addTags
            });
            $("#selectedTags").on("change", function() {
                var items = $(this).find("option:selected");
                self.selectedTags = [];
                for (var n = 0; n < items.length; n++) {
                    self.selectedTags.push(items[n].text);
                }
            });
        }
        else {
            sortable(".blocks", "disable");
            sortable(".blocks", "enable");
        }

        this.loading = false;
    },
    components: {
        datepicker: vuejsDatepicker
    }
});
