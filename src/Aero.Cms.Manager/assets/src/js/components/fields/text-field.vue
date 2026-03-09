<template>
    <div>
        <div v-if="maxLength() > 0" class="input-group">
            <textarea class="form-control" rows="4" :maxlength="maxLength()" :required="isRequired()" :placeholder="meta.placeholder" v-model="model.value" v-on:change="update()"></textarea>
            <div class="input-group-append">
                <div class="input-group-text text-muted">
                    {{ piranha.utils.strLength(model.value) + "/" + maxLength() }}
                </div>
            </div>
        </div>
        <textarea v-else class="form-control" rows="4" :maxlength="maxLength()" :required="isRequired()" :placeholder="meta.placeholder" v-model="model.value" v-on:change="update()"></textarea>
    </div>
</template>

<script>
export default {
    props: ["uid", "model", "meta"],
    methods: {
        update: function () {
            // Tell parent that title has been updated
            if (this.meta.notifyChange) {
                var title = this.model.value;
                if (title.length > 40) {
                    title = title.substring(0, 40) + "...";
                }

                this.$emit('update-title', {
                    uid: this.uid,
                    title: title
                });
            }
        },
        maxLength: function () {
            return this.meta.settings.MaxLength != null && this.meta.settings.MaxLength > 0 ?
                this.meta.settings.MaxLength : null;
        },
        isRequired: function () {
            return false;
            //return this.meta.settings.IsRequired != null && this.meta.settings.IsRequired;
        }
    }
}
</script>