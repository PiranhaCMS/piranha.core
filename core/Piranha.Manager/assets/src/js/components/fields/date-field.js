Vue.component("date-field", {
    props: ["uid", "model", "meta"],
    template: "<datepicker v-model='model.value' :format='_options.format' :bootstrap-styling='true'></datepicker>",
    components: {
        datepicker: vuejsDatepicker
    },
    created: function () {
        this._options = {
            format = "yyyy-MM-dd"
        };
    }
});
