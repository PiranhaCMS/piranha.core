/*global
    piranha
*/

piranha.contentedit = new Vue({
    el: "#contentedit",
    data: {
        loading: true,
        id: null,
        languageId: null,
        typeId: null,
        typeTitle: null,
        groupId: null,
        groupTitle: null,
        title: null,
        excerpt: null,
        state: "new",
        regions: [],
        editors: [],
        useCategory: false,
        useTags: false,
        usePrimaryImage: true,
        useExcerpt: true,
        useHtmlExcerpt: true,
        useTranslations: false,
        permissions: [],
        languages: [],
        primaryImage: {
            id: null,
            media: null
        },
        selectedPermissions: [],
        saving: false,
        selectedRegion: "",
        selectedSetting: "uid-settings",
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
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        },
        isExcerptEmpty: function () {
            return piranha.utils.isEmptyText(this.excerpt);
        },
        currentLanguage: function () {
            if (this.languages === null)
            {
                return {
                    id: "",
                    title: ""
                };
            }
            else
            {
                var self = this;
                return self.languages.find(function (l) {
                    return l.id === self.languageId;
                });
            }
        }
    },
    mounted() {
        //document.addEventListener("keydown", this.doHotKeys);
    },
    beforeDestroy() {
        //document.removeEventListener("keydown", this.doHotKeys);
    },
    methods: {
        bind: function (model) {
            this.id = model.id;
            this.languageId = model.languageId;
            this.typeId = model.typeId;
            this.typeTitle = model.typeTitle;
            this.groupId = model.groupId;
            this.groupTitle = model.groupTitle;
            this.title = model.title;
            this.excerpt = model.excerpt;
            this.state = model.state;
            this.regions = model.regions;
            this.editors = model.editors;
            this.languages = model.languages;
            this.useCategory = model.useCategory;
            this.useTags = model.useTags;
            this.usePrimaryImage = model.usePrimaryImage;
            this.useExcerpt = model.useExcerpt;
            this.useHtmlExcerpt = model.useHtmlExcerpt;
            this.useTranslations = model.useTranslations;
            this.permissions = model.permissions;
            this.primaryImage = model.primaryImage;
            this.selectedPermissions = model.selectedPermissions;

            // First choice, select the first custom editor
            if (this.editors.length > 0) {
                this.selectedRegion = this.editors[0];
            }
            // Second choice, select the first content region
            else if (this.contentRegions.length > 0) {
                this.selectedRegion = this.contentRegions[0].meta;
            }
        },
        load: function (id, languageId) {
            var self = this;

            var url = piranha.baseUrl + "manager/api/content/" + id;
            if (languageId != null) {
                url += "/" + languageId;
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        create: function (contentType) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/content/create/" + contentType)
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
                this.save();
            }
        },
        save: function ()
        {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/content/save");
        },
        saveInternal: function (route) {
            var self = this;

            var model = {
                id: self.id,
                languageId: self.languageId,
                typeId: self.typeId,
                title: self.title,
                excerpt: self.excerpt,
                regions: JSON.parse(JSON.stringify(self.regions)),
                selectedRoute: self.selectedRoute,
                selectedPermissions: self.selectedPermissions,
                primaryImage: {
                    id: self.primaryImage.id
                },
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
                self.state = result.state;
                self.selectedRoute = result.selectedRoute;

                if (oldState === 'new' && result.state !== 'new') {
                    window.history.replaceState({ state: "created"}, "Edit content", piranha.baseUrl + "manager/content/edit/" + result.typeId + "/" + result.id);
                }
                piranha.notifications.push(result.status);

                self.saving = false;

                self.eventBus.$emit("onSaved", self.state)
            })
            .catch(function (error) {
                console.log("error:", error);
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
                    var groupId = self.groupId;

                    fetch(piranha.baseUrl + "manager/api/content/delete/" + self.id)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        window.location = piranha.baseUrl + "manager/content/" + groupId;
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
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
            this.excerpt = tinyMCE.activeEditor.getContent();
        }
    },
    created: function () {
    },
    updated: function () {
        this.loading = false;
    },
    components: {
        datepicker: vuejsDatepicker
    }
});
