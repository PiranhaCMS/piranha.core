<template>
    <div class="input-group color-field">
        <div class="input-group-prepend">
            <div class="color-preview" :style="{ backgroundColor: model.value }"></div>
            <input class="form-control" type="color" v-model="model.value">
        </div>
        <input class="form-control" type="text" v-model="model.value" v-on:change="update()" :readonly="readonly()" :placeholder="meta.placeholder">
    </div>    
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
        },
        readonly: function () {
            return this.meta.settings.DisallowInput != null && this.meta.settings.DisallowInput;
        }
    }
}
</script>