Vue.component("text-field", {
    props: ["uid", "model", "meta"],
    template:
        "<textarea class='form-control' rows='4' :placeholder='meta.placeholder' v-model='model.value'></textarea>"
});
