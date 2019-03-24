Vue.component("sitemap-item", {
    props: ["item"],
    template:
        "<li class='dd-item'>" +
        "  <div class='sitemap-item'>" +
        "    <div class='handle dd-handle'><i class='fas fa-ellipsis-v'></i></div>" +
        "    <div class='link'><a href='{{ item.id }}'>{{ item.title }}</a></div>" +
        "    <div class='type d-none d-md-block'>{{ item.typeName }}</div>" +
        "    <div class='date d-none d-md-block'>{{ item.published }}</div>" +
        "    <div class='actions'>" +
        "      <a href='#'><i class='fas fa-angle-down'></i></a>" +
        "      <a href='#'><i class='fas fa-angle-right'></i></a>" +
        "      <a v-if='item.items.length === 0' v-on:click.prevent='piranha.pagelist.remove(item.id)' class='danger' href='#'><i class='fas fa-trash'></i></a>" +
        "    </div>" +
        "  </div>" +
        "  <ol v-if='item.items.length > 0' class='dd-list'>" +
        "    <sitemap-item v-for='child in item.items' v-bind:key='child.id' v-bind:item='child'>" +
        "    </sitemap-item>" +
        "  </ol>" +
        "</li>"
});

piranha.pagelist = new Vue({
    el: "#pagelist",
    data: {
        items: []
    },
    methods: {
        load: function () {
            fetch(piranha.baseUrl + "manager/api/page/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pagelist.items = result.items;
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        remove: function (id) {
            console.log("Remove page: ", id);
        }
    },
    created: function () {
        this.load();
    }
});
