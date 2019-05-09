Vue.component("number-field", {
    props: ["uid", "model", "meta"],
    template:
        "<input class='form-control' type='text' :placeholder='meta.placeholder' v-model='model.value'>"
});
