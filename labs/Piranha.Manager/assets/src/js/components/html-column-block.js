Vue.component("html-column-block", {
    props: ["uid", "block"],
    methods: {
        onBlurCol1: function (e) {
            this.block.column1.value = e.target.innerHTML;
        },
        onBlurCol2: function (e) {
            this.block.column2.value = e.target.innerHTML;
        }
    },
    mounted: function () {
        piranha.editor.addInline(this.uid + 1);
        piranha.editor.addInline(this.uid + 2);
    },
    beforeDestroy: function () {
        piranha.editor.remove(this.uid + 1);
        piranha.editor.remove(this.uid + 2);
    },
    template:
        "<div class='row'>" +
        "  <div :id='uid + 1' class='col-md-6'>" +
        "    <div contenteditable='true' spellcheck='false' v-html='block.column1.value' v-on:blur='onBlurCol1'></div>" +
        "  </div>" +
        "  <div :id='uid + 2' class='col-md-6'>" +
        "    <div contenteditable='true' spellcheck='false' v-html='block.column2.value' v-on:blur='onBlurCol2'></div>" +
        "  </div>" +
        "</div>"
});
