Vue.component("number-field", {
    props: ["uid", "model", "meta"],
    template:
        "<input class='form-control' type='number' :placeholder='meta.placeholder' v-model='model.value'>"
});
