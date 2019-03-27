Vue.component("html-block", {
    props: ["uid", "block"],
    methods: {
        onBlur: function (e) {
            this.block.body.value = e.target.innerHTML;
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.uid);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid);
    },
    template:
        "<div contenteditable='true' :id='uid' spellcheck='false' v-html='block.body.value' v-on:blur='onBlur'></div>"
});
