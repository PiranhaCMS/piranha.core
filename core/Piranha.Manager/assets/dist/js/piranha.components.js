Vue.component("region", {
  props: ["model", "content", "type"],
  data: function () {
    return {
      itemAdded: false
    };
  },
  methods: {
    moveItem: function (from, to) {
      this.model.items.splice(to, 0, this.model.items.splice(from, 1)[0]);
    },
    addItem: function () {
      var self = this;
      fetch(piranha.baseUrl + "manager/api/content/region/" + this.content + "/" + this.type + "/" + this.model.meta.id).then(function (response) {
        return response.json();
      }).then(function (result) {
        self.model.items.push(result);
        self.itemAdded = true;
      }).catch(function (error) {
        console.log("error:", error);
      });
    },
    removeItem: function (item) {
      this.model.items.splice(this.model.items.indexOf(item), 1);
    },
    updateTitle: function (e) {
      var self = this;

      if (self.model.meta.isCollection) {
        for (var n = 0; n < self.model.items.length; n++) {
          var item = self.model.items[n];

          for (var m = 0; m < item.fields.length; m++) {
            var field = item.fields[m];

            if (field.meta.uid === e.uid) {
              self.model.items[n].title = e.title;
              break;
            }
          }
        }
      }
    }
  },
  mounted: function () {
    if (this.model.meta.isCollection) {
      var self = this;
      sortable("#" + this.model.meta.uid, {
        handle: '.card-header a:first-child',
        items: ":not(.unsortable)"
      })[0].addEventListener("sortupdate", function (e) {
        self.moveItem(e.detail.origin.index, e.detail.destination.index);
      });
    }
  },
  updated: function () {
    if (this.model.meta.isCollection && this.itemAdded) {
      sortable("#" + this.model.meta.uid, "disable");
      sortable("#" + this.model.meta.uid, "enable");

      if (!this.model.meta.expanded) {
        $("#" + this.model.meta.uid + " .card:last-child .card-header > a").click();
      }

      this.itemAdded = false;
    }
  },
  template: "\n<div class=\"row\" v-if=\"!model.meta.isCollection\">\n    <div class=\"col-sm-12\" v-if=\"model.meta.description != null\">\n        <div class=\"alert alert-info\" v-html=\"model.meta.description\"></div>\n    </div>\n    <div class=\"form-group\" :class=\"{ 'col-sm-6': field.meta.isHalfWidth, 'col-sm-12': !field.meta.isHalfWidth }\" v-bind:key=\"'field' + field.meta.uid\" v-for=\"field in model.items[0].fields\">\n        <label v-if=\"model.items[0].fields.length > 1\">{{ field.meta.name }}</label>\n        <div v-if=\"field.meta.description != null\" v-html=\"field.meta.description\" class=\"field-description small text-muted\"></div>\n        <div class=\"field-body\">\n            <div :id=\"'tb-' + field.meta.uid\" class=\"component-toolbar\"></div>\n            <component v-if=\"field.model != null\" v-bind:is=\"field.meta.component\" v-bind:uid=\"field.meta.uid\" v-bind:meta=\"field.meta\" v-bind:toolbar=\"'tb-' + field.meta.uid\" v-bind:model=\"field.model\"></component>\n        </div>\n    </div>\n</div>\n<div v-else>\n    <div v-if=\"model.meta.description != null\">\n        <div class=\"alert alert-info\" v-html=\"model.meta.description\"></div>\n    </div>\n    <div :id=\"model.meta.uid\" class=\"accordion sortable\" :class=\"model.items.length !== 0 ? 'mb-3' : ''\">\n        <div class=\"card\" :key=\"item.uid\" v-for=\"(item) in model.items\">\n            <div class=\"card-header\">\n                <a href=\"#\" :data-toggle=\"!model.meta.expanded ? 'collapse' : false\" :data-target=\"'#body' + item.uid\">\n                    <div class=\"handle\">\n                        <i class=\"fas fa-ellipsis-v\"></i>\n                    </div>\n                    {{ item.title }}\n                </a>\n                <span class=\"actions float-right\">\n                    <a v-on:click.prevent=\"removeItem(item)\" href=\"#\" class=\"danger\"><i class=\"fas fa-trash\"></i></a>\n                </span>\n            </div>\n            <div :id=\"'body' + item.uid\" :class=\"{ 'collapse' : !model.meta.expanded }\" :data-parent=\"'#' + model.meta.uid\">\n                <div class=\"card-body\">\n                    <div class=\"row\">\n                        <div class=\"form-group\" :class=\"{ 'col-sm-6': field.meta.isHalfWidth, 'col-sm-12': !field.meta.isHalfWidth }\" v-bind:key=\"field.meta.uid\" v-for=\"field in item.fields\">\n                            <label v-if=\"item.fields.length > 1 || field.meta.id !== 'Default'\">{{ field.meta.name }}</label>\n                            <div v-if=\"field.meta.description != null\" v-html=\"field.meta.description\" class=\"field-description small text-muted\"></div>\n                            <div class=\"field-body\">\n                                <div :id=\"'tb-' + field.meta.uid\" class=\"component-toolbar\"></div>\n                                <component v-if=\"field.model != null\" v-bind:is=\"field.meta.component\" v-bind:uid=\"field.meta.uid\" v-bind:meta=\"field.meta\" v-bind:toolbar=\"'tb-' + field.meta.uid\" v-bind:model=\"field.model\" v-on:update-title=\"updateTitle($event)\"></component>\n                            </div>\n                        </div>\n                    </div>\n                </div>\n            </div>\n        </div>\n    </div>\n    <a href=\"#\" class=\"block-add\" v-on:click.prevent=\"addItem()\">\n        <hr>\n        <i class=\"fas fa-plus-circle\"></i>\n    </a>\n    <div v-if=\"model.items.length === 0\" class=\"empty-info unsortable\">\n        <p>{{ piranha.resources.texts.emptyAddAbove }}</p>\n    </div>\n</div>\n"
});
Vue.component("post-archive", {
  props: ["uid", "id"],
  data: function () {
    return {
      items: [],
      categories: [],
      postTypes: [],
      totalPosts: 0,
      totalPages: 0,
      index: 0,
      status: "all",
      category: piranha.resources.texts.allCategories
    };
  },
  methods: {
    load: function (index) {
      var self = this;

      if (!index) {
        index = 0;
      }

      fetch(piranha.baseUrl + "manager/api/post/list/" + self.id + "/" + index).then(function (response) {
        return response.json();
      }).then(function (result) {
        self.items = result.posts;
        self.categories = result.categories;
        self.postTypes = result.postTypes;
        self.totalPosts = result.totalPosts;
        self.totalPages = result.totalPages;
        self.index = result.index;
      }).catch(function (error) {
        console.log("error:", error);
      });
    },
    remove: function (postId) {
      var self = this;
      piranha.alert.open({
        title: piranha.resources.texts.delete,
        body: piranha.resources.texts.deletePostConfirm,
        confirmCss: "btn-danger",
        confirmIcon: "fas fa-trash",
        confirmText: piranha.resources.texts.delete,
        onConfirm: function () {
          fetch(piranha.baseUrl + "manager/api/post/delete", {
            method: "delete",
            headers: piranha.utils.antiForgeryHeaders(),
            body: JSON.stringify(postId)
          }).then(function (response) {
            return response.json();
          }).then(function (result) {
            piranha.notifications.push(result);
            self.load();
          }).catch(function (error) {
            console.log("error:", error);
          });
        }
      });
    },
    first: function () {
      if (this.hasPrev()) {
        this.load(0);
      }
    },
    prev: function () {
      if (this.hasPrev()) {
        this.load(this.index - 1);
      }
    },
    next: function () {
      if (this.hasNext()) {
        this.load(this.index + 1);
      }
    },
    last: function () {
      if (this.hasNext()) {
        this.load(this.totalPages - 1);
      }
    },
    hasPrev: function () {
      return this.index > 0;
    },
    hasNext: function () {
      return this.index < this.totalPages - 1;
    },
    isSelected: function (item) {
      // Check category
      if (this.category !== piranha.resources.texts.allCategories && item.category !== this.category) {
        return false;
      } // Check status


      if (this.status === "draft") {
        return item.status === "draft" || item.status === "unpublished";
      } else if (this.status === 'scheduled') {
        return item.isScheduled;
      } // Selected


      return true;
    },
    selectStatus: function (status) {
      this.status = status;
    },
    selectCategory: function (category) {
      this.category = category;
    },
    onSaved: function (state) {
      this.load(this.index);
    }
  },
  computed: {
    selectedPosts: function () {
      var self = this;
      return this.items.filter(function (item) {
        return self.isSelected(item);
      });
    }
  },
  mounted: function () {
    this.load();
    this.eventBus.$on("onSaved", this.onSaved);
  },
  beforeDestroy: function () {
    this.eventBus.$off("onSaved");
  },
  template: "\n<div :id=\"uid\">\n    <div class=\"mb-2\">\n        <div class=\"btn-group\" role=\"group\">\n            <button v-on:click=\"selectStatus('all')\" class=\"btn btn-sm\" :class=\"status === 'all' ? 'btn-primary' : 'btn-light'\" href=\"#\">{{ piranha.resources.texts.all }}</button>\n            <button v-on:click=\"selectStatus('draft')\" class=\"btn btn-sm\" :class=\"status === 'draft' ? 'btn-primary' : 'btn-light'\" href=\"#\">{{ piranha.resources.texts.drafts }}</button>\n            <button v-on:click=\"selectStatus('scheduled')\" class=\"btn btn-sm\" :class=\"status === 'scheduled' ? 'btn-primary' : 'btn-light'\" href=\"#\">{{ piranha.resources.texts.scheduled }}</button>\n        </div>\n        <div v-if=\"postTypes.length > 1\" class=\"btn-group\" role=\"group\">\n            <button type=\"button\" class=\"btn btn-sm btn-light dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">\n                {{ piranha.resources.texts.all }}\n            </button>\n            <div class=\"dropdown-menu dropdown-menu-right\">\n                <a v-for=\"type in postTypes\" v-bind:key=\"type.id\" href=\"#\" class=\"dropdown-item\">{{ type.title }}</a>\n            </div>\n        </div>\n        <div v-if=\"categories.length > 1\" class=\"btn-group\" role=\"group\">\n            <button type=\"button\" class=\"btn btn-sm dropdown-toggle\" :class=\"category === piranha.resources.texts.allCategories ? 'btn-light' : 'btn-primary'\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">\n                {{ category }}\n            </button>\n            <div class=\"dropdown-menu dropdown-menu-right\">\n                <a v-on:click.prevent=\"selectCategory(piranha.resources.texts.allCategories)\" href=\"#\" class=\"dropdown-item\">{{ piranha.resources.texts.allCategories }}</a>\n                <a v-on:click.prevent=\"selectCategory(category.title)\" v-for=\"category in categories\" v-bind:key=\"category.slug\" href=\"#\" class=\"dropdown-item\">{{ category.title }}</a>\n            </div>\n        </div>\n        <div v-if=\"postTypes.length > 1 && piranha.permissions.posts.add\" class=\"btn-group float-right\">\n            <button id=\"addPostGroup\" class=\"btn btn-sm btn-primary btn-labeled dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><i class=\"fas fa-plus\"></i>{{ piranha.resources.texts.add }}</button>\n            <div class=\"dropdown-menu dropdown-menu-right\" aria-labelledby=\"addPostGroup\">\n                <a class=\"dropdown-item\" :href=\"piranha.baseUrl + type.addUrl + id + '/' + type.id\" v-bind:key=\"'add-' + type.id\" v-for=\"type in postTypes\">{{ type.title }}</a>\n            </div>\n        </div>\n        <a v-if=\"postTypes.length === 1 && piranha.permissions.posts.add\" :href=\"piranha.baseUrl + postTypes[0].addUrl + id + '/' + postTypes[0].id\" class=\"btn btn-sm btn-primary btn-labeled float-right\"><i class=\"fas fa-plus\"></i>{{ piranha.resources.texts.add }}</a>\n    </div>\n    <table v-if=\"items.length > 0\" class=\"table\">\n        <tbody>\n            <tr v-bind:key=\"post.id\" v-for=\"post in selectedPosts\" :class=\"{ unpublished: post.status === 'unpublished' || post.isScheduled }\">\n                <td>\n                    <a :href=\"piranha.baseUrl + post.editUrl + post.id\">{{ post.title }}</a>\n                    <small v-if=\"post.status === 'published' || post.status === 'draft'\" class=\"text-muted\">| {{ post.published }}</small>\n                    <small v-else-if=\"post.status === 'unpublished'\" class=\"text-muted\">| Unpublished</small>\n                    <span v-if=\"post.status === 'draft'\" class=\"badge badge-info float-right\">{{ piranha.resources.texts.draft }}</span>\n                    <span v-if=\"post.isScheduled\" class=\"badge badge-info float-right\">{{ piranha.resources.texts.scheduled }}</span>\n                </td>\n                <td>\n                    {{ post.typeName }}\n                </td>\n                <td>\n                    {{ post.category }}\n                </td>\n                <td class=\"actions one\">\n                    <a v-if=\"piranha.permissions.posts.delete\" v-on:click.prevent=\"remove(post.id)\" class=\"danger\"><i class=\"fas fa-trash\"></i></a>\n                </td>\n            </tr>\n        </tbody>\n    </table>\n    <div v-else class=\"empty-info\">\n        <p>Looks like there's no posts here. Click on the Add button above to get started!</p>\n    </div>\n    <nav v-if=\"totalPages > 1\">\n        <ul class=\"pagination justify-content-center\">\n            <li class=\"page-item\" :class=\"{ disabled: !hasPrev() }\"><button v-on:click.prevent=\"first()\" :disabled=\"!hasPrev()\" class=\"page-link\" href=\"#\"><i class=\"fas fa-angle-double-left\"></i></button></li>\n            <li class=\"page-item\" :class=\"{ disabled: !hasPrev() }\"><button v-on:click.prevent=\"prev()\" :disabled=\"!hasPrev()\" class=\"page-link\" href=\"#\"><i class=\"fas fa-chevron-left\"></i></button></li>\n            <li class=\"page-item disabled\"><span class=\"page-link\">{{ index + 1}} / {{ totalPages }}</span></li>\n            <li class=\"page-item\" :class=\"{ disabled: !hasNext() }\"><button v-on:click.prevent=\"next()\" :disabled=\"!hasNext()\" class=\"page-link\" href=\"#\"><i class=\"fas fa-chevron-right\"></i></button></li>\n            <li class=\"page-item\" :class=\"{ disabled: !hasNext() }\"><button v-on:click.prevent=\"last()\" :disabled=\"!hasNext()\" class=\"page-link\" href=\"#\"><i class=\"fas fa-angle-double-right\"></i></button></li>\n        </ul>\n    </nav>\n</div>\n"
});
Vue.component("block-group", {
  props: ["uid", "toolbar", "model"],
  methods: {
    selectItem: function (item) {
      for (var n = 0; n < this.model.items.length; n++) {
        if (this.model.items[n] == item) {
          this.model.items[n].isActive = true;
        } else {
          this.model.items[n].isActive = false;
        }
      }
    },
    removeItem: function (item) {
      var itemActive = item.isActive;
      var itemIndex = this.model.items.indexOf(item);
      this.model.items.splice(itemIndex, 1);

      if (itemActive) {
        this.selectItem(this.model.items[Math.min(itemIndex, this.model.items.length - 1)]);
      }
    },
    addGroupBlock: function (type, pos) {
      var self = this;
      fetch(piranha.baseUrl + "manager/api/content/block/" + type).then(function (response) {
        return response.json();
      }).then(function (result) {
        self.model.items.push(result.body);
        self.selectItem(result.body);
      }).catch(function (error) {
        console.log("error:", error);
      });
    },
    updateTitle: function (e) {
      for (var n = 0; n < this.model.items.length; n++) {
        if (this.model.items[n].meta.uid === e.uid) {
          this.model.items[n].meta.title = e.title;
          break;
        }
      }
    },
    toggleHeader: function () {
      this.model.meta.showHeader = !this.model.meta.showHeader;
    },
    moveItem: function (from, to) {
      this.model.items.splice(to, 0, this.model.items.splice(from, 1)[0]);
    }
  },
  mounted: function () {
    var self = this;
    sortable("#" + this.uid + " .list-group", {
      items: ":not(.unsortable)"
    })[0].addEventListener("sortupdate", function (e) {
      self.moveItem(e.detail.origin.index, e.detail.destination.index);
    });
  },
  template: "\n<div :id=\"uid\" class=\"block-group\">\n    <div v-if=\"model.fields.length > 0\" class=\"actions block-group-actions\">\n        <button v-on:click.prevent=\"toggleHeader()\" class=\"btn btn-sm\" :class=\"{ selected: model.meta.showHeader }\">\n            <i class=\"fas fa-list\"></i>\n        </button>\n    </div>\n    <div class=\"block-group-header\">\n        <div v-if=\"model.meta.showHeader\" class=\"row\">\n            <div class=\"form-group\" :class=\"{ 'col-sm-6': field.meta.isHalfWidth, 'col-sm-12': !field.meta.isHalfWidth }\" v-bind:key=\"field.meta.id\" v-for=\"field in model.fields\">\n                <label>{{ field.meta.name }}</label>\n                <div v-if=\"field.meta.description != null\" v-html=\"field.meta.description\" class=\"field-description small text-muted\"></div>\n                <component v-bind:is=\"field.meta.component\" v-bind:uid=\"field.meta.uid\" v-bind:meta=\"field.meta\" v-bind:toolbar=\"toolbar\" v-bind:model=\"field.model\"></component>\n            </div>\n        </div>\n    </div>\n    <div class=\"row\">\n        <div class=\"col-md-4\">\n            <div class=\"list-group list-group-flush\">\n                <div class=\"list-group-item\" :class=\"{ active: child.isActive }\" v-for=\"child in model.items\" v-bind:key=\"child.meta.uid\">\n                    <a href=\"#\" v-on:click.prevent=\"selectItem(child)\">\n                        <div class=\"handle\">\n                            <i class=\"fas fa-ellipsis-v\"></i>\n                        </div>\n                        {{ child.meta.title }}\n                    </a>\n                    <span class='actions'>\n                        <a v-on:click.prevent=\"removeItem(child)\" href=\"#\" class=\"danger\"><i class=\"fas fa-trash\"></i></a>\n                    </span>\n                </div>\n            </div>\n            <button v-on:click.prevent=\"piranha.blockpicker.open(addGroupBlock, 0, model.type)\" class=\"btn btn-sm btn-primary btn-labeled mt-3\">\n                <i class=\"fas fa-plus\"></i>{{ piranha.resources.texts.add }}\n            </button>\n        </div>\n        <div class='col-md-8'>\n            <div v-if=\"model.items.length === 0\" class=\"empty-info unsortable\">\n                <p>{{ piranha.resources.texts.emptyAddLeft }}</p>\n            </div>\n            <template v-for=\"child in model.items\">\n                <div class=\"block\" :class=\"child.meta.component\" v-if=\"child.isActive\" v-bind:key=\"'details-' + child.meta.uid\">\n                    <component v-bind:is=\"child.meta.component\" v-bind:uid=\"child.meta.uid\" v-bind:toolbar=\"toolbar\" v-bind:model=\"child.model\" v-on:update-title=\"updateTitle($event)\"></component>\n                </div>\n            </template>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("block-group-horizontal", {
  props: ["uid", "toolbar", "model"],
  methods: {
    removeItem: function (item) {
      var itemIndex = this.model.items.indexOf(item);
      this.model.items.splice(itemIndex, 1);
    },
    addGroupBlock: function (type, pos) {
      var self = this;
      fetch(piranha.baseUrl + "manager/api/content/block/" + type).then(function (response) {
        return response.json();
      }).then(function (result) {
        sortable("#" + self.uid + " .block-group-items", "destroy");
        self.model.items.push(result.body);
        Vue.nextTick(function () {
          sortable("#" + self.uid + " .block-group-items", {
            handle: '.handle',
            items: ":not(.unsortable)",
            placeholderClass: "col sortable-placeholder"
          })[0].addEventListener("sortupdate", function (e) {
            self.moveItem(e.detail.origin.index, e.detail.destination.index);
          });
        });
      }).catch(function (error) {
        console.log("error:", error);
      });
    },
    toggleHeader: function () {
      this.model.meta.showHeader = !this.model.meta.showHeader;
    },
    moveItem: function (from, to) {
      this.model.items.splice(to, 0, this.model.items.splice(from, 1)[0]);
    }
  },
  mounted: function () {
    var self = this;
    sortable("#" + this.uid + " .block-group-items", {
      handle: '.handle',
      items: ":not(.unsortable)",
      placeholderClass: "col sortable-placeholder"
    })[0].addEventListener("sortupdate", function (e) {
      self.moveItem(e.detail.origin.index, e.detail.destination.index);
    });
  },
  template: "\n<div :id=\"uid\" class=\"block-group\">\n    <div class=\"actions block-group-actions\">\n        <button v-on:click.prevent=\"piranha.blockpicker.open(addGroupBlock, 0, model.type)\" class=\"btn btn-sm add\">\n            <i class=\"fas fa-plus\"></i>\n        </button>\n        <button v-on:click.prevent='toggleHeader()' v-if='model.fields.length > 0' class='btn btn-sm' :class='{ selected: model.meta.showHeader }'>\n            <i class=\"fas fa-list\"></i>\n        </button>\n    </div>\n    <div v-if=\"model.meta.showHeader && model.fields.length > 0\" class=\"block-group-header\">\n        <div class=\"row\">\n            <div class=\"form-group\" :class=\"{ 'col-sm-6': field.meta.isHalfWidth, 'col-sm-12': !field.meta.isHalfWidth }\" v-bind:key=\"field.meta.id\" v-for=\"field in model.fields\">\n                <label>{{ field.meta.name }}</label>\n                <div v-if=\"field.meta.description != null\" v-html=\"field.meta.description\" class=\"field-description small text-muted\"></div>\n                <component v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:toolbar='toolbar' v-bind:model='field.model'></component>\n            </div>\n        </div>\n    </div>\n    <div class=\"row block-group-items\">\n        <div v-if=\"model.items.length === 0\" class=\"col\">\n            <div class=\"empty-info unsortable\">\n                <p>{{ piranha.resources.texts.emptyAddAbove }}</p>\n            </div>\n        </div>\n        <div v-for=\"child in model.items\" v-bind:key=\"child.meta.uid\" class=\"col\">\n            <div class=\"block\" :class=\"child.meta.component\">\n                <div class=\"block-header\">\n                    <div class=\"title\">\n                        <i :class=\"child.meta.icon\"></i><strong>{{ child.meta.name }}</strong>\n                    </div>\n                    <div class=\"actions\">\n                        <span class=\"btn btn-sm handle\">\n                            <i class=\"fas fa-ellipsis-v\"></i>\n                        </span>\n                        <button v-on:click.prevent=\"removeItem(child)\" class=\"btn btn-sm danger\" tabindex=\"-1\">\n                            <i class=\"fas fa-trash\"></i>\n                        </button>\n                    </div>\n                </div>\n                <component v-bind:is=\"child.meta.component\" v-bind:uid=\"child.meta.uid\" v-bind:toolbar=\"toolbar\" v-bind:model=\"child.model\"></component>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("block-group-vertical", {
  props: ["uid", "toolbar", "model"],
  methods: {
    collapseItem: function (item) {
      item.meta.isCollapsed = !item.meta.isCollapsed;
    },
    removeItem: function (item) {
      var itemIndex = this.model.items.indexOf(item);
      this.model.items.splice(itemIndex, 1);
    },
    addGroupBlock: function (type, pos) {
      var self = this;
      fetch(piranha.baseUrl + "manager/api/content/block/" + type).then(function (response) {
        return response.json();
      }).then(function (result) {
        sortable("#" + self.uid + " .block-group-items", "destroy");
        self.model.items.splice(pos, 0, result.body);
        Vue.nextTick(function () {
          sortable("#" + self.uid + " .block-group-items", {
            handle: '.handle',
            items: ":not(.unsortable)",
            placeholderClass: "sortable-placeholder"
          })[0].addEventListener("sortupdate", function (e) {
            self.moveItem(e.detail.origin.index, e.detail.destination.index);
          });
        });
      }).catch(function (error) {
        console.log("error:", error);
      });
    },
    toggleHeader: function () {
      this.model.meta.showHeader = !this.model.meta.showHeader;
    },
    moveItem: function (from, to) {
      this.model.items.splice(to, 0, this.model.items.splice(from, 1)[0]);
    }
  },
  mounted: function () {
    var self = this;
    sortable("#" + this.uid + " .block-group-items", {
      handle: '.handle',
      items: ":not(.unsortable)",
      placeholderClass: "sortable-placeholder"
    })[0].addEventListener("sortupdate", function (e) {
      self.moveItem(e.detail.origin.index, e.detail.destination.index);
    });
  },
  template: "\n<div :id=\"uid\" class=\"block-group\">\n    <div class=\"actions block-group-actions\">\n        <button v-on:click.prevent=\"toggleHeader()\" v-if=\"model.fields.length > 0\" class=\"btn btn-sm\" :class=\"{ selected: model.meta.showHeader }\">\n            <i class=\"fas fa-list\"></i>\n        </button>\n    </div>\n    <div v-if=\"model.meta.showHeader && model.fields.length > 0\" class=\"block-group-header\">\n        <div class=\"row\">\n            <div class=\"form-group\" :class=\"{ 'col-sm-6': field.meta.isHalfWidth, 'col-sm-12': !field.meta.isHalfWidth }\" v-bind:key=\"field.meta.uid\" v-for=\"field in model.fields\">\n                <label>{{ field.meta.name }}</label>\n                <div v-if=\"field.meta.description != null\" v-html=\"field.meta.description\" class=\"field-description small text-muted\"></div>\n                <component v-bind:is=\"field.meta.component\" v-bind:uid=\"field.meta.uid\" v-bind:meta=\"field.meta\" v-bind:toolbar=\"toolbar\" v-bind:model=\"field.model\"></component>\n            </div>\n        </div>\n    </div>\n    <div class=\"block-group-items\">\n        <a href=\"#\" class=\"block-add unsortable\" v-on:click.prevent=\"piranha.blockpicker.open(addGroupBlock, 0, model.type)\">\n            <hr>\n            <i class=\"fas fa-plus-circle\"></i>\n        </a>\n        <div v-if=\"model.items.length === 0\" class=\"col\">\n            <div class=\"empty-info unsortable\">\n                <p>{{ piranha.resources.texts.emptyAddAbove }}</p>\n            </div>\n        </div>\n        <div v-for=\"(child, index) in model.items\" v-bind:key=\"child.meta.uid\">\n            <div class=\"block\" :class=\"child.meta.component + (child.meta.isCollapsed ? ' collapsed' : '')\">\n                <div class=\"block-header\">\n                    <div class=\"title\">\n                        <i :class=\"child.meta.icon\"></i><strong>{{ child.meta.name }}</strong>\n                    </div>\n                    <div class=\"actions\">\n                        <span v-on:click.prevent=\"collapseItem(child)\" class=\"btn btn-sm\">\n                            <i v-if=\"child.meta.isCollapsed\" class=\"fas fa-chevron-down\"></i>\n                            <i v-else class=\"fas fa-chevron-up\"></i>\n                        </span>\n                        <span class=\"btn btn-sm handle\">\n                            <i class=\"fas fa-ellipsis-v\"></i>\n                        </span>\n                        <button v-on:click.prevent=\"removeItem(child)\" class=\"btn btn-sm danger\" tabindex=\"-1\">\n                            <i class=\"fas fa-trash\"></i>\n                        </button>\n                    </div>\n                </div>\n                <component v-bind:is=\"child.meta.component\" v-bind:uid=\"child.meta.uid\" v-bind:toolbar=\"toolbar\" v-bind:model=\"child.model\"></component>\n            </div>\n            <a href=\"#\" class=\"block-add unsortable\" v-on:click.prevent=\"piranha.blockpicker.open(addGroupBlock, index + 1, model.type)\">\n                <hr>\n                <i class=\"fas fa-plus-circle\"></i>\n            </a>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("generic-block", {
  props: ["uid", "toolbar", "model"],
  methods: {
    updateTitle: function (e) {
      this.$emit('update-title', {
        uid: this.uid,
        title: e.title
      });
    }
  },
  template: "\n<div class=\"block-body\">\n    <div class=\"row\">\n        <div class=\"form-group\" :class=\"{ 'col-sm-6': field.meta.isHalfWidth, 'col-sm-12': !field.meta.isHalfWidth }\" v-bind:key=\"'field' + field.meta.uid\" v-for=\"field in model\">\n            <label>{{ field.meta.name }}</label>\n            <div v-if=\"field.meta.description != null\" v-html=\"field.meta.description\" class=\"field-description small text-muted\"></div>\n            <component v-bind:is=\"field.meta.component\" v-bind:uid=\"field.meta.uid\" v-bind:meta=\"field.meta\" v-bind:toolbar=\"toolbar\" v-bind:model=\"field.model\" v-on:update-title=\"updateTitle($event)\"></component>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("audio-block", {
  props: ["uid", "model"],
  methods: {
    clear: function () {// clear media from block
    },
    select: function () {
      if (this.model.body.media != null) {
        piranha.mediapicker.open(this.update, "Audio", this.model.body.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, "Audio");
      }
    },
    remove: function () {
      this.model.body.id = null;
      this.model.body.media = null;
    },
    update: function (media) {
      if (media.type === "Audio") {
        this.model.body.id = media.id;
        this.model.body.media = {
          id: media.id,
          folderId: media.folderId,
          type: media.type,
          filename: media.filename,
          contentType: media.contentType,
          publicUrl: media.publicUrl
        }; // Tell parent that title has been updated

        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.body.media.filename
        });
      } else {
        console.log("No video was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.body.media == null;
    },
    mediaUrl: function () {
      if (this.model.body.media != null) {
        return piranha.utils.formatUrl(this.model.body.media.publicUrl);
      }
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.filename;
      } else {
        return "No audio selected";
      }
    };
  },
  template: "\n<div class=\"block-body has-media-picker d-flex align-items-center\" :class=\"{ empty: isEmpty }\">\n    <audio class=\"flex-grow-1 w-50\" :src=\"mediaUrl\" controls></audio>\n    <div class=\"media-picker slide-in\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                &nbsp;\n            </div>\n            <div class=\"card-body\" v-else>\n                {{ model.body.media.filename }}\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("content-block", {
  props: ["uid", "model"],
  methods: {
    select: function () {
      piranha.contentpicker.open(null, this.update);
    },
    remove: function () {
      this.model.body.id = null;
      this.model.body.content = null;
    },
    update: function (content) {
      if (content !== null) {
        var self = this;
        fetch(piranha.baseUrl + "manager/api/content/info/" + content.id).then(function (response) {
          return response.json();
        }).then(function (result) {
          self.model.body.id = result.id;
          self.model.body.content = result; // Tell parent that title has been updated

          self.$emit('update-title', {
            uid: self.uid,
            title: self.model.body.content.title
          });
        }).catch(function (error) {
          console.log("error:", error);
        });
      } else {
        console.log("No content was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.body.content == null;
    },
    contentImage: function () {
      if (this.hasContentImage) {
        return piranha.baseUrl + "manager/api/media/url/" + this.model.body.content.primaryImage.id + "/446/220";
      } else {
        return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
      }
    },
    hasContentImage: function () {
      return this.model.body.content !== null && this.model.body.content.primaryImage.media !== null;
    },
    contentTitle: function () {
      if (this.hasContentTitle) {
        return this.model.body.content.title;
      }

      return "Lorem Ipsum";
    },
    hasContentTitle: function () {
      return this.model.body.content !== null;
    },
    contentExcerpt: function () {
      if (this.hasContentExcerpt) {
        return this.model.body.content.excerpt;
      }

      return "Donec id elit non mi porta gravida at eget metus. Cras mattis consectetur purus sit amet fermentum. Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
    },
    hasContentExcerpt: function () {
      return this.model.body.content !== null && this.model.body.content.excerpt !== null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.body.content !== null) {
        return this.model.body.content.title;
      } else {
        return "No content selected";
      }
    };
  },
  template: "\n<div class=\"block-body has-media-picker rounded clearfix\" :class=\"{ empty: isEmpty }\">\n    <div>\n        <div class=\"page-image\" :style=\"'background-image:url(' + contentImage + ')'\">\n            <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/primaryimage-placeholder.png')\">\n        </div>\n        <h3 :class=\"{ 'text-light': !hasContentTitle }\">{{ contentTitle }}</h3>\n        <p :class=\"{ 'text-light': !hasContentExcerpt }\" v-html=\"contentExcerpt\"></p>\n    </div>\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                &nbsp;\n            </div>\n            <div class=\"card-body\" v-else>\n                <a :href=\"piranha.baseUrl + 'manager/content/edit/' + model.body.content.typeId + '/' + model.body.content.id\" target=\"_blank\">{{ model.body.content.title }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("html-block", {
  props: ["uid", "toolbar", "model"],
  data: function () {
    return {
      body: this.model.body.value
    };
  },
  methods: {
    onBlur: function (e) {
      this.model.body.value = tinyMCE.activeEditor.getContent();
    },
    onChange: function (data) {
      this.model.body.value = data;
    }
  },
  computed: {
    isEmpty: function () {
      return piranha.utils.isEmptyHtml(this.model.body.value);
    }
  },
  mounted: function () {
    piranha.editor.addInline(this.uid, this.toolbar, this.onChange);
  },
  beforeDestroy: function () {
    piranha.editor.remove(this.uid);
  },
  template: "\n<div class=\"block-body\" :class=\"{ empty: isEmpty }\">\n    <div contenteditable=\"true\" :id=\"uid\" v-html=\"body\" v-on:blur=\"onBlur\"></div>\n</div>\n"
});
Vue.component("html-column-block", {
  props: ["uid", "toolbar", "model"],
  data: function () {
    return {
      column1: this.model.column1.value,
      column2: this.model.column2.value
    };
  },
  methods: {
    onBlurCol1: function (e) {
      this.model.column1.value = tinyMCE.activeEditor.getContent();
      ;
    },
    onBlurCol2: function (e) {
      this.model.column2.value = tinyMCE.activeEditor.getContent();
      ;
    },
    onChangeCol1: function (data) {
      this.model.column1.value = data;
    },
    onChangeCol2: function (data) {
      this.model.column2.value = data;
    }
  },
  computed: {
    isEmpty1: function () {
      return piranha.utils.isEmptyHtml(this.model.column1.value);
    },
    isEmpty2: function () {
      return piranha.utils.isEmptyHtml(this.model.column2.value);
    }
  },
  mounted: function () {
    piranha.editor.addInline(this.uid + 1, this.toolbar, this.onChangeCol1);
    piranha.editor.addInline(this.uid + 2, this.toolbar, this.onChangeCol2);
  },
  beforeDestroy: function () {
    piranha.editor.remove(this.uid + 1);
    piranha.editor.remove(this.uid + 2);
  },
  template: "\n<div class=\"row block-body\">\n    <div class=\"col-md-6\">\n        <div :class=\"{ empty: isEmpty1 }\">\n            <div :id=\"uid + 1\" contenteditable=\"true\" v-html=\"column1\" v-on:blur=\"onBlurCol1\"></div>\n        </div>\n    </div>\n    <div class='col-md-6'>\n        <div :class='{ empty: isEmpty2 }'>\n            <div :id=\"uid + 2\" contenteditable=\"true\" v-html=\"column2\" v-on:blur=\"onBlurCol2\"></div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("image-block", {
  props: ["uid", "model"],
  methods: {
    clear: function () {// clear media from block
    },
    select: function () {
      if (this.model.body.media != null) {
        piranha.mediapicker.open(this.update, "Image", this.model.body.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, "Image");
      }
    },
    remove: function () {
      this.model.body.id = null;
      this.model.body.media = null;
    },
    update: function (media) {
      if (media.type === "Image") {
        this.model.body.id = media.id;
        this.model.body.media = {
          id: media.id,
          folderId: media.folderId,
          type: media.type,
          filename: media.filename,
          contentType: media.contentType,
          publicUrl: media.publicUrl
        }; // Tell parent that title has been updated

        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.body.media.filename
        });
      } else {
        console.log("No image was selected");
      }
    },
    selectAspect: function (val) {
      this.model.aspect.value = val;
    },

    isAspectSelected(val) {
      return this.model.aspect.value === val;
    }

  },
  computed: {
    isEmpty: function () {
      return this.model.body.media == null;
    },
    mediaUrl: function () {
      if (this.model.body.media != null) {
        return piranha.utils.formatUrl(this.model.body.media.publicUrl);
      } else {
        return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
      }
    },
    iconUrl: function () {
      if (this.model.aspect.value > 0) {
        if (this.model.aspect.value === 1 || this.model.aspect.value === 3) {
          return piranha.utils.formatUrl("~/manager/assets/img/icons/img-landscape.svg");
        } else if (this.model.aspect.value == 2) {
          return piranha.utils.formatUrl("~/manager/assets/img/icons/img-portrait.svg");
        } else if (this.model.aspect.value == 4) {
          return piranha.utils.formatUrl("~/manager/assets/img/icons/img-square.svg");
        }
      }

      return null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.filename;
      } else {
        return "No image selected";
      }
    };
  },
  template: "\n<div class=\"block-body has-media-picker rounded\" :class=\"{ empty: isEmpty }\">\n    <img class=\"rounded\" :src=\"mediaUrl\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button :id=\"uid + '-aspect'\" class=\"btn btn-info btn-aspect text-center\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">\n                <i v-if=\"model.aspect.value === 0\" class=\"fas fa-cog\"></i>\n                <img v-else :src=\"iconUrl\">\n            </button>\n            <div class=\"dropdown-menu aspect-menu\" :aria-labelledby=\"uid + '-aspect'\">\n                <label class=\"mb-0\">{{ piranha.resources.texts.aspectLabel }}</label>\n                <div class=\"dropdown-divider\"></div>\n                <a v-on:click.prevent=\"selectAspect(0)\" class=\"dropdown-item\" :class=\"{ active: isAspectSelected(0) }\" href=\"#\">\n                    <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/icons/img-original.svg')\"><span>{{ piranha.resources.texts.aspectOriginal }}</span>\n                </a>\n                <a v-on:click.prevent=\"selectAspect(1)\" class=\"dropdown-item\" :class=\"{ active: isAspectSelected(1) }\" href=\"#\">\n                    <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/icons/img-landscape.svg')\"><span>{{ piranha.resources.texts.aspectLandscape }}</span>\n                </a>\n                <a v-on:click.prevent=\"selectAspect(2)\" class=\"dropdown-item\" :class=\"{ active: isAspectSelected(2) }\" href=\"#\">\n                    <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/icons/img-portrait.svg')\"><span>{{ piranha.resources.texts.aspectPortrait }}</span>\n                </a>\n                <a v-on:click.prevent=\"selectAspect(3)\" class=\"dropdown-item\" :class=\"{ active: isAspectSelected(3) }\" href=\"#\">\n                    <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/icons/img-landscape.svg')\"><span>{{ piranha.resources.texts.aspectWidescreen }}</span>\n                </a>\n                <a v-on:click.prevent=\"selectAspect(4)\" class=\"dropdown-item\" :class=\"{ active: isAspectSelected(4) }\" href=\"#\">\n                    <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/icons/img-square.svg')\"><span>{{ piranha.resources.texts.aspectSquare }}</span>\n                </a>\n            </div>\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                &nbsp;\n            </div>\n            <div class=\"card-body\" v-else>\n                {{ model.body.media.filename }}\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("markdown-block", {
  props: ["uid", "model"],
  methods: {
    onBlur: function (e) {
      // Tell parent that title has been updated
      var title = this.model.body.value.replace(/(<([^>]+)>)/ig, "");

      if (title.length > 40) {
        title = title.substring(0, 40) + "...";
      }

      this.$emit('update-title', {
        uid: this.uid,
        title: title
      });
    }
  },
  computed: {
    isEmpty: function () {
      return piranha.utils.isEmptyText(this.model.body.value);
    }
  },
  template: "\n<div class=\"block-body\" :class=\"{ empty: isEmpty }\">\n    <markdown-field :uid=\"uid\" :model=\"model.body\" />\n</div>\n"
});
Vue.component("missing-block", {
  props: ["model"],
  template: "\n<div class=\"alert alert-danger text-center\" role=\"alert\">No component registered for <code>{{ model.type }}</code></div>\n"
});
Vue.component("page-block", {
  props: ["uid", "model"],
  methods: {
    select: function () {
      piranha.pagepicker.open(this.update);
    },
    remove: function () {
      this.model.body.id = null;
      this.model.body.page = null;
    },
    update: function (page) {
      if (page !== null) {
        var self = this;
        fetch(piranha.baseUrl + "manager/api/page/info/" + page.id).then(function (response) {
          return response.json();
        }).then(function (result) {
          self.model.body.id = result.id;
          self.model.body.page = result; // Tell parent that title has been updated

          self.$emit('update-title', {
            uid: self.uid,
            title: self.model.body.page.title
          });
        }).catch(function (error) {
          console.log("error:", error);
        });
      } else {
        console.log("No page was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.body.page == null;
    },
    pageImage: function () {
      if (this.hasPageImage) {
        return piranha.baseUrl + "manager/api/media/url/" + this.model.body.page.primaryImage.id + "/446/220"; //return piranha.utils.formatUrl(this.model.body.page.primaryImage.media.publicUrl);
      } else {
        return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
      }
    },
    hasPageImage: function () {
      return this.model.body.page !== null && this.model.body.page.primaryImage.media !== null;
    },
    pageTitle: function () {
      if (this.hasPageTitle) {
        return this.model.body.page.title;
      }

      return "Lorem Ipsum";
    },
    hasPageTitle: function () {
      return this.model.body.page !== null;
    },
    pageExcerpt: function () {
      if (this.hasPageExcerpt) {
        return this.model.body.page.excerpt;
      }

      return "Donec id elit non mi porta gravida at eget metus. Cras mattis consectetur purus sit amet fermentum. Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
    },
    hasPageExcerpt: function () {
      return this.model.body.page !== null && this.model.body.page.excerpt !== null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.body.page !== null) {
        return this.model.body.page.title;
      } else {
        return "No page selected";
      }
    };
  },
  template: "\n<div class=\"block-body has-media-picker rounded clearfix\" :class=\"{ empty: isEmpty }\">\n    <div>\n        <div class=\"page-image\" :style=\"'background-image:url(' + pageImage + ')'\">\n            <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/primaryimage-placeholder.png')\">\n        </div>\n        <h3 :class=\"{ 'text-light': !hasPageTitle }\">{{ pageTitle }}</h3>\n        <p :class=\"{ 'text-light': !hasPageExcerpt }\" v-html=\"pageExcerpt\"></p>\n    </div>\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                &nbsp;\n            </div>\n            <div class=\"card-body\" v-else>\n                <a :href=\"piranha.baseUrl + 'manager/page/edit/' + model.body.page.id\" target=\"_blank\">{{ model.body.page.title }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("post-block", {
  props: ["uid", "model"],
  methods: {
    select: function () {
      piranha.postpicker.open(this.update);
    },
    remove: function () {
      this.model.body.id = null;
      this.model.body.post = null;
    },
    update: function (post) {
      if (post !== null) {
        var self = this;
        fetch(piranha.baseUrl + "manager/api/post/info/" + post.id).then(function (response) {
          return response.json();
        }).then(function (result) {
          self.model.body.id = result.id;
          self.model.body.post = result; // Tell parent that title has been updated

          self.$emit('update-title', {
            uid: self.uid,
            title: self.model.body.post.title
          });
        }).catch(function (error) {
          console.log("error:", error);
        });
      } else {
        console.log("No post was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.body.post == null;
    },
    postImage: function () {
      if (this.hasPostImage) {
        return piranha.baseUrl + "manager/api/media/url/" + this.model.body.post.primaryImage.id + "/446/220";
      } else {
        return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
      }
    },
    hasPostImage: function () {
      return this.model.body.post !== null && this.model.body.post.primaryImage.media !== null;
    },
    postTitle: function () {
      if (this.hasPostTitle) {
        return this.model.body.post.title;
      }

      return "Lorem Ipsum";
    },
    hasPostTitle: function () {
      return this.model.body.post !== null;
    },
    postExcerpt: function () {
      if (this.hasPostExcerpt) {
        return this.model.body.post.excerpt;
      }

      return "Donec id elit non mi porta gravida at eget metus. Cras mattis consectetur purus sit amet fermentum. Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
    },
    hasPostExcerpt: function () {
      return this.model.body.post !== null && this.model.body.post.excerpt !== null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.body.post !== null) {
        return this.model.body.post.title;
      } else {
        return "No post selected";
      }
    };
  },
  template: "\n<div class=\"block-body has-media-picker rounded clearfix\" :class=\"{ empty: isEmpty }\">\n    <div>\n        <div class=\"page-image\" :style=\"'background-image:url(' + postImage + ')'\">\n            <img :src=\"piranha.utils.formatUrl('~/manager/assets/img/primaryimage-placeholder.png')\">\n        </div>\n        <h3 :class=\"{ 'text-light': !hasPostTitle }\">{{ postTitle }}</h3>\n        <p :class=\"{ 'text-light': !hasPostExcerpt }\" v-html=\"postExcerpt\"></p>\n    </div>\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                &nbsp;\n            </div>\n            <div class=\"card-body\" v-else>\n                <a :href=\"piranha.baseUrl + 'manager/post/edit/' + model.body.post.id\" target=\"_blank\">{{ model.body.post.title }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("quote-block", {
  props: ["uid", "model"],
  data: function () {
    return {
      placeholder: {
        body: "",
        author: ""
      }
    };
  },
  methods: {
    onAuthorBlur: function (e) {
      this.model.author.value = e.target.innerText;
    },
    onBodyBlur: function (e) {
      this.model.body.value = e.target.innerText; // Tell parent that title has been updated

      var title = this.model.body.value.replace(/(<([^>]+)>)/ig, "");

      if (title.length > 40) {
        title = title.substring(0, 40) + "...";
      }

      this.$emit('update-title', {
        uid: this.uid,
        title: title
      });
    }
  },
  created: function () {
    var quotes = [{
      author: "Nelson Mandela",
      body: "The greatest glory in living lies not in never falling, but in rising every time we fall."
    }, {
      author: "Walt Disney",
      body: "The way to get started is to quit talking and begin doing."
    }, {
      author: "Eleanor Roosevelt",
      body: "The future belongs to those who believe in the beauty of their dreams."
    }, {
      author: "John Lennon",
      body: "Life is what happens when you're busy making other plans."
    }, {
      author: "Audrey Hepburn",
      body: "Nothing is impossible, the word itself says, 'I'm possible!'"
    }, {
      author: "Mark Twain",
      body: "Twenty years from now you will be more disappointed by the things that you didn't do than by the ones you did do."
    }, {
      author: "Maya Angelou",
      body: "You will face many defeats in life, but never let yourself be defeated."
    }];
    this.placeholder = quotes[Math.floor(Math.random() * quotes.length)];
  },
  template: "\n<div class=\"block-body\">\n    <blockquote class=\"blockquote\">\n        <p contenteditable=\"true\" class=\"blockquote-body\" v-html=\"model.body.value\" v-on:blur=\"onBodyBlur\" :data-placeholder=\"placeholder.body\"></p>\n        <footer contenteditable=\"true\" class=\"blockquote-footer\" v-html=\"model.author.value\" v-on:blur=\"onAuthorBlur\" :data-placeholder=\"placeholder.author\"></footer>\n    </blockquote>\n</div>\n"
});
Vue.component("separator-block", {
  props: ["model"],
  template: "\n<div class='block-body'>\n    <hr>\n</div>\n"
});
Vue.component("text-block", {
  props: ["uid", "model"],
  methods: {
    onBlur: function (e) {
      // this.model.body.value = e.target.innerHTML;
      // Tell parent that title has been updated
      var title = this.model.body.value.replace(/(<([^>]+)>)/ig, "");

      if (title.length > 40) {
        title = title.substring(0, 40) + "...";
      }

      this.$emit('update-title', {
        uid: this.uid,
        title: title
      });
    }
  },
  computed: {
    isEmpty: function () {
      return piranha.utils.isEmptyText(this.model.body.value);
    }
  },
  template: "\n<div class=\"block-body\" :class=\"{ empty: isEmpty }\">\n    <pre class=\"invisible\" v-html=\"model.body.value\"></pre>\n    <textarea v-model=\"model.body.value\" v-on:blur=\"onBlur\"></textarea>\n</div>\n"
});
Vue.component("video-block", {
  props: ["uid", "model"],
  methods: {
    clear: function () {// clear media from block
    },
    select: function () {
      if (this.model.body.media != null) {
        piranha.mediapicker.open(this.update, "Video", this.model.body.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, "Video");
      }
    },
    remove: function () {
      this.model.body.id = null;
      this.model.body.media = null;
    },
    update: function (media) {
      if (media.type === "Video") {
        this.model.body.id = media.id;
        this.model.body.media = {
          id: media.id,
          folderId: media.folderId,
          type: media.type,
          filename: media.filename,
          contentType: media.contentType,
          publicUrl: media.publicUrl
        }; // Tell parent that title has been updated

        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.body.media.filename
        });
      } else {
        console.log("No video was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.body.media == null;
    },
    mediaUrl: function () {
      if (this.model.body.media != null) {
        return piranha.utils.formatUrl(this.model.body.media.publicUrl);
      }
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.filename;
      } else {
        return "No video selected";
      }
    };
  },
  template: "\n<div class=\"block-body has-media-picker\" :class=\"{ empty: isEmpty }\">\n    <video class=\"w-100 mx-100\" :src=\"mediaUrl\" controls></video>\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                &nbsp;\n            </div>\n            <div class=\"card-body\" v-else>\n                {{ model.body.media.filename }}\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("audio-field", {
  props: ["uid", "model", "meta"],
  methods: {
    select: function () {
      if (this.model.media != null) {
        piranha.mediapicker.open(this.update, "Audio", this.model.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, "Audio");
      }
    },
    remove: function () {
      this.model.id = null;
      this.model.media = null;
    },
    update: function (media) {
      if (media.type === "Audio") {
        this.model.id = media.id;
        this.model.media = {
          id: media.id,
          folderId: media.folderId,
          type: media.type,
          filename: media.filename,
          title: media.title,
          contentType: media.contentType,
          publicUrl: media.publicUrl
        }; // Tell parent that title has been updated

        if (this.meta.notifyChange) {
          this.$emit('update-title', {
            uid: this.uid,
            title: this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename
          });
        }
      } else {
        console.log("No audio was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.media == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename;
      } else {
        return "No audio selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else-if=\"model.media.title != null\">\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.title }} ({{ model.media.filename }})</a>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.filename }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("checkbox-field", {
  props: ["uid", "model", "meta"],
  template: "\n<div class=\"form-group form-check\">\n    <input type=\"checkbox\" class=\"form-check-input\" :id=\"meta.uid\" v-model=\"model.value\">\n    <label class=\"form-check-label\" :for=\"meta.uid\">{{ meta.placeholder}}</label>\n</div>\n"
});
Vue.component("color-field", {
  props: ["uid", "model", "meta"],
  methods: {
    update: function () {
      // Tell parent that title has been updated
      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.value
        });
      }
    },
    readonly: function () {
      return this.meta.settings.DisallowInput != null && this.meta.settings.DisallowInput;
    }
  },
  template: "\n<div class=\"input-group color-field\">\n    <div class=\"input-group-prepend\">\n        <div class=\"color-preview\" :style=\"{ backgroundColor: model.value }\"></div>\n        <input class=\"form-control\" type=\"color\" v-model=\"model.value\">\n    </div>\n    <input class=\"form-control\" type=\"text\" v-model=\"model.value\" v-on:change=\"update()\" :readonly=\"readonly()\" :placeholder=\"meta.placeholder\">\n</div>    \n"
});
Vue.component("content-field", {
  props: ["uid", "model", "meta"],
  methods: {
    select: function () {
      piranha.contentpicker.open(this.meta.settings.Group, this.update);
    },
    remove: function () {
      this.model.id = null;
      this.model.content = null;
    },
    update: function (content) {
      this.model.id = content.id;
      this.model.content = content; // Tell parent that title has been updated

      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.content.title
        });
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.content == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.content != null) {
        return this.model.content.title;
      } else {
        return "No content selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a :href=\"piranha.baseUrl + 'manager/content/edit/' + model.content.typeId + '/' + model.content.id\" target=\"_blank\">{{ model.content.title }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("data-select-field", {
  props: ["uid", "model", "meta"],
  methods: {
    update: function () {
      // Tell parent that title has been updated
      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.items.$values.find(v => v.id === this.model.id).name
        });
      }
    }
  },
  template: "\n<select class=\"form-control\" v-model=\"model.id\" v-on:change=\"update()\">\n    <option v-for=\"(item) in model.items.$values\" v-bind:key=\"item.id\" v-bind:value=\"item.id\">\n        {{ item.name }}\n    </option>\n</select>\n"
});
Vue.component("date-field", {
  props: ["uid", "model", "meta"],
  components: {
    datepicker: vuejsDatepicker
  },
  methods: {
    onClosed: function () {
      var d = this.model.value;
      var str = d.getFullYear() + "-" + (d.getMonth() < 9 ? "0" : "") + (d.getMonth() + 1) + "-" + (d.getDate() < 10 ? "0" : "") + d.getDate();
      this.model.value = str; // Tell parent that title has been updated

      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.value
        });
      }
    }
  },
  created: function () {
    this._options = {
      bootstrapStyling: true,
      mondayFirst: true,
      format: "yyyy-MM-dd",
      typeable: true
    };
  },
  template: "\n<datepicker v-on:closed=\"onClosed($event)\" v-model=\"model.value\" :format=\"_options.format\" :monday-first=\"_options.mondayFirst\" :typeable=\"_options.typeable\" :bootstrap-styling=\"_options.bootstrapStyling\"></datepicker>\n"
});
Vue.component("document-field", {
  props: ["uid", "model", "meta"],
  methods: {
    select: function () {
      if (this.model.media != null) {
        piranha.mediapicker.open(this.update, "Document", this.model.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, "Document");
      }
    },
    remove: function () {
      this.model.id = null;
      this.model.media = null;
    },
    update: function (media) {
      if (media.type === "Document") {
        this.model.id = media.id;
        this.model.media = {
          id: media.id,
          folderId: media.folderId,
          type: media.type,
          filename: media.filename,
          title: media.title,
          contentType: media.contentType,
          publicUrl: media.publicUrl
        }; // Tell parent that title has been updated

        if (this.meta.notifyChange) {
          this.$emit('update-title', {
            uid: this.uid,
            title: this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename
          });
        }
      } else {
        console.log("No document was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.media == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename;
      } else {
        return "No document selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else-if=\"model.media.title != null\">\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.title }} ({{ model.media.filename }})</a>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.filename }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("html-field", {
  props: ["uid", "toolbar", "model", "meta"],
  data: function () {
    return {
      body: this.model.value
    };
  },
  methods: {
    onBlur: function (e) {
      this.model.value = tinyMCE.activeEditor.getContent(); // Tell parent that title has been updated

      if (this.meta.notifyChange) {
        var title = this.model.value.replace(/(<([^>]+)>)/ig, "");

        if (title.length > 40) {
          title = title.substring(0, 40) + "...";
        }

        this.$emit('update-title', {
          uid: this.uid,
          title: title
        });
      }
    },
    onChange: function (data) {
      this.model.value = data; // Tell parent that title has been updated

      if (this.meta.notifyChange) {
        var title = this.model.value.replace(/(<([^>]+)>)/ig, "");

        if (title.length > 40) {
          title = title.substring(0, 40) + "...";
        }

        if (this.meta.notifyChange) {
          this.$emit('update-title', {
            uid: this.uid,
            title: title
          });
        }
      }
    }
  },
  computed: {
    isEmpty: function () {
      return piranha.utils.isEmptyHtml(this.model.value);
    }
  },
  mounted: function () {
    piranha.editor.addInline(this.uid, this.toolbar, this.onChange);
  },
  beforeDestroy: function () {
    piranha.editor.remove(this.uid);
  },
  template: "\n<div class=\"field html-field\" :class=\"{ empty: isEmpty }\">\n    <div contenteditable=\"true\" :id=\"uid\" v-html=\"body\" v-on:blur=\"onBlur\"></div>\n</div>\n"
});
Vue.component("image-field", {
  props: ["uid", "model", "meta"],
  methods: {
    select: function () {
      if (this.model.media != null) {
        piranha.mediapicker.open(this.update, "Image", this.model.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, "Image");
      }
    },
    remove: function () {
      this.model.id = null;
      this.model.media = null;
    },
    update: function (media) {
      if (media.type === "Image") {
        this.model.id = media.id;
        this.model.media = {
          id: media.id,
          folderId: media.folderId,
          type: media.type,
          filename: media.filename,
          title: media.title,
          contentType: media.contentType,
          publicUrl: media.publicUrl
        }; // Tell parent that title has been updated

        if (this.meta.notifyChange) {
          this.$emit('update-title', {
            uid: this.uid,
            title: this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename
          });
        }
      } else {
        console.log("No image was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.media == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename;
      } else {
        return "No image selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else-if=\"model.media.title != null\">\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.title }} ({{ model.media.filename }})</a>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.filename }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("markdown-field", {
  props: ["uid", "model", "meta"],
  data: function () {
    return {
      body: this.model.value
    };
  },
  methods: {
    update: function (md) {
      this.model.value = md; // Tell parent that title has been updated

      if (this.meta && this.meta.notifyChange) {
        var title = this.model.value;

        if (title.length > 40) {
          title = title.substring(0, 40) + "...";
        }

        this.$emit('update-title', {
          uid: this.uid,
          title: title
        });
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.value == null || this.model.value === "";
    }
  },
  mounted: function () {
    var self = this;
    piranha.editor.addInlineMarkdown(self.uid, self.model.value, self.update);
  },
  beforeDestroy: function () {
    piranha.editor.remove(this.uid);
  },
  template: "\n<div class=\"markdown-field\" :class=\"{ empty: isEmpty }\">\n    <textarea :id=\"uid\" v-html=\"model.value\"></textarea>\n    <div class=\"markdown-preview\"></div>\n</div>\n"
});
Vue.component("media-field", {
  props: ["uid", "model", "meta"],
  methods: {
    select: function () {
      if (this.model.media != null) {
        piranha.mediapicker.open(this.update, null, this.model.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, null);
      }
    },
    remove: function () {
      this.model.id = null;
      this.model.media = null;
    },
    update: function (media) {
      this.model.id = media.id;
      this.model.media = {
        id: media.id,
        folderId: media.folderId,
        type: media.type,
        filename: media.filename,
        title: media.title,
        contentType: media.contentType,
        publicUrl: media.publicUrl
      }; // Tell parent that title has been updated

      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename
        });
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.media == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename;
      } else {
        return "No media selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else-if=\"model.media.title != null\">\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.title }} ({{ model.media.filename }})</a>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.filename }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("missing-field", {
  props: ["meta", "model"],
  template: "\n<div class=\"alert alert-danger text-center\" role=\"alert\">No component registered for <code>{{ meta.type }}</code></div>\n"
});
Vue.component("number-field", {
  props: ["uid", "model", "meta"],
  template: "\n<input class=\"form-control\" type=\"text\" :placeholder=\"meta.placeholder\" v-model=\"model.value\">\n"
});
Vue.component("page-field", {
  props: ["uid", "model", "meta"],
  methods: {
    select: function () {
      var siteId = null;

      if (this.model.page) {
        siteId = this.model.page.siteId;
      } else if (piranha.pageedit) {
        siteId = piranha.pageedit.siteId;
      }

      piranha.pagepicker.open(this.update, siteId);
    },
    remove: function () {
      this.model.id = null;
      this.model.page = null;
    },
    update: function (page) {
      this.model.id = page.id;
      this.model.page = page; // Tell parent that title has been updated

      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.page.title
        });
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.page == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.page != null) {
        return this.model.page.title;
      } else {
        return "No page selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a :href=\"piranha.baseUrl + 'manager/page/edit/' + model.page.id\" target=\"_blank\">{{ model.page.title }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("post-field", {
  props: ["uid", "model", "meta"],
  methods: {
    select: function () {
      piranha.postpicker.open(this.update);
    },
    remove: function () {
      this.model.id = null;
      this.model.post = null;
    },
    update: function (post) {
      this.model.id = post.id;
      this.model.post = post; // Tell parent that title has been updated

      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.post.title
        });
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.post == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.post != null) {
        return this.model.post.title;
      } else {
        return "No post selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a :href=\"piranha.baseUrl + 'manager/post/edit/' + model.post.id\" target=\"_blank\">{{ model.post.title }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});
Vue.component("readonly-field", {
  props: ["uid", "model", "meta"],
  template: "\n<div class=\"alert alert-secondary mb-0\">\n    <pre class=\"mb-0\">{{ model.value }}</pre>\n</div>\n"
});
Vue.component("select-field", {
  props: ["uid", "model", "meta"],
  methods: {
    update: function () {
      // Tell parent that title has been updated
      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.meta.options[this.model.value]
        });
      }
    }
  },
  template: "\n<select class=\"form-control\" v-model=\"model.value\" v-on:change=\"update()\">\n    <option v-for=\"(name, value) in meta.options\" v-bind:key=\"value\" v-bind:value=\"value\">\n        {{ name }}\n    </option>\n</select>\n"
});
Vue.component("string-field", {
  props: ["uid", "model", "meta"],
  methods: {
    update: function () {
      // Tell parent that title has been updated
      if (this.meta.notifyChange) {
        this.$emit('update-title', {
          uid: this.uid,
          title: this.model.value
        });
      }
    },
    maxLength: function () {
      return this.meta.settings.MaxLength != null && this.meta.settings.MaxLength > 0 ? this.meta.settings.MaxLength : null;
    },
    isRequired: function () {
      return false; //return this.meta.settings.IsRequired != null && this.meta.settings.IsRequired;
    }
  },
  template: "\n<div>\n    <div v-if=\"maxLength() > 0\" class=\"input-group\">\n        <input class=\"form-control\" type=\"text\" :maxlength=\"maxLength()\" :required=\"isRequired()\" :placeholder=\"meta.placeholder\" v-model=\"model.value\" v-on:change=\"update()\">\n        <div class=\"input-group-append\">\n            <div class=\"input-group-text text-muted\">\n                {{ piranha.utils.strLength(model.value) + \"/\" + maxLength() }}\n            </div>\n        </div>\n    </div>\n    <input v-else class=\"form-control\" type=\"text\" :maxlength=\"maxLength()\" :required=\"isRequired()\" :placeholder=\"meta.placeholder\" v-model=\"model.value\" v-on:change=\"update()\">\n</div>\n"
});
Vue.component("text-field", {
  props: ["uid", "model", "meta"],
  methods: {
    update: function () {
      // Tell parent that title has been updated
      if (this.meta.notifyChange) {
        var title = this.model.value;

        if (title.length > 40) {
          title = title.substring(0, 40) + "...";
        }

        this.$emit('update-title', {
          uid: this.uid,
          title: title
        });
      }
    },
    maxLength: function () {
      return this.meta.settings.MaxLength != null && this.meta.settings.MaxLength > 0 ? this.meta.settings.MaxLength : null;
    },
    isRequired: function () {
      return false; //return this.meta.settings.IsRequired != null && this.meta.settings.IsRequired;
    }
  },
  template: "\n<div>\n    <div v-if=\"maxLength() > 0\" class=\"input-group\">\n        <textarea class=\"form-control\" rows=\"4\" :maxlength=\"maxLength()\" :required=\"isRequired()\" :placeholder=\"meta.placeholder\" v-model=\"model.value\" v-on:change=\"update()\"></textarea>\n        <div class=\"input-group-append\">\n            <div class=\"input-group-text text-muted\">\n                {{ piranha.utils.strLength(model.value) + \"/\" + maxLength() }}\n            </div>\n        </div>\n    </div>\n    <textarea v-else class=\"form-control\" rows=\"4\" :maxlength=\"maxLength()\" :required=\"isRequired()\" :placeholder=\"meta.placeholder\" v-model=\"model.value\" v-on:change=\"update()\"></textarea>\n</div>\n"
});
Vue.component("video-field", {
  props: ["uid", "model", "meta"],
  methods: {
    clear: function () {// clear media from block
    },
    select: function () {
      if (this.model.media != null) {
        piranha.mediapicker.open(this.update, "Video", this.model.media.folderId);
      } else {
        piranha.mediapicker.openCurrentFolder(this.update, "Video");
      }
    },
    remove: function () {
      this.model.id = null;
      this.model.media = null;
    },
    update: function (media) {
      if (media.type === "Video") {
        this.model.id = media.id;
        this.model.media = {
          id: media.id,
          folderId: media.folderId,
          type: media.type,
          filename: media.filename,
          title: media.title,
          contentType: media.contentType,
          publicUrl: media.publicUrl
        }; // Tell parent that title has been updated

        if (this.meta.notifyChange) {
          this.$emit('update-title', {
            uid: this.uid,
            title: this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename
          });
        }
      } else {
        console.log("No video was selected");
      }
    }
  },
  computed: {
    isEmpty: function () {
      return this.model.media == null;
    }
  },
  mounted: function () {
    this.model.getTitle = function () {
      if (this.model.media != null) {
        return this.model.media.title != null ? this.model.media.title + ' (' + this.model.media.filename + ')' : this.model.media.filename;
      } else {
        return "No video selected";
      }
    };
  },
  template: "\n<div class=\"media-field\" :class=\"{ empty: isEmpty }\">\n    <div class=\"media-picker\">\n        <div class=\"btn-group float-right\">\n            <button v-on:click.prevent=\"select\" class=\"btn btn-primary text-center\">\n                <i class=\"fas fa-plus\"></i>\n            </button>\n            <button v-on:click.prevent=\"remove\" class=\"btn btn-danger text-center\">\n                <i class=\"fas fa-times\"></i>\n            </button>\n        </div>\n        <div class=\"card text-left\">\n            <div class=\"card-body\" v-if=\"isEmpty\">\n                <span v-if=\"meta.placeholder != null\" class=\"text-secondary\">{{ meta.placeholder }}</span>\n                <span v-if=\"meta.placeholder == null\" class=\"text-secondary\">&nbsp;</span>\n            </div>\n            <div class=\"card-body\" v-else-if=\"model.media.title != null\">\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.title }} ({{ model.media.filename }})</a>\n            </div>\n            <div class=\"card-body\" v-else>\n                <a href=\"#\" v-on:click.prevent=\"piranha.preview.open(model.id)\">{{ model.media.filename }}</a>\n            </div>\n        </div>\n    </div>\n</div>\n"
});