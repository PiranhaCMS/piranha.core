Vue.component("region", {
    props: ["model"],
    template:
        "<div class='region' v-if='!model.meta.isCollection'>" +
        "  <div class='alert alert-info' v-if='model.meta.description != null'>" +
        "    {{ model.meta.description }}" +
        "  </div>" +
        "  <div class='form-group' v-for='field in model.items[0].fields'>" +
        "    <label>{{ field.meta.name }}</label>" +
        "    <component v-if='field.model != null' v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:model='field.model'></component>" +
        "  </div>" +
        "</div>" +
        "<div class='list-group region-list' v-else>" +
        "  <div class='alert alert-info' v-if='model.meta.description != null'>" +
        "    {{ model.meta.description }}" +
        "  </div>" +
        "  <div class='list-group-item region-list-item' v-for='item in model.items'>" +
        "    <div class='region-list-item-title'>" +
        "      <a data-toggle='collapse'>List Item Title</a>" +
        "    </div>" +
        "    <div class='form-group' v-for='field in item.fields'>" +
        "      <label>{{ field.meta.name }}</label>" +
        "      <component v-if='field.model != null' v-bind:is='field.meta.component' v-bind:uid='field.meta.uid' v-bind:meta='field.meta' v-bind:model='field.model'></component>" +
        "    </div>" +
        "  </div>" +
        "</div>"
});
