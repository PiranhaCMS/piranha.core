<template>
    <input class="form-control" type="text" :placeholder="meta.placeholder" v-model="model.value" v-on:change="update()">
</template>

<script>
export default {
    props: ["uid", "model", "meta"],
    methods: {
        update: function () {
            if (this.meta.notifyChange) {
                // Tell parent that value has been updated
                this.$emit('update-field', {
                    uid: this.uid,
                    title: this.model.value
                });
            }
        }
    },
}
</script>