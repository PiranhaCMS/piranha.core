Vue.component("region", {
    props: ["model"],
    methods: {
        moveItem: function (from, to) {
            this.model.items.splice(to, 0, this.model.items.splice(from, 1)[0])
        },
        addItem: function () {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/content/page/region/" + piranha.pageedit.typeId + "/" + this.model.meta.name)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.model.items.push(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        removeItem: function (item) {
            this.model.items.splice(this.model.items.indexOf(item), 1);
        },
        updateTitle: function (e) {
            for (var n = 0; n < this.model.items.length; n++) {
                if (this.model.items[n].uid === e.uid) {
                    this.model.items[n].title = e.title;
                    break;
                }
            }
        },
    },
    mounted: function () {
        if (this.model.meta.isCollection)
        {
            var self = this;

            sortable("#" + this.model.meta.uid, {
                handle: '.card-header a:first-child',
                items: ":not(.unsortable)"
            })[0].addEventListener("sortupdate", function (e) {
                self.moveItem(e.detail.origin.index, e.detail.destination.index);
            });
        }
    },
    template:
        "<div class='row' v-if='!model.meta.isCollection'>" +
        "  <div class='col-sm-12' v-if='model.meta.description != null'>" +
        "    <div class='alert alert-info'>" +
        "      {{ model.meta.description }}" +
        "    </div>" +
        "  </div>" +
        "  <div class='form-group' :class='{ \"col-sm-6\": field.meta.isHalfWidth, \"col-sm-12\": !field.meta.isHalfWidth }' v-for='field in model.items[0].fields'>" +
        "    <label>{{ field.meta.name }}</label>" +
        "    <component v-if='field.model != null' v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:model='field.model'></component>" +
        "  </div>" +
        "</div>" +
        "<div v-else>" +
        "  <div v-if='model.meta.description != null'>" +
        "    <div class='alert alert-info'>" +
        "      {{ model.meta.description }}" +
        "    </div>" +
        "  </div>" +
        "  <div :id='model.meta.uid' class='accordion sortable mb-3'>" +
        "    <div v-if='model.items.length === 0' class='empty-info unsortable'>" +
        "      <p>Looks like there's no items here. Click on the button below to get started!</p>" +
        "    </div>" +
        "    <div class='card' :key='item.uid' v-for='(item, index) in model.items'>" +
        "      <div class='card-header'>" +
        "        <a href='#' data-toggle='collapse' :data-target='\"#body\" + item.uid'>" +
        "          <div class='handle'>" +
        "            <i class='fas fa-ellipsis-v'></i>" +
        "          </div>" +
        "          {{ item.title }}" +
        "        </a>" +
        "        <span class='actions float-right'>" +
        "          <a v-on:click.prevent='removeItem(item)' href='#' class='danger'><i class='fas fa-trash'></i></a>" +
        "        </span>" +
        "      </div>" +
        "      <div :id='\"body\" + item.uid' class='collapse' :data-parent='\"#\" + model.meta.uid'>" +
        "        <div class='card-body'>" +
        "          <div class='row'>" +
        "            <div class='form-group' :class='{ \"col-sm-6\": field.meta.isHalfWidth, \"col-sm-12\": !field.meta.isHalfWidth }' v-for='field in item.fields'>" +
        "              <label>{{ field.meta.name }}</label>" +
        "              <component v-if='field.model != null' v-bind:is='field.meta.component' v-bind:uid='item.uid' v-bind:meta='field.meta' v-bind:model='field.model' v-on:update-field='updateTitle($event)'></component>" +
        "            </div>" +
        "          </div>" +
        "        </div>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "  <button class='btn btn-primary btn-labeled' v-on:click.prevent='addItem()'>" +
        "    <i class='fas fa-plus'></i>Add item" +
        "  </button>" +
        "</div>"
});
