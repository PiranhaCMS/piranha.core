Vue.component("block-group-vertical", {
    props: ["uid", "toolbar", "model"],
    methods: {
        collapseItem: function (item) {
            item.meta.isCollapsed = !item.meta.isCollapsed;
        },
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

                    //self.model.items.push(result.body);
                    self.model.items.splice(pos, 0, result.body);

                    Vue.nextTick(function () {
                        sortable("#" + self.uid + " .block-group-items", {
                            handle: '.handle',
                            items: ":not(.unsortable)",
                            placeholderClass: "sortable-placeholder"
                        })[0].addEventListener("sortupdate", function (e) {
                            self.moveItem(e.detail.origin.index, e.detail.destination.index);
                        });
                    });
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        toggleHeader: function () {
            this.model.meta.showHeader = !this.model.meta.showHeader;
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
            placeholderClass: "sortable-placeholder"
        })[0].addEventListener("sortupdate", function (e) {
            self.moveItem(e.detail.origin.index, e.detail.destination.index);
        });
    },
    beforeDestroy: function () {
    },
    template:
        "<div :id='uid' class='block-group'>" +
        "  <div class='actions block-group-actions'>" +
        "    <button v-on:click.prevent='toggleHeader()' v-if='model.fields.length > 0' class='btn btn-sm' :class='{ selected: model.meta.showHeader }'>" +
        "      <i class='fas fa-list'></i>" +
        "    </button>" +
        "  </div>" +
        "  <div v-if='model.meta.showHeader && model.fields.length > 0' class='block-group-header'>" +
        "    <div class='row'>" +
        "      <div class='form-group' :class='{ \"col-sm-6\": field.meta.isHalfWidth, \"col-sm-12\": !field.meta.isHalfWidth }' v-for='field in model.fields'>" +
        "        <label>{{ field.meta.name }}</label>" +
        "        <div v-if='field.meta.description != null' v-html='field.meta.description' class='field-description small text-muted'></div>" +
        "        <component v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:toolbar='toolbar' v-bind:model='field.model'></component>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "  <div class='block-group-items'>" +
        "    <a href='#' class='block-add unsortable' v-on:click.prevent='piranha.blockpicker.open(addGroupBlock, 0, model.type)'>" +
        "      <hr>" +
        "      <i class='fas fa-plus-circle'></i>" +
        "    </a>" +
        "    <div v-if='model.items.length === 0' class='col'>" +
        "      <div class='empty-info unsortable'>" +
        "        <p>{{ piranha.resources.texts.emptyAddAbove }}</p>" +
        "      </div>" +
        "    </div>" +
        "    <div v-for='(child, index) in model.items' v-bind:key='child.meta.uid'>" +
        "      <div :class='\"block \" + child.meta.component + \" \" + (child.meta.isCollapsed ? \"collapsed\" : \"\")'>" +
        "        <div class='block-header'>" +
        "          <div class='title'>" +
        "            <i :class='child.meta.icon'></i><strong>{{ child.meta.name }}</strong>" +
        "          </div>" +
        "          <div class='actions'>" +
        "            <span v-on:click.prevent='collapseItem(child)' class='btn btn-sm'>" +
        "              <i v-if='child.meta.isCollapsed' class='fas fa-chevron-down'></i>" +
        "              <i v-else class='fas fa-chevron-up'></i>" +
        "            </span>" +
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
        "      <a href='#' class='block-add unsortable' v-on:click.prevent='piranha.blockpicker.open(addGroupBlock, index + 1, model.type)'>" +
        "        <hr>" +
        "        <i class='fas fa-plus-circle'></i>" +
        "      </a>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
