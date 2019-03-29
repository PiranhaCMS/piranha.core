Vue.component("block-group", {
    props: ["uid", "block"],
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
            var itemActive = item.isActive;
            var itemIndex = this.block.items.indexOf(item);

            this.block.items.splice(itemIndex, 1);

            if (itemActive) {
                this.selectItem(this.block.items[Math.min(itemIndex, this.block.items.length - 1)]);
            }
        },
        addGroupBlock: function (type, pos) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.block.items.push(result);
                    self.selectItem(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
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
        "        <div class='list-group-item' :class='{ active: child.isActive }' v-for='child in block.items' v-bind:key='child.uid'>" +
        "          <a href='#' v-on:click.prevent='selectItem(child)'>" +
        "            <span class='handle sortable-handle'>" +
        "              <i class='fas fa-ellipsis-v'></i>" +
        "            </span>" +
        "            List item" +
        "          </a>" +
        "          <span class='actions float-right'>" +
        "            <a v-on:click.prevent='removeItem(child)' href='#' class='danger'><i class='fas fa-trash'></i></a>" +
        "          </span>" +
        "        </div>" +
        "      </div>" +
        "      <button v-on:click.prevent='piranha.blockpicker.open(addGroupBlock, 0, block.type)' class='btn btn-sm btn-primary btn-labeled mt-3'><i class='fas fa-plus'></i>Add item</button>" +
        "    </div>" +
        "    <div class='col-md-8'>" +
        "      <div v-for='child in block.items' v-if='child.isActive' :class='\"block \" + child.component'>" +
        "        <component v-bind:is='child.component' v-bind:uid='child.uid' v-bind:block='child.item'></component>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
