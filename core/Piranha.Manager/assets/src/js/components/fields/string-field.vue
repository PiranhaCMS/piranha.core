<template>
    <input class="form-control" type="text" :placeholder="meta.placeholder" v-model="model.value" v-on:change="update()">
</template>

<script>
export default {
    props: ["uid", "model", "meta"],
    methods: {
        update: function () {
            // Tell parent that title has been updated
            if (this.meta.notifyChange) {
                this.$emit('update-title', {
                    uid: this.uid,
                    title: this.model.value
                });
            }
        }
    }
}
</script>