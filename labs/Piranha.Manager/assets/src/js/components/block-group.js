Vue.component("block-group", {
    props: ["uid", "block", "index"],
    methods: {
        selectItem: function (item) {
            for (var n = 0; n < this.block.items.length; n++) {
                if (this.block.items[n] == item) {
                    this.block.items[n].isActive = true;
                } else {
                    this.block.items[n].isActive = false;
                }
            }
        },
        removeItem: function (item) {
            this.block.items.splice(this.block.items.indexOf(item), 1);
        }
    },
    mounted: function () {
    },
    beforeDestroy: function () {
    },
    template:
        "<div class='block-group'>" +
        "  <div class='block-group-header'>" +
        "    TODO: Global group fields" +
        "  </div>" +
        "  <div class='row'>" +
        "    <div class='col-md-4'>" +
        "      <div class='list-group list-group-flush'>" +
        "        <a href='#' v-on:click.prevent='selectItem(child)' class='list-group-item' :class='{ active: child.isActive }' v-for='child in block.items'>" +
        "          <span class='handle sortable-handle'>" +
        "            <i class='fas fa-ellipsis-v'></i>" +
        "          </span>" +
        "          List item" +
        "          <span class='actions float-right'>" +
        "            <a v-on:click.prevent='removeItem(child)' href='#' class='danger'><i class='fas fa-trash'></i></a>" +
        "          </span>" +
        "        </a>" +
        "      </div>" +
        "      <button v-on:click.prevent='piranha.blockpicker.open(index, piranha.pageedit.addGroupBlock)' class='btn btn-sm btn-primary btn-labeled mt-3'><i class='fas fa-plus'></i>Add item</button>" +
        "    </div>" +
        "    <div class='col-md-8'>" +
        "      <div v-for='child in block.items' v-if='child.isActive' :class='\"block \" + child.component'>" +
        "        <component v-bind:is='child.component' v-bind:uid='child.uid' v-bind:block='child.item'></component>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
