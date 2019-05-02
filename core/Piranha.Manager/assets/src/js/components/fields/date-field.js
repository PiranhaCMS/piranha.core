Vue.component("date-field", {
    props: ["uid", "model", "meta"],
    template:
        "<input class='form-control' type='text' :placeholder='meta.placeholder' v-model='model.value'>"
});
