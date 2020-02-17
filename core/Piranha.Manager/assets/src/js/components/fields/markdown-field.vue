<template>
    <div class="markdown-field" :class="{ empty: isEmpty }">
        <textarea :id="uid" spellcheck="false" v-html="model.value"></textarea>
        <div class="markdown-preview"></div>
    </div>
</template>

<script>
export default {
    props: ["uid", "model"],
    data: function () {
        return {
            body: this.model.value
        };
    },
    methods: {
        update: function (md) {
            this.model.value = md;
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