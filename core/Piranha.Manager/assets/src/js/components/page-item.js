/*global
    piranha
*/

Vue.component("page-item", {
    props: ["item"],
    template:
        "<li class='dd-item' :data-id='item.id'>" +
        "  <div class='sitemap-item'>" +
        "    <div class='link'>" +
        "      <a href='#' v-on:click.prevent='piranha.pagepicker.select(item)'>" +
        "        {{ item.title }}" +
        "      </a>" +
        "    </div>" +
        "  </div>" +
        "  <ol v-if='item.items.length > 0' class='dd-list'>" +
        "    <page-item v-for='child in item.items' v-bind:key='child.id' v-bind:item='child'>" +
        "    </page-item>" +
        "  </ol>" +
        "</li>"
});
