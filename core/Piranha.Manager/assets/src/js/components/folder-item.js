/*global
    piranha
*/

Vue.component("folder-item", {
    props: ["item", "selected"],
    methods: {
        toggleItem: function (item) {
            item.isExpanded = !item.isExpanded;
        }
    },
    template:
        "<li class='dd-item expanded' :class='{ active: item.id === selected, expanded: item.isExpanded || item.items.length === 0 }' :data-id='item.id'>" +
        "  <a class='droppable' v-on:click.prevent='piranha.media.load(item.id)' href='#' v-on:dragover='piranha.media.dragover' v-on:dragleave='piranha.media.dragleave' v-on:drop='piranha.media.drop($event, item.id)'>" +
        "    <i class='fas fa-folder'></i>{{ item.name }}" +
        "    <span class='badge badge-light float-right'>{{ item.mediaCount }}</span>" +
        "  </a>" +
        "  <ol v-if='item.items.length > 0' class='dd-list'>" +
        "    <folder-item v-for='child in item.items' v-bind:key='child.id' v-bind:selected='selected' v-bind:item='child'></folder-item>" +
        "  </ol>" +
        "</li>"
});
