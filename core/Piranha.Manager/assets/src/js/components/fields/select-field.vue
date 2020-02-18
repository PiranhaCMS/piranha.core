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
            if (this.meta.notifyChange) {
                // Tell parent that value has been updated
                this.$emit('update-field', {
                    uid: this.uid,
                    title: this.meta.options[this.model.value]
                });
            }
        }
    }
}
</script>