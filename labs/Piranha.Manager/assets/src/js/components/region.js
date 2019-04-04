Vue.component("region", {
    props: ["model"],
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
        "  <div :id='model.meta.uid' class='accordion'>" +
        "    <div class='card' v-for='item in model.items'>" +
        "      <div class='card-header'>" +
        "        <a href='#' data-toggle='collapse' :data-target='\"#body\" + item.uid'>List Item Title</a>" +
        "      </div>" +
        "      <div :id='\"body\" + item.uid' class='collapse' :data-parent='\"#\" + model.meta.uid'>" +
        "        <div class='card-body'>" +
        "          <div class='row'>" +
        "            <div class='form-group' :class='{ \"col-sm-6\": field.meta.isHalfWidth, \"col-sm-12\": !field.meta.isHalfWidth }' v-for='field in item.fields'>" +
        "              <label>{{ field.meta.name }}</label>" +
        "              <component v-if='field.model != null' v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:model='field.model'></component>" +
        "            </div>" +
        "          </div>" +
        "        </div>" +
        "      </div>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
