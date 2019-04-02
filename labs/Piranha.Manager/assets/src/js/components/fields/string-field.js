Vue.component("string-field", {
    props: ["uid", "model"],
    template:
        "<input class='form-control' type='text' v-model='model.value'>"
});
