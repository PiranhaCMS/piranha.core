<template>
    <div class="block-body" :class="{ empty: isEmpty }">
        <div contenteditable="true" :id="uid" v-html="body" v-on:blur="onBlur"></div>
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
            this.model.body.value = tinyMCE.activeEditor.getContent();
        },
        onChange: function (data) {
            this.model.body.value = data;
        }
    },
    computed: {
        isEmpty: function () {
            return Aero.utils.isEmptyHtml(this.model.body.value);
        }
    },
    mounted: function () {
        Aero.editor.addInline(this.uid, this.toolbar, this.onChange);
    },
    beforeDestroy: function () {
        Aero.editor.remove(this.uid);
    }
}
</script>