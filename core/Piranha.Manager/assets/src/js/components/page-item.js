/*global
    piranha
*/

Vue.component("page-item", {
    props: ["item"],
    template:
        "<li :data-id='item.id'>" +
        "  <div class='sitemap-item expanded'>" +
        "    <div class='link'>" +
        "      <a href='#' v-on:click.prevent='piranha.pagepicker.select(item)'>" +
        "        {{ item.title }}" +
        "      </a>" +
        "    </div>" +
        "    <div class='type d-none d-md-block'>" +
        "      {{ item.pageTypeName }}" +
        "    </div>" +
        "  </div>" +
        "  <ol v-if='item.items.length > 0' class='dd-list'>" +
        "    <page-item v-for='child in item.items' v-bind:key='child.id' v-bind:item='child'>" +
        "    </page-item>" +
        "  </ol>" +
        "</li>"
});
