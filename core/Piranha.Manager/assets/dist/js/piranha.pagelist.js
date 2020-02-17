Vue.component("pagecopy-item", {
  props: ["item"],
  methods: {
    toggleItem: function (item) {
      item.isExpanded = !item.isExpanded;
    }
  },
  template: "\n<li class=\"dd-item\" :class=\"{ expanded: item.isExpanded || item.items.length === 0 }\">\n    <div class=\"sitemap-item expanded\">\n        <div class=\"link\" :class=\"{ readonly: item.isCopy }\">\n            <a v-if=\"!item.isCopy\" :href=\"piranha.baseUrl + 'manager/page/copyrelative/' + item.id + '/' + piranha.pagelist.addPageId + '/' + piranha.pagelist.addAfter\">\n                {{ item.title }}\n            </a>\n            <a href=\"#\" v-else>\n                {{ item.title }}\n                <span v-if=\"item.isCopy\" class=\"badge badge-warning\">{{ piranha.resources.texts.copy }}</span>\n            </a>\n            <div class=\"content-blocker\"></div>\n        </div>\n        <div class=\"type d-none d-md-block\">\n            {{ item.typeName }}\n        </div>\n    </div>\n    <ol class=\"dd-list\" v-if=\"item.items.length > 0\">\n        <pagecopy-item v-for=\"child in item.items\" v-bind:key=\"child.id\" v-bind:item=\"child\"></pagecopy-item>\n    </ol>\n</li>\n"
});
Vue.component("sitemap-item", {
  props: ["item"],
  methods: {
    toggleItem: function (item) {
      item.isExpanded = !item.isExpanded;
    }
  },
  template: "\n<li class=\"dd-item\" :class=\"{ expanded: item.isExpanded || item.items.length === 0 }\" :data-id=\"item.id\">\n    <div class=\"sitemap-item\">\n        <div class=\"handle dd-handle\"><i class=\"fas fa-ellipsis-v\"></i></div>\n        <div class=\"link\">\n            <span class=\"actions\">\n                <a v-if=\"item.items.length > 0 && item.isExpanded\" v-on:click.prevent=\"toggleItem(item)\" class=\"expand\" href=\"#\"><i class=\"fas fa-minus\"></i></a>\n                <a v-if=\"item.items.length > 0 && !item.isExpanded\" v-on:click.prevent=\"toggleItem(item)\" class=\"expand\" href=\"#\"><i class=\"fas fa-plus\"></i></a>\n            </span>\n            <a v-if=\"piranha.permissions.pages.edit\" :href=\"piranha.baseUrl + item.editUrl + item.id\">\n                <span v-html=\"item.title\"></span>\n                <span v-if=\"item.status\" class=\"badge badge-info\">{{ item.status }}</span>\n                <span v-if=\"item.isCopy\" class=\"badge badge-warning\">{{ piranha.resources.texts.copy }}</span>\n            </a>\n            <span v-else class=\"title\">\n                <span v-html=\"item.title\"></span>\n                <span v-if=\"item.status\" class=\"badge badge-info\">{{ item.status }}</span>\n                <span v-if=\"item.isCopy\" class=\"badge badge-warning\">{{ piranha.resources.texts.copy }}</span>\n            </span>\n        </div>\n        <div class=\"type d-none d-md-block\">{{ item.typeName }}</div>\n        <div class=\"date d-none d-lg-block\">{{ item.published }}</div>\n        <div class=\"actions\">\n            <a v-if=\"piranha.permissions.pages.add\" href=\"#\" v-on:click.prevent=\"piranha.pagelist.add(item.siteId, item.id, true)\"><i class=\"fas fa-angle-down\"></i></a>\n            <a v-if=\"piranha.permissions.pages.add\" href=\"#\" v-on:click.prevent=\"piranha.pagelist.add(item.siteId, item.id, false)\"><i class=\"fas fa-angle-right\"></i></a>\n            <a v-if=\"piranha.permissions.pages.delete && item.items.length === 0\" v-on:click.prevent=\"piranha.pagelist.remove(item.id)\" class=\"danger\" href=\"#\"><i class=\"fas fa-trash\"></i></a>\n        </div>\n    </div>\n    <ol v-if=\"item.items.length > 0\" class=\"dd-list\">\n        <sitemap-item v-for=\"child in item.items\" v-bind:key=\"child.id\" v-bind:item=\"child\">\n        </sitemap-item>\n    </ol>\n</li>\n"
});
/*global
    piranha
*/

piranha.pagelist = new Vue({
    el: "#pagelist",
    data: {
        loading: true,
        updateBindings: false,
        items: [],
        sites: [],
        pageTypes: [],
        addSiteId: null,
        addSiteTitle: null,
        addPageId: null,
        addAfter: true
    },
    methods: {
        load: function () {
            var self = this;
            console.log(piranha.baseUrl + "manager/api/page/list");
            piranha.permissions.load(function () {
                fetch(piranha.baseUrl + "manager/api/page/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.sites = result.sites;
                    self.pageTypes = result.pageTypes;
                    self.updateBindings = true;
                })
                .catch(function (error) { console.log("error:", error ); });
            });
        },
        remove: function (id) {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deletePageConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/page/delete/" + id)
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        self.load();
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
        },
        bind: function () {
            var self = this;

            $(".sitemap-container").each(function (i, e) {
                $(e).nestable({
                    maxDepth: 100,
                    group: i,
                    callback: function (l, e) {
                        fetch(piranha.baseUrl + "manager/api/page/move", {
                            method: "post",
                            headers: {
                                "Content-Type": "application/json"
                            },
                            body: JSON.stringify({
                                id: $(e).attr("data-id"),
                                items: $(l).nestable("serialize")
                            })
                        })
                        .then(function (response) { return response.json(); })
                        .then(function (result) {
                            piranha.notifications.push(result.status);

                            if (result.status.type === "success") {
                                $('.sitemap-container').nestable('destroy');
                                self.sites = [];
                                Vue.nextTick(function () {
                                    self.sites = result.sites;
                                    Vue.nextTick(function () {
                                        self.bind();
                                    });
                                });
                            }
                        })
                        .catch(function (error) {
                            console.log("error:", error);
                        });
                    }
                })
            });
        },
        add: function (siteId, pageId, after) {
            var self = this;

            self.addSiteId = siteId;
            self.addPageId = pageId;
            self.addAfter = after;

            // Get the site title
            self.sites.forEach(function (e) {
                if (e.id === siteId) {
                    self.addSiteTitle = e.title;
                }
            });

            // Open the modal
            $("#pageAddModal").modal("show");
        },
        selectSite: function (siteId) {
            var self = this;

            self.addSiteId = siteId;

            // Get the site title
            self.sites.forEach(function (e) {
                if (e.id === siteId) {
                    self.addSiteTitle = e.title;
                }
            });
        }
    },
    created: function () {
    },
    updated: function () {
        if (this.updateBindings)
        {
            this.bind();
            this.updateBindings = false;
        }

        this.loading = false;
    }
});
