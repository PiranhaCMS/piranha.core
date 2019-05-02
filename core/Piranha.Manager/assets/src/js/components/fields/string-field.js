Vue.component("string-field", {
    props: ["uid", "model", "meta"],
    methods: {
        update: function () {
            if (this.meta.notifyChange) {
                console.log("update field: ", {
                    uid: this.uid,
                    title: this.model.value
                });

                // Tell parent that value has been updated
                this.$emit('update-field', {
                    uid: this.uid,
                    title: this.model.value
                });
            }
        }
    },
    template:
        "<input class='form-control' type='text' :placeholder='meta.placeholder' v-model='model.value' v-on:change='update()'>"
});
