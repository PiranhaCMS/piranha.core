<template>
    <input class="form-control" type="text" :maxlength="maxLength()" :required="isRequired()" :placeholder="meta.placeholder" v-model="model.value" v-on:change="update()">
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
        maxLength: function () {
            return this.meta.settings.MaxLength != null && this.meta.settings.MaxLength > 0 ?
                this.meta.settings.MaxLength : null;
        },
        isRequired: function () {
            return this.meta.settings.IsRequired != null && this.meta.settings.IsRequired;
        }
    }
}
</script>