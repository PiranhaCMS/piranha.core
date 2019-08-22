Vue.component("block-group-horizontal", {
    props: ["uid", "toolbar", "model"],
    methods: {
        removeItem: function (item) {
            var itemIndex = this.model.items.indexOf(item);

            this.model.items.splice(itemIndex, 1);
        },
        addGroupBlock: function (type, pos) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
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
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        moveItem: function (from, to) {
            this.model.items.splice(to, 0, this.model.items.splice(from, 1)[0])
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
    beforeDestroy: function () {
    },
    template:
        "<div :id='uid' class='block-group'>" +
        "  <div class='actions block-group-actions'>" +
        "    <button v-on:click.prevent='piranha.blockpicker.open(addGroupBlock, 0, model.type)' class='btn btn-sm add'>" +
        "      <i class='fas fa-plus'></i>" +
        "    </button>" +
        "  </div>" +
        "  <div class='block-group-header'>" +
        "    <div class='form-group' v-for='field in model.fields'>" +
        "      <label>{{ field.meta.name }}</label>" +
        "      <component v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:toolbar='toolbar' v-bind:model='field.model'></component>" +
        "    </div>" +
        "  </div>" +
        "  <div class='row block-group-items'>" +
        "    <div v-if='model.items.length === 0' class='col'>" +
        "      <div class='empty-info unsortable'>" +
        "        <p>{{ piranha.resources.texts.emptyAddAbove }}</p>" +
        "      </div>" +
        "    </div>" +
        "    <div v-for='child in model.items' v-bind:key='child.meta.uid' class='col'>" +
        "      <div :class='\"block \" + child.meta.component'>" +
        "        <div class='block-header'>" +
        "          <div class='title'>" +
        "            <i :class='child.meta.icon'></i><strong>{{ child.meta.name }}</strong>" +
        "          </div>" +
        "          <div class='actions'>" +
        "            <span class='btn btn-sm handle'>" +
        "              <i class='fas fa-ellipsis-v'></i>" +
        "            </span>" +
        "            <button v-on:click.prevent='removeItem(child)' class='btn btn-sm danger' tabindex='-1'>" +
        "              <i class='fas fa-trash'></i>" +
        "            </button>" +
        "          </div>" +
        "        </div>" +
        "        <component v-bind:is='child.meta.component' v-bind:uid='child.meta.uid' v-bind:toolbar='toolbar' v-bind:model='child.model'></component>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
