<template>
    <textarea class="form-control" rows="4" :placeholder="meta.placeholder" v-model="model.value" v-on:change="update()"></textarea>
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
        }
    }
}
</script>