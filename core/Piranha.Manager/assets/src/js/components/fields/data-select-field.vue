<template>
    <select class="form-control" v-model="model.id" v-on:change="update()">
        <option v-for="(item) in model.items.$values" v-bind:key="item.id" v-bind:value="item.id">
            {{ item.name }}
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
                    title: this.model.items.$values[this.model.id]
                });
            }
        }
    }
}
</script>