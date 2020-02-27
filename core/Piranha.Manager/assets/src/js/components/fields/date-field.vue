<template>
    <datepicker v-on:closed="onClosed($event)" v-model="model.value" :format="_options.format" :monday-first="_options.mondayFirst" :typeable="_options.typeable" :bootstrap-styling="_options.bootstrapStyling"></datepicker>
</template>

<script>
export default {
    props: ["uid", "model", "meta"],
    components: {
        datepicker: vuejsDatepicker
    },
    methods: {
        onClosed: function () {
            var d = this.model.value;

            var str = d.getFullYear() + "-" +
                (d.getMonth() < 9 ? "0" : "") + (d.getMonth() + 1) + "-" +
                (d.getDate() < 10 ? "0" : "") + d.getDate();

            this.model.value = str;
        }
    },
    created: function () {
        this._options = {
            bootstrapStyling: true,
            mondayFirst: true,
            format: "yyyy-MM-dd",
            typeable: true
        };
    }
}
</script>