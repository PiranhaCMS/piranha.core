Vue.component("checkbox-field", {
    props: ["uid", "model", "meta"],
    template:
        "<div class='form-group form-check'>" +
        "  <input type='checkbox' class='form-check-input' :id='meta.uid' v-model='model.value'>" +
        "  <label class='form-check-label' :for='meta.uid'>{{ meta.placeholder}}</label>" +
        "</div>"
});
