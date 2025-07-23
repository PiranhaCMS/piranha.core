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
            // Tell parent that title has been updated
            if (this.meta.notifyChange) {
                this.$emit('update-title', {
                    uid: this.uid,
                    title: this.model.items.$values.find(v => v.id === this.model.id).name
                });
            }
        }
    }
}
</script>