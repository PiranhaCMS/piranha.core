<template>
    <div class="markdown-field" :class="{ empty: isEmpty }">
        <textarea :id="uid" v-html="model.value"></textarea>
        <div class="markdown-preview"></div>
    </div>
</template>

<script>
export default {
    props: ["uid", "model", "meta"],
    data: function () {
        return {
            body: this.model.value
        };
    },
    methods: {
        update: function (md) {
            this.model.value = md;

            // Tell parent that title has been updated
            if (this.meta && this.meta.notifyChange) {
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
    },
    computed: {
        isEmpty: function () {
            return this.model.value == null || this.model.value === "";
        }
    },
    mounted: function () {
        var self = this;

        piranha.editor.addInlineMarkdown(self.uid, self.model.value, self.update);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid);
    }
}
</script>