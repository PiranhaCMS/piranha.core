/*global
    piranha
*/

Vue.component("pagecopy-item", {
    props: ["item"],
    methods: {
        toggleItem: function (item) {
            item.isExpanded = !item.isExpanded;
        }
    },
    template:
        "<li class='dd-item' :class='{ expanded: item.isExpanded || item.items.length === 0 }'>" +
        "  <div class='sitemap-item expanded'>" +
        "    <div class='link' :class='{ readonly: item.isCopy }'>" +
        "      <a v-if='!item.isCopy' :href='piranha.baseUrl + \"manager/page/copyrelative/\" + item.id + \"/\" + piranha.pagelist.addPageId + \"/\" + piranha.pagelist.addAfter'>" +
        "        {{ item.title }}" +
        "      </a>" +
        "      <a href='#' v-else>" +
        "        {{ item.title }}" +
        "        <span v-if='item.isCopy' class='badge badge-warning'>{{ piranha.resources.texts.copy }}</span>" +
        "      </a>" +
        "      <div class='content-blocker'></div>" +
        "    </div>" +
        "    <div class='type d-none d-md-block'>" +
        "      {{ item.typeName }}" +
        "    </div>" +
        "  </div>" +
        "  <ol class='dd-list' v-if='item.items.length > 0' class='dd-list'>" +
        "    <pagecopy-item v-for='child in item.items' v-bind:key='child.id' v-bind:item='child'>" +
        "    </page-item>" +
        "  </ol>" +
        "</li>"
});

/*global
    piranha
*/

Vue.component("sitemap-item", {
    props: ["item"],
    methods: {
        toggleItem: function (item) {
            item.isExpanded = !item.isExpanded;
        }
    },
    template:
        "<li class='dd-item' :class='{ expanded: item.isExpanded || item.items.length === 0 }' :data-id='item.id'>" +
        "  <div class='sitemap-item'>" +
        "    <div class='handle dd-handle'><i class='fas fa-ellipsis-v'></i></div>" +
        "    <div class='link'>" +
        "      <span class='actions'>" +
        "        <a v-if='item.items.length > 0 && item.isExpanded' v-on:click.prevent='toggleItem(item)' class='expand' href='#'><i class='fas fa-minus'></i></a>" +
        "        <a v-if='item.items.length > 0 && !item.isExpanded' v-on:click.prevent='toggleItem(item)' class='expand' href='#'><i class='fas fa-plus'></i></a>" +
        "      </span>" +
        "      <a :href='piranha.baseUrl + item.editUrl + item.id'>" +
        "        <span v-html='item.title'></span>" +
        "        <span v-if='item.status' class='badge badge-info'>{{ item.status }}</span>" +
        "        <span v-if='item.isCopy' class='badge badge-warning'>{{ piranha.resources.texts.copy }}</span>" +
        "      </a>" +
        "    </div>" +
        "    <div class='type d-none d-md-block'>{{ item.typeName }}</div>" +
        "    <div class='date d-none d-lg-block'>{{ item.published }}</div>" +
        "    <div class='actions'>" +
        "      <a href='#' v-on:click.prevent='piranha.pagelist.add(item.siteId, item.id, true)'><i class='fas fa-angle-down'></i></a>" +
        "      <a href='#' v-on:click.prevent='piranha.pagelist.add(item.siteId, item.id, false)'><i class='fas fa-angle-right'></i></a>" +
        "      <a v-if='item.items.length === 0' v-on:click.prevent='piranha.pagelist.remove(item.id)' class='danger' href='#'><i class='fas fa-trash'></i></a>" +
        "    </div>" +
        "  </div>" +
        "  <ol v-if='item.items.length > 0' class='dd-list'>" +
        "    <sitemap-item v-for='child in item.items' v-bind:key='child.id' v-bind:item='child'>" +
        "    </sitemap-item>" +
        "  </ol>" +
        "</li>"
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
            fetch(piranha.baseUrl + "manager/api/page/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pagelist.sites = result.sites;
                    piranha.pagelist.pageTypes = result.pageTypes;

                    piranha.pagelist.updateBindings = true;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        remove: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/delete/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.notifications.push(result);

                    self.load();
                })
                .catch(function (error) { console.log("error:", error ); });
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
                                "Content-Type": "application/json",
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
