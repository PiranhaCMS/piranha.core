Vue.component("html-block", {
    props: ["gid", "block"],
    methods: {
        onBlur: function (e) {
            this.block.body.value = e.target.innerHTML;
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.gid);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.gid);
    },
    template:
        "<div contenteditable='true' :id='gid' spellcheck='false' v-html='block.body.value' v-on:blur='onBlur'></div>"
});
