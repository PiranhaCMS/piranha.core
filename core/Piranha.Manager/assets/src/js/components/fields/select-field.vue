<template>
    <select class="form-control" v-model="model.value" v-on:change="update()">
        <option v-for="(name, value) in meta.options" v-bind:key="value" v-bind:value="value">
            {{ name }}
        </option>
    </select>
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
                    title: this.meta.options[this.model.value]
                });
            }
        }
    }
}
</script>