Vue.component("select-field", {
    props: ["uid", "model", "meta"],
    methods: {
        update: function () {
            if (this.meta.notifyChange) {
                console.log("update field: ", {
                    uid: this.uid,
                    title: this.meta.options[this.model.value]
                });

                // Tell parent that value has been updated
                this.$emit('update-field', {
                    uid: this.uid,
                    title: this.meta.options[this.model.value]
                });
            }
        }
    },
    template:
        "<select class='form-control' v-model='model.value' v-on:change='update()'>" +
            "<option v-for='(name, value) in meta.options' v-bind:value='value'>" +
                "{{ name }}" +
            "</option>" +
        "</select>"
});
