Vue.component("content-nav", {
  props: ["groupid", "contentid"],
  data: function () {
    return {
      id: null,
      loading: true,
      filter: null,
      group: null,
      items: [],
      selectedItem: null
    };
  },
  computed: {
    filteredItems: function () {
      var self = this;
      if (!this.filter || this.filter === "") return this.items;
      var lcFilter = this.filter.toLowerCase();
      return this.items.filter(function (i) {
        return i.title.toLowerCase().indexOf(lcFilter) !== -1;
      });
    }
  },
  methods: {
    load: function () {
      self = this;
      self.loading = true;

      if (this.id === null) {
        this.id = this.contentid;
      }

      var route = "";

      if (this.id !== null) {
        route = piranha.baseUrl + "manager/api/content/" + this.id + "/listbyid";
      } else {
        route = piranha.baseUrl + "manager/api/content/" + this.groupid + "/list";
      }

      fetch(route).then(function (response) {
        return response.json();
      }).then(function (result) {
        self.group = result.group;
        self.items = result.items.map(function (i) {
          var type = result.types.find(function (t) {
            return t.id === i.typeId;
          });
          i.type = type.title || i.typeId;

          if (i.id === self.id) {
            self.selectedItem = i;
          }

          return i;
        });
        self.loading = false;
      }).catch(function (error) {
        console.log("error:", error);
      });
    },
    select: function (item) {
      history.pushState({
        id: item.id
      }, "", piranha.baseUrl + "manager/content/edit/" + item.id);
      piranha.content.load("content", item.id);
      this.selectedItem = item;
    },
    onSaved: function () {
      this.id = piranha.content.id;
      this.load();
    }
  },
  mounted: function () {
    this.load();
    this.eventBus.$on("onSaved", this.onSaved);
  },
  beforeDestroy: function () {
    this.eventBus.$off("onSaved");
  },
  template: "\n<div class=\"col side-nav\">\n    <ul class=\"list-group list-group-flush list-content app\" :class=\"{ ready: !loading }\">\n        <li class=\"list-group-item header\">\n            <div class=\"input-group\">\n                <input v-model=\"filter\" type=\"text\" class=\"form-control\" :placeholder=\"piranha.resources.texts.search\">\n                <div class=\"input-group-append\">\n                    <button class=\"btn btn-primary btn-icon\">\n                        <i class=\"fas fa-plus\"></i>\n                    </button>\n                </div>\n            </div>\n        </li>\n        <li v-for=\"item in filteredItems\" v-bind:key=\"item.id\" class=\"list-group-item\" :class=\"{ active: item === selectedItem}\">\n            <a v-on:click.prevent=\"select(item)\" href=\"#\">\n                <img v-if=\"group.listImage\" class=\"float-left\" :src=\"piranha.utils.formatUrl(item.imageUrl)\" :alt=\"item.title\">\n                <span>{{ item.title }}</span>\n                <span>{{ item.type }}</span>\n            </a>\n        </li>\n    </ul>\n</div>\n"
});
/*global
    piranha
*/

piranha.content = new Vue({
    el: "#content",
    data: {
        contentType: null,
        onSave: null,
        prevId: null,

        // Features
        features: {
            useAltTitle: false,
            useBlocks: false,
            useCategory: false,
            useComments: false,
            usePrimaryImage: false,
            useExcerpt: false,
            useHtmlExcerpt: false,
            usePublish: false,
            useTags: false,
            useTranslations: false
        },

        // Model data
        id: null,
        languageId: null,
        languageTitle: null,
        parentId: null,
        typeId: null,
        typeTitle: null,
        groupId: null,
        groupTitle: null,
        position: null,
        meta: null,
        comments: null,
        permissions: {
            selectedPermissions: [],
            permissions: []
        },
        routes: null,
        taxonomies: null,
        title: null,
        altTitle: null,
        slug: null,
        primaryImage: null,
        excerpt: null,
        state: null,
        isReadOnly: false,
        isScheduled: false,
        published: null,
        publishedTime: null,

        blocks: [],
        editors: [],
        regions: [],
        languages: [],
        sections: [],

        // State members
        loading: true,
        saving: false,
        savingDraft: false,
        selectedRegion: "",
        selectedSetting: "uid-settings"
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
        isPreviewActive: function () {
            return !this.features.useBlocks &&
                this.regions.length === 0 &&
                this.editors.length === 0;
        },
        isSettingsActive: function () {
            return !this.isPreviewActive ||
                this.meta !== null ||
                this.features.usePublish ||
                this.features.useAltTitle;
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
    methods: {
        reset: function (contentType) {
            // Set the content type
            this.contentType = contentType;

            // First let's update the features
            this.features.useAltTitle = false;
            this.features.useBlocks = false;
            this.features.useCategory = false;
            this.features.useComments = false;
            this.features.usePrimaryImage = false;
            this.features.useExcerpt = false;
            this.features.useHtmlExcerpt = false;
            this.features.usePublish = false;
            this.features.useTags = false;
            this.features.useTranslations = false;

            // Now let's update the rest of the model
            this.id = null;
            this.languageId = null;
            this.languageTitle = null;
            this.parentId = null;
            this.typeId = null;
            this.typeTitle = null;
            this.groupId = null;
            this.groupTitle = null;
            this.position = null;
            this.meta = null;
            this.comments = null;
            this.permissions = null;
            this.routes = null;
            this.taxonomies = null;
            this.title = null;
            this.altTitle = null;
            this.slug = null;
            this.primaryImage = null;
            this.excerpt = null;
            this.state = null;
            this.isReadOnly = false;
            this.isScheduled = false;
            this.published = null;
            this.publishedTime = null;

            this.editors = [];
            this.regions = [];
            this.languages = [];
            this.sections = [];
        },
        bind: function (model) {
            var self = this;

            // First let's update the features
            this.features.useAltTitle = model.features.useAltTitle;
            this.features.useBlocks = model.features.useBlocks;
            this.features.useCategory = model.features.useCategory;
            this.features.useComments = model.features.useComments;
            this.features.usePrimaryImage = model.features.usePrimaryImage;
            this.features.useExcerpt = model.features.useExcerpt;
            this.features.useHtmlExcerpt = model.features.useHtmlExcerpt;
            this.features.usePublish = model.features.usePublish;
            this.features.useTags = model.features.useTags;
            this.features.useTranslations = model.features.useTranslations;

            // Now let's update the rest of the model
            this.id = model.id;
            this.languageId = model.languageId;
            this.languageTitle = model.languageTitle;
            this.parentId = model.parentId;
            this.typeId = model.typeId;
            this.typeTitle = model.typeTitle;
            this.groupId = model.groupId;
            this.groupTitle = model.groupTitle;
            this.position = model.position;
            this.meta = model.meta;
            this.comments = model.comments;
            this.permissions = model.permissions;
            this.routes = model.routes;
            this.taxonomies = model.taxonomies;
            this.title = model.title;
            this.altTitle = model.altTitle;
            this.slug = model.slug;
            this.primaryImage = model.primaryImage;
            this.excerpt = model.excerpt;
            this.state = model.state;
            this.isReadOnly = model.isReadOnly;
            this.isScheduled = model.isScheduled;
            this.published = model.published;
            this.publishedTime = model.publishedTime;

            this.editors = model.editors;
            this.regions = model.regions;
            this.languages = model.languages;
            this.sections = model.sections;

            if (this.prevId === null || this.prevId !== this.id) {
                if (this.sections.length === 0) {
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
                        //uid: "uid-blocks",
                        uid: this.sections[0].uid,
                        name: null,
                        icon: null,
                    };
                }
            }

            this.prevId = this.id;

            // Initialize UI components
            Vue.nextTick(function () {
                if (self.features.useCategory)
                {
                    $("#selectedCategory").select2({
                        tags: true,
                        selectOnClose: true,
                        placeholder: piranha.resources.texts.addCategory
                    });
                    $("#selectedCategory").on("change", function() {
                        var item = $(this).find("option:selected").text();
                        self.taxonomies.selectedCategory = item;
                    });
                }
                if (self.features.useTags)
                {
                    $("#selectedTags").select2({
                        tags: true,
                        selectOnClose: false,
                        placeholder: piranha.resources.texts.addTags
                    });
                    $("#selectedTags").on("change", function() {
                        var items = $(this).find("option:selected");
                        self.taxonomies.selectedTags = [];
                        for (var n = 0; n < items.length; n++) {
                            self.taxonomies.selectedTags.push(items[n].text);
                        }
                    });
                }

                // Loading is finished
                self.loading = false;
            });
        },
        load: function (contentType, id, languageId) {
            var self = this;

            this.loading = true;
            this.contentType = contentType;

            var url = piranha.baseUrl + "manager/api/labs/" + self.contentType + "/" + id;
            if (languageId != null) {
                url += "/" + languageId;
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.reset(contentType);
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        save: function () {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/labs/" + this.contentType);
        },
        saveDraft: function () {
            this.savingDraft = true;
            this.saveInternal(piranha.baseUrl + "manager/api/labs/" + this.contentType + "/draft");
        },
        revert: function () {
            var self = this;
            self.savingDraft = true;

            fetch(piranha.baseUrl + "manager/api/labs/" + this.contentType + "/revert", {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(self.id)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.bind(result);

                piranha.notifications.push(result.status);

                self.saving = false;
                self.savingDraft = false;

                self.eventBus.$emit("onSaved", self.state);
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        unpublish: function () {
            var self = this;
            self.saving = true;

            fetch(piranha.baseUrl + "manager/api/labs/" + this.contentType + "/unpublish", {
                method: "post",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(self.id)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.bind(result);
                self.saving = false;
                self.savingDraft = false;

                self.eventBus.$emit("onSaved", self.state);
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        saveInternal: function (route) {
            var self = this;

            var model = {
                id: self.id,
                languageId: self.languageId,
                parentId: self.parentId,
                typeId: self.typeId,
                groupId: self.groupId,
                position: JSON.parse(JSON.stringify(self.position)),
                meta: JSON.parse(JSON.stringify(self.meta)),
                permissions: {
                    selectedPermissions: self.permissions.selectedPermissions
                },
                routes: JSON.parse(JSON.stringify(self.routes)),
                taxonomies: JSON.parse(JSON.stringify(self.taxonomies)),
                title: self.title,
                altTitle: self.altTitle,
                slug: self.slug,
                primaryImage: {
                    id: self.primaryImage.id
                },
                excerpt: self.excerpt,
                published: self.published,
                publishedTime: self.publishedTime,

                regions: JSON.parse(JSON.stringify(self.regions)),
                sections: JSON.parse(JSON.stringify(self.sections))
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
                self.bind(result);

                piranha.notifications.push(result.status);

                self.saving = false;
                self.savingDraft = false;

                self.eventBus.$emit("onSaved", self.state);
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        create: function (id, type) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/" + self.contentType + "/create/" + id + "/" + pageType)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        remove: function () {
            self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: 'Are you sure you want to delete "' + self.title + '"',
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    self.loading = true;

                    fetch(piranha.baseUrl + "manager/api/labs/" + self.contentType, {
                        method: "delete",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify(self.id)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        self.reset();
                        self.eventBus.$emit("onSaved", self.state);
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
            if (this.features.useHtmlExcerpt) {
                this.excerpt = tinyMCE.activeEditor.getContent();
            } else {
                this.excerpt = e.target.innerHTML;
            }
        }
    },
    components: {
        datepicker: vuejsDatepicker
    }
});