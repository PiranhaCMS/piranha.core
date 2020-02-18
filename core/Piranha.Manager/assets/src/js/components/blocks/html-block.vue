<template>
    <div class="block-body" :class="{ empty: isEmpty }">
        <div contenteditable="true" :id="uid" spellcheck="false" v-html="body" v-on:blur="onBlur"></div>
    </div>
</template>

<script>
export default {
    props: ["uid", "toolbar", "model"],
    data: function () {
        return {
            body: this.model.body.value
        };
    },
    methods: {
        onBlur: function (e) {
            this.model.body.value = e.target.innerHTML;
        },
        onChange: function (data) {
            this.model.body.value = data;
        }
    },
    computed: {
        isEmpty: function () {
            return piranha.utils.isEmptyHtml(this.model.body.value);
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.uid, this.toolbar, this.onChange);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid);
    }
}
</script>