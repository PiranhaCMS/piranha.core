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
        "    <div class='link'>" +
        "      <span class='actions'></span>" +
        "      <a :href='piranha.baseUrl + \"manager/page/copyrelative/\" + item.id + \"/\" + piranha.pagelist.addPageId + \"/\" + piranha.pagelist.addAfter'>" +
        "        {{ item.title }}" +
        "      </a>" +
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
