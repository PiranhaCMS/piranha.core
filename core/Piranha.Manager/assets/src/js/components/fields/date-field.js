Vue.component("date-field", {
    props: ["uid", "model", "meta"],
<<<<<<< Updated upstream
    template:
        "<input class='form-control' type='text' :placeholder='meta.placeholder' v-model='model.value'>"
=======
    template: "<datepicker v-model='model.value' :format='_options.format' :monday-first='_options.mondayFirst' :typeable='_options.typeable' :bootstrap-styling='_options.bootstrapStyling'></datepicker>",
    components: {
        datepicker: vuejsDatepicker
    },
    created: function () {
        this._options = {
            bootstrapStyling = true,
            mondayFirst = true,
            format = "yyyy-MM-dd",
            typeable = true
        };
    }
>>>>>>> Stashed changes
});
